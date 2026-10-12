namespace VideoRecovery.Core.Abstractions.Infrastructure;

/// <summary>
/// 복구 작업 영역 관리자. 원본을 직접 수정하지 않고 격리된 세션 폴더에서
/// 모든 복구/분석 산출물을 생성한다. 복구 실패 시 동일 원본으로 재시도할 수 있다.
/// </summary>
public interface IWorkspaceManager
{
    /// <summary>새 작업 세션 폴더를 생성하고 경로를 반환한다.</summary>
    string CreateSession(string originalPath);

    /// <summary>세션 내부의 결과물 경로를 조합한다.</summary>
    string GetArtifactPath(string sessionPath, string fileName);

    /// <summary>세션을 정리(삭제)한다.</summary>
    void CleanupSession(string sessionPath);
}
