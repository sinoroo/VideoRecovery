using System.IO;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using VideoRecovery.AI;
using VideoRecovery.Analysis;
using VideoRecovery.App.Services;
using VideoRecovery.App.ViewModels;
using VideoRecovery.App.Views;
using VideoRecovery.Core.Abstractions.AI;
using VideoRecovery.Core.Abstractions.Analysis;
using VideoRecovery.Core.Abstractions.CrossMatch;
using VideoRecovery.Core.Abstractions.Infrastructure;
using VideoRecovery.Core.Abstractions.Recovery;
using VideoRecovery.Core.Abstractions.Scanner;
using VideoRecovery.CrossMatch;
using VideoRecovery.Infrastructure.FFmpeg;
using VideoRecovery.Infrastructure.FileSystem;
using VideoRecovery.Recovery;
using VideoRecovery.Scanner;

namespace VideoRecovery.App;

/// <summary>
/// 애플리케이션 진입점. DI 컨테이너 구성, Serilog 설정, 셸 윈도우 표시만 담당한다.
/// 비즈니스 로직은 포함하지 않는다.
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _services;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Directory.CreateDirectory(AppPaths.LogDirectory);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File(AppPaths.LogFilePath, rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Log.Information("VideoRecovery Application Start");

        _services = ConfigureServices();

        DispatcherUnhandledException += (_, args) =>
        {
            Log.Error(args.Exception, "Unhandled UI exception");
            args.Handled = true;
        };

        var window = _services.GetRequiredService<MainWindow>();
        window.Show();
    }

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddLogging(builder =>
            builder.AddProvider(new SerilogLoggerProvider(Log.Logger, dispose: false)));

        // 설정 로드 (하드코딩 금지, 요구사항 2.2)
        var ffmpegOptions = FFmpegConfigLoader.Load(AppPaths.FFmpegConfigPath);
        services.AddSingleton(ffmpegOptions);

        // Infrastructure
        services.AddSingleton<IFileSystemService, FileSystemService>();
        services.AddSingleton<IFFprobeService, FFprobeService>();
        services.AddSingleton<IFFmpegService, FFmpegService>();
        services.AddSingleton<IWorkspaceManager>(sp =>
            new WorkspaceManager(
                AppPaths.WorkspaceDirectory,
                sp.GetRequiredService<ILogger<WorkspaceManager>>()));

        // Scanner
        services.AddSingleton<IVideoStorageScanner, VideoStorageScanner>();

        // Analysis
        services.AddSingleton<IVideoAnalyzer, VideoAnalyzer>();
        services.AddSingleton<IFrameAnalyzer, FrameAnalyzer>();
        services.AddSingleton<IAudioAnalyzer, AudioAnalyzer>();
        services.AddSingleton<ICorruptionDetector, CorruptionDetector>();
        services.AddSingleton<IHealthAnalyzer, HealthAnalyzer>();

        // Cross-Video
        services.AddSingleton<ISimilarityEngine, SimilarityEngine>();
        services.AddSingleton<IFeatureMatcher, FeatureMatcher>();
        services.AddSingleton<IVideoMatcher, VideoMatcher>();
        services.AddSingleton<IFrameMatcher, FrameMatcher>();

        // Recovery
        services.AddSingleton<IRecoveryPlanner, RecoveryPlanner>();
        services.AddSingleton<IContainerRecovery, ContainerRecovery>();
        services.AddSingleton<IFrameRecovery, FrameRecovery>();
        services.AddSingleton<ICrossVideoRecovery, CrossVideoRecovery>();
        services.AddSingleton<IRecoveryValidator, RecoveryValidator>();

        // AI (Phase 5 전까지 placeholder)
        services.AddSingleton<IAIInferenceEngine, NullAIInferenceEngine>();
        services.AddSingleton<IFrameInterpolation, FrameInterpolation>();
        services.AddSingleton<ISuperResolution, SuperResolution>();
        services.AddSingleton<IArtifactRemoval, ArtifactRemoval>();

        // ViewModels
        services.AddSingleton<MainViewModel>();

        // Views
        services.AddSingleton<MainWindow>();

        return services.BuildServiceProvider();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("VideoRecovery Application Exit");
        _services?.Dispose();
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}
