# 미완료 배포 항목

> 이 파일은 현재 미완료된 수동 검증/배포 항목만 유지합니다.
> - **sprint-close** 에이전트가 스프린트 마무리 시 새 항목을 추가하고, 기존 완료 기록을 `docs/deploy-history/YYYY-MM-DD.md`로 아카이빙합니다.
> - **sprint-review** 에이전트가 코드 리뷰와 자동 검증 결과를 이 파일에 기록합니다.
> - 완료된 항목은 `✅`, 미완료 항목은 `⬜`로 표시합니다.

### Phase 3 Sprint 1: 시스템 트레이 + 서식 지원 (2026-04-17)

PR: https://github.com/rynnkitty/OverlayNotepad/compare/develop...phase3-sprint1 (사용자가 GitHub 웹에서 수동 생성 필요)
- 브랜치: `phase3-sprint1` → `develop`
- 제목(권장): `feat: Sprint 1 완료 - 시스템 트레이 + 서식 지원 (글꼴/크기/색상)`

- ⬜ GitHub 웹에서 PR 생성 (phase3-sprint1 → develop)

**코드 리뷰: (2026-04-17 sprint-review 수행)**
- ✅ 보안: 하드코딩 시크릿/API 키 없음, SQL/XSS 해당 없음 (데스크톱 앱)
- ✅ TrayIconManager: IDisposable 구현, Window_Closing + EmergencyDisposeTray 이중 Dispose 보장
- ✅ FontHelper: static HashSet 캐시로 InstalledFontCollection 1회만 로드, 미설치 글꼴 필터링
- ✅ 서식 설정 영속화: ApplyFont*/persist 패턴으로 초기화 시 불필요한 Save() 방지
- ✅ 글꼴 메뉴 초기화: _fontMenuInitialized 플래그로 SubmenuOpened 중복 빌드 방지
- ⚠️ Medium: FontDialog_Click, ColorDialog_Click에서 dialog.Dispose() 미호출 (GC 위임 — 기능 이상 없으나 명시적 해제 권장)
- ⚠️ Low: MainWindow.xaml에서 BackgroundTransparentMenuItem IsChecked="True" 하드코딩 — 저장된 settings와 초기 상태 불일치 가능성 (Window_Loaded에서 설정 반영되어 실질적 영향 없음)

**자동 검증:**
- ⬜ Docker/서버 환경 없음 — WPF 데스크톱 앱으로 Docker Compose 미적용 (자동 검증 미수행)
- ⬜ 단위 테스트 없음 — 이 프로젝트는 테스트 프로젝트 미포함 (수동 검증으로 대체)

**수동 검증 필요 항목:**
- ⬜ 앱 실행 후 시스템 트레이 아이콘 표시 확인
- ⬜ 최소화 시 작업표시줄에서 사라지고 트레이에만 표시 확인
- ⬜ 트레이 더블클릭으로 윈도우 복원 확인
- ⬜ 트레이 우클릭 메뉴(표시/숨김, 항상 위에, 종료) 동작 확인
- ⬜ 우클릭 메뉴 > 글꼴 서브메뉴에서 프리셋 + "더 보기..." 동작 확인
- ⬜ 우클릭 메뉴 > 글자 크기 서브메뉴 7단계 프리셋 + "직접 입력..." 동작 확인
- ⬜ 우클릭 메뉴 > 글자 색상 서브메뉴 10색 + "사용자 지정..." 동작 확인
- ⬜ 서식 변경 후 종료 → 재실행 시 서식 설정 복원 확인
- ⬜ PR 머지 후 develop 브랜치 로컬 동기화

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
