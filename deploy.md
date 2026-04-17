# 미완료 배포 항목

> 이 파일은 현재 미완료된 수동 검증/배포 항목만 유지합니다.
> - **sprint-close** 에이전트가 스프린트 마무리 시 새 항목을 추가하고, 기존 완료 기록을 `docs/deploy-history/YYYY-MM-DD.md`로 아카이빙합니다.
> - **sprint-review** 에이전트가 코드 리뷰와 자동 검증 결과를 이 파일에 기록합니다.
> - 완료된 항목은 `✅`, 미완료 항목은 `⬜`로 표시합니다.

### Phase 2 Sprint 2: 설정 관리 + 자동 저장 + 윈도우 관리 (2026-04-17)

PR: https://github.com/rynnkitty/OverlayNotepad/compare/develop...phase2-sprint2 (사용자가 GitHub 웹에서 수동 생성 필요)
- 브랜치: `phase2-sprint2` → `develop`
- 제목(권장): `feat: Sprint 2 완료 - 설정 관리 + 자동 저장 + 윈도우 관리`

- ⬜ GitHub 웹에서 PR 생성 (phase2-sprint2 → develop)

**코드 리뷰:**
- ⬜ 코드 리뷰 미수행 (sprint-review 에이전트로 실행 필요)

**자동 검증:**
- ⬜ 자동 검증 미수행 (sprint-review 에이전트로 실행 필요)

**수동 검증 필요 항목:**
- ⬜ 사용자가 GitHub 웹에서 PR 생성 (phase2-sprint2 → develop)
- ⬜ 텍스트 입력 후 2초 뒤 `%AppData%/OverlayNotepad/memo.txt` 갱신 확인
- ⬜ 프로그램 재실행 시 이전 텍스트/위치/크기 복원 확인
- ⬜ settings.json 삭제/손상 후 기본값 복원 확인
- ⬜ PR 머지 후 develop 브랜치 로컬 동기화

---

## 참고

- 검증 원칙: `docs/dev-process.md` 섹션 5
- 배포 이력: `docs/deploy-history/`
- 롤백 방법: `docs/dev-process.md` 섹션 6.4
