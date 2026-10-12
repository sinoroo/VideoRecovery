using VideoRecovery.Core.Enums;

namespace VideoRecovery.Core.Models;

/// <summary>단일 미디어 스트림(ffprobe stream)의 정보.</summary>
public sealed class StreamInfo
{
    /// <summary>스트림 인덱스.</summary>
    public int Index { get; init; }

    /// <summary>스트림 종류.</summary>
    public StreamKind Kind { get; init; }

    /// <summary>코덱 이름(예: h264, aac).</summary>
    public string Codec { get; init; } = string.Empty;

    /// <summary>가로 해상도(비디오).</summary>
    public int Width { get; init; }

    /// <summary>세로 해상도(비디오).</summary>
    public int Height { get; init; }

    /// <summary>초당 프레임 수(비디오).</summary>
    public double Fps { get; init; }

    /// <summary>샘플레이트(오디오).</summary>
    public int SampleRate { get; init; }

    /// <summary>채널 수(오디오).</summary>
    public int Channels { get; init; }

    /// <summary>타임베이스 문자열(예: 1/90000).</summary>
    public string TimeBase { get; init; } = string.Empty;

    /// <summary>비트레이트(bps).</summary>
    public long BitRate { get; init; }
}

/// <summary>
/// ffprobe로 수집한 컨테이너/스트림 메타데이터. Cross-video 분석의
/// 후보 선정(1단계)에 사용되는 핵심 특성을 포함한다.
/// </summary>
public sealed class VideoMetadata
{
    /// <summary>대상 파일 경로.</summary>
    public required string FullPath { get; init; }

    /// <summary>포맷 이름(예: mov,mp4,m4a,...).</summary>
    public string FormatName { get; init; } = string.Empty;

    /// <summary>전체 길이.</summary>
    public TimeSpan Duration { get; init; }

    /// <summary>전체 비트레이트(bps).</summary>
    public long BitRate { get; init; }

    /// <summary>추정 전체 프레임 수.</summary>
    public long FrameCount { get; init; }

    /// <summary>스트림 목록.</summary>
    public IReadOnlyList<StreamInfo> Streams { get; init; } = Array.Empty<StreamInfo>();

    /// <summary>ffprobe가 노출하는 추가 태그(카메라 모델 등).</summary>
    public IReadOnlyDictionary<string, string> Tags { get; init; }
        = new Dictionary<string, string>();

    /// <summary>ffprobe가 해당 파일을 전혀 열지 못한 경우 true.</summary>
    public bool ProbeFailed { get; init; }

    /// <summary>probe 실패 사유(있는 경우).</summary>
    public string? ProbeError { get; init; }

    /// <summary>첫 번째 비디오 스트림(없으면 null).</summary>
    public StreamInfo? PrimaryVideo
        => Streams.FirstOrDefault(s => s.Kind == StreamKind.Video);

    /// <summary>첫 번째 오디오 스트림(없으면 null).</summary>
    public StreamInfo? PrimaryAudio
        => Streams.FirstOrDefault(s => s.Kind == StreamKind.Audio);
}
