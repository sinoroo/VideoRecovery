using System.Diagnostics;
using System.Text;

namespace VideoRecovery.Infrastructure.FFmpeg;

/// <summary>단일 외부 프로세스 실행 결과.</summary>
public sealed class ProcessResult
{
    /// <summary>종료 코드.</summary>
    public int ExitCode { get; init; }

    /// <summary>표준 출력.</summary>
    public string StandardOutput { get; init; } = string.Empty;

    /// <summary>표준 오류.</summary>
    public string StandardError { get; init; } = string.Empty;

    /// <summary>프로세스가 정상적으로 시작/종료되었는지 여부.</summary>
    public bool Started { get; init; }
}

/// <summary>
/// 외부 CLI(ffmpeg/ffprobe)를 호출하는 저수준 프로세스 러너.
/// 바이너리가 설치되어 있지 않아도 예외로 죽지 않고 결과로 보고한다.
/// </summary>
public static class ProcessRunner
{
    /// <summary>프로세스를 실행하고 표준 출력/오류를 수집한다.</summary>
    public static async Task<ProcessResult> RunAsync(
        string fileName,
        string arguments,
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        using var process = new Process { StartInfo = psi };

        var stdout = new StringBuilder();
        var stderr = new StringBuilder();
        process.OutputDataReceived += (_, e) => { if (e.Data is not null) stdout.AppendLine(e.Data); };
        process.ErrorDataReceived += (_, e) => { if (e.Data is not null) stderr.AppendLine(e.Data); };

        try
        {
            if (!process.Start())
            {
                return new ProcessResult { Started = false, ExitCode = -1 };
            }
        }
        catch (Exception ex)
        {
            // 바이너리 미설치 등 — 호출자가 처리하도록 결과로 반환.
            return new ProcessResult
            {
                Started = false,
                ExitCode = -1,
                StandardError = ex.Message
            };
        }

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(timeout);

        try
        {
            await process.WaitForExitAsync(timeoutCts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            TryKill(process);
            throw;
        }

        return new ProcessResult
        {
            Started = true,
            ExitCode = process.ExitCode,
            StandardOutput = stdout.ToString(),
            StandardError = stderr.ToString()
        };
    }

    private static void TryKill(Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
            }
        }
        catch
        {
            // 무시 — 정리 목적.
        }
    }
}
