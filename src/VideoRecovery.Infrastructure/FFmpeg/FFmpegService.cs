using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.Infrastructure;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Infrastructure.FFmpeg;

/// <summary>
/// ffmpeg CLI 기반 <see cref="IFFmpegService"/> 구현. (요구사항 12)
/// TODO(Phase 1/2): 디코드 테스트 로그 파싱 및 리먹싱/프레임 추출 구현.
/// </summary>
public sealed class FFmpegService : IFFmpegService
{
    private readonly FFmpegOptions _options;
    private readonly ProcessRunner _runner;
    private readonly ILogger<FFmpegService> _logger;

    public FFmpegService(FFmpegOptions options, ILogger<FFmpegService> logger)
    {
        _options = options;
        _logger = logger;
        _runner = new ProcessRunner(logger);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<DecodeSegment> DecodeTestAsync(
        string filePath,
        IProgress<double>? progress = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // 의도된 CLI 호출:
        //   ffmpeg -v error -i "<filePath>" -f null -
        // stderr 의 decode error 로그 위치를 프레임 구간으로 변환한다.
        _logger.LogInformation("Decode test requested for {FilePath}", filePath);
        await Task.CompletedTask.ConfigureAwait(false);
        throw new NotImplementedException(
            "FFmpegService.DecodeTestAsync: 디코드 오류 로그 파싱을 Phase 1 에서 구현한다.");
#pragma warning disable CS0162 // 도달할 수 없는 코드: async iterator 시그니처 유지를 위한 yield.
        yield break;
#pragma warning restore CS0162
    }

    /// <inheritdoc />
    public Task RemuxAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken = default)
    {
        // 의도된 CLI 호출: ffmpeg -i "<source>" -c copy "<destination>"
        _logger.LogInformation("Remux requested {Source} -> {Destination}", sourcePath, destinationPath);
        throw new NotImplementedException(
            "FFmpegService.RemuxAsync: 컨테이너 리먹싱을 Phase 2 에서 구현한다.");
    }

    /// <inheritdoc />
    public Task ExtractFramesAsync(
        string filePath,
        long startFrame,
        long endFrame,
        string outputDirectory,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Extract frames requested {FilePath} [{Start}-{End}] -> {Output}",
            filePath, startFrame, endFrame, outputDirectory);
        throw new NotImplementedException(
            "FFmpegService.ExtractFramesAsync: 프레임 추출을 Phase 2 에서 구현한다.");
    }
}
