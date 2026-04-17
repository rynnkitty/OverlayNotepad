# 프로젝트 로드맵 - OverlayNotepad (투명 메모장)

> 이 파일은 프로젝트 전체 진행 상황의 Single Source of Truth입니다.
> - **prd-to-roadmap** 에이전트가 PRD를 기반으로 초기 로드맵을 생성합니다.
> - **sprint-close** 에이전트가 스프린트 완료 시 상태를 업데이트합니다.
> - Phase/Sprint 구조는 `docs/phase/phase{N}/phase{N}.md`, `docs/phase/phase{P}/sprint{N}/sprint{N}.md`와 연동됩니다.

## 개요

- **목표**: 화면 위에 항상 떠 있는 투명 메모장 - 배경을 투명하게 하여 뒤의 콘텐츠를 보면서 메모할 수 있는 오버레이 도구
- **기술 스택**: .NET Framework 4.8, WPF (XAML + 코드비하인드), Win32 API 인터롭
- **팀 규모**: 1인 개발
- **배포**: 단일 EXE (설치 불필요, .NET 4.8은 Windows 10+에 기본 포함)

## 진행 상태 범례

- ✅ 완료 | 🔄 진행 중 | 📋 예정 | ⏸️ 보류

## 프로젝트 현황 대시보드

- 전체 진행률: Phase 2/4 완료 + Phase 3 Sprint 1 완료 (Sprint 1~2 + Phase 2 Sprint 1~2 + Phase 3 Sprint 1 완료)
- 현재 Phase: Phase 3 진행 중 (Sprint 1 완료, Sprint 2 예정)
- 완료된 스프린트: Phase 1 Sprint 1 (2026-04-17), Phase 1 Sprint 2 (2026-04-17), Phase 2 Sprint 1 (2026-04-17), Phase 2 Sprint 2 (2026-04-17), Phase 3 Sprint 1 (2026-04-17)
- 다음 마일스톤: Phase 3 완료 (다크/라이트 테마 + 컨텍스트 메뉴 완성 - MVP 출시)

## 기술 아키텍처 결정 사항

| 결정 | 선택 | 이유 |
|------|------|------|
| 프레임워크 | .NET Framework 4.8 + WPF | Windows 10+에 기본 포함, 단일 EXE 배포 가능 |
| 투명 구현 | WPF 네이티브 (AllowsTransparency=True) | 배경만 투명, 텍스트 불투명 유지가 네이티브로 가능 |
| 텍스트 입력 | WPF 표준 TextBox | 투명 배경에서 정상 동작, IME/한글 자동 지원, 표준 편집 기능 내장 |
| 텍스트 효과 | FormattedText + Geometry (커스텀 컨트롤) | 텍스트 아웃라인/테두리 효과를 정밀 제어 |
| 그림자 효과 | DropShadowEffect | WPF 네이티브 그림자 효과 |
| 전체 투명 모드 | Window.Opacity | 배경 투명 모드와 별도로 간단 구현 가능 |
| 설정 저장 | JSON (AppData 폴더) | 레지스트리 의존 최소화, 이식성 |
| 시스템 트레이 | WinForms NotifyIcon 인터롭 | 외부 NuGet 없이 단일 EXE 유지 |
| Click-Through | Win32 WS_EX_TRANSPARENT 인터롭 | 동적 윈도우 스타일 변경 |

## 의존성 맵

```
Phase 0: 프로젝트 설정 (완료)
  |
  v
Phase 1: WPF 기본 윈도우 + 투명 배경 + 텍스트 입력  <- P0 기초 기능
  |
  v
Phase 2: 텍스트 테두리/그림자 + 자동 저장 + 윈도우 관리  <- P0 완성 + P1 핵심
  |
  v
Phase 3: 시스템 트레이 + 서식/테마 + 컨텍스트 메뉴 완성  <- MVP 완성 지점
  |
  v
Phase 4: 글로벌 핫키 + Click-Through + 최종 마무리  <- P2-P3 기능, 전체 완성
```

