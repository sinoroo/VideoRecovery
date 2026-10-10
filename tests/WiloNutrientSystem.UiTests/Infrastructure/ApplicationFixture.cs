using System.IO;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;

namespace WiloNutrientSystem.UiTests.Infrastructure;

/// <summary>
/// Launches the WPF application once for a whole test collection and exposes the
/// main window and automation session. Also captures screenshots for the report.
/// </summary>
public sealed class ApplicationFixture : IDisposable
{
    public Application App { get; }
    public UIA3Automation Automation { get; }
    public Window MainWindow { get; }
    public string ArtifactDirectory { get; }

    public ApplicationFixture()
    {
        Automation = new UIA3Automation();
        var exePath = ResolveApplicationPath();
        App = Application.Launch(exePath);

        MainWindow = Retry(() => App.GetMainWindow(Automation), TimeSpan.FromSeconds(20))
            ?? throw new InvalidOperationException("Main window did not appear.");

        ArtifactDirectory = Path.Combine(AppContext.BaseDirectory, "ui-artifacts");
        Directory.CreateDirectory(ArtifactDirectory);
    }

    /// <summary>Saves a PNG screenshot of the whole desktop under the artifact dir.</summary>
    public string Capture(string name)
    {
        var path = Path.Combine(ArtifactDirectory, $"{name}.png");
        FlaUI.Core.Capturing.Capture.Screen().ToFile(path);
        return path;
    }

    private static string ResolveApplicationPath()
    {
        // .../tests/WiloNutrientSystem.UiTests/bin/<config>/net8.0-windows
        var testDir = AppContext.BaseDirectory;
        var config = testDir.Contains($"{Path.DirectorySeparatorChar}Release{Path.DirectorySeparatorChar}",
            StringComparison.OrdinalIgnoreCase) ? "Release" : "Debug";

        var solutionRoot = new DirectoryInfo(testDir).Parent!.Parent!.Parent!.Parent!.Parent!.FullName;
        var exe = Path.Combine(solutionRoot, "src", "WiloNutrientSystem.App", "bin", config,
            "net8.0-windows", "WiloNutrientSystem.App.exe");

        if (!File.Exists(exe))
        {
            throw new FileNotFoundException($"Application executable not found: {exe}");
        }

        return exe;
    }

    private static T? Retry<T>(Func<T?> action, TimeSpan timeout) where T : class
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            var result = action();
            if (result is not null)
            {
                return result;
            }

            Thread.Sleep(300);
        }

        return null;
    }

    public void Dispose()
    {
        try
        {
            App.Close();
        }
        catch
        {
            // best effort
        }

        Automation.Dispose();
    }
}

[CollectionDefinition("App collection")]
public sealed class AppCollection : ICollectionFixture<ApplicationFixture>;
