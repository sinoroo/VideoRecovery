namespace VideoRecovery.App.Services;

/// <summary>
/// 복구 결과물 저장(내보내기) 추상화. 작업 영역의 복구본을 사용자가 지정한 위치로
/// 복사하거나, 결과물이 있는 폴더를 탐색기로 연다. 테스트 용이성을 위해 인터페이스로 분리한다.
/// </summary>
public interface IFileSaveService
{
    /// <summary>
    /// 저장 다이얼로그를 띄워 <paramref name="sourcePath"/>의 복구본을 사용자가 선택한 위치로 복사한다.
    /// </summary>
    /// <param name="sourcePath">작업 영역의 복구 결과물 경로.</param>
    /// <param name="suggestedFileName">기본 제안 파일명.</param>
    /// <returns>저장된 대상 경로(취소 시 null).</returns>
    string? SaveCopy(string sourcePath, string suggestedFileName);

    /// <summary>지정 파일이 포함된 폴더를 탐색기로 열고 해당 파일을 선택한다.</summary>
    void RevealInExplorer(string path);
}
