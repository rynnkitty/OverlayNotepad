---
name: WPF 전환 로드맵 재생성
description: 2026-04-16 WinForms에서 WPF로 기술 스택 전환에 따라 ROADMAP.md 완전 재생성. Phase 5개 -> 4개, Sprint 11개 -> 8개로 축소.
type: project
---

2026-04-16: PRD가 WinForms -> WPF로 변경되어 ROADMAP.md를 완전 재생성함.

핵심 변경:
- .NET Framework 4.8 + WPF (AllowsTransparency=True)로 투명 배경 네이티브 지원
- 표준 TextBox가 투명 배경에서 정상 동작하여 GDI+ 커스텀 에디터 불필요
- IME/한글 입력 WPF 자동 지원
- Phase 5개 -> 4개, Sprint 11개 -> 8개로 약 30% 축소
- 주요 기술 리스크: FormattedText + Geometry 기반 텍스트 테두리 커스텀 렌더링만 남음

**Why:** PRD 기술 스택 변경으로 기존 로드맵의 GDI+ 중심 Phase 구조가 완전히 무효화됨
**How to apply:** 향후 Phase/Sprint 계획 시 WPF 네이티브 기능 활용을 전제로 설계
