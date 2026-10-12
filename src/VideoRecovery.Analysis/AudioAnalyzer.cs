using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.Analysis;
using VideoRecovery.Core.Enums;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Analysis;

/// <summary>
/// 오디오 스트림 분석기. Phase 1에서는 스트림 메타데이터 유무로 기본 점검을 수행하며,
/// 정밀한 A/V 동기 측정은 디코드 타임스탬프 분석(후속 단계)에서 확장한다.
/// </summary>
public sealed class AudioAnalyzer : IAudioAnalyzer
{
    private readonly ILogger<AudioAnalyzer> _logger;

    public AudioAnalyzer(ILogger<AudioAnalyzer> logger) => _logger = logger;

    public Task<double?> MeasureAvSyncAsync(
        VideoMetadata metadata, CancellationToken cancellationToken = default)
    {
        var audio = metadata.PrimaryAudio;
        var video = metadata.PrimaryVideo;

        if (audio is null || video is null)
        {
            // 한쪽 스트림이 없으면 동기 측정 대상이 아님.
            return Task.FromResult<double?>(null);
        }

        if (audio.SampleRate <= 0)
        {
            _logger.LogWarning("오디오 스트림 샘플레이트 비정상: {Path}", metadata.FullPath);
        }

        // 정밀 측정 미구현(Phase 2+): 현재는 측정 불가(null)로 보고한다.
        return Task.FromResult<double?>(null);
    }
}
