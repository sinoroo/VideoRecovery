using VideoRecovery.Core.Abstractions.Infrastructure;

namespace VideoRecovery.Infrastructure.FileSystem;

/// <summary>
/// 격리된 복구 작업 영역 관리자. 원본 디렉터리가 아닌 별도 루트에 세션 폴더를
/// 만들어 모든 산출물을 생성한다. 원본은 절대 수정하지 않는다.
/// </summary>
public sealed class WorkspaceManager : IWorkspaceManager
{
    private readonly string _root;

    /// <summary>작업 루트를 지정한다(기본: %LOCALAPPDATA%/VideoRecovery/workspace).</summary>
    public WorkspaceManager(string? rootDirectory = null)
    {
        _root = rootDirectory ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "VideoRecovery", "workspace");
        Directory.CreateDirectory(_root);
    }

    public string CreateSession(string originalPath)
    {
        var name = Path.GetFileNameWithoutExtension(originalPath);
        var session = Path.Combine(_root, $"{name}_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}");
        Directory.CreateDirectory(session);
        return session;
    }

    public string GetArtifactPath(string sessionPath, string fileName)
        => Path.Combine(sessionPath, fileName);

    public void CleanupSession(string sessionPath)
    {
        try
        {
            if (Directory.Exists(sessionPath))
            {
                Directory.Delete(sessionPath, recursive: true);
            }
        }
        catch
        {
            // 무시 — 정리 목적.
        }
    }
}