## 리스크 및 완화 전략

| 리스크 | 영향도 | 완화 방안 |
|--------|--------|----------|
| 텍스트 테두리 커스텀 렌더링 복잡도 | 중간 | Phase 2에서 FormattedText + Geometry 방식 검증, DropShadowEffect를 폴백으로 확보 |
| AllowsTransparency=True 성능 (소프트웨어 렌더링) | 낮음 | 메모장 용도이므로 성능 영향 미미, 필요 시 렌더링 최적화 |
| WinForms NotifyIcon 인터롭 안정성 | 낮음 | System.Windows.Forms 참조 추가로 간단 해결, Dispose 처리 주의 |
| Click-Through + 투명 모드 조합 UX | 낮음 | Phase 4에서 시각적 피드백 설계로 해결 |

---

## Phase 0: 프로젝트 초기 설정 (Sprint 0) ✅

### 목표
프로젝트 저장소, 개발 환경, CI/CD 파이프라인 구성

### 작업 목록
#### Sprint 0: 프로젝트 초기 설정 ✅
- ✅ 저장소 생성 및 브랜치 전략 설정
- ✅ Claude Code 에이전트 설정
- ✅ CI/CD 파이프라인 구성
- ✅ 개발 프로세스 문서화

### 완료 기준 (Definition of Done)
- ✅ Git 저장소 및 브랜치 전략 동작
- ✅ CI/CD 파이프라인 정상 실행
- ✅ 개발 프로세스 문서 완성

---

## Phase 1: WPF 기본 윈도우 + 투명 배경 + 텍스트 입력 (Sprint 1~2) ✅

### 목표
WPF 프로젝트를 생성하고 핵심 기반을 구축한다. 타이틀바 없는 투명 윈도우에서 텍스트를 입력/편집할 수 있는 동작하는 프로토타입을 완성한다. WPF의 AllowsTransparency + 표준 TextBox 조합으로 P0 기능(F1 투명 배경, F2 Always on Top)의 기초를 확보한다.

### 작업 목록
#### Sprint 1: WPF 프로젝트 구조 + 투명 윈도우 + 기본 텍스트 입력 ✅ (2026-04-17 완료)
- ✅ .NET Framework 4.8 WPF 프로젝트 생성 및 솔루션 구조 설정
- ✅ 투명 윈도우 구현 (WindowStyle=None, AllowsTransparency=True, Background=Transparent)
- ✅ 커스텀 드래그 영역(상단 바)으로 윈도우 이동
- ✅ WindowChrome 리사이즈 핸들로 윈도우 크기 조절
- ✅ 표준 TextBox 배치 및 투명 배경 위 텍스트 입력 동작 확인
- ✅ 한글/영문 IME 입력 정상 동작 확인

#### Sprint 2: 투명도 모드 + Always on Top ✅ (2026-04-17 완료)
- ✅ 배경 투명 모드 (AllowsTransparency + Background=Transparent) 동작 확인
- ✅ 전체 투명 모드 (Window.Opacity) 구현
- ✅ 배경 투명 / 전체 투명 모드 전환 로직
- ✅ 투명도 수치 조절 (20%/40%/60%/80%/100% 단계)
- ✅ Always on Top (Topmost) 토글 구현
- ✅ 기본 우클릭 컨텍스트 메뉴 골격 (투명도 조절, 모드 전환, Always on Top, 종료)

### 완료 기준 (Definition of Done)
- ✅ 타이틀바 없는 투명 윈도우에서 드래그 이동 및 리사이즈 동작
- ✅ 투명 배경 위에서 한글/영문 텍스트 입력 정상 동작
- ✅ 복사/붙여넣기, Undo/Redo 등 표준 편집 기능 동작
- ✅ 배경 투명 / 전체 투명 모드 전환 동작
- ✅ Always on Top 토글 동작
- ✅ 기본 컨텍스트 메뉴에서 투명도 조절 및 모드 전환 동작
- ✅ 메모리 사용량 80MB 이하

