using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.Infrastructure;
using VideoRecovery.Core.Abstractions.Scanner;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Scanner;

/// <summary>
/// <see cref="IVideoStorageScanner"/> 의 기본 구현. 파일 열거 + ffprobe 메타데이터 수집. (요구사항 3.1)
/// Scanner Agent. 원본은 읽기 전용으로만 접근한다. (요구사항 16, 17-1)
/// </summary>
public sealed class VideoStorageScanner : IVideoStorageScanner
{
    private readonly IFileSystemService _fileSystem;
    private readonly IFFprobeService _ffprobe;
    private readonly ILogger<VideoStorageScanner> _logger;

    public VideoStorageScanner(
        IFileSystemService fileSystem,
        IFFprobeService ffprobe,
        ILogger<VideoStorageScanner> logger)
    {
        _fileSystem = fileSystem;
        _ffprobe = ffprobe;
        _logger = logger;
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<VideoFileInfo> ScanAsync(
        string root,
        IProgress<double>? progress = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Scan started: {Root}", root);

        var paths = _fileSystem.EnumerateVideoFiles(root).ToList();
        var total = paths.Count;
        var done = 0;

        foreach (var path in paths)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var fileInfo = new FileInfo(path);
            var info = new VideoFileInfo
            {
                FilePath = path,
                FileName = fileInfo.Name,
                SizeBytes = fileInfo.Length,
                CreatedAt = fileInfo.CreationTimeUtc,
                ModifiedAt = fileInfo.LastWriteTimeUtc,
            };

            // 메타데이터 수집은 손상 파일에서 실패할 수 있으므로 격리하여 계속 진행한다.
            try
            {
                info.Metadata = await _ffprobe.ProbeAsync(path, cancellationToken).ConfigureAwait(false);
            }
            catch (NotImplementedException)
            {
                // Phase 1 구현 전까지는 메타데이터 없이 파일 정보만 반환한다.
                _logger.LogDebug("Probe not implemented yet for {Path}", path);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Probe failed for {Path}; file may be corrupted", path);
            }

            done++;
            progress?.Report(total == 0 ? 1.0 : (double)done / total);

            yield return info;
        }

        _logger.LogInformation("Scan completed: {Count} files", total);
    }
}
