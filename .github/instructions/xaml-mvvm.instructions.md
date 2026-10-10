---
applyTo: '**/*.xaml'
---

# XAML / View 규칙

- View는 **표시만**. 비즈니스 로직/Serial/MQTT 코드 금지.
- code-behind는 `InitializeComponent()` + 순수 뷰 배선만.
- 모든 상호작용 요소(Button, TextBox, 등)에 `AutomationProperties.AutomationId` 부여(FlaUI 대상).
- Material Design 리소스/스타일 사용(`MaterialDesignRaisedButton`, `materialDesign:Card`, `HintAssist.Hint` 등).
- 데이터는 `{Binding}`으로. Command는 `[RelayCommand]` 생성 이름(`XxxCommand`)에 바인딩.
- `MainWindow`: `WindowState=Maximized`, `WindowStyle=None`, `ResizeMode=NoResize` 유지.
- ViewModel↔View 매핑은 `App.xaml`의 `DataTemplate`(DataType)로 한다.

## AutomationId 네이밍
- 네비게이션: `Nav<Page>` (예: `NavSettings`).
- 버튼: `<Action>Button` (예: `SaveButton`, `SerialConnectButton`).
- 입력: `<Field>Box` (예: `PortNameBox`).
- 상태 텍스트: `<Name>Text` (예: `SerialStateText`).
