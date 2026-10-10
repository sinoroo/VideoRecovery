using Microsoft.Extensions.Logging;
using VideoRecovery.Core.Abstractions.Infrastructure;

namespace VideoRecovery.Infrastructure.FileSystem;

/// <summary>
/// <see cref="IFileSystemService"/> 의 기본 구현. 원본은 읽기 전용으로만 다룬다. (요구사항 17-1)
/// </summary>
public sealed class FileSystemService : IFileSystemService
{
    private static readonly string[] SupportedExtensions =
        [".mp4", ".mov", ".avi", ".mkv", ".mts", ".m2ts"];

    private readonly ILogger<FileSystemService> _logger;

    public FileSystemService(ILogger<FileSystemService> logger) => _logger = logger;

    /// <inheritdoc />
    public IEnumerable<string> EnumerateVideoFiles(string root, bool recursive = true)
    {
        if (!Directory.Exists(root))
        {
            _logger.LogWarning("Scan root does not exist: {Root}", root);
            yield break;
        }

        var option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        foreach (var path in Directory.EnumerateFiles(root, "*.*", option))
        {
            var ext = Path.GetExtension(path);
            if (SupportedExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
            {
                yield return path;
            }
        }
    }

    /// <inheritdoc />
    public bool FileExists(string path) => File.Exists(path);

    /// <inheritdoc />
    public async Task CopyAsync(string source, string destination, CancellationToken cancellationToken = default)
    {
        EnsureDirectory(Path.GetDirectoryName(destination) ?? ".");

        await using var src = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read);
        await using var dst = new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.None);
        await src.CopyToAsync(dst, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Copied {Source} -> {Destination}", source, destination);
    }

    /// <inheritdoc />
    public void EnsureDirectory(string path) => Directory.CreateDirectory(path);
}
