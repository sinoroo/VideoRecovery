using VideoRecovery.Core.Enums;

namespace VideoRecovery.Core.Models;

/// <summary>
/// 디코드 테스트로 확인한 연속 구간과 그 상태. (요구사항 4 - Decode Test)
/// </summary>
public sealed class DecodeSegment
{
    /// <summary>시작 프레임 인덱스(포함).</summary>
    public long StartFrame { get; init; }

    /// <summary>끝 프레임 인덱스(포함).</summary>
    public long EndFrame { get; init; }

    /// <summary>이 구간이 정상 디코딩되는지 여부.</summary>
    public bool IsHealthy { get; init; }

    /// <summary>비정상인 경우 감지된 손상 유형.</summary>
    public CorruptionType Corruption { get; init; }

    /// <summary>디코더가 출력한 오류 메시지(있다면).</summary>
    public string? Message { get; init; }
}
