---
applyTo: 'tests/**/*.cs'
---

# 테스트 규칙

- 프레임워크: xUnit + Moq + FluentAssertions.
- 명명: `Method_Scenario_ExpectedResult`.
- Arrange/Act/Assert 구분.
- 외부 의존(Serial/MQTT/파일)은 Moq 또는 임시 디렉터리로 격리.

## Unit (`WiloNutrientSystem.UnitTests`)
- 대상: Model(90%+), ViewModel(80%+), 파서, 매니저, INI 로더.
- ViewModel 테스트는 `Application.Current`가 null인 환경에서 동작해야 함(동기 실행 경로).
- 파일 IO 테스트는 `Path.GetTempPath()` 하위 임시 폴더 사용 후 정리.

## UI (`WiloNutrientSystem.UiTests`, FlaUI/UIA3)
- `AutomationId`로만 요소를 찾는다(텍스트/좌표 금지).
- `ApplicationFixture`(collection fixture)로 앱 1회 실행 공유.
- 실패/주요 단계에서 `fixture.Capture(name)`으로 스크린샷 저장(`ui-artifacts/`).
- 데스크톱 세션 필요. CI에서는 UI 러너/에이전트에서만 실행.
