---
name: 현재 스프린트 상태
description: Phase 1~3 전체 Sprint 계획 수립 완료, 구현 시작 전 상태
type: project
---

Phase 1 Sprint 1~2 계획 수립 완료 (2026-04-17).
Phase 2 Sprint 1~2 계획 수립 완료 (2026-04-17).
Phase 3 Sprint 1~2 계획 수립 완료 (2026-04-17).

- Phase 1 Sprint 1: WPF 프로젝트 구조 + 투명 윈도우 + 기본 텍스트 입력 (planned)
- Phase 1 Sprint 2: 투명도 모드 + Always on Top + 컨텍스트 메뉴 골격 (planned)
- Phase 2 Sprint 1: 텍스트 테두리 및 그림자 효과 (planned) -- Phase 1 완료 선행 조건
- Phase 2 Sprint 2: 설정 관리 + 자동 저장 + 윈도우 관리 (planned) -- Phase 2 Sprint 1 선행
- Phase 3 Sprint 1: 시스템 트레이 + 서식 지원 (planned) -- Phase 2 완료 선행 조건
- Phase 3 Sprint 2: 다크/라이트 테마 + 컨텍스트 메뉴 완성 (planned) -- Phase 3 Sprint 1 선행

**Why:** Phase 1 -> Phase 2 -> Phase 3 순서. Phase 3 Sprint 2는 Sprint 1의 TrayIconManager + 서식 메뉴를 기반으로 ThemeManager(다크/라이트 테마) + 컨텍스트 메뉴 PRD F11 완성을 추가함.
**How to apply:** Phase 3 Sprint 1까지 완료 후에만 Phase 3 Sprint 2 구현 시작. Task 2, 3에 feature-dev skill 배정 (기존 파일 3개+ 통합).
