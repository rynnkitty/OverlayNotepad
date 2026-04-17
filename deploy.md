# 미완료 배포 항목

> 이 파일은 현재 미완료된 수동 검증/배포 항목만 유지합니다.
> - **sprint-close** 에이전트가 스프린트 마무리 시 새 항목을 추가하고, 기존 완료 기록을 `docs/deploy-history/YYYY-MM-DD.md`로 아카이빙합니다.
> - **sprint-review** 에이전트가 코드 리뷰와 자동 검증 결과를 이 파일에 기록합니다.
> - 완료된 항목은 `✅`, 미완료 항목은 `⬜`로 표시합니다.

### Phase 4 Sprint 1: 글로벌 핫키 + Click-Through (2026-04-17)

PR: https://github.com/rynnkitty/OverlayNotepad/pull/ (GitHub 웹에서 수동 생성 필요)
- 브랜치: `phase4-sprint1` → `develop`
- 제목(권장): `feat: Sprint 1 완료 - 글로벌 핫키 + Click-Through (PRD F10/F6)`

**코드 리뷰 결과 (2026-04-17):**
- ✅ 보안 이슈 없음 (하드코딩 시크릿, P/Invoke 안전 패턴 준수)
- ✅ Critical/High 이슈 없음
- Medium 이슈 0건
- Low 이슈 1건: `AppSettings.IsClickThrough`에 `[DataMember]` 없으나 의도적 설계 (런타임 전용, 재시작 시 OFF 보장) — 문서 주석으로 명확히 표기됨, 문제 없음
- Low 이슈 1건 (이월): `BackgroundTransparentMenuItem_Click` 등 일부 핸들러에서 `sender as MenuItem` null 체크 누락 — Phase 3 Sprint 2 이월, Sprint 2에서 개선 권장

**자동 검증 결과 (2026-04-17):**
- ✅ MSBuild Debug 빌드: 경고 0, 오류 0
- ✅ MSBuild Release 빌드: 경고 0, 오류 0
- ⬜ Docker 환경 없음 — pytest/API/Playwright 자동 검증 해당 없음 (WPF 데스크톱 앱)

**수동 검증 필요 항목:**
- ⬜ GitHub 웹에서 PR 생성 (phase4-sprint1 → develop)
- ⬜ Ctrl+Shift+N으로 표시/숨김 토글 (다른 앱 포커스 상태)
- ⬜ Ctrl+Shift+T로 Click-Through 토글 (다른 앱 포커스 상태)
- ⬜ Click-Through ON 시 마우스 클릭이 뒤의 앱에 전달되는지 확인
- ⬜ Click-Through ON 시 빨간 점선 테두리 + "CLICK-THROUGH" 상단 텍스트 표시 확인
- ⬜ Click-Through ON 시 트레이 아이콘/툴팁 변경 확인
- ⬜ Click-Through ON 시 트레이 메뉴 체크마크 동기화 확인
- ⬜ 투명도 10% 상태에서 Click-Through ON 시 자동으로 20%로 조정 확인
- ⬜ 핫키 등록 실패 시 트레이 벌룬 알림 표시 확인
- ⬜ Click-Through 핫키 실패 시 메뉴 항목 비활성화 확인
- ⬜ 최초 Click-Through 활성화 시 벌룬 안내 ("해제: Ctrl+Shift+T 또는 트레이 메뉴") 표시 확인
- ⬜ 두 번째 이후 Click-Through 활성화 시 벌룬 미표시 확인
- ⬜ Click-Through ON 상태로 종료 후 재시작 시 Click-Through OFF 확인
- ⬜ 정상 종료 시 트레이 아이콘 사라짐 확인
- ⬜ PR 머지 후 develop 브랜치 로컬 동기화

---

### Phase 3 Sprint 2: 다크/라이트 테마 + 컨텍스트 메뉴 완성 (2026-04-17)

(아카이빙됨: docs/deploy-history/2026-04-17.md)

**코드 리뷰 결과 (2026-04-17):**
- ✅ 보안 이슈 없음
- ✅ Critical/High 이슈 없음
- Medium 이슈 0건
- Low 이슈 1건: `BackgroundTransparentMenuItem_Click` 등 4개 핸들러에서 `sender as MenuItem` null 체크 누락 — 실제 런타임 위험 낮음, Phase 4 Sprint 이후 개선 권장

**자동 검증 결과 (2026-04-17):**
- ✅ MSBuild Debug 빌드: 경고 0, 오류 0
- ✅ MSBuild Release 빌드: 경고 0, 오류 0

**수동 검증 필요 항목:**
- ⬜ GitHub 웹에서 PR 생성 (phase3-sprint2 → develop)
- ⬜ 앱 실행 시 기본 다크 테마 적용 확인
- ⬜ 테마 전환 시 모든 색상 일괄 변경 확인
- ⬜ 테마 설정이 settings.json에 저장되어 재실행 시 복원 확인
- ⬜ PRD F11 모든 항목이 컨텍스트 메뉴에 표시 확인

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
