# VideoRecovery

AI 기반 영상 복구 프로그램. 오래되거나 손상된 영상 파일을 **분석 → 손상 탐지 → 다른 정상 영상과 비교 → 복구 전략 결정 → 복구 → 검증**까지 수행하는 것을 목표로 한다.

> 현재 구현 수준: **Phase 2(기본 복구)** 동작. 진단(Phase 1)에 더해 Container/Index/Timestamp 복구와 Frame 복구(손상 패킷 제거 후 재인코딩)가 실제로 수행되며, `IRecoveryExecutor`가 계획에 따라 가장 안전한 레벨부터 복구를 시도하고 결과를 검증한다. Cross-Video(Level 3) 복구와 AI(Level 4)는 인터페이스/스텁으로 분리되어 있으며, 각 스텁의 예외 메시지에 담당 Phase가 명시되어 있다.

## 기술 스택

.NET 8 · C# WPF · MaterialDesignThemes · CommunityToolkit.Mvvm · Microsoft.Extensions.DependencyInjection · Serilog · FFmpeg/FFprobe(CLI) · System.Text.Json

## 솔루션 구조 (요구사항 13장)

```
VideoRecovery/
├─ src/
│  ├─ VideoRecovery.Core            # Models, Enums, Abstractions(모든 인터페이스)
│  ├─ VideoRecovery.Infrastructure  # FFmpeg/FFprobe(ProcessRunner), FileSystem, Workspace
│  ├─ VideoRecovery.Scanner         # VideoStorageScanner
│  ├─ VideoRecovery.Analysis        # Video/Frame/Audio Analyzer, CorruptionDetector, HealthAnalyzer
│  ├─ VideoRecovery.CrossMatch      # VideoMatcher, FrameMatcher, FeatureMatcher, SimilarityEngine
│  ├─ VideoRecovery.Recovery        # RecoveryPlanner, Container/Frame/CrossVideo Recovery, Validator
│  ├─ VideoRecovery.AI              # AIInferenceEngine(Null), Interpolation/SuperRes/ArtifactRemoval
│  └─ VideoRecovery.App             # WPF UI, DI 부트스트랩, DiagnosisPipeline(MVP 오케스트레이터)
└─ VideoRecovery.sln
```

의존 방향: `App → (Infrastructure, Scanner, Analysis, CrossMatch, Recovery, AI) → Core`. 역참조 금지. 외부 라이브러리(FFmpeg)는 **Infrastructure 계층에서만** 호출되고 상위는 `Core.Abstractions`의 인터페이스에만 의존한다.

## 핵심 설계 원칙 반영

- **원본 보존**: `IWorkspaceManager`가 격리된 세션 폴더를 만들고 모든 산출물을 그곳에 생성. `IFileSystemService`는 읽기 전용 열거만 제공.
- **자동화 우선**: `IDiagnosisPipeline`이 스캔→분석→분류→매칭→계획을 자동 수행. `IRecoveryPlanner`가 손상 유형별 전략을 자동 결정.
- **라이브러리 활용**: 동일 폴더의 정상 영상 메타데이터를 reference 라이브러리로 구성하여 참조 후보를 추천.
- **신뢰도 제공**: `RecoveryPlan`, `ReferenceCandidate`, `CrossVideoMatch`, `RecoveryResult`에 Confidence Score.
- **단계 분리**: 각 복구 레벨은 `IRecoveryStrategy`를 구현해 독립 교체·재시도 가능. AI는 별도 프로젝트로 분리.

## 빌드 & 실행

```powershell
dotnet build VideoRecovery.sln
dotnet run --project src/VideoRecovery.App/VideoRecovery.App.csproj
```

### FFmpeg 준비

분석은 `ffmpeg`/`ffprobe` CLI에 의존한다. PATH에 없으면
`src/VideoRecovery.App/config/ffmpeg_config.ini`에 전체 경로를 지정한다.

