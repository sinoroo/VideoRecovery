using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.Analysis;
using VideoRecovery.Core.Abstractions.CrossMatch;
using VideoRecovery.Core.Abstractions.Recovery;
using VideoRecovery.Core.Abstractions.Scanner;
using VideoRecovery.Core.Models;

namespace VideoRecovery.App.Services;

/// <summary>
/// <see cref="IDiagnosisPipeline"/> 구현. 저장소 전체를 스캔한 뒤 각 영상을 분석하고,
/// 정상 영상들의 메타데이터를 라이브러리로 삼아 손상 영상의 참조 후보를 추천한다.
/// </summary>
public sealed class DiagnosisPipeline : IDiagnosisPipeline
{
    private readonly IVideoStorageScanner _scanner;
    private readonly IHealthAnalyzer _healthAnalyzer;
    private readonly ICorruptionDetector _corruptionDetector;
    private readonly IVideoMatcher _matcher;
    private readonly IRecoveryPlanner _planner;
    private readonly ILogger<DiagnosisPipeline> _logger;

    public DiagnosisPipeline(
        IVideoStorageScanner scanner,
        IHealthAnalyzer healthAnalyzer,
        ICorruptionDetector corruptionDetector,
        IVideoMatcher matcher,
        IRecoveryPlanner planner,
        ILogger<DiagnosisPipeline> logger)
    {
        _scanner = scanner;
        _healthAnalyzer = healthAnalyzer;
        _corruptionDetector = corruptionDetector;
        _matcher = matcher;
        _planner = planner;
        _logger = logger;
    }

    public async Task<IReadOnlyList<DiagnosisResult>> RunAsync(
        string rootDirectory,
        bool recursive = true,
        IProgress<DiagnosisProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("진단 파이프라인 시작: {Root}", rootDirectory);

        // 1) 스캔.
        progress?.Report(new DiagnosisProgress { Stage = "저장소 스캔 중...", CurrentFile = rootDirectory });
        var files = await _scanner.ScanAsync(rootDirectory, recursive, null, cancellationToken).ConfigureAwait(false);

        // 2) 각 영상 건강 분석(메타데이터 + 디코드 테스트).
        var total = files.Count;
        var healthByPath = new Dictionary<string, HealthReport>(StringComparer.OrdinalIgnoreCase);
        var completed = 0;

        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report(new DiagnosisProgress
            {
                Stage = "건강 분석 중...",
                CurrentFile = file.FullPath,
                Completed = completed,
                Total = total
            });

            try
            {
                healthByPath[file.FullPath] = await _healthAnalyzer
                    .AnalyzeAsync(file.FullPath, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError_Safe(ex, file.FullPath);
            }

            completed++;
        }

        // 3) 정상 영상의 메타데이터를 참조 라이브러리로 구성.
        var library = healthByPath.Values
            .Where(h => h.Metadata is { ProbeFailed: false })
            .Select(h => h.Metadata!)
            .ToList();

        // 4) 각 영상: 손상 분류 → 참조 후보 → 복구 계획.
        var results = new List<DiagnosisResult>(total);
        completed = 0;

        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report(new DiagnosisProgress
            {
                Stage = "손상 분류 및 참조 매칭 중...",
                CurrentFile = file.FullPath,
                Completed = completed,
                Total = total
            });

            if (!healthByPath.TryGetValue(file.FullPath, out var health))
            {
                results.Add(new DiagnosisResult { File = file, Error = "분석 실패" });
                completed++;
                continue;
            }

            var corruption = _corruptionDetector.Classify(health);

            IReadOnlyList<ReferenceCandidate> candidates = Array.Empty<ReferenceCandidate>();
            if (!corruption.IsHealthy && health.Metadata is { ProbeFailed: false } meta)
            {
                candidates = _matcher.FindCandidates(meta, library);
            }

            var plan = _planner.CreatePlan(corruption, candidates);

            results.Add(new DiagnosisResult
            {
                File = file,
                Health = health,
                Corruption = corruption,
                Candidates = candidates,
                Plan = plan
            });

            completed++;
        }

        progress?.Report(new DiagnosisProgress
        {
            Stage = "완료",
            Completed = total,
            Total = total
        });

        _logger.LogInformation(
            "진단 파이프라인 완료: {Total}개 중 손상 {Damaged}개",
            total, results.Count(r => r.Corruption is { IsHealthy: false }));

        return results;
    }
}

/// <summary>로깅 헬퍼(분석 개별 실패가 전체 파이프라인을 멈추지 않도록).</summary>
internal static class LoggerSafeExtensions
{
    public static void LogError_Safe(this ILogger logger, Exception ex, string path)
        => logger.LogError(ex, "영상 분석 실패: {Path}", path);
}
