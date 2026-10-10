namespace VideoRecovery.Core.Models;

/// <summary>
/// ffprobe 로 수집한 컨테이너/스트림 메타데이터. (요구사항 3.1, 4)
/// Cross-Video 분석의 reference 특성으로도 활용된다. (요구사항 2.3)
/// </summary>
public sealed class VideoMetadata
{
    /// <summary>컨테이너 포맷 (예: mov,mp4,m4a / matroska).</summary>
    public string? Container { get; init; }

    /// <summary>비디오 코덱 (예: h264, hevc).</summary>
    public string? VideoCodec { get; init; }

    /// <summary>오디오 코덱 (예: aac).</summary>
    public string? AudioCodec { get; init; }

    /// <summary>가로 해상도(px).</summary>
    public int Width { get; init; }

    /// <summary>세로 해상도(px).</summary>
    public int Height { get; init; }

    /// <summary>초당 프레임 수.</summary>
    public double Fps { get; init; }

    /// <summary>평균 비트레이트(bps).</summary>
    public long BitrateBps { get; init; }

    /// <summary>재생 길이.</summary>
    public TimeSpan Duration { get; init; }

    /// <summary>전체 프레임 수(추정 또는 실측).</summary>
    public long FrameCount { get; init; }

    /// <summary>타임베이스 (예: 1/90000).</summary>
    public string? TimeBase { get; init; }

    /// <summary>GOP 크기(키프레임 간격). 알 수 없으면 null.</summary>
    public int? GopSize { get; init; }

    /// <summary>오디오 샘플레이트(Hz).</summary>
    public int AudioSampleRate { get; init; }

    /// <summary>오디오 채널 수.</summary>
    public int AudioChannels { get; init; }

    /// <summary>SPS(Base64). H.264/HEVC reference 매칭에 사용.</summary>
    public string? Sps { get; init; }

    /// <summary>PPS(Base64).</summary>
    public string? Pps { get; init; }
}
