# Definition of Done (DoD)

기능이 "완료"로 간주되려면 아래 **전부** 충족해야 한다.

## 기능
- [ ] Material Design 적용.
- [ ] MVVM 구조 준수(View 무로직, ViewModel 인터페이스 의존).
- [ ] Serial Open/Close/Reconnect/Send/Receive 동작.
- [ ] MQTT Publish/Subscribe 동작.
- [ ] 설정 파일(`serial_config.ini`, `mqtt_config.ini`) 로딩.
- [ ] 전체화면 고정(Maximized/None/NoResize).

## 품질
- [ ] `dotnet build` 0 error.
- [ ] 모든 Unit 테스트 통과(Model 90%+, ViewModel 80%+).
- [ ] 모든 FlaUI 테스트 통과.
- [ ] 주요 이벤트 Serilog 로깅.
- [ ] 신규 UI 요소 `AutomationId` 존재.
- [ ] 하드코딩 설정 없음.

## 프로세스
- [ ] Reviewer Agent APPROVED.
- [ ] QA Agent PASS(스크린샷/리포트 포함).
- [ ] handoff 산출물(`docs/handoffs/`) 완비.
- [ ] Caveman 컨텍스트 규칙 준수.
