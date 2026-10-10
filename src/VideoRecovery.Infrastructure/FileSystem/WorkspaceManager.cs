using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.Infrastructure;

namespace VideoRecovery.Infrastructure.FileSystem;

/// <summary>
/// <see cref="IWorkspaceManager"/> 의 기본 구현. 원본과 분리된 작업 영역을 관리한다. (요구사항 2.1)
/// </summary>
public sealed class WorkspaceManager : IWorkspaceManager
{
    private readonly ILogger<WorkspaceManager> _logger;

    public WorkspaceManager(string workspaceRoot, ILogger<WorkspaceManager> logger)
    {
        WorkspaceRoot = workspaceRoot;
        _logger = logger;
        Directory.CreateDirectory(workspaceRoot);
    }

    /// <inheritdoc />
    public string WorkspaceRoot { get; }

    /// <inheritdoc />
    public string CreateSessionDirectory(string sourceFilePath)
    {
        var baseName = Path.GetFileNameWithoutExtension(sourceFilePath);
        var stamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fff");
        var session = Path.Combine(WorkspaceRoot, $"{baseName}_{stamp}");
        Directory.CreateDirectory(session);
        _logger.LogInformation("Created recovery session directory: {Session}", session);
        return session;
    }

    /// <inheritdoc />
    public void CleanupSession(string sessionDirectory)
    {
        if (Directory.Exists(sessionDirectory))
        {
            Directory.Delete(sessionDirectory, recursive: true);
            _logger.LogInformation("Cleaned up session directory: {Session}", sessionDirectory);
        }
    }
}
