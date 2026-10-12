using VideoRecovery.Core.Abstractions.Analysis;
using VideoRecovery.Core.Enums;
using VideoRecovery.Core.Models;

namespace VideoRecovery.Analysis;

/// <summary>
/// 손상 유형 분류기. 건강 리포트의 메타데이터/디코드/프레임 신호를 손상 유형
/// 플래그로 변환한다. 하나의 영상에 여러 유형이 동시에 존재할 수 있다.
/// </summary>
public sealed class CorruptionDetector : ICorruptionDetector
{
    public CorruptionReport Classify(HealthReport health)
    {
        var findings = new List<CorruptionFinding>();
        var types = CorruptionType.None;

        // 1) ffprobe가 파일을 열지 못함 → 컨테이너/인덱스/메타데이터 손상.
        if (health.Metadata is null || health.Metadata.ProbeFailed)
        {
            types |= CorruptionType.ContainerCorruption | CorruptionType.MetadataCorruption;
            findings.Add(new CorruptionFinding
            {
                Type = CorruptionType.ContainerCorruption,
                Confidence = 0.8,
                Detail = health.Metadata?.ProbeError ?? "메타데이터를 읽을 수 없음"
            });
        }
        else
        {
            var meta = health.Metadata;

            // 비디오 스트림이 없으면 스트림/인덱스 손상 의심.
            if (meta.PrimaryVideo is null)
            {
                types |= CorruptionType.IndexCorruption;
                findings.Add(new CorruptionFinding
                {
                    Type = CorruptionType.IndexCorruption,
                    Confidence = 0.6,
                    Detail = "비디오 스트림을 찾을 수 없음"
                });
            }

            // Duration이 0인데 파일 크기가 있으면 타임스탬프/인덱스 손상 의심.
            if (meta.Duration <= TimeSpan.Zero)
            {
                types |= CorruptionType.TimestampCorruption;
                findings.Add(new CorruptionFinding
                {
                    Type = CorruptionType.TimestampCorruption,
                    Confidence = 0.5,
                    Detail = "Duration이 비정상(0)"
                });
            }
        }

        // 2) 디코드 오류 구간 → DecodeError + CorruptedFrame.
        foreach (var seg in health.DecodeSegments.Where(s => s.IsError))
        {
            types |= CorruptionType.DecodeError | CorruptionType.CorruptedFrame;
            findings.Add(new CorruptionFinding
            {
                Type = CorruptionType.DecodeError,
                StartFrame = seg.StartFrame,
                EndFrame = seg.EndFrame,
                Confidence = 0.85,
                Detail = seg.Detail ?? "디코드 오류 구간"
            });
        }

        // 3) 프레임 이상(급변/중복 등) → CorruptedFrame.
        if (health.FrameAnalysis is not null)
        {
            var anomalyCount = health.FrameAnalysis.Anomalies.Count();
            if (anomalyCount > 0)
            {
                types |= CorruptionType.CorruptedFrame;
            }
        }

        // 4) A/V 동기 오차가 크면 AudioVideoSyncError.
        if (health.AvSyncOffsetSeconds is { } off && Math.Abs(off) > 0.5)
        {
            types |= CorruptionType.AudioVideoSyncError;
            findings.Add(new CorruptionFinding
            {
                Type = CorruptionType.AudioVideoSyncError,
                Confidence = 0.6,
                Detail = $"A/V 동기 오차 {off:0.00}s"
            });
        }

        var severity = DetermineSeverity(types, health);

        return new CorruptionReport
        {
            FullPath = health.FullPath,
            Types = types,
            Findings = findings,
            Severity = severity
        };
    }

    private static HealthSeverity DetermineSeverity(CorruptionType types, HealthReport health)
    {
        if (types == CorruptionType.None)
        {
            return HealthSeverity.Healthy;
        }

        if (types.HasFlag(CorruptionType.ContainerCorruption)
            && (health.Metadata is null || health.Metadata.ProbeFailed))
        {
            return HealthSeverity.Severe;
        }

        var errorFrames = health.ErrorFrameCount;
        var total = health.Metadata?.FrameCount ?? 0;

        if (total > 0)
        {
            var ratio = (double)errorFrames / total;
            if (ratio > 0.2)
            {
                return HealthSeverity.Severe;
            }

            if (ratio > 0.05)
            {
                return HealthSeverity.Moderate;
            }
        }

        return errorFrames > 0 ? HealthSeverity.Moderate : HealthSeverity.Minor;
    }
}
