using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace VideoRecovery.App.Services;

/// <summary>
/// WPF <see cref="SaveFileDialog"/> 기반 <see cref="IFileSaveService"/> 구현.
/// 복구본을 사용자가 선택한 위치로 복사하며 원본 작업 영역 파일은 그대로 둔다.
/// </summary>
public sealed class FileSaveService : IFileSaveService
{
    public string? SaveCopy(string sourcePath, string suggestedFileName)
    {
        if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
        {
            throw new FileNotFoundException("복구 결과물을 찾을 수 없습니다.", sourcePath);
        }

        var extension = Path.GetExtension(suggestedFileName);
        if (string.IsNullOrEmpty(extension))
        {
            extension = Path.GetExtension(sourcePath);
        }

        var filter = string.IsNullOrEmpty(extension)
            ? "모든 파일 (*.*)|*.*"
            : $"비디오 파일 (*{extension})|*{extension}|모든 파일 (*.*)|*.*";

        var dialog = new SaveFileDialog
        {
            Title = "복구된 영상 저장",
            FileName = suggestedFileName,
            DefaultExt = extension,
            Filter = filter,
            AddExtension = true,
            OverwritePrompt = true
        };

        if (dialog.ShowDialog() != true)
        {
            return null;
        }

        File.Copy(sourcePath, dialog.FileName, overwrite: true);
        return dialog.FileName;
    }

    public void RevealInExplorer(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            return;
        }

        // 탐색기에서 해당 파일을 선택한 상태로 폴더를 연다.
        Process.Start(new ProcessStartInfo
        {
            FileName = "explorer.exe",
            Arguments = $"/select,\"{path}\"",
            UseShellExecute = true
        });
    }
}
