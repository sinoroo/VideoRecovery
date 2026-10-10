using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.Scanner;

/// <summary>
/// 영상 저장소 스캐너. 폴더/HDD 에서 영상 파일을 검색하고 기본 정보를 수집한다. (요구사항 3.1)
/// Scanner Agent 의 계약. (요구사항 16)
/// </summary>
public interface IVideoStorageScanner
{
    /// <summary>
    /// 지정한 루트 경로를 스캔하여 영상 파일 정보를 비동기로 열거한다.
    /// </summary>
    /// <param name="root">스캔 대상 폴더/드라이브 경로.</param>
    /// <param name="progress">0.0~1.0 진행률 보고(선택).</param>
    IAsyncEnumerable<VideoFileInfo> ScanAsync(
        string root,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default);
}