### 기술 고려사항
- AllowsTransparency=True 시 하드웨어 가속 비활성화 -> 소프트웨어 렌더링 전환, 메모장 용도에서는 영향 미미
- WPF 표준 TextBox가 투명 배경에서 정상 동작하므로 GDI+ 커스텀 에디터 불필요
- IME/한글 입력은 WPF TextBox가 자동 지원
- WindowChrome 사용 시 리사이즈 그립 영역 설정 주의
- 컨텍스트 메뉴는 WPF ContextMenu 사용 (Phase 3에서 확장)

> Phase 상세 계획: `docs/phase/phase1/phase1.md` (phase-planner가 생성)

---

## Phase 2: 텍스트 테두리/그림자 + 자동 저장 + 윈도우 관리 (Sprint 1~2) ✅

### 목표
이 Phase의 핵심 기술 과제인 텍스트 테두리/그림자 효과(F3)를 구현하고, 자동 저장(F7)과 윈도우 관리(F8)로 데이터 안전성과 사용 편의성을 확보한다. P0 기능 완성 + P1 핵심 기능 구현.

### 작업 목록
#### Sprint 1: 텍스트 테두리 및 그림자 효과 ✅ (2026-04-17 완료)
- ✅ FormattedText + Geometry 기반 텍스트 아웃라인 커스텀 컨트롤 구현
- ✅ DropShadowEffect를 활용한 그림자 효과 구현
- ✅ 테두리/그림자 색상 설정 (기본값: 다크 테마 기준)
- ✅ 테두리+그림자 효과 토글 (켜기/끄기)
- ✅ 테두리 ON 시 커스텀 렌더링, OFF 시 일반 TextBox 전환 로직
- ✅ 투명 배경 위에서 다양한 배경(밝은/어두운)에 대한 가독성 검증

#### Sprint 2: 설정 관리 + 자동 저장 + 윈도우 관리 ✅ (2026-04-17 완료)
- ✅ 설정 파일 구조 설계 (settings.json, %AppData%/OverlayNotepad/)
- ✅ 설정 로드/저장 로직 (JSON 직렬화)
- ✅ 설정 파일 손상 시 기본값 복원 처리
- ✅ 텍스트 자동 저장 (변경 후 2초 디바운스)
- ✅ 비정상 종료 시 데이터 보존 (AppDomain.UnhandledException)
- ✅ 윈도우 위치/크기 저장 및 복원
- ✅ 화면 범위 보정 (저장된 위치가 현재 화면 밖이면 기본 위치로)

### 완료 기준 (Definition of Done)
- ✅ 텍스트 테두리/그림자 효과 적용 시 다양한 배경(밝은/어두운)에서 가독성 확보
- ✅ 테두리 효과 토글(켜기/끄기) 동작
- ✅ 텍스트 입력 후 2초 뒤 자동 저장, 프로그램 재실행 시 복원 확인
- ✅ 비정상 종료(프로세스 kill) 후에도 마지막 저장 데이터 복원
- ✅ 설정 파일 삭제 시 기본값으로 정상 시작
- ✅ 윈도우 위치/크기가 다음 실행 시 복원
- ✅ 다중 모니터 환경에서 모니터 분리 후 재실행 시 화면 내 복원

### 기술 고려사항
- FormattedText + Geometry 방식은 텍스트 아웃라인을 정밀 제어 가능하나 렌더링 복잡도 있음
- 테두리 모드에서는 커스텀 컨트롤로 전환되므로 TextBox의 표준 편집 기능과의 연동 방식 설계 필요
- DropShadowEffect는 WPF 네이티브이므로 구현 난이도 낮음
- 자동 저장은 DispatcherTimer + 디바운스 패턴
- JSON 직렬화: System.Text.Json 또는 수동 파싱 (외부 NuGet 최소화)

> Phase 상세 계획: `docs/phase/phase2/phase2.md` (phase-planner가 생성)

---

## Phase 3: 시스템 트레이 + 서식/테마 + 컨텍스트 메뉴 완성 (Sprint 1~2) 📋

