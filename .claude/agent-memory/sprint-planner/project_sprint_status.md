---
name: 현재 스프린트 상태
description: Phase 1~4 전체 Sprint 계획 수립 완료, 구현 시작 전 상태
type: project
---

Phase 1 Sprint 1~2 계획 수립 완료 (2026-04-17).
Phase 2 Sprint 1~2 계획 수립 완료 (2026-04-17).
Phase 3 Sprint 1~2 계획 수립 완료 (2026-04-17).
Phase 4 Sprint 1~2 계획 수립 완료 (2026-04-17).

- Phase 1 Sprint 1: WPF 프로젝트 구조 + 투명 윈도우 + 기본 텍스트 입력 (planned)
- Phase 1 Sprint 2: 투명도 모드 + Always on Top + 컨텍스트 메뉴 골격 (planned)
- Phase 2 Sprint 1: 텍스트 테두리 및 그림자 효과 (planned) -- Phase 1 완료 선행 조건
- Phase 2 Sprint 2: 설정 관리 + 자동 저장 + 윈도우 관리 (planned) -- Phase 2 Sprint 1 선행
- Phase 3 Sprint 1: 시스템 트레이 + 서식 지원 (planned) -- Phase 2 완료 선행 조건
- Phase 3 Sprint 2: 다크/라이트 테마 + 컨텍스트 메뉴 완성 (planned) -- Phase 3 Sprint 1 선행
- Phase 4 Sprint 1: 글로벌 핫키 + Click-Through (planned) -- Phase 3 완료 선행 조건
- Phase 4 Sprint 2: 최종 마무리 + 배포 준비 (planned) -- Phase 4 Sprint 1 선행

**Why:** Phase 1 -> Phase 2 -> Phase 3 -> Phase 4 순서. Phase 4 Sprint 2는 전체 프로젝트의 마지막 Sprint으로 통합 테스트, 성능 최적화, DPI/아이콘/배포 설정을 완료하여 배포 가능 상태로 만듦.
**How to apply:** Phase 4 Sprint 1(글로벌 핫키 + Click-Through) 완료 후에만 Sprint 2 구현 시작. 모든 Task가 순차 실행 (MainWindow/프로젝트 파일 공유).
