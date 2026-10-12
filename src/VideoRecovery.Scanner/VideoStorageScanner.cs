using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.Infrastructure;
using VideoRecovery.Core.Abstractions.Scanner;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Scanner;

/// <summary>
/// 폴더/HDD를 스캔하여 영상 파일 기본 정보를 수집하는 <see cref="IVideoStorageScanner"/> 구현.
/// 원본은 변경하지 않으며, 파일 시스템 접근은 <see cref="IFileSystemService"/>로 추상화한다.
/// </summary>
public sealed class VideoStorageScanner : IVideoStorageScanner
{
    private readonly IFileSystemService _fileSystem;
    private readonly ILogger<VideoStorageScanner> _logger;

    public VideoStorageScanner(IFileSystemService fileSystem, ILogger<VideoStorageScanner> logger)
    {
        _fileSystem = fileSystem;
        _logger = logger;
    }

    public Task<IReadOnlyList<VideoFileInfo>> ScanAsync(
        string rootDirectory,
        bool recursive = true,
        IProgress<VideoFileInfo>? progress = null,
        CancellationToken cancellationToken = default)
    {
        return Task.Run<IReadOnlyList<VideoFileInfo>>(() =>
        {
            var results = new List<VideoFileInfo>();

            if (!_fileSystem.DirectoryExists(rootDirectory))
            {
                _logger.LogWarning("스캔 대상 폴더가 존재하지 않음: {Root}", rootDirectory);
                return results;
            }

            _logger.LogInformation("저장소 스캔 시작: {Root} (recursive={Recursive})", rootDirectory, recursive);

            foreach (var file in _fileSystem.EnumerateVideos(rootDirectory, recursive))
            {
                cancellationToken.ThrowIfCancellationRequested();
                results.Add(file);
                progress?.Report(file);
            }

            _logger.LogInformation("저장소 스캔 완료: {Count}개 영상 발견", results.Count);
            return results;
        }, cancellationToken);
    }
}
