---
name: Phase 2 계획 수립
description: Phase 2 (텍스트 테두리/그림자 + 자동 저장 + 윈도우 관리) 계획 완료. Sprint 2개 분할, 이중 레이어 텍스트 렌더링 구조 채택.
type: project
---

Phase 2 계획 수립 완료 (2026-04-17).

- Sprint 1: 텍스트 테두리/그림자 효과 (OutlinedTextControl + DropShadowEffect)
- Sprint 2: 설정 관리 + 자동 저장 + 윈도우 관리 (SettingsManager + AutoSaveManager)

**Why:** PRD P0(F3) + P1(F7, F8) 기능 구현. 투명 배경 위 가독성 확보가 핵심 목표.

**How to apply:**
- 이중 레이어 구조: TextBox(입력) + OutlinedTextControl(렌더링) 분리
- JSON 직렬화: NuGet 의존성 최소화를 위해 수동 파싱 또는 DataContractJsonSerializer
- 메모와 설정 파일 분리 (memo.txt / settings.json)
- 폴백 계획: FormattedText 문제 시 DropShadowEffect만 사용