### 목표
시스템 트레이(F9), 서식 지원(F4), 다크/라이트 테마(F5), 컨텍스트 메뉴(F11) 전체 항목을 구현하여 MVP를 완성한다. 이 Phase 완료 시 일상 사용 가능한 수준의 완성도를 갖춘다.

**이 Phase 완료 시 MVP 출시 가능.**

### 작업 목록
#### Sprint 1: 시스템 트레이 + 서식 지원 ✅ (2026-04-17 완료)
- ✅ 시스템 트레이 아이콘 (WinForms NotifyIcon 인터롭)
- ✅ 최소화 시 트레이로 숨김, 더블클릭 복원
- ✅ 트레이 우클릭 메뉴 (표시/숨김, Always on Top, 종료)
- ✅ 글꼴 변경 (시스템 설치 글꼴 선택)
- ✅ 글자 크기 변경
- ✅ 글자 색상 변경
- ✅ 서식 설정 저장/복원 (settings.json 연동)

#### Sprint 2: 다크/라이트 테마 + 컨텍스트 메뉴 완성 🔄
- 다크 테마 정의 (글자색 흰색, 배경 검정 반투명, 테두리 검정)
- 라이트 테마 정의 (글자색 검정, 배경 흰색 반투명, 테두리 흰색)
- 테마 전환 시 글자색/배경색/테두리색 일괄 변경
- 컨텍스트 메뉴 확장: 글꼴/크기/색상 설정
- 컨텍스트 메뉴 확장: 테마 전환 토글
- 컨텍스트 메뉴 확장: 텍스트 테두리/그림자 토글 및 색상
- 컨텍스트 메뉴 확장: 자동저장 상태 표시
- 컨텍스트 메뉴 확장: 최소화

### 완료 기준 (Definition of Done)
- ⬜ 시스템 트레이 아이콘 표시 및 최소화/복원 동작
- ⬜ 글꼴/크기/색상 변경이 전체 텍스트에 즉시 반영
- ⬜ 변경된 서식이 저장되어 재실행 시 유지
- ⬜ 다크/라이트 테마 전환 시 모든 색상이 일괄 변경
- ⬜ 컨텍스트 메뉴에서 PRD F11의 모든 항목 접근 가능
- ⬜ 테마 전환 시 텍스트 내용 유지
- ⬜ 시작 시간 2초 이내

### 기술 고려사항
- WPF에 네이티브 NotifyIcon이 없으므로 System.Windows.Forms 참조 추가 필요
- NotifyIcon은 반드시 Dispose 처리 (트레이 아이콘 잔존 방지)
- 테마 전환 시 텍스트 테두리 커스텀 컨트롤의 색상도 함께 변경해야 함
- 컨텍스트 메뉴에 투명도 슬라이더를 넣으려면 커스텀 MenuItem 활용
- 서식은 전체 텍스트에 일괄 적용 (부분 서식 미지원 - PRD 제약)

> Phase 상세 계획: `docs/phase/phase3/phase3.md` (phase-planner가 생성)

---

## Phase 4: 글로벌 핫키 + Click-Through + 최종 마무리 (Sprint 1~2) 📋

### 목표
나머지 P2-P3 기능을 구현하고 전체 품질을 다듬는다. 글로벌 핫키(F10)로 다른 앱에서도 메모장을 제어하고, Click-Through(F6) 모드로 읽기 전용 오버레이를 지원한다. 단일 EXE 배포 준비까지 완료.

### 작업 목록
#### Sprint 1: 글로벌 핫키 + Click-Through 🔄
- RegisterHotKey Win32 API 인터롭으로 글로벌 핫키 등록
- 표시/숨김 토글 핫키 (기본: Ctrl+Shift+N)
- Click-Through 토글 핫키 (기본: Ctrl+Shift+T)
- Click-Through 모드 구현 (Win32 WS_EX_TRANSPARENT 인터롭)
- Click-Through 상태 시각적 표시 (테두리 색상 변경)
- 트레이 메뉴에 Click-Through 토글 추가
- 핫키 충돌 처리 (이미 등록된 경우 알림)

