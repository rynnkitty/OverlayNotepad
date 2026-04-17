# Sprint Planner 메모리

이 파일은 sprint-planner 에이전트의 영구 메모리입니다.
프로젝트 진행 상황, 기술 스택, 패턴 등을 기록합니다.

## 문서 구조 (2026-03-27 업데이트)

- 문서 경로: `docs/phase/phase{N}/sprint{N}/task{N}/`
- Sprint 번호: phase 내 로컬 번호 (phase1/sprint1, phase1/sprint2, phase2/sprint1...)
- 브랜치명: `phase{P}-sprint{N}` (예: phase1-sprint1)
- 모든 Sprint는 반드시 Phase를 경유하여 생성
- index.json: `docs/index.json` — 프로젝트 히스토리 관리
- Hotfix 문서: `docs/hotfix/{name}/hotfix.md`

## 스프린트 현황

- [Phase 1 Sprint 1 계획](project_phase1_sprint1.md) — WPF 프로젝트 구조 + 투명 윈도우 + 텍스트 입력, **구현 완료 (2026-04-17)**
- Phase 1 Sprint 2 — 투명도 모드 + Always on Top + 컨텍스트 메뉴 골격, **구현 완료 (2026-04-17)**
- [현재 스프린트 상태](project_sprint_status.md) — Phase 1~3 완료, Phase 4 Sprint 1 구현 대기
- [Phase 2 Sprint 1 계획](project_phase2_sprint1.md) — 텍스트 테두리/그림자 효과, 이중 레이어 구조, **구현 완료 (2026-04-17)**
- [Phase 2 Sprint 2 계획](project_phase2_sprint2.md) — 설정 관리 + 자동 저장 + 윈도우 관리, **구현 완료 (2026-04-17)**
- [Phase 3 Sprint 1 계획](project_phase3_sprint1.md) — 시스템 트레이 + 서식 지원 (글꼴/크기/색상), **구현 완료 (2026-04-17)**
- [Phase 3 Sprint 2 계획](project_phase3_sprint2.md) — 다크/라이트 테마 + 컨텍스트 메뉴 PRD F11 완성, **구현 완료 (2026-04-17)**
- 다음 스프린트: Phase 4 Sprint 1 (글로벌 핫키 + Click-Through) -- 구현 대기
- [Phase 4 Sprint 1 계획](project_phase4_sprint1.md) — 글로벌 핫키 + Click-Through (WS_EX_TRANSPARENT), **계획 수립 완료, 구현 대기**
- [Phase 4 Sprint 2 계획](project_phase4_sprint2.md) — 최종 마무리 + 배포 준비 (DPI/아이콘/성능/통합테스트), 계획 완료
