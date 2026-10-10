namespace VideoRecovery.Core.Abstractions.Infrastructure;

/// <summary>
/// 복구 작업 영역 관리. 원본과 분리된 별도 영역에서 모든 작업을 수행한다. (요구사항 2.1)
/// Original → Analysis → Recovery Workspace → Recovered Video
/// </summary>
public interface IWorkspaceManager
{
    /// <summary>작업 영역 루트 경로.</summary>
    string WorkspaceRoot { get; }

    /// <summary>
    /// 특정 원본 파일에 대한 격리된 작업 디렉터리를 생성/반환한다.
    /// 복구 실패 시 재시도를 위해 세션 단위로 분리한다. (요구사항 2.1, 17-6)
    /// </summary>
    string CreateSessionDirectory(string sourceFilePath);

    /// <summary>세션 작업 디렉터리를 정리한다.</summary>
    void CleanupSession(string sessionDirectory);
}
