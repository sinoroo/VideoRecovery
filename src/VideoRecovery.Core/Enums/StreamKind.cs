namespace VideoRecovery.Core.Enums;

/// <summary>미디어 스트림 종류.</summary>
public enum StreamKind
{
    /// <summary>알 수 없음.</summary>
    Unknown = 0,

    /// <summary>비디오 스트림.</summary>
    Video = 1,

    /// <summary>오디오 스트림.</summary>
    Audio = 2,

    /// <summary>자막 스트림.</summary>
    Subtitle = 3,

    /// <summary>데이터 스트림.</summary>
    Data = 4
}
