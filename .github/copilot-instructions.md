# Copilot Instructions — WiloNutrientSystem

산업용 WPF Desktop Application. 이 파일은 저장소 전역 규칙이다. 모든 Copilot/Agent 작업은 이 규칙을 따른다.

## 1. Product

- .NET 8 (LTS), C# WPF, Material Design In XAML Toolkit.
- MVVM + DI(`Microsoft.Extensions.DependencyInjection`) + Serilog.
- 기능: Serial 통신, MQTT Pub/Sub, INI 설정 파일, 전체화면 고정 UI.

## 2. Layer / Project map

| Layer | Project | 책임 |
|-------|---------|------|
| Presentation | `src/WiloNutrientSystem.App` | Views(XAML), ViewModels, DI 부트스트랩 |
| Business | `src/WiloNutrientSystem.Core` | Services 인터페이스, Managers, 파서, 이벤트 |
| Infrastructure | `src/WiloNutrientSystem.Infrastructure` | SerialPort, MQTTnet, INI FileLoader 구현 |
| Domain | `src/WiloNutrientSystem.Domain` | Model / Enum |
| Test | `tests/WiloNutrientSystem.UnitTests` | xUnit + Moq + FluentAssertions |
| Test | `tests/WiloNutrientSystem.UiTests` | FlaUI(UIA3) UI 자동화 |

의존 방향: `App → Infrastructure → Core → Domain`. 역방향 참조 금지.

## 3. Hard rules (절대 규칙)

- View(XAML/code-behind)에 비즈니스 로직/Serial/MQTT 코드 금지. code-behind는 `InitializeComponent`와 순수 뷰 배선만.
- ViewModel은 인터페이스(`ISerialDeviceService`, `IMqttService`, `IConfigFileLoader`, `ICommunicationManager`)에만 의존. 구현 클래스 직접 참조 금지.
- 새 서비스는 반드시 `Core/Abstractions`에 인터페이스 → `Infrastructure`에 구현 → `App/App.xaml.cs`에 DI 등록.
- MainWindow는 `WindowState=Maximized`, `WindowStyle=None`, `ResizeMode=NoResize` 유지.
- 모든 상호작용 UI 요소에 `AutomationProperties.AutomationId` 부여(FlaUI 대상).
- 주요 이벤트(App Start/Exit, Serial/MQTT Connect/Disconnect, Publish/Subscribe, Error)는 Serilog로 로깅.
- 설정은 코드 하드코딩 금지. `serial_config.ini`, `mqtt_config.ini`에서 로드.

## 4. Definition of Done

`docs/DoD.md`를 따른다. 요약: Material 적용 + MVVM 준수 + Serial/MQTT 동작 + 설정 로딩 + 전체화면 + 모든 Unit/FlaUI 테스트 통과 + Reviewer/QA 승인.

## 5. Multi-Agent harness

역할별 지침은 `.github/agents/*.agent.md`, 대화 모드는 `.github/chatmodes/*.chatmode.md`.
컨텍스트 최소화 규칙은 `.github/CAVEMAN.md`를 **항상** 준수한다.

- Planner → Architect → Developer → Reviewer → QA 순서로 작업이 흐른다.
- Agent 간 인수인계는 대화 내용이 아니라 `docs/handoffs/`의 산출물 파일로 한다.

## 6. Build & Test 명령

```powershell
dotnet build WiloNutrientSystem.sln
dotnet test tests/WiloNutrientSystem.UnitTests/WiloNutrientSystem.UnitTests.csproj
dotnet test tests/WiloNutrientSystem.UiTests/WiloNutrientSystem.UiTests.csproj   # 데스크톱 세션 필요
```

## 7. Coding conventions

- Nullable enable, ImplicitUsings enable, file-scoped namespace.
- CommunityToolkit.Mvvm: `[ObservableProperty]`, `[RelayCommand]`, `partial class`.
- 공개 타입/멤버에 XML 요약 주석.
- `.editorconfig`를 따른다.
