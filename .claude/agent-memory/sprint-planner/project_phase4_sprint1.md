---
name: Phase 4 Sprint 1 계획
description: 글로벌 핫키 (Ctrl+Shift+N/T) + Click-Through (WS_EX_TRANSPARENT) + 시각 피드백 + 안전장치, 계획 완료
type: project
---

Phase 4 Sprint 1: 글로벌 핫키 + Click-Through (2026-04-17 계획 수립)

Task 5개, 전체 순차 실행:
- Task 1: NativeMethods P/Invoke + HotkeyService (신규 파일 2개)
- Task 2: ClickThroughService (신규 파일 1개 + AppSettings 수정)
- Task 3: Click-Through 시각 피드백 UI (MainWindow.xaml 수정)
- Task 4: MainWindow 통합 + 트레이/메뉴 연동 (feature-dev skill, 기존 파일 4개 수정)
- Task 5: 통합 검증 + 엣지 케이스 + 안전장치 확인

핵심 파라미터:
- 핫키: Ctrl+Shift+N (표시/숨김), Ctrl+Shift+T (Click-Through)
- 시각 피드백: 빨간 점선 테두리 + 상단 바 텍스트 + 트레이 아이콘 변경 (3중 신호)
- 안전장치: 투명도 하한 20%, 핫키 실패 시 CT 비활성화, 재시작 시 OFF

**Why:** Phase 3 완료 후 실행. PRD F6(Click-Through) + F10(글로벌 핫키) 구현.
**How to apply:** Phase 3 Sprint 2까지 완료 선행 필수. Task 4에 feature-dev skill.
