# Phase 4 검토 리포트 — 이기획 (프로젝트 PO)

> 검토일: 2026-04-17
> 검토 대상: Phase 4 아키텍처 초안 (글로벌 핫키 + Click-Through + 최종 마무리)

## 요약

- ✅ Sprint 분할 적절 — Sprint 1(기능), Sprint 2(품질/배포) 분리가 합리적
- ✅ PRD F6, F10 기능 범위 충족 — Click-Through, 글로벌 핫키 모두 포함
- ⚠️ 핫키 사용자 변경 기능 — PRD에 "변경 가능 고려"로 명시, MVP에서는 고정값으로 충분
- ⚠️ Click-Through + 투명 모드 조합 — UX 혼란 가능성, 시각적 피드백 설계 중요

## 항목별 검증 결과

### 1. 글로벌 핫키 (F10)
- ✅ Ctrl+Shift+N (표시/숨김), Ctrl+Shift+T (Click-Through) — PRD 기본값과 일치
- ✅ RegisterHotKey/UnregisterHotKey 쌍 — 리소스 해제 패턴 적절
- ⚠️ 핫키 충돌 시 사용자 알림 방법 — 트레이 벌룬 알림이 가장 자연스러움

### 2. Click-Through (F6)
- ✅ WS_EX_TRANSPARENT 인터롭 — PRD 명세와 일치
- ✅ 트레이 메뉴/핫키로만 해제 — Click-Through 상태에서 우클릭 불가이므로 필수
- ⚠️ Click-Through + 전체 투명 모드 동시 활성 시 — "보이지도 않고 클릭도 안 되는" 상태 방지 필요

### 3. 배포 준비 (Sprint 2)
- ✅ 단일 EXE — .NET Framework 4.8은 Win10+ 기본 포함, System.Windows.Forms도 기본 포함
- ✅ DPI 인식 — app.manifest에 선언만으로 충분
- ✅ 성능 기준 — 메모리 80MB, 시작 2초, 유휴 CPU 0% — PRD 비기능 요구사항과 일치

## 파라미터 조정 권고

| 항목 | 원래 설계 | 권고값 | 근거 |
|------|----------|--------|------|
| 핫키 사용자 변경 | 미정 | MVP 제외 (고정값) | PRD에 "고려"로만 명시, 백로그에 이미 포함 |
| Click-Through + 전체 투명 조합 | 허용 | 전체 투명도 최소 20% 제한 | 완전히 보이지 않는 윈도우 방지 |

## 리스크

1. **핫키 충돌**: 다른 앱이 같은 핫키를 사용하는 경우 — RegisterHotKey 실패 시 트레이 알림으로 해결
2. **Click-Through 복구 불가**: 트레이 아이콘이 숨겨진 상태에서 Click-Through 활성화 시 — 핫키가 유일한 해제 수단, 핫키 등록 실패 시 Click-Through 비활성화 강제
