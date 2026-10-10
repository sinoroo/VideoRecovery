# 아키텍처

## 레이어 & 의존 방향

```
┌───────────────────────────────────────────────┐
│ Presentation  (WiloNutrientSystem.App)         │
│  Views(XAML) ── DataTemplate ── ViewModels     │
│  App.xaml.cs = DI + Serilog 부트스트랩          │
└───────────────┬───────────────────────────────┘
                │ 인터페이스 의존
┌───────────────▼───────────────────────────────┐
│ Infrastructure (…Infrastructure)               │
│  SerialDeviceService / MqttService /           │
│  IniConfigFileLoader                            │
└───────────────┬───────────────────────────────┘
                │ 구현
┌───────────────▼───────────────────────────────┐
│ Business (…Core)                               │
│  Abstractions(인터페이스) / Managers / Parser  │
└───────────────┬───────────────────────────────┘
                │
┌───────────────▼───────────────────────────────┐
│ Domain (…Domain)  Models / Enums               │
└────────────────────────────────────────────────┘
```

의존 방향: `App → Infrastructure → Core → Domain` (역참조 금지).

## 데이터 흐름

```
[Serial 장치] --RX line--> SerialDeviceService
    --DataReceived--> CommunicationManager
        --Parse--> DeviceDataParser --> DeviceData
            --DeviceDataUpdated--> DashboardViewModel (UI)
            --Publish--> MqttService --> [Broker: Factory/Pump/Data]

[Broker: Factory/Pump/Cmd] --> MqttService --CommandReceived-->
    CommunicationManager --Send--> SerialDeviceService --> [Serial 장치]
```

## DI 등록 (App.xaml.cs)
- `IConfigFileLoader` → `IniConfigFileLoader`
- `ISerialDeviceService` → `SerialDeviceService`
- `IMqttService` → `MqttService`
- `IDeviceDataParser` → `DeviceDataParser`
- `ICommunicationManager` → `CommunicationManager`
- `INavigationService`, `IDialogService`
- ViewModels + `MainWindow`

## 화면
- MainWindow: 좌측 네비게이션 레일 + 우측 `ContentControl`(CurrentViewModel).
- Dashboard / Communication / Settings (UserControl).
- ViewModel↔View 매핑은 `App.xaml`의 `DataTemplate`.