#### Sprint 2: 최종 마무리 + 배포 준비 🔄
- 전체 기능 통합 테스트
- 성능 최적화 (메모리 80MB 이하, 유휴 CPU 0%, 시작 2초 이내)
- DPI 인식 설정 (app.manifest에 dpiAware 선언)
- 앱 아이콘 설정
- 단일 EXE 빌드 설정 확인
- 알려진 엣지 케이스 수정

### 완료 기준 (Definition of Done)
- ⬜ 다른 앱 포커스 상태에서 글로벌 핫키로 메모장 표시/숨김 동작
- ⬜ Click-Through 모드에서 마우스 클릭이 뒤의 앱에 전달
- ⬜ Click-Through 모드에서 핫키/트레이로만 해제 가능
- ⬜ Click-Through 상태가 시각적으로 구분 가능
- ⬜ 메모리 80MB 이하, 유휴 CPU 0%, 시작 2초 이내
- ⬜ 단일 EXE 파일로 정상 실행 (설치 불필요)
- ⬜ Windows 10/11에서 정상 동작

### 기술 고려사항
- RegisterHotKey는 프로세스 수준 등록, 프로그램 종료 시 UnregisterHotKey 필수
- WPF에서 WS_EX_TRANSPARENT는 HwndSource를 통한 Win32 인터롭으로 구현
- Click-Through와 배경 투명 모드 조합 시 시각적 피드백 설계 중요
- 단일 EXE 배포 시 System.Windows.Forms.dll 참조가 별도 DLL로 필요하지 않은지 확인 (.NET Framework 기본 포함)

> Phase 상세 계획: `docs/phase/phase4/phase4.md` (phase-planner가 생성)

---

## 마일스톤

| 마일스톤 | Phase | 핵심 성과 | 상태 |
|---------|-------|----------|------|
| P0 기초 | Phase 1 | WPF 투명 윈도우 + 텍스트 입력 동작 | ✅ 완료 (Sprint 1~2 완료) |
| P0 완성 | Phase 2 | 텍스트 테두리/그림자 + 자동 저장 + 윈도우 관리 | ✅ 완료 (Sprint 1~2 완료) |
| MVP 출시 | Phase 3 | 트레이 + 서식 + 테마 + 메뉴 완성, 일상 사용 가능 | 📋 예정 |
| 기능 완성 | Phase 4 | PRD 전체 기능 구현 완료 | 📋 예정 |

## WPF 전환에 따른 변경 사항 (WinForms 대비)

| 항목 | WinForms (이전) | WPF (현재) | 영향 |
|------|----------------|-----------|------|
| 투명 구현 | Win32 Layered Window | AllowsTransparency=True | 네이티브 지원, 구현 난이도 대폭 감소 |
| 텍스트 입력 | GDI+ 커스텀 에디터 (3 Sprint) | 표준 TextBox (즉시 사용) | Phase 1이 3 Sprint -> 2 Sprint로 축소 |
| IME/한글 | Win32 IME API 직접 처리 | WPF 자동 지원 | 구현 불필요 |
| 텍스트 테두리 | GDI+ GraphicsPath | FormattedText + Geometry | 유사한 복잡도이나 WPF 렌더링 파이프라인 활용 |
| 그림자 효과 | GDI+ 수동 오프셋 렌더링 | DropShadowEffect | 네이티브 지원 |
| 전체 Phase | 5개 Phase (Sprint 11개) | 4개 Phase (Sprint 8개) | 약 30% 축소 |

## 향후 계획 (Backlog)

PRD에 명시되지 않았으나, MVP 이후 고려 가능한 기능:
- 글로벌 핫키 사용자 변경 기능
- 투명도 슬라이더 UI 개선 (P3)
- 멀티 모니터 DPI 개별 처리
- 다중 메모 탭 (현재 PRD에서 명시적 미지원)
