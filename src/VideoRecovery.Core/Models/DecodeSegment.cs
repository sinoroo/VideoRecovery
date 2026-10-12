namespace VideoRecovery.Core.Models;

/// <summary>
/// 디코드 테스트 결과의 연속 구간. 정상/오류 구간을 프레임 범위로 표현한다.
/// 예) Frame 001523 ~ 001531 : Decode Error.
/// </summary>
public sealed class DecodeSegment
{
    /// <summary>구간 시작 프레임(포함).</summary>
    public long StartFrame { get; init; }

    /// <summary>구간 끝 프레임(포함).</summary>
    public long EndFrame { get; init; }

    /// <summary>이 구간이 디코드 오류인지 여부.</summary>
    public bool IsError { get; init; }

    /// <summary>오류 메시지(있는 경우).</summary>
    public string? Detail { get; init; }

    /// <summary>구간 길이(프레임 수).</summary>
    public long FrameLength => EndFrame - StartFrame + 1;

    public override string ToString()
        => $"Frame {StartFrame:000000} ~ {EndFrame:000000} : {(IsError ? "Decode Error" : "정상")}";
}
