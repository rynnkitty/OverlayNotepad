---
name: Phase 4 계획 수립 완료
description: Phase 4 (글로벌 핫키 + Click-Through + 최종 마무리) 계획 세부 사항과 전문가 검토 결과
type: project
---

Phase 4 계획 수립 완료 (2026-04-17).

- Sprint 1: 글로벌 핫키 (RegisterHotKey) + Click-Through (WS_EX_TRANSPARENT)
- Sprint 2: 최종 마무리 + 배포 준비 (DPI, 아이콘, 단일 EXE, 성능 최적화)

**Why:** PRD F6(Click-Through), F10(글로벌 핫키) 구현 + 전체 품질 마무리로 기능 완성
**How to apply:** Phase 1~3 완료 후 진행. Click-Through 투명도 하한 20%, 핫키 등록 실패 시 Click-Through 비활성화 등 안전장치가 확정 파라미터.

핵심 전문가 검토 결과:
- Click-Through 시각 피드백: 빨간 점선 테두리 + 상단 바 텍스트 + 트레이 아이콘 변경 (3중 신호)
- 핫키 사용자 변경: MVP 제외 (백로그)
- Click-Through 재시작 시 OFF 기본값 (안전 설계)
