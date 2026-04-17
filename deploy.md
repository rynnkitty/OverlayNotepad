# 미완료 배포 항목

> 이 파일은 현재 미완료된 수동 검증/배포 항목만 유지합니다.
> - **sprint-close** 에이전트가 스프린트 마무리 시 새 항목을 추가하고, 기존 완료 기록을 `docs/deploy-history/YYYY-MM-DD.md`로 아카이빙합니다.
> - **sprint-review** 에이전트가 코드 리뷰와 자동 검증 결과를 이 파일에 기록합니다.
> - 완료된 항목은 `✅`, 미완료 항목은 `⬜`로 표시합니다.

### Phase 1 Sprint 1: WPF 프로젝트 구조 + 투명 윈도우 + 기본 텍스트 입력 (2026-04-17)

PR: https://github.com/rynnkitty/OverlayNotepad/compare/develop...phase1-sprint1 (사용자가 GitHub 웹에서 수동 생성 필요)
- 브랜치: `phase1-sprint1` → `develop`
- 제목(권장): `feat(phase1-sprint1): WPF 투명 윈도우 + 텍스트 입력 프로토타입`

- ⬜ GitHub 웹에서 PR 생성 (phase1-sprint1 → develop)

**코드 리뷰 결과 (2026-04-17, sprint-review):**
- ✅ Critical 이슈: 0건
- ✅ High 이슈: 0건
- Medium 이슈 1건: 드래그 Border `Background="Transparent"` → `#00FFFFFF` 명확화 권고 (현재 동작 정상, Sprint 2에서 개선 권장)
- 보안/성능/패턴 준수: 전 항목 이상 없음

**자동 검증 결과 (2026-04-17):**
- ⬜ Docker 환경 없음 — 자동 검증 미수행 (docker-compose.yml 없는 데스크톱 WPF 프로젝트)
- ✅ MSBuild 빌드: 0 Errors 확인 완료 (구현 중 검증)
- ✅ 수동 동작 검증 전 항목 통과 (sprint1.md 완료 기준 참조)

**수동 검증 필요 항목:**
- ⬜ 사용자가 GitHub 웹에서 PR 생성 (phase1-sprint1 → develop)
- ⬜ UI 디자인/시각적 품질 최종 판단 (투명도, 하이라이트 피드백 등)
- ⬜ PR 머지 후 develop 브랜치 로컬 동기화 (`git checkout develop && git pull origin develop`)

**Phase 문서 반영 상태:**
- ✅ phase1.md Sprint 분할 계획 테이블: Sprint 1 ✅ 표시
- ✅ phase1.md Sprint 1 상세 섹션: ✅ 완료 + PR 날짜 메모 추가
- ✅ phase1.md 완료 기준 테이블: Sprint 1 항목 ✅ 완료 반영
- ✅ phase1.md 미해결 사항: Medium 이슈 (Background 히트 테스트 명확화) 추가

---

## 참고

- 검증 원칙: `docs/dev-process.md` 섹션 5
- 배포 이력: `docs/deploy-history/`
- 롤백 방법: `docs/dev-process.md` 섹션 6.4
