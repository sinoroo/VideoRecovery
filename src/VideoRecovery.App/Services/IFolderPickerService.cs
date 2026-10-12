namespace VideoRecovery.App.Services;

/// <summary>폴더 선택 다이얼로그 추상화(테스트 용이성을 위해 인터페이스로 분리).</summary>
public interface IFolderPickerService
{
    /// <summary>폴더 선택 다이얼로그를 띄우고 선택된 경로를 반환한다(취소 시 null).</summary>
    string? PickFolder(string title);
}
