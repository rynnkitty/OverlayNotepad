# OverlayNotepad (투명 메모장)

> 화면 위에 항상 떠 있는 투명 메모장 — 배경을 투명하게 하여 뒤의 콘텐츠를 보면서 메모할 수 있는 오버레이 도구

![Platform](https://img.shields.io/badge/platform-Windows%2010%2F11-blue)
![Framework](https://img.shields.io/badge/.NET%20Framework-4.8-purple)
![UI](https://img.shields.io/badge/UI-WPF-green)
![Deploy](https://img.shields.io/badge/deploy-Single%20EXE-orange)

---

## 프로젝트 소개

듀얼 모니터가 없거나, 화면 위에 메모를 띄워두고 참고하며 작업해야 하는 사용자를 위한 도구입니다.
설치 없이 EXE 하나만 실행하면 투명한 메모장이 화면 위에 떠서, 뒤의 콘텐츠를 보면서 메모할 수 있습니다.

### 대상 사용자

- 개발자 (API 문서 보면서 코딩)
- 학생 (강의 화면 위에 필기)
- 번역가 (원문 위에 번역 메모)
- 실시간 참고자료가 필요한 모든 직군

---

## 핵심 기능

| 기능 | 설명 |
|------|------|
| **투명 배경** | 배경만 투명 / 전체 투명 모드 전환, 투명도 단계 조절 (20~100%) |
| **Always on Top** | 항상 최상위 표시 토글 |
| **텍스트 테두리/그림자** | FormattedText + Geometry 아웃라인, DropShadowEffect 그림자로 어떤 배경에서든 가독성 확보 |
| **서식 지원** | 글꼴, 크기, 색상 변경 (전체 텍스트 일괄 적용) |
| **다크/라이트 테마** | 원클릭 테마 전환, 글자색·배경색·테두리색 일괄 변경 |
| **Click-Through** | 마우스 클릭이 메모장을 통과하여 뒤 앱에 전달 (Win32 WS_EX_TRANSPARENT) |
| **자동 저장** | 텍스트 변경 후 2초 디바운스 자동 저장, 비정상 종료 시에도 데이터 보존 |
| **윈도우 관리** | 타이틀바 없는 커스텀 드래그/리사이즈, 위치·크기 저장/복원 |
| **시스템 트레이** | 최소화 시 트레이 숨김, 더블클릭 복원 |
| **글로벌 핫키** | Ctrl+Shift+N (표시/숨김), Ctrl+Shift+T (Click-Through) |
| **컨텍스트 메뉴** | 투명도, 테마, 서식, 테두리, Always on Top 등 전체 설정 접근 |

---

## 기술 스택

| 항목 | 선택 | 이유 |
|------|------|------|
| 프레임워크 | .NET Framework 4.8 | Windows 10+에 기본 포함, 별도 런타임 설치 불필요 |
| UI | WPF (XAML + 코드비하인드) | 투명 윈도우 네이티브 지원, TextBox IME/한글 자동 지원 |
| 투명 구현 | AllowsTransparency=True | 배경만 투명, 텍스트 불투명 유지 |
| 텍스트 효과 | FormattedText + Geometry | 텍스트 아웃라인 정밀 제어 |
| Click-Through | Win32 WS_EX_TRANSPARENT 인터롭 | 동적 윈도우 스타일 변경 |
| 설정 저장 | JSON (%AppData%/OverlayNotepad/) | 레지스트리 의존 최소화 |
| 시스템 트레이 | WinForms NotifyIcon 인터롭 | 외부 NuGet 없이 단일 EXE 유지 |
| 배포 | 단일 EXE | 설치/언설치 프로세스 없음 |

---

## 개발 로드맵

```
Phase 0: 프로젝트 설정 ✅
  │
  v
Phase 1: WPF 투명 윈도우 + 텍스트 입력 (Sprint 1~2)  📋
  │       └ Sprint 1: 프로젝트 구조 + 투명 윈도우 + TextBox
  │       └ Sprint 2: 투명도 모드 + Always on Top + 컨텍스트 메뉴
  v
Phase 2: 텍스트 효과 + 자동 저장 + 윈도우 관리 (Sprint 1~2)  📋
  │       └ Sprint 1: 텍스트 테두리/그림자 커스텀 컨트롤
  │       └ Sprint 2: 설정 관리 + 자동 저장 + 위치/크기 저장
  v
Phase 3: 시스템 트레이 + 서식/테마 + 메뉴 완성 (Sprint 1~2)  📋  ← MVP
  │       └ Sprint 1: 시스템 트레이 + 글꼴/크기/색상
  │       └ Sprint 2: 다크/라이트 테마 + 컨텍스트 메뉴 완성
  v
Phase 4: 글로벌 핫키 + Click-Through + 마무리 (Sprint 1~2)  📋
          └ Sprint 1: RegisterHotKey + Click-Through 구현
          └ Sprint 2: 통합 테스트 + 성능 최적화 + 배포 준비
```

> ✅ 완료 | 🔄 진행 중 | 📋 계획 수립 완료

### 마일스톤

| 마일스톤 | Phase | 핵심 성과 |
|---------|-------|----------|
| P0 기초 | Phase 1 | 투명 윈도우에서 텍스트 입력 동작 |
| P0 완성 | Phase 2 | 텍스트 테두리/그림자 + 자동 저장 |
| **MVP 출시** | **Phase 3** | **트레이 + 서식 + 테마, 일상 사용 가능** |
| 기능 완성 | Phase 4 | PRD 전체 기능 구현 완료 |

---

## 비기능 요구사항

| 항목 | 목표 |
|------|------|
| 시작 시간 | 2초 이내 |
| 메모리 사용량 | 80MB 이하 (유휴 상태) |
| CPU 사용 | 유휴 시 0% |
| 호환성 | Windows 10 (1903+) / Windows 11 |
| 배포 | 단일 EXE, 설치 불필요 |

---

## 프로젝트 구조

```
OverlayNotepad/
├── src/                           ← 소스 코드 (Phase 1부터 생성)
│   └── OverlayNotepad/
├── docs/
│   ├── prd.md                     # 제품 요구사항 문서
│   ├── dev-process.md             # 개발 프로세스 가이드
│   ├── phase/                     # Phase/Sprint 계획 문서
│   │   ├── phase1/
│   │   │   ├── phase1.md          # Phase 1 실행 계획
│   │   │   ├── sprint1/sprint1.md # Sprint 실행 명세서
│   │   │   └── sprint2/sprint2.md
│   │   ├── phase2/
│   │   ├── phase3/
│   │   └── phase4/
│   ├── experts/                   # 도메인 전문가 프로필
│   └── templates/                 # 문서 작성 템플릿
├── .claude/                       # Claude Code 에이전트 설정
│   ├── agents/                    # 8개 특화 에이전트
│   ├── commands/                  # 슬래시 커맨드
│   ├── rules/                     # 코딩/워크플로우 규칙
│   └── hooks/                     # 자동화 훅
├── CLAUDE.md                      # Claude Code 프로젝트 설정
├── ROADMAP.md                     # 프로젝트 로드맵 (SSOT)
└── deploy.md                      # 수동 검증 항목
```

---

## 개발 환경

- **IDE**: Visual Studio 2022+
- **프레임워크**: .NET Framework 4.8
- **OS**: Windows 10/11
- **AI 도구**: Claude Code (에이전트 기반 개발)

---

## 라이선스

이 프로젝트는 개인 프로젝트입니다.
