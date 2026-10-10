using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;

namespace VideoRecovery.Infrastructure.FFmpeg;

/// <summary>
/// 외부 CLI(ffmpeg/ffprobe) 실행 공통 헬퍼. 모든 외부 프로세스 호출을 이 계층에 격리한다.
/// (요구사항 12, 17-9)
/// </summary>
internal sealed class ProcessRunner
{
    private readonly ILogger _logger;

    public ProcessRunner(ILogger logger) => _logger = logger;

    /// <summary>
    /// 지정한 실행 파일을 인자와 함께 실행하고 (ExitCode, StdOut, StdErr) 를 반환한다.
    /// 원본 파일은 인자로만 전달하며 절대 쓰기 모드로 열지 않는다. (요구사항 17-1)
    /// </summary>
    public async Task<ProcessResult> RunAsync(
        string fileName,
        string arguments,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Executing: {FileName} {Arguments}", fileName, arguments);

        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process = new Process { StartInfo = psi };
        var stdout = new StringBuilder();
        var stderr = new StringBuilder();

        process.OutputDataReceived += (_, e) => { if (e.Data is not null) stdout.AppendLine(e.Data); };
        process.ErrorDataReceived += (_, e) => { if (e.Data is not null) stderr.AppendLine(e.Data); };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);

        return new ProcessResult(process.ExitCode, stdout.ToString(), stderr.ToString());
    }
}

/// <summary>프로세스 실행 결과.</summary>
internal readonly record struct ProcessResult(int ExitCode, string StandardOutput, string StandardError);
