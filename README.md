# VideoRecovery

AI 기반 영상 복구 프로그램. 손상된 영상 파일을 **분석 → 손상 탐지 → Cross-Video 비교 → 복구 전략 결정 → 복구 → 검증**하는 Windows 데스크톱 애플리케이션이다.

> 이 솔루션은 섹션 13의 권장 소프트웨어 구조를 기준으로 스캐폴딩되었다.
> 비즈니스 로직 대부분은 각 단계(Phase)에서 구현할 **스텁(stub)** 상태이며, 인터페이스/모델/DI 배선은 완성되어 있다.

## 설계 원칙

1. **원본 보존** — 원본 파일/HDD 는 절대 수정하지 않는다. 모든 작업은 `workspace/` 세션 디렉터리에서 수행한다. (요구사항 2.1, 17-1)
2. **자동화 우선** — 사용자가 FFmpeg/OpenCV 를 직접 고르지 않는다. 프로그램이 분석 후 전략을 추천한다. (요구사항 2.2)
3. **전체 라이브러리 활용** — 동일 폴더/HDD 의 정상 영상을 reference 로 사용한다. (요구사항 2.3)
4. **외부 라이브러리 추상화** — FFmpeg/FFprobe/OpenCV 는 Infrastructure 계층에서만 추상화한다. (요구사항 17-9)
5. **신뢰도 기반 의사결정** — 복구 방법/결과에 Confidence Score 를 제공한다. (요구사항 17-4)

## 프로젝트 구조

| 프로젝트 | 책임 |
|----------|------|
| `VideoRecovery.App` | WPF UI (MVVM, Material Design, DI 부트스트랩) |
| `VideoRecovery.Core` | Models / Enums / Abstractions(인터페이스). 의존성 없음 |
| `VideoRecovery.Scanner` | 저장소 스캔 + 기본 정보 수집 (Scanner Agent) |
| `VideoRecovery.Analysis` | 컨테이너/비디오/오디오/프레임 분석, 손상 분류 |
| `VideoRecovery.CrossMatch` | 영상 간 메타/시각/시간축 유사성 분석 |
| `VideoRecovery.Recovery` | 복구 전략 수립 + Level 1~3 복구 + 검증 |
| `VideoRecovery.AI` | AI 복원(보간/초해상/아티팩트 제거) — placeholder |
| `VideoRecovery.Infrastructure` | FFmpeg/FFprobe CLI, FileSystem, Workspace |

의존 방향: `App → (Scanner/Analysis/CrossMatch/Recovery/AI/Infrastructure) → Core`. 역방향 참조 금지.

## 에이전트 ↔ 인터페이스 매핑 (요구사항 16)

| Agent | 인터페이스 |
|-------|-----------|
| Scanner Agent | `IVideoStorageScanner` |
| Video Analysis Agent | `IVideoAnalyzer`, `IHealthAnalyzer` |
| Corruption Detection Agent | `ICorruptionDetector`, `IFrameAnalyzer` |
| Cross Video Analysis Agent | `IVideoMatcher`, `IFrameMatcher`, `IFeatureMatcher`, `ISimilarityEngine` |
| Recovery Planning Agent | `IRecoveryPlanner` |
| Recovery Agent | `IContainerRecovery`, `IFrameRecovery`, `ICrossVideoRecovery` (← `IRecoveryStrategy`) |
| Validation Agent | `IRecoveryValidator` |
| AI Recovery Agent | `IAIInferenceEngine`, `IFrameInterpolation`, `ISuperResolution`, `IArtifactRemoval` |

## 개발 단계(Phase) 로드맵

- **Phase 1 — 분석 (MVP 핵심)**: `FFprobeService.ProbeAsync`, `FFmpegService.DecodeTestAsync`, `VideoAnalyzer`, `FrameAnalyzer`, `AudioAnalyzer`, `CorruptionDetector`, `HealthAnalyzer` 구현.
- **Phase 2 — 기본 복구**: `ContainerRecovery`, `FrameRecovery`, `RecoveryPlanner`, `RecoveryValidator` + `FFmpegService.RemuxAsync/ExtractFramesAsync`.
- **Phase 3 — Cross-Video 분석**: `VideoMatcher`, `FrameMatcher`, `FeatureMatcher`, `SimilarityEngine.StructuralSimilarityAsync`.
- **Phase 4 — Cross-Video 복구**: `CrossVideoRecovery`.
- **Phase 5 — AI 복구**: `VideoRecovery.AI` 의 placeholder 를 실제 ONNX/CUDA/TensorRT 모델로 교체.

각 `NotImplementedException` 에는 해당 Phase 를 나타내는 TODO 주석이 포함되어 있다.

## 설정

실행 파일 경로는 하드코딩하지 않고 `VideoRecovery.App/config/ffmpeg_config.ini` 에서 로드한다. (요구사항 2.2)

```ini
FFmpegPath=ffmpeg
FFprobePath=ffprobe
```

> Phase 1 디코드 테스트/프로브 기능을 사용하려면 FFmpeg/FFprobe 가 PATH 에 있거나 위 경로가 유효해야 한다.

## 빌드 & 실행

```powershell
dotnet build VideoRecovery.sln
dotnet run --project src/VideoRecovery.App/VideoRecovery.App.csproj
```

현재 MVP UI: **폴더 선택 → 분석 시작 → 결과 그리드 표시**. 스캔(파일 열거)은 동작하며, 메타데이터/건강 분석은 Phase 1 구현 전까지 "미구현" 상태로 표시된다.

## 기술 스택

- .NET 8, C# WPF, Material Design In XAML Toolkit
- MVVM (CommunityToolkit.Mvvm) + DI (`Microsoft.Extensions.DependencyInjection`) + Serilog
- 영상 처리: FFmpeg / FFprobe (CLI, Infrastructure 계층 추상화)
- 프레임 처리: OpenCV (Phase 1/3 에서 연동 예정)
- AI 추론: ONNX Runtime / CUDA / TensorRT (Phase 5, 현재 추상화 계층만)
