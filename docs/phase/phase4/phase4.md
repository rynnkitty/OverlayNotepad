# Phase 4: 글로벌 핫키 + Click-Through + 최종 마무리 — 실행 계획

> **Status**: 계획 수립 완료 (2026-04-17)
> **ROADMAP 참조**: `ROADMAP.md` Phase 4
> **검토 리포트**: `phase4-product-owner-review.md`, `phase4-clinic-admin-review.md`, `phase4-ux-specialist-review.md`

---

## 개요

Phase 1~3에서 구축한 투명 메모장의 기반 위에, 나머지 PRD 기능(F6 Click-Through, F10 글로벌 핫키)을 구현하고 전체 품질을 다듬어 배포 가능한 상태로 완성한다.

```
[사용자 입력]
    |
    v
┌─────────────────────────────────────────────┐
│  MainWindow (WPF)                           │
│  ┌─────────────────────────────────────────┐│
│  │ HwndSource + WndProc Hook               ││
│  │  ├─ WM_HOTKEY 메시지 처리               ││
│  │  │   ├─ HOTKEY_TOGGLE_VISIBILITY        ││
│  │  │   └─ HOTKEY_TOGGLE_CLICKTHROUGH      ││
│  │  └─ RegisterHotKey / UnregisterHotKey   ││
│  ├─────────────────────────────────────────┤│
│  │ Click-Through 관리                      ││
│  │  ├─ SetWindowLong (GWL_EXSTYLE)         ││
│  │  │   └─ WS_EX_TRANSPARENT 토글         ││
│  │  ├─ 시각 피드백 (테두리 + 상단 바 텍스트)││
│  │  └─ 투명도 하한 강제 (Opacity >= 0.2)   ││
│  ├─────────────────────────────────────────┤│
│  │ 기존 기능 (Phase 1~3)                   ││
│  │  ├─ 투명 윈도우 + TextBox               ││
│  │  ├─ 테두리/그림자 효과                  ││
│  │  ├─ 자동 저장 + 설정 관리               ││
│  │  ├─ 시스템 트레이                       ││
│  │  ├─ 서식/테마                           ││
│  │  └─ 컨텍스트 메뉴                       ││
│  └─────────────────────────────────────────┘│
└─────────────────────────────────────────────┘
    |
    v
[시스템 트레이] ← Click-Through 상태 반영 (아이콘 변경)
```

---

## 검토팀 확정 파라미터 (2026-04-17)

> 검토 참여: 이기획 (PO), 김수진 (최종 사용자), 한유엑 (UX 전문가)

| 항목 | 원래 설계 | 확정값 | 근거 |
|------|----------|--------|------|
| 핫키: 표시/숨김 | Ctrl+Shift+N | **Ctrl+Shift+N** (확정) | PRD 기본값, "N"ote 연상 기억 |
| 핫키: Click-Through | Ctrl+Shift+T | **Ctrl+Shift+T** (확정) | PRD 기본값, "T"hrough 연상 기억 |
| 핫키 사용자 변경 | 미정 | **MVP 제외** (백로그) | PO: PRD에 "고려"로만 명시, 고정값으로 충분 |
| Click-Through 시각 피드백 | 테두리 색상 변경 | **빨간 점선 테두리 + 상단 바 "CLICK-THROUGH" 텍스트 + 트레이 아이콘 변경** | UX: 색상 단독은 인지 부족, 3중 신호 필요 (색맹 고려) |
| Click-Through 시 투명도 하한 | 없음 | **최소 20% (Opacity >= 0.2)** | UX+PO: 완전 투명 + Click-Through = 제어 불가 위험 |
| Click-Through 최초 활성화 | 즉시 전환 | **첫 1회 트레이 벌룬으로 해제 방법 안내** | 사용자: 학습 곡선 완화, "갇힘" 방지 |
| 핫키 충돌 알림 | 미정 | **시작 시 트레이 벌룬 1회 표시** | UX: 무반응 방지, 등록 실패 시 사용자에게 명확히 알림 |
| 핫키 등록 실패 시 Click-Through | 허용 | **Click-Through 비활성화 강제** | PO: 핫키 없이는 Click-Through 해제 불가, 안전장치 필수 |
| 성능 기준: 메모리 | 80MB | **80MB** (확정) | PRD 비기능 요구사항 |
| 성능 기준: 시작 시간 | 2초 | **2초** (확정) | PRD 비기능 요구사항 |
| 성능 기준: 유휴 CPU | 0% | **0%** (확정) | PRD 비기능 요구사항 |
| DPI 인식 | app.manifest | **System DPI Aware** (확정) | PRD: Per-Monitor 불필요, manifest 선언으로 충분 |

---

## Sprint 분할 계획

| Sprint | 주제 | 주요 작업 | 의존성 |
|--------|------|----------|--------|
| 1 | 글로벌 핫키 + Click-Through | RegisterHotKey 인터롭, WS_EX_TRANSPARENT 토글, 시각 피드백, 트레이 연동 | Phase 3 완료 (시스템 트레이, 컨텍스트 메뉴) |
| 2 | 최종 마무리 + 배포 준비 | 통합 테스트, 성능 최적화, DPI, 앱 아이콘, 단일 EXE, 엣지 케이스 | Sprint 1 |

