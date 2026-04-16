---
name: Phase 2 Sprint 1 계획
description: 텍스트 테두리/그림자 효과 구현 계획 -- FormattedText+Geometry 이중 레이어 + DropShadowEffect
type: project
---

Phase 2 Sprint 1 계획 수립 완료 (2026-04-17).

**핵심 아키텍처**: 이중 레이어 구조
- TextBox: 항상 입력 담당 (커서, 선택, IME, Undo/Redo 보존)
- OutlinedTextControl: FormattedText + Geometry 기반 아웃라인 렌더링 전용
- 테두리 ON: TextBox.Foreground = Transparent, OutlinedTextControl 표시
- 테두리 OFF: OutlinedTextControl 숨김, TextBox 직접 표시
- DropShadowEffect: TextBox에 적용, 테두리 ON/OFF와 독립

**확정 파라미터**:
- 테두리 두께: 1.0px (범위 0.5~3.0)
- 그림자: blur 5px, offset 1.5/1.5, opacity 0.8
- 기본 상태: 테두리 ON, 그림자 ON

**Task 4개**: 모두 순차 실행 (동일 파일군 수정)
1. TextEffectSettings 모델 + OutlinedTextControl
2. DropShadowEffect + MainWindow 이중 레이어 통합
3. 테두리/그림자 토글 + 컨텍스트 메뉴 연동
4. 통합 검증 + 가독성 테스트

**폴백**: FormattedText 방식이 실패하면 DropShadowEffect만 사용 또는 다중 DropShadow로 테두리 시뮬레이션

**Why:** Phase 1 완료 후 실행. Phase 1의 MainWindow.xaml/cs, TextBox, ContextMenu를 기반으로 구축.
**How to apply:** Task 2에 `feature-dev:feature-dev` skill 배정 (기존 3개+ 파일 통합). 나머지 Task는 skill 없음.
