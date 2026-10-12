using Microsoft.Win32;

namespace VideoRecovery.App.Services;

/// <summary>
/// .NET 8 WPF의 <see cref="OpenFolderDialog"/> 기반 폴더 선택 서비스.
/// 추가 의존성 없이 네이티브 폴더 선택을 제공한다.
/// </summary>
public sealed class FolderPickerService : IFolderPickerService
{
    public string? PickFolder(string title)
    {
        var dialog = new OpenFolderDialog
        {
            Title = title,
            Multiselect = false
        };

        return dialog.ShowDialog() == true ? dialog.FolderName : null;
    }
}