---

## Sprint 1 상세 — 글로벌 핫키 + Click-Through

### 백엔드 (Win32 인터롭)

| 파일 | 내용 |
|------|------|
| `Interop/NativeMethods.cs` (수정) | RegisterHotKey, UnregisterHotKey, SetWindowLong, GetWindowLong P/Invoke 선언 추가 |
| `Services/HotkeyService.cs` (신규) | 글로벌 핫키 등록/해제/WndProc 훅 관리 클래스 |
| `Services/ClickThroughService.cs` (신규) | WS_EX_TRANSPARENT 토글, 투명도 하한 강제 로직 |

#### HotkeyService 설계
```
- RegisterHotKey(hwnd, id, MOD_CONTROL | MOD_SHIFT, VK_N) → 표시/숨김
- RegisterHotKey(hwnd, id, MOD_CONTROL | MOD_SHIFT, VK_T) → Click-Through
- HwndSource.AddHook(WndProc) → WM_HOTKEY 메시지 수신
- 등록 실패 시: 트레이 벌룬 알림 + Click-Through 핫키 실패 시 Click-Through 기능 비활성화
- Dispose 시: UnregisterHotKey 호출
```

#### ClickThroughService 설계
```
- SetWindowLong(GWL_EXSTYLE, WS_EX_TRANSPARENT | WS_EX_LAYERED) → 클릭 통과 활성화
- SetWindowLong(GWL_EXSTYLE, ~WS_EX_TRANSPARENT) → 클릭 통과 비활성화
- 활성화 시: Window.Opacity >= 0.2 강제
- 상태 변경 이벤트 발행 → UI, 트레이, 설정 연동
```

### 프론트엔드 (WPF)

| 파일 | 내용 |
|------|------|
| `MainWindow.xaml` (수정) | Click-Through 상태 시각 피드백 UI 요소 추가 (상단 바 텍스트, 테두리 스타일) |
| `MainWindow.xaml.cs` (수정) | HotkeyService, ClickThroughService 초기화 및 이벤트 핸들링 |
| `Resources/Styles.xaml` (수정) | Click-Through 모드 테두리 스타일 (빨간 점선) 추가 |

#### Click-Through 시각 피드백 구현
- **테두리**: `Border.BorderBrush` → 빨간색 (`#FF4444`), `BorderDashArray` → 점선 패턴
- **상단 바**: 드래그 영역에 "CLICK-THROUGH" 텍스트 표시 (Click-Through 모드에서는 드래그 불필요하므로 영역 재활용)
- **트레이 아이콘**: Click-Through 시 별도 아이콘 또는 색상 오버레이

### 트레이 메뉴 연동

| 파일 | 내용 |
|------|------|
| 트레이 메뉴 (수정) | "Click-Through" 토글 항목 추가, 상태 체크마크 표시 |

### 컨텍스트 메뉴 연동

| 파일 | 내용 |
|------|------|
| 컨텍스트 메뉴 (수정) | "Click-Through" 토글 항목 — 단, Click-Through 활성 시 메뉴 자체가 열리지 않으므로 비활성화 안내 텍스트 |

### 설정 저장 연동

| 파일 | 내용 |
|------|------|
| `Models/AppSettings.cs` (수정) | `IsClickThrough` (bool, 기본값: false) 속성 추가 |
| `Services/SettingsService.cs` (수정) | Click-Through 상태 저장/복원 |

### 재사용 자산

| 기존 모듈 | 재사용 방법 |
|----------|------------|
| Phase 1 HwndSource 인터롭 패턴 | RegisterHotKey도 동일한 HwndSource.AddHook 패턴 사용 |
| Phase 2 SettingsService | Click-Through 상태를 기존 설정 JSON에 추가 |
| Phase 3 시스템 트레이 (NotifyIcon) | 트레이 메뉴에 Click-Through 항목 추가, 벌룬 알림 활용 |
| Phase 3 컨텍스트 메뉴 | 메뉴 항목 추가 |

---

## Sprint 2 상세 — 최종 마무리 + 배포 준비

### 통합 테스트

| 테스트 항목 | 검증 내용 |
|------------|----------|
| 글로벌 핫키 | 다른 앱 포커스 상태에서 Ctrl+Shift+N/T 동작 확인 |
| Click-Through | 마우스 클릭이 뒤의 앱에 전달되는지 확인 |
| Click-Through 시각 피드백 | 빨간 점선 테두리 + 상단 바 텍스트 + 트레이 아이콘 변경 확인 |
| Click-Through + 투명도 조합 | 전체 투명도 10%에서 Click-Through 시 20%로 강제되는지 확인 |
| 핫키 충돌 | 다른 앱에서 같은 핫키 사용 시 알림 표시 확인 |
| 전체 기능 연동 | 투명도, 테마, 서식, 자동저장, 트레이 등 모든 기능의 조합 동작 |
| 시작/종료 사이클 | 설정 저장 → 종료 → 재시작 → 설정 복원 전체 흐름 |
| 비정상 종료 복구 | 프로세스 kill 후 재시작 시 데이터 복원 |

