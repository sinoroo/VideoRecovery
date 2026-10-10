---
applyTo: '**/*.cs'
---

# C# 규칙 (모든 .cs)

- file-scoped namespace, `Nullable` enable, `ImplicitUsings` enable.
- 공개 타입/멤버에 XML `<summary>` 주석.
- 비동기: `Task` 반환 + `CancellationToken` 파라미터, `ConfigureAwait` 적절 사용.
- `IDisposable` 구현 시 이벤트 구독 해제.
- 예외는 삼키지 말고 `ILogger`로 기록.

## 레이어 규칙
- `Domain`: 모델/enum만. 외부 의존 없음.
- `Core`: 인터페이스(`Abstractions`), 매니저, 파서. WPF/Infra 참조 금지.
- `Infrastructure`: `Core` 인터페이스 구현. SerialPort/MQTTnet/파일 IO.
- `App`: ViewModel/View/DI. 인터페이스에만 의존.

## MVVM (ViewModel)
- `CommunityToolkit.Mvvm` 사용: `[ObservableProperty]`, `[RelayCommand]`, `partial class`.
- 구현 클래스가 아닌 **인터페이스**에 의존.
- UI 스레드 마셜링은 `Application.Current?.Dispatcher`로, null 가드 포함(테스트 대비).