```ini
[FFmpeg]
FFmpegPath=C:\ffmpeg\bin\ffmpeg.exe
FFprobePath=C:\ffmpeg\bin\ffprobe.exe
TimeoutMinutes=30
```

> ffmpeg가 없어도 앱은 실행되며, 각 파일은 "컨테이너를 열 수 없음(심각)"으로 안전하게 보고된다.

## MVP 사용 흐름 (Phase 1)

```
[폴더 선택] → [분석 시작]
      ↓
저장소 스캔 → 메타데이터(ffprobe) → 디코드 테스트(ffmpeg) → 손상 구간 탐지
      ↓
손상 유형 분류 → 정상 영상 reference 추천 → 복구 가능성 표시
```

결과 그리드: 파일명 · 크기 · 심각도 · 손상 유형 · 복구 가능성 · 추천 방법 · Reference. 행 선택 시 상세 패널 표시.

## 개발 단계 로드맵 (요구사항 14장)

| Phase | 내용 | 상태 |
|-------|------|------|
| 1 | 분석/진단 (스캔·메타데이터·디코드 테스트·손상 구간) | ✅ 동작 |
| 2 | 기본 복구 (Container/Index/Timestamp, 프레임 제거/재인코딩) + Executor/Validator | ✅ 동작 |
| 3 | Cross-Video 분석 (유사도/시간축/contamination) | 🟡 엔진 구현, 통합 확장 예정 |
| 4 | Cross-Video 복구 | ⬜ 스텁(`NotImplementedException`) |
| 5 | AI 복구 (보간/초해상/디노이즈) | ⬜ 추상화만 |

## 테스트

```powershell
dotnet test tests/VideoRecovery.UnitTests/VideoRecovery.UnitTests.csproj   # xUnit + Moq + FluentAssertions
dotnet test tests/VideoRecovery.UiTests/VideoRecovery.UiTests.csproj        # FlaUI(UIA3), 데스크톱 세션 필요
```

- **Unit**: `RecoveryPlanner`, `RecoveryExecutor`, `FrameRecovery`, `RecoveryValidator`, `MainViewModel`, `DiagnosisRowViewModel`.
- **UI(FlaUI)**: 메인 윈도우 로드, 핵심 컨트롤 존재(AutomationId), 폴더 입력 시 분석 버튼 활성화, 복구 버튼 초기 비활성. 스크린샷은 `ui-artifacts/`에 저장.

## Harness Agent ↔ 인터페이스 매핑 (요구사항 16장)

| Agent | 인터페이스 / 구현 |
|-------|-------------------|
| Scanner Agent | `IVideoStorageScanner` → `VideoStorageScanner` |
| Video Analysis Agent | `IVideoAnalyzer` → `VideoAnalyzer` |
| Corruption Detection Agent | `ICorruptionDetector` → `CorruptionDetector` |
| Cross Video Analysis Agent | `IVideoMatcher`/`IFrameMatcher` → `VideoMatcher`/`FrameMatcher` |
| Recovery Planning Agent | `IRecoveryPlanner` → `RecoveryPlanner` |
| Recovery Agent | `IRecoveryStrategy`(`IContainerRecovery`/`IFrameRecovery`/`ICrossVideoRecovery`) |
| Validation Agent | `IRecoveryValidator` → `RecoveryValidator` |
| AI Recovery Agent | `IAIInferenceEngine` 외 `VideoRecovery.AI` |

## 다음 단계

1. **Phase 3 통합**: `FrameMatcher` 결과를 `DiagnosisPipeline`에 연결하여 `CrossVideoContamination` 자동 태깅.
2. **Phase 4 Cross-Video Recovery**: `CrossVideoRecovery.RecoverAsync` — reference 영상에서 매칭 구간 프레임을 추출하여 손상 구간에 삽입.
3. **OpenCV 연동**: `FrameAnalyzer`/`FeatureMatcher`에 SSIM·히스토그램·feature 비교 추가.