### 성능 최적화

| 파일 | 내용 |
|------|------|
| `app.manifest` (수정) | `<dpiAware>true</dpiAware>` DPI 인식 선언 |
| 프로젝트 설정 (수정) | 앱 아이콘 (.ico) 설정 |
| 빌드 설정 | Release 빌드 확인, 단일 EXE 출력 확인 |

#### 성능 측정 체크리스트
- 메모리: 작업 관리자에서 유휴 상태 메모리 80MB 이하 확인
- CPU: 유휴 상태 CPU 0% 확인 (불필요한 타이머/폴링 제거)
- 시작 시간: 앱 실행 후 윈도우 표시까지 2초 이내 확인

### 엣지 케이스 수정

| 케이스 | 처리 |
|--------|------|
| 핫키 등록 실패 + Click-Through | Click-Through 기능 자체를 비활성화 (메뉴 항목 회색 처리) |
| Click-Through 상태에서 앱 종료 | 다음 시작 시 Click-Through OFF로 시작 (안전 기본값) |
| 다중 모니터 → 단일 모니터 전환 | Phase 2 화면 범위 보정 로직 활용 |
| DPI 변경 중 앱 실행 | WPF 기본 DPI 스케일링으로 처리 |

### DPI 인식 설정

```xml
<!-- app.manifest -->
<application xmlns="urn:schemas-microsoft-com:asm.v3">
  <windowsSettings>
    <dpiAware xmlns="http://schemas.microsoft.com/SMI/2005/WindowsSettings">true</dpiAware>
  </windowsSettings>
</application>
```

### 앱 아이콘

| 파일 | 내용 |
|------|------|
| `Resources/app.ico` (신규) | 앱 아이콘 — 투명 메모장 컨셉의 간단한 아이콘 |
| 프로젝트 파일 (수정) | ApplicationIcon 설정 |

### 배포 확인

- .NET Framework 4.8은 Windows 10 1903+ / Windows 11에 기본 포함
- System.Windows.Forms.dll은 .NET Framework GAC에 포함 (별도 배포 불필요)
- Release 빌드 시 단일 EXE + .pdb + .exe.config 출력
- .exe.config와 .pdb는 선택 사항 (EXE만으로 실행 가능)

---

## 미해결 사항 / 리스크

| 항목 | 심각도 | 대응 방안 | Sprint |
|------|--------|----------|--------|
| 핫키 충돌 (다른 앱 선점) | ⚠️ 중간 | 트레이 벌룬 알림 + Click-Through 기능 비활성화 | Sprint 1 |
| Click-Through "갇힘" (핫키/트레이 모름) | ⚠️ 중간 | 첫 1회 벌룬 안내 + 트레이 아이콘 상태 변경 | Sprint 1 |
| 전체 투명 + Click-Through 조합 위험 | ⚠️ 중간 | 투명도 하한 20% 강제 | Sprint 1 |
| 다중 모니터 DPI 차이 | 낮음 | PRD 범위 밖, 백로그 | — |
| 앱 아이콘 디자인 | 낮음 | 기본 아이콘으로 시작, 추후 개선 | Sprint 2 |
| Click-Through 상태에서 앱 재시작 | 낮음 | 재시작 시 Click-Through OFF 기본값 (안전 설계) | Sprint 1 |

---

## 완료 기준 (Phase 전체)

| 항목 | 기준 | 상태 |
|------|------|------|
| 글로벌 핫키 동작 | 다른 앱 포커스 상태에서 Ctrl+Shift+N으로 표시/숨김 토글 | ⬜ |
| Click-Through 동작 | Ctrl+Shift+T로 Click-Through 토글, 마우스 클릭이 뒤의 앱에 전달 | ⬜ |
| Click-Through 시각 피드백 | 빨간 점선 테두리 + 상단 바 텍스트 + 트레이 아이콘 변경 | ⬜ |
| Click-Through 안전장치 | 투명도 하한 20%, 핫키 실패 시 기능 비활성화, 재시작 시 OFF | ⬜ |
| 트레이 메뉴 연동 | Click-Through 토글 항목 동작 및 상태 표시 | ⬜ |
| 핫키 충돌 처리 | 등록 실패 시 트레이 벌룬 알림 표시 | ⬜ |
| 메모리 사용량 | 80MB 이하 (유휴 상태) | ⬜ |
| 시작 시간 | 2초 이내 | ⬜ |
| 유휴 CPU | 0%에 근접 | ⬜ |
| DPI 인식 | app.manifest dpiAware 선언, Win10/11 정상 표시 | ⬜ |
| 앱 아이콘 | EXE 파일 및 작업 표시줄에 아이콘 표시 | ⬜ |
| 단일 EXE 배포 | 설치 없이 EXE만으로 실행 가능 | ⬜ |
| Windows 10/11 지원 | 양쪽 OS에서 전체 기능 정상 동작 | ⬜ |
