using System.Diagnostics;

namespace VideoRecovery.Infrastructure.FFmpeg;

/// <summary>
/// ffmpeg/ffprobe 실행 파일을 탐색한다. 설정 경로가 절대 경로면 그대로 사용하고,
/// 단순 명령("ffprobe")이면 PATH → 알려진 설치 위치(winget/choco/scoop/Program Files)
/// 순으로 찾는다. 설치 직후 PATH가 아직 갱신되지 않은 세션에서도 동작하게 한다.
/// </summary>
public static class FFmpegLocator
{
    /// <summary>설정의 ffmpeg/ffprobe 경로를 실제 실행 가능한 경로로 해석한다.</summary>
    public static FFmpegOptions Resolve(FFmpegOptions options)
    {
        options.FFmpegPath = ResolveExecutable(options.FFmpegPath, "ffmpeg");
        options.FFprobePath = ResolveExecutable(options.FFprobePath, "ffprobe");
        return options;
    }

    /// <summary>단일 실행 파일 경로를 해석한다. 찾지 못하면 원래 명령명을 반환한다.</summary>
    public static string ResolveExecutable(string configured, string commandName)
    {
        // 1) 절대/상대 경로로 지정되어 실제 존재하면 그대로 사용.
        if (!string.IsNullOrWhiteSpace(configured)
            && (configured.Contains(Path.DirectorySeparatorChar)
                || configured.Contains(Path.AltDirectorySeparatorChar))
            && File.Exists(configured))
        {
            return configured;
        }

        var exe = OperatingSystem.IsWindows() ? $"{commandName}.exe" : commandName;

        // 2) PATH에서 탐색.
        var fromPath = FindOnPath(exe);
        if (fromPath is not null)
        {
            return fromPath;
        }

        // 3) 알려진 설치 위치에서 탐색.
        foreach (var dir in CandidateDirectories())
        {
            var candidate = Path.Combine(dir, exe);
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        // 4) 못 찾으면 원래 명령명 반환(ProcessRunner가 실패를 결과로 보고).
        return configured;
    }

    private static string? FindOnPath(string exe)
    {
        var path = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }

        foreach (var dir in path.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
        {
            try
            {
                var full = Path.Combine(dir.Trim('"'), exe);
                if (File.Exists(full))
                {
                    return full;
                }
            }
            catch
            {
                // 잘못된 PATH 항목은 무시.
            }
        }

        return null;
    }

    /// <summary>winget/choco/scoop/Program Files 등 알려진 설치 위치의 bin 폴더들을 열거한다.</summary>
    private static IEnumerable<string> CandidateDirectories()
    {
        var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        // winget: ...\WinGet\Packages\Gyan.FFmpeg*\ffmpeg-*\bin
        var wingetPackages = Path.Combine(local, "Microsoft", "WinGet", "Packages");
        foreach (var bin in EnumerateBinDirs(wingetPackages, "Gyan.FFmpeg*"))
        {
            yield return bin;
        }

        // chocolatey.
        yield return Path.Combine(
            Environment.GetEnvironmentVariable("ChocolateyInstall") ?? @"C:\ProgramData\chocolatey",
            "bin");

        // scoop.
        yield return Path.Combine(userProfile, "scoop", "apps", "ffmpeg", "current", "bin");
        yield return Path.Combine(userProfile, "scoop", "shims");

        // 수동 설치(일반적 위치).
        yield return Path.Combine(programFiles, "ffmpeg", "bin");
        yield return @"C:\ffmpeg\bin";
    }

    /// <summary>지정 폴더 아래에서 패턴에 맞는 패키지의 bin 폴더를 찾는다.</summary>
    private static IEnumerable<string> EnumerateBinDirs(string root, string packagePattern)
    {
        if (!Directory.Exists(root))
        {
            yield break;
        }

        IEnumerable<string> packages;
        try
        {
            packages = Directory.EnumerateDirectories(root, packagePattern);
        }
        catch
        {
            yield break;
        }

        foreach (var package in packages)
        {
            string[] bins;
            try
            {
                // ffmpeg-*-full_build\bin 형태.
                bins = Directory.GetDirectories(package, "bin", SearchOption.AllDirectories);
            }
            catch
            {
                continue;
            }

            foreach (var bin in bins)
            {
                yield return bin;
            }
        }
    }
}
