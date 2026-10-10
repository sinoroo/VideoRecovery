---
mode: 'agent'
description: '새 기능을 Planner→Architect→Developer→Reviewer→QA 파이프라인으로 진행한다.'
---

# 새 기능 파이프라인

기능명: `${input:feature:기능 이름을 입력하세요}`

다음 순서로 진행하라. 각 단계는 `.github/CAVEMAN.md` 예산을 지키고, 산출물을 `docs/handoffs/`에 남긴다.

1. **Planner** (`.github/agents/planner.agent.md`)
   → `docs/handoffs/plan-${input:feature}.md`
2. **Architect** (`.github/agents/architect.agent.md`)
   → `docs/handoffs/design-${input:feature}.md`
3. **Developer** (`.github/agents/developer.agent.md`)
   → 코드 + 단위 테스트 + `docs/handoffs/impl-${input:feature}.md`
4. **Reviewer** (`.github/agents/reviewer.agent.md`)
   → `docs/handoffs/review-${input:feature}.md`
5. **QA** (`.github/agents/qa.agent.md`)
   → `docs/handoffs/qa-${input:feature}.md`

각 단계 완료 후 다음 단계로 넘어가기 전에 handoff 파일 경로를 보고하라.
반려 발생 시 해당 단계 Agent로 되돌아가라.
