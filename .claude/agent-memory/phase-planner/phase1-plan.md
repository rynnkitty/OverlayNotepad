---
name: Phase 1 계획 수립 완료
description: Phase 1 (WPF 투명 윈도우 + 텍스트 입력) 계획 수립 완료. Sprint 2개, 확정 파라미터 포함.
type: project
---

Phase 1 계획 수립 완료 (2026-04-17)

- 목표: WPF 투명 윈도우 + TextBox 기반 텍스트 입력 프로토타입
- Sprint 1: 프로젝트 생성, 투명 윈도우, 드래그/리사이즈, TextBox
- Sprint 2: 투명도 모드 전환, Always on Top, 컨텍스트 메뉴 골격

**Why:** Phase 0(프로젝트 설정)이 완료되어 첫 번째 기능 구현 Phase를 시작한다.
**How to apply:** sprint-planner가 phase1.md를 참조하여 Sprint 문서를 생성할 때 확정 파라미터를 반드시 준수해야 한다.

주요 확정 파라미터 변경:
- 드래그 영역: 8px -> 12px (UX Fitts' Law)
- 리사이즈: 커스텀 핸들 -> WindowChrome ResizeBorderThickness=8
- 투명도 조절: 슬라이더 -> 단계별 선택 (20/40/60/80/100%)
- TextBox 자동 포커스: 필수

전문가 검토 주의사항:
- Phase 2 전환 시 TextBox -> 커스텀 컨트롤 교체 용이성 확보 필요
- AllowsTransparency 소프트웨어 렌더링 성능은 메모장 용도에서 미미
