using VideoRecovery.Core.Models;

namespace VideoRecovery.Core.Abstractions.Infrastructure;

/// <summary>
/// 파일 시스템 접근 추상화. 원본 보존 원칙에 따라 읽기 전용 열거/조회만 제공한다.
/// </summary>
public interface IFileSystemService
{
    /// <summary>지정 폴더(및 하위)에서 영상 파일 정보를 열거한다(원본 미변경).</summary>
    IEnumerable<VideoFileInfo> EnumerateVideos(string rootDirectory, bool recursive = true);

    /// <summary>경로 존재 여부.</summary>
    bool FileExists(string path);

    /// <summary>디렉터리 존재 여부.</summary>
    bool DirectoryExists(string path);
}
