namespace VideoRecovery.Core.Abstractions.Infrastructure;

/// <summary>
/// 파일 시스템 접근 추상화. 테스트 용이성과 원본 보존 규칙 강제를 위해 사용. (요구사항 17-1)
/// </summary>
public interface IFileSystemService
{
    /// <summary>지정한 디렉터리에서 영상 파일 경로를 열거한다.</summary>
    /// <param name="root">검색 루트 경로.</param>
    /// <param name="recursive">하위 폴더 포함 여부.</param>
    IEnumerable<string> EnumerateVideoFiles(string root, bool recursive = true);

    /// <summary>파일 존재 여부.</summary>
    bool FileExists(string path);

    /// <summary>원본을 수정하지 않고 작업 영역으로 안전하게 복사한다.</summary>
    Task CopyAsync(string source, string destination, CancellationToken cancellationToken = default);

    /// <summary>디렉터리를 생성한다(이미 있으면 무시).</summary>
    void EnsureDirectory(string path);
}
