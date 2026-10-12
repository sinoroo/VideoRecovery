using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.Scanner;

/// <summary>
/// 영상 저장소 스캐너. 폴더/HDD를 스캔하여 영상 파일을 찾고 기본 정보를 수집한다.
/// Scanner Agent의 인터페이스.
/// </summary>
public interface IVideoStorageScanner
{
    /// <summary>지정 폴더(및 하위)의 영상 파일 기본 정보를 수집한다.</summary>
    Task<IReadOnlyList<VideoFileInfo>> ScanAsync(
        string rootDirectory,
        bool recursive = true,
        IProgress<VideoFileInfo>? progress = null,
        CancellationToken cancellationToken = default);
}
