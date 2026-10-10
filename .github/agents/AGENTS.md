# Agent Harness — Index & Orchestration

이 저장소는 5개 역할 Agent로 기능을 완성한다. 모든 Agent는 `.github/CAVEMAN.md`를 따른다.

## Pipeline

```
Planner ──▶ Architect ──▶ Developer ──▶ Reviewer ──▶ QA
  요구      설계/인터페이스   구현/단위테스트   코드리뷰      UI/통합 검증
   │           │              │              │            │
   ▼           ▼              ▼              ▼            ▼
plan-*.md   design-*.md    impl-*.md      review-*.md   qa-*.md
```

- 각 Agent는 **이전 단계의 handoff 파일만** 입력으로 읽는다(대화 히스토리 X).
- 통과 못 하면 이전 Agent로 되돌린다(Reviewer 실패 → Developer, QA 실패 → Developer).

## Agents

| Stage | Agent | 지침 파일 | Chatmode |
|-------|-------|-----------|----------|
| 1 | Planner | `planner.agent.md` | `../chatmodes/planner.chatmode.md` |
| 2 | Architect | `architect.agent.md` | `../chatmodes/architect.chatmode.md` |
| 3 | Developer | `developer.agent.md` | `../chatmodes/developer.chatmode.md` |
| 4 | Reviewer | `reviewer.agent.md` | `../chatmodes/reviewer.chatmode.md` |
| 5 | QA | `qa.agent.md` | `../chatmodes/qa.chatmode.md` |

## 사용법 (VS Code)

1. Chat 뷰에서 모드 선택기를 열어 해당 단계 chatmode를 고른다.
2. 작업 지시 + 이전 handoff 경로를 준다.
3. Agent가 산출물 handoff를 `docs/handoffs/`에 남기면 다음 단계로 넘어간다.

## 공통 규칙

- Context Budget(`CAVEMAN.md` §1) 초과 금지.
- Hard rules(`copilot-instructions.md` §3) 위반 금지.
- 새 파일/템플릿은 `_TEMPLATE.agent.md`를 복제.
