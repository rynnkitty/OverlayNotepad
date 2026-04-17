---
name: Phase 3 Sprint 2 계획
description: 다크/라이트 테마(ThemeManager) + 컨텍스트 메뉴 PRD F11 완성 + Sprint 1 미해결 개선, 계획 수립 완료
type: project
---

Phase 3 Sprint 2: 다크/라이트 테마 + 컨텍스트 메뉴 완성 (2026-04-17 계획 수립, 상세 보완 완료).

Task 4개, 모두 순차 실행 (MainWindow.xaml/xaml.cs 공유):
- Task 1: ThemeDefinition 모델 + ThemeManager 서비스 (skill: --)
- Task 2: 테마 전환 기능 + MainWindow 통합 + Sprint 1 미해결 개선 (skill: feature-dev)
  - FontDialog/ColorDialog Dispose using 블록 개선
  - XAML IsChecked와 settings.json 동기화 (SyncMenuCheckedStates)
  - AutoSaveManager.HasUnsavedChanges 프로퍼티 추가
  - 배경 레이어 Border 추가 (x:Name="MainBackground")
  - 드래그바에 x:Name="DragBar" 추가
  - 배경투명메뉴에 x:Name="BackgroundTransparentMenuItem" 추가
- Task 3: 컨텍스트 메뉴 최종 구성 PRD F11 완성 (skill: feature-dev)
  - 테두리 색상 서브메뉴 신규
  - 테마 전환 MenuItem 신규
  - 자동저장 상태 표시 신규
  - 최소화 MenuItem 신규
  - ContextMenu.Opened 이벤트로 전체 메뉴 상태 동기화
- Task 4: 통합 검증 + Phase 3 DoD 확인 (skill: --)

확정 파라미터 핵심:
- 다크: 글자 #FFFFFF, 배경 #1E1E1E/80%, 테두리 #000000, 그림자 #000000, 드래그바 #333333/90%
- 라이트: 글자 #000000, 배경 #FFFFFF/80%, 테두리 #FFFFFF, 그림자 #808080, 드래그바 #E0E0E0/90%
- 서브메뉴 최대 1단계, 테마 전환 일괄 변경(깜빡임 방지)

**Why:** Phase 3 완료 시 MVP 출시 가능. Sprint 2는 Sprint 1(트레이+서식)을 기반으로 테마 전환 + 메뉴 완성으로 MVP 기능 세트를 완성함.
**How to apply:** Phase 3 Sprint 1 완료 후 구현 시작. 테마 전환은 Dispatcher.Invoke로 일괄 처리, 사용자 지정 색상은 테마 전환 시 테마 기본값으로 리셋. MainWindow.xaml에 MainBackground Border, DragBar x:Name, BackgroundTransparentMenuItem x:Name 추가 필요.
