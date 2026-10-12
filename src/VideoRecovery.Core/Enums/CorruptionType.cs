namespace VideoRecovery.Core.Enums;

/// <summary>
/// 손상 유형. 하나의 영상에 여러 유형이 동시에 존재할 수 있으므로
/// <see cref="FlagsAttribute"/>로 조합 가능하게 정의한다.
/// </summary>
[Flags]
public enum CorruptionType
{
    /// <summary>손상 없음.</summary>
    None = 0,

    /// <summary>컨테이너 구조 손상.</summary>
    ContainerCorruption = 1 << 0,

    /// <summary>인덱스(moov/idx) 손상.</summary>
    IndexCorruption = 1 << 1,

    /// <summary>메타데이터 손상.</summary>
    MetadataCorruption = 1 << 2,

    /// <summary>타임스탬프(PTS/DTS) 손상.</summary>
    TimestampCorruption = 1 << 3,

    /// <summary>프레임 누락.</summary>
    MissingFrame = 1 << 4,

    /// <summary>디코드 오류.</summary>
    DecodeError = 1 << 5,

    /// <summary>프레임 자체 손상(깨짐/블록 노이즈).</summary>
    CorruptedFrame = 1 << 6,

    /// <summary>프레임 중복.</summary>
    DuplicateFrame = 1 << 7,

    /// <summary>프레임 순서 이상.</summary>
    FrameOrderError = 1 << 8,

    /// <summary>오디오/비디오 동기화 오류.</summary>
    AudioVideoSyncError = 1 << 9,

    /// <summary>다른 영상 데이터 혼입.</summary>
    CrossVideoContamination = 1 << 10,

    /// <summary>분류되지 않은 손상.</summary>
    Unknown = 1 << 11
}
