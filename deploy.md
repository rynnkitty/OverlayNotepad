# 미완료 배포 항목

> 이 파일은 현재 미완료된 수동 검증/배포 항목만 유지합니다.
> - **sprint-close** 에이전트가 스프린트 마무리 시 새 항목을 추가하고, 기존 완료 기록을 `docs/deploy-history/YYYY-MM-DD.md`로 아카이빙합니다.
> - **sprint-review** 에이전트가 코드 리뷰와 자동 검증 결과를 이 파일에 기록합니다.
> - 완료된 항목은 `✅`, 미완료 항목은 `⬜`로 표시합니다.

### Phase 3 Sprint 2: 다크/라이트 테마 + 컨텍스트 메뉴 완성 (2026-04-17)

PR: https://github.com/rynnkitty/OverlayNotepad/compare/develop...phase3-sprint2 (사용자가 GitHub 웹에서 수동 생성 필요)
- 브랜치: `phase3-sprint2` → `develop`
- 제목(권장): `feat: Sprint 2 완료 - 다크/라이트 테마 + 컨텍스트 메뉴 완성 (PRD F11)`

- ⬜ GitHub 웹에서 PR 생성 (phase3-sprint2 → develop)
- ⬜ 코드 리뷰 미수행 (sprint-review 에이전트로 실행 필요)
- ⬜ 자동 검증 미수행 (sprint-review 에이전트로 실행 필요)

**수동 검증 필요 항목:**
- ⬜ 앱 실행 시 기본 다크 테마 적용 확인 (흰색 글자, #1E1E1E 배경, 검정 테두리)
- ⬜ 컨텍스트 메뉴 > 테마 전환으로 다크 ↔ 라이트 전환 동작 확인
- ⬜ 테마 전환 시 모든 색상 일괄 변경 (글자색/배경색/테두리색/드래그바)
- ⬜ 테마 전환 후 텍스트 내용 유지 확인
- ⬜ 테마 설정이 settings.json에 저장되어 재실행 시 복원 확인
- ⬜ 컨텍스트 메뉴 > 테두리 색상 서브메뉴에서 색상 변경 동작 확인
- ⬜ 컨텍스트 메뉴 > 자동저장 상태 표시 ("자동 저장됨" / "저장 대기 중...") 확인
- ⬜ 컨텍스트 메뉴 > 최소화 클릭 시 트레이로 숨김 확인
- ⬜ PRD F11 모든 항목이 컨텍스트 메뉴에 표시 확인 (Click-Through 제외)
- ⬜ 메뉴 IsChecked 상태가 실제 settings 값과 일치 확인
- ⬜ PR 머지 후 develop 브랜치 로컬 동기화

---

### Phase 3 Sprint 1: 시스템 트레이 + 서식 지원 (2026-04-17)

(아카이빙됨: docs/deploy-history/2026-04-17.md)
- ⬜ 코드 리뷰 미수행 (sprint-review 에이전트로 실행 필요)
- ⬜ 자동 검증 미수행 (sprint-review 에이전트로 실행 필요)

---

### Phase 2 Sprint 2: 설정 관리 + 자동 저장 + 윈도우 관리 (2026-04-17)

(아카이빙됨: docs/deploy-history/2026-04-17.md)
- ⬜ 코드 리뷰 미수행 (sprint-review 에이전트로 실행 필요)
- ⬜ 자동 검증 미수행 (sprint-review 에이전트로 실행 필요)

---

## 참고

- 검증 원칙: `docs/dev-process.md` 섹션 5
- 배포 이력: `docs/deploy-history/`
- 롤백 방법: `docs/dev-process.md` 섹션 6.4
