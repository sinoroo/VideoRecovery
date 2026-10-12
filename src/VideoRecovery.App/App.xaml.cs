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
/// 비즈니스 로직은 여기에 두지 않는다.
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

        Log.Information("VideoRecovery 시작");

        _services = ConfigureServices();

        DispatcherUnhandledException += (_, args) =>
        {
            Log.Error(args.Exception, "처리되지 않은 UI 예외");
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

        // Infrastructure — FFmpeg/FFprobe 설정 및 파일 시스템.
        var ffmpegOptions = FFmpegLocator.Resolve(FFmpegConfigLoader.Load(AppPaths.FFmpegConfigPath));
        Log.Information("FFmpeg 경로: {FFmpeg}", ffmpegOptions.FFmpegPath);
        Log.Information("FFprobe 경로: {FFprobe}", ffmpegOptions.FFprobePath);
        services.AddSingleton(ffmpegOptions);
        services.AddSingleton<IFFprobeService, FFprobeService>();
        services.AddSingleton<IFFmpegService, FFmpegService>();
        services.AddSingleton<IFileSystemService, FileSystemService>();
        services.AddSingleton<IWorkspaceManager>(_ => new WorkspaceManager());

        // Scanner.
        services.AddSingleton<IVideoStorageScanner, VideoStorageScanner>();

        // Analysis.
        services.AddSingleton<IVideoAnalyzer, VideoAnalyzer>();
        services.AddSingleton<IFrameAnalyzer, FrameAnalyzer>();
        services.AddSingleton<IAudioAnalyzer, AudioAnalyzer>();
        services.AddSingleton<ICorruptionDetector, CorruptionDetector>();
        services.AddSingleton<IHealthAnalyzer, HealthAnalyzer>();

        // Cross-match.
        services.AddSingleton<ISimilarityEngine, SimilarityEngine>();
        services.AddSingleton<IFeatureMatcher, FeatureMatcher>();
        services.AddSingleton<IVideoMatcher, VideoMatcher>();
        services.AddSingleton<IFrameMatcher, FrameMatcher>();

        // Recovery.
        services.AddSingleton<IRecoveryPlanner, RecoveryPlanner>();
        services.AddSingleton<IContainerRecovery, ContainerRecovery>();
        services.AddSingleton<IFrameRecovery, FrameRecovery>();
        services.AddSingleton<ICrossVideoRecovery, CrossVideoRecovery>();
        services.AddSingleton<IRecoveryValidator, RecoveryValidator>();

        // AI (추상화만 — Phase 5에서 실제 모델 연결).
        services.AddSingleton<IAIInferenceEngine, NullAIInferenceEngine>();
        services.AddSingleton<IFrameInterpolation, FrameInterpolation>();
        services.AddSingleton<ISuperResolution, SuperResolution>();
        services.AddSingleton<IArtifactRemoval, ArtifactRemoval>();

        // App services.
        services.AddSingleton<IFolderPickerService, FolderPickerService>();
        services.AddSingleton<IDiagnosisPipeline, DiagnosisPipeline>();

        // ViewModels + Views.
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();

        return services.BuildServiceProvider();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("VideoRecovery 종료");
        _services?.Dispose();
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}
