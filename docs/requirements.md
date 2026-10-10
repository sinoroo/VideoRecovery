# 요구사항 — WiloNutrientSystem

산업용 WPF Desktop Application. Serial/MQTT 통신, 설정 파일 기반, 전체화면 고정, Material Design, MVVM, 자동화/단위 테스트.

## 1. 기술 스택
- .NET 8 (LTS), C# WPF.
- UI: MaterialDesignThemes, MaterialDesignColors.
- 아키텍처: MVVM (View → ViewModel → Service → Model).
- DI: Microsoft.Extensions.DependencyInjection.
- 로깅: Serilog.
- 테스트: xUnit(+Moq, FluentAssertions), FlaUI(UIA3).

## 2. UI 요구
- 전체화면: `WindowState=Maximized`, `WindowStyle=None`, `ResizeMode=NoResize`.
- 창 크기 변경 불가, 테두리/타이틀바 제거.
- 모든 상호작용 요소에 `AutomationProperties.AutomationId`.

## 3. Serial
- 기능: Open / Close / Reconnect / Send / Receive.
- 설정 파일: `serial_config.ini` (`[Serial]` PortName, BaudRate, DataBits, Parity, StopBits).

## 4. MQTT
- Publish: 장치 데이터 → `Factory/Pump/Data`.
- Subscribe: 커맨드 수신 ← `Factory/Pump/Cmd`.
- 라이브러리: MQTTnet.
- 설정 파일: `mqtt_config.ini` (`[MQTT]` Broker, Port, ClientId, UserName, Password).

## 5. 레이어
- Presentation: Views, ViewModels.
- Business: Services, Managers.
- Infrastructure: Serial, MQTT, FileLoader.
- Domain: Models.

## 6. 로깅 대상
App Start/Exit, Serial Connect/Disconnect, MQTT Connect/Disconnect, Publish, Subscribe, Error.

## 7. 테스트
- Unit: Model(90%+), ViewModel(80%+).
- UI(FlaUI): Main Window 로드, Navigation, Dialog 열기/닫기, Settings 저장/읽기, Serial/MQTT 연결/해제.
- QA: 실행 → 시나리오 → 스크린샷 → 실패 로그 분석 → 리포트.

## 8. AI Agent 역할
Planner(요구/분해/일정), Architect(설계/구조/패턴), Developer(구현), Reviewer(코드/보안/성능), QA(테스트/리포트).
상세: `.github/agents/`.
