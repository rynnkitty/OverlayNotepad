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

- [Phase 1 Sprint 1 계획](project_phase1_sprint1.md) — WPF 프로젝트 구조 + 투명 윈도우 + 텍스트 입력, 계획 완료
- [현재 스프린트 상태](project_sprint_status.md) — Phase 1 Sprint 1~2 + Phase 2 Sprint 1~2 계획 수립 완료, 구현 시작 전
- [Phase 2 Sprint 1 계획](project_phase2_sprint1.md) — 텍스트 테두리/그림자 효과, 이중 레이어 구조, 계획 완료
- [Phase 2 Sprint 2 계획](project_phase2_sprint2.md) — 설정 관리 + 자동 저장 + 윈도우 관리, 계획 완료
