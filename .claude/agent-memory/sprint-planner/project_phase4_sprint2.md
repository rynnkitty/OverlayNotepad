---
name: Phase 4 Sprint 2 계획
description: 최종 마무리 + 배포 준비 — DPI/아이콘/성능/Release 빌드/통합 테스트, 계획 수립 완료 + sprint2.md 업데이트 완료
type: project
---

Phase 4 Sprint 2: 최종 마무리 + 배포 준비 (2026-04-17 계획 수립, sprint2.md 업데이트).

Task 5개 (모두 순차 실행):
1. DPI 인식 (app.manifest dpiAware=true) + 앱 아이콘 설정
2. 성능 최적화 (메모리 80MB 이하, CPU 0%, 시작 2초 이내) + 리소스 정리
3. 단일 EXE Release 빌드 + 독립 실행 테스트
4. 전체 기능 통합 테스트 (핫키, Click-Through, 투명도, 테마, 서식, 자동저장, 트레이 조합) + 엣지 케이스 9종 (BackgroundTransparentMenuItem_Click null 체크 포함)
5. Phase 4 DoD + PRD F1~F11 전체 기능 + 비기능 요구사항 최종 확인

**Why:** 프로젝트의 마지막 Sprint. Phase 1~4 Sprint 1의 모든 기능이 완성된 후 품질 마무리와 배포 준비에 집중.
**How to apply:** 새 기능 추가 없음. 기존 기능의 검증과 최적화에만 집중. 모든 Task가 MainWindow/프로젝트 파일을 공유하므로 병렬 불가.

주의사항:
- app.manifest가 아직 존재하지 않음 — Task 1에서 신규 생성 필요
- Resources/app.ico도 아직 없음 — Task 1에서 생성 필요 (ICO 바이너리 파일이므로 간단한 아이콘으로 대체하거나 기존 시스템 아이콘 활용)
- AutoSaveManager의 DispatcherTimer 100ms 간격 폴링 — CPU 유휴 시 0%인지 확인 필요
- IsClickThrough 필드에 [DataMember] 어트리뷰트 없음 — 의도적 (재시작 시 OFF 강제)
