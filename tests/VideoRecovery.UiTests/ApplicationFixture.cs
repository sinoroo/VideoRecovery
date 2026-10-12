using System.IO;
using FlaUI.Core;
using FlaUI.UIA3;
using Xunit;

namespace VideoRecovery.UiTests;

/// <summary>
/// 앱을 1회 실행하여 모든 UI 테스트가 공유하는 fixture(collection fixture).
/// 요소는 항상 AutomationId로만 찾는다. 주요 단계에서 스크린샷을 ui-artifacts/에 저장한다.
/// </summary>
public sealed class ApplicationFixture : IDisposable
{
    public Application App { get; }
    public UIA3Automation Automation { get; }

    private readonly string _artifactDir;

    public ApplicationFixture()
    {
        var exePath = LocateAppExecutable();
        Automation = new UIA3Automation();
        App = Application.Launch(exePath);

        _artifactDir = Path.Combine(AppContext.BaseDirectory, "ui-artifacts");
        Directory.CreateDirectory(_artifactDir);

        // 메인 윈도우가 준비될 때까지 대기.
        App.GetMainWindow(Automation, TimeSpan.FromSeconds(20));
    }

    /// <summary>현재 화면 스크린샷을 저장한다.</summary>
    public void Capture(string name)
    {
        try
        {
            var path = Path.Combine(_artifactDir, $"{name}.png");
            FlaUI.Core.Capturing.Capture.Screen().ToFile(path);
        }
        catch
        {
            // 스크린샷 실패는 테스트를 중단시키지 않는다.
        }
    }

    /// <summary>빌드된 VideoRecovery.App.exe 경로를 저장소 구조에서 탐색한다.</summary>
    private static string LocateAppExecutable()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "VideoRecovery.sln")))
        {
            dir = dir.Parent;
        }

        if (dir is null)
        {
            throw new FileNotFoundException("저장소 루트(VideoRecovery.sln)를 찾을 수 없습니다.");
        }

        var appBin = Path.Combine(dir.FullName, "src", "VideoRecovery.App", "bin");
        if (!Directory.Exists(appBin))
        {
            throw new FileNotFoundException($"앱 출력 폴더가 없습니다: {appBin}. 먼저 솔루션을 빌드하세요.");
        }

        var exe = Directory
            .EnumerateFiles(appBin, "VideoRecovery.App.exe", SearchOption.AllDirectories)
            .OrderByDescending(File.GetLastWriteTimeUtc)
            .FirstOrDefault();

        if (exe is null)
        {
            throw new FileNotFoundException(
                $"VideoRecovery.App.exe를 찾을 수 없습니다({appBin}). 먼저 솔루션을 빌드하세요.");
        }

        return exe;
    }

    public void Dispose()
    {
        try
        {
            if (!App.HasExited)
            {
                App.Close();
                App.WaitWhileMainHandleIsMissing(TimeSpan.FromSeconds(2));
            }
        }
        catch
        {
            // 무시 — 정리.
        }
        finally
        {
            App.Dispose();
            Automation.Dispose();
        }
    }
}

/// <summary>UI 테스트 collection 정의(앱 1회 실행 공유).</summary>
[CollectionDefinition("App collection")]
public sealed class AppCollection : ICollectionFixture<ApplicationFixture>;
