using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.Infrastructure;

/// <summary>
/// ffmpeg CLI 래퍼. 디코드 테스트 및 복구용 트랜스코딩/리먹싱을 수행한다.
/// 외부 라이브러리는 Infrastructure 계층에서만 추상화한다. (요구사항 12, 17-9)
/// </summary>
public interface IFFmpegService
{
    /// <summary>
    /// 전체 영상을 디코딩하면서 오류 위치를 구간으로 기록한다. (요구사항 4 - Decode Test)
    /// </summary>
    /// <param name="filePath">원본 파일 경로(읽기 전용).</param>
    /// <param name="progress">0.0~1.0 진행률 보고(선택).</param>
    IAsyncEnumerable<DecodeSegment> DecodeTestAsync(
        string filePath,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 컨테이너를 재구성(리먹싱)하여 작업 영역에 출력한다. (요구사항 8 Level 1)
    /// </summary>
    Task RemuxAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// 지정한 프레임 범위를 이미지로 추출한다. (프레임 분석/Cross-Video 비교용)
    /// </summary>
    Task ExtractFramesAsync(
        string filePath,
        long startFrame,
        long endFrame,
        string outputDirectory,
        CancellationToken cancellationToken = default);
}
