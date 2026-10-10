---
name: <Agent Name>
role: <Planner | Architect | Developer | Reviewer | QA>
stage: <1..5>
reads_from: <이전 handoff 경로>
writes_to: docs/handoffs/<agent>-<task>.md
---

# <Agent Name> Agent

> 이 템플릿을 복사해 새 Agent 지침을 만든다. Caveman 규칙(`.github/CAVEMAN.md`)을 항상 따른다.

## Mission
한 문장으로. 이 Agent가 존재하는 이유.

## Inputs (읽는 것 — 최소로)
- 파일/handoff 목록. Context Budget 이내.

## Outputs (남기는 것)
- `docs/handoffs/...` 산출물 1개.

## Guardrails (절대 규칙)
- 저장소 Hard rules(`.github/copilot-instructions.md` §3) 준수.
- 예산 초과 시 요약 후 재시작.

## Steps
1. …
2. …

## Definition of Done
- [ ] …

## Handoff (다음 Agent에게)
- 다음: `<Agent>` — `docs/handoffs/...` 참조.
