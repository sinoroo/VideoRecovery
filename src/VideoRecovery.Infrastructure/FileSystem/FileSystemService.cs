using VideoRecovery.Core.Abstractions.Infrastructure;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Infrastructure.FileSystem;

/// <summary>
/// 읽기 전용 파일 시스템 접근 구현. 원본 보존 원칙에 따라 열거/조회만 제공한다.
/// </summary>
public sealed class FileSystemService : IFileSystemService
{
    private static readonly HashSet<string> VideoExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".mp4", ".mov", ".avi", ".mkv", ".mts", ".m2ts", ".m4v", ".wmv", ".flv", ".webm", ".ts", ".mpg", ".mpeg"
    };

    public IEnumerable<VideoFileInfo> EnumerateVideos(string rootDirectory, bool recursive = true)
    {
        if (!Directory.Exists(rootDirectory))
        {
            yield break;
        }

        var option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        IEnumerable<string> files;
        try
        {
            files = Directory.EnumerateFiles(rootDirectory, "*.*", option);
        }
        catch (UnauthorizedAccessException)
        {
            yield break;
        }

        foreach (var file in files)
        {
            var ext = Path.GetExtension(file);
            if (!VideoExtensions.Contains(ext))
            {
                continue;
            }

            VideoFileInfo? info = null;
            try
            {
                var fi = new FileInfo(file);
                info = new VideoFileInfo
                {
                    FullPath = fi.FullName,
                    FileName = fi.Name,
                    SizeBytes = fi.Length,
                    CreatedAt = fi.CreationTimeUtc,
                    ModifiedAt = fi.LastWriteTimeUtc,
                    Container = ext.TrimStart('.').ToLowerInvariant()
                };
            }
            catch (IOException)
            {
                // 접근 불가 파일은 건너뛴다.
            }

            if (info is not null)
            {
                yield return info;
            }
        }
    }

    public bool FileExists(string path) => File.Exists(path);

    public bool DirectoryExists(string path) => Directory.Exists(path);
}
