# Phase 3: 시스템 트레이 + 서식/테마 + 컨텍스트 메뉴 완성 — 실행 계획

> **Status**: 계획 수립 완료 (2026-04-17)
> **ROADMAP 참조**: `ROADMAP.md` Phase 3
> **검토 리포트**: `phase3-product-owner-review.md`, `phase3-clinic-admin-review.md`, `phase3-ux-specialist-review.md`

---

## 개요

Phase 3은 MVP 완성을 목표로 한다. Phase 1-2에서 구축한 투명 윈도우 + 텍스트 입력 + 테두리/그림자 + 자동 저장 + 윈도우 관리 기반 위에, 시스템 트레이(F9), 서식 지원(F4), 다크/라이트 테마(F5), 컨텍스트 메뉴(F11) 전체 항목을 구현한다.

**이 Phase 완료 시 일상 사용 가능한 MVP 출시 가능.**

### 시스템 아키텍처 (Phase 3 추가 요소)

```
┌─────────────────────────────────────────────────────┐
│                    MainWindow.xaml                    │
│  ┌───────────────────────────────────────────────┐  │
│  │  드래그바 (Phase 1)                            │  │
│  ├───────────────────────────────────────────────┤  │
│  │  TextBox / OutlinedTextControl (Phase 1-2)    │  │
│  │  ├─ 글꼴/크기/색상 바인딩 ← [Phase 3 신규]    │  │
│  │  └─ 테마 색상 바인딩 ← [Phase 3 신규]         │  │
│  └───────────────────────────────────────────────┘  │
│  ContextMenu (Phase 1 골격 → Phase 3 완성)          │
│  ├─ 투명도 (단계별 선택)                             │
│  ├─ 배경 투명 모드 전환                              │
│  ├─ Always on Top 토글                              │
│  ├─ 테마 전환 (다크/라이트)           [Phase 3]      │
│  ├─ 글꼴 설정 서브메뉴               [Phase 3]      │
│  ├─ 텍스트 테두리/그림자 토글+색상    [Phase 3]      │
│  ├─ 자동저장 상태 표시               [Phase 3]      │
│  ├─ 최소화                           [Phase 3]      │
│  └─ 종료                                            │
└─────────────────────────────────────────────────────┘

┌─────────────────────────┐
│  TrayIconManager        │  [Phase 3 신규]
│  (WinForms NotifyIcon)  │
│  ├─ 트레이 아이콘 표시   │
│  ├─ 더블클릭 복원        │
│  └─ 우클릭 메뉴          │
│     ├─ 표시/숨김         │
│     ├─ Always on Top     │
│     └─ 종료              │
└─────────────────────────┘

┌─────────────────────────┐
│  ThemeManager           │  [Phase 3 신규]
│  ├─ DarkTheme 정의       │
│  ├─ LightTheme 정의      │
│  └─ ApplyTheme()         │
└─────────────────────────┘

┌─────────────────────────┐
│  SettingsManager        │  [Phase 2 확장]
│  (settings.json)        │
│  ├─ +FontFamily          │
│  ├─ +FontSize            │
│  ├─ +FontColor           │
│  ├─ +Theme               │
│  └─ 기존 설정 유지       │
└─────────────────────────┘
```

### 선행 조건

- Phase 1 완료: WPF 투명 윈도우, 텍스트 입력, 투명도 모드, Always on Top, 기본 컨텍스트 메뉴
- Phase 2 완료: 텍스트 테두리/그림자, 자동 저장, 설정 관리(settings.json), 윈도우 관리

---

## 검토팀 확정 파라미터 (2026-04-17)

> 이기획(PO), 김수진(행정 전문가), 한유엑(UX 전문가) 검토 완료

| 항목 | 원래 설계 | 확정값 | 근거 |
|------|----------|--------|------|
| 투명도 조절 UI | 슬라이더 (0~100%) | 단계별 선택: 20/40/60/80/100% (5단계), 현재 값 체크마크 | PO+UX: MVP에서 커스텀 슬라이더 비용 대비 효용 낮음 |
| 다크 테마 색상 | 미확정 | 글자 #FFFFFF, 배경 #1E1E1E/80% 투명도, 테두리 #000000 | PO: PRD 기본값 기반 확정 |
| 라이트 테마 색상 | 미확정 | 글자 #000000, 배경 #FFFFFF/80% 투명도, 테두리 #FFFFFF | PO: PRD 기본값 기반 확정 |
| 글꼴 선택 방식 | 시스템 전체 목록 | 프리셋 5~7개 + "더 보기..." → FontDialog | 행정+UX: 빈번 사용 시 빠른 선택 필요 |
| 글꼴 크기 | 자유 입력 | 프리셋 7단계 (10/12/14/16/20/24/32pt) + 직접 입력 | UX: 자주 쓰는 크기 빠른 선택 |
| 색상 선택 방식 | 자유 색상 선택기 | 프리셋 8~10색 팔레트 + "사용자 지정..." → ColorDialog | 행정: 반복 작업 빠른 선택 우선 |
| 컨텍스트 메뉴 서브메뉴 깊이 | 미정의 | 최대 1단계 | UX: 투명 오버레이 위 다단 서브메뉴 조작 불편 |
| 트레이 메뉴 항목 | 표시/숨김, Always on Top, 종료 | 표시/숨김, Always on Top, 종료 (Click-Through는 Phase 4) | PO: Phase 4 기능 선반영 금지 |
| 테마 전환 방식 | 미정의 | 일괄 변경 (깜빡임 방지) | UX: 순차 변경 시 flicker 발생 |

---

## Sprint 분할 계획

| Sprint | 주제 | 주요 작업 | 의존성 |
|--------|------|----------|--------|
| 1 ✅ | 시스템 트레이 + 서식 지원 | TrayIconManager, 글꼴/크기/색상 변경, 서식 저장 | Phase 1-2 완료 |
| 2 ✅ | 다크/라이트 테마 + 컨텍스트 메뉴 완성 | ThemeManager, 테마 전환, 컨텍스트 메뉴 F11 전체 항목 | Sprint 1 |

---

## Sprint 1 상세 — 시스템 트레이 + 서식 지원 ✅ 완료

> 완료: PR phase3-sprint1 → develop, 2026-04-17

### 목표
WinForms NotifyIcon 인터롭으로 시스템 트레이 기능을 구현하고, 글꼴/크기/색상 서식 변경 기능을 추가한다. 서식 설정은 settings.json에 저장/복원한다.

### 백엔드 (비즈니스 로직)

| 파일 | 작업 | 설명 |
|------|------|------|
| `TrayIconManager.cs` (신규) | 생성 | WinForms NotifyIcon 래퍼. 아이콘 생성, 트레이 메뉴 구성, 이벤트 핸들링, IDisposable 구현 |
| `SettingsManager.cs` (수정) | 확장 | FontFamily, FontSize, FontColor 속성 추가. 기존 저장/로드 로직에 서식 설정 포함 |
| `AppSettings.cs` (수정) | 확장 | 서식 관련 설정 모델 속성 추가 |

### 프론트엔드 (UI/XAML)

| 파일 | 작업 | 설명 |
|------|------|------|
| `MainWindow.xaml` (수정) | 확장 | TextBox/커스텀 컨트롤에 FontFamily, FontSize, Foreground 바인딩 추가 |
| `MainWindow.xaml.cs` (수정) | 확장 | 트레이 아이콘 초기화/해제, 최소화→트레이 숨김 로직, 서식 변경 이벤트 핸들러 |
| `App.xaml` (수정) | 확장 | System.Windows.Forms 참조 추가 확인 |

### TrayIconManager 설계

```csharp
public class TrayIconManager : IDisposable
{
    private System.Windows.Forms.NotifyIcon notifyIcon;
    private System.Windows.Forms.ContextMenuStrip trayMenu;

    // 이벤트: 표시/숨김 토글, 종료 요청
    public event EventHandler ToggleVisibilityRequested;
    public event EventHandler ExitRequested;

    public void Initialize(System.Drawing.Icon appIcon);
    public void UpdateAlwaysOnTopState(bool isTopmost);
    public void Dispose();  // 반드시 호출 — 트레이 아이콘 잔존 방지
}
```

### 서식 변경 구현 방식

- **글꼴**: 컨텍스트 메뉴 서브메뉴에 프리셋 5~7개 (맑은 고딕, 나눔고딕, 나눔바른고딕, D2Coding, Consolas, Arial, 굴림) + "더 보기..." → System.Windows.Forms.FontDialog
- **크기**: 컨텍스트 메뉴 서브메뉴에 7단계 프리셋 (10/12/14/16/20/24/32pt) + "직접 입력..."
- **색상**: 컨텍스트 메뉴 서브메뉴에 프리셋 팔레트 (흰색, 검정, 빨강, 파랑, 초록, 노랑, 회색, 주황, 보라, 하늘) + "사용자 지정..." → System.Windows.Forms.ColorDialog

### 재사용 자산

| 기존 모듈 | 재사용 방식 |
|----------|-----------|
| SettingsManager (Phase 2) | 속성 확장하여 서식 설정 저장/복원 |
| AppSettings 모델 (Phase 2) | 새 속성 추가 |
| ContextMenu 골격 (Phase 1) | 서브메뉴 추가 |
| 자동 저장 디바운스 (Phase 2) | 서식 변경 시에도 동일 패턴으로 저장 트리거 |

---

## Sprint 2 상세 — 다크/라이트 테마 + 컨텍스트 메뉴 완성 ✅ 완료

> 완료: PR phase3-sprint2 → develop, 2026-04-17 (빌드 경고 0, 오류 0)

### 목표
ThemeManager로 다크/라이트 테마를 정의하고 전환 기능을 구현한다. 컨텍스트 메뉴에 PRD F11의 모든 항목을 배치하여 MVP 메뉴를 완성한다.

### 백엔드 (비즈니스 로직)

| 파일 | 작업 | 설명 |
|------|------|------|
| `ThemeManager.cs` (신규) | 생성 | 다크/라이트 테마 정의, 테마 전환 로직, 일괄 색상 변경 |
| `ThemeDefinition.cs` (신규) | 생성 | 테마별 색상 세트 모델 (글자색, 배경색, 테두리색, 그림자색) |
| `SettingsManager.cs` (수정) | 확장 | Theme 속성 추가 (dark/light) |
| `AppSettings.cs` (수정) | 확장 | Theme 설정 모델 속성 추가 |

### 프론트엔드 (UI/XAML)

| 파일 | 작업 | 설명 |
|------|------|------|
| `MainWindow.xaml` (수정) | 확장 | 테마 바인딩 (배경색, 드래그바 색상 등) |
| `MainWindow.xaml.cs` (수정) | 확장 | 테마 전환 핸들러, 컨텍스트 메뉴 전체 항목 구성 |

### ThemeManager 설계

```csharp
public class ThemeManager
{
    public ThemeDefinition DarkTheme { get; }
    public ThemeDefinition LightTheme { get; }
    public ThemeDefinition CurrentTheme { get; private set; }

    public void ApplyTheme(ThemeDefinition theme, MainWindow window);
    public void ToggleTheme(MainWindow window);
}

public class ThemeDefinition
{
    public string Name { get; set; }           // "dark" / "light"
    public Color TextColor { get; set; }        // 글자색
    public Color BackgroundColor { get; set; }  // 배경색 (반투명)
    public double BackgroundOpacity { get; set; }  // 배경 투명도
    public Color OutlineColor { get; set; }     // 테두리색
    public Color ShadowColor { get; set; }      // 그림자색
    public Color DragBarColor { get; set; }     // 드래그바 색상
}
```

### 테마 색상 확정값

| 요소 | 다크 테마 | 라이트 테마 |
|------|----------|-----------|
| 글자색 | #FFFFFF | #000000 |
| 배경색 | #1E1E1E (80% 투명) | #FFFFFF (80% 투명) |
| 테두리색 | #000000 | #FFFFFF |
| 그림자색 | #000000 | #808080 |
| 드래그바 | #333333 (90% 불투명) | #E0E0E0 (90% 불투명) |

### 컨텍스트 메뉴 최종 구성 (PRD F11 완성)

```
[컨텍스트 메뉴]
├─ 투명도 ▸ 20% / 40% / 60% / ✓80% / 100%
├─ ─────────────
├─ ☑ 배경 투명 모드
├─ ☑ Always on Top
├─ ─────────────
├─ 테마: 다크 ↔ 라이트
├─ 글꼴 ▸ ✓맑은 고딕 / 나눔고딕 / ... / 더 보기...
├─ 글자 크기 ▸ 10 / 12 / ✓14 / 16 / 20 / 24 / 32 / 직접 입력...
├─ 글자 색상 ▸ [색상 팔레트] / 사용자 지정...
├─ ─────────────
├─ ☑ 텍스트 테두리
├─ 테두리 색상 ▸ [색상 팔레트] / 사용자 지정...
├─ ─────────────
├─ 💾 자동 저장됨 (비활성)
├─ ─────────────
├─ 최소화
└─ 종료
```

### 재사용 자산

| 기존 모듈 | 재사용 방식 |
|----------|-----------|
| SettingsManager (Phase 2 + Sprint 1 확장) | Theme 속성 추가 |
| OutlinedTextControl (Phase 2) | 테마 전환 시 색상 업데이트 메서드 호출 |
| ContextMenu 골격 (Phase 1 + Sprint 1 확장) | 나머지 항목 추가 |
| 서식 변경 로직 (Sprint 1) | 테마 전환 시 글자색 자동 변경에 재활용 |

---

## 미해결 사항 / 리스크

| 항목 | 심각도 | 관련 Sprint | 대응 방안 |
|------|--------|------------|----------|
| ~~서식 변경 시 텍스트 테두리 재렌더링 성능~~ | ~~⚠️ 중간~~ | ~~Sprint 1~~ | ~~글꼴/크기 변경 시 OutlinedTextControl 재렌더링 성능 측정. 문제 시 테두리 OFF 상태에서만 변경 후 ON 전환~~ ✅ 해결 (Sprint 1 구현에서 FontFamily/FontSize 즉시 바인딩 방식으로 성능 문제 없음 확인) |
| ~~테마 전환 시 깜빡임~~ | ~~⚠️ 중간~~ | ~~Sprint 2~~ | ~~일괄 변경 패턴 적용. BeginInit/EndInit 또는 Dispatcher.Invoke로 한 프레임 내 처리~~ ✅ 해결 (ApplyTheme()에서 SolidColorBrush.Freeze() 호출로 일괄 변경 구현) |
| ~~NotifyIcon Dispose 누락 시 트레이 잔존~~ | ~~⚠️ 중간~~ | ~~Sprint 1~~ | ~~Application.Exit, AppDomain.UnhandledException 모두에서 Dispose 호출 보장~~ ✅ 해결 (App.xaml.cs EmergencyDisposeTray + Window_Closing 이중 보장 구현) |
| ~~글꼴 프리셋 중 시스템 미설치 글꼴~~ | ~~⚠️ 낮음~~ | ~~Sprint 1~~ | ~~프리셋 표시 전 InstalledFontCollection으로 설치 여부 확인, 미설치 시 목록에서 제외~~ ✅ 해결 (FontHelper.IsInstalled + HashSet 캐시로 구현) |
| 투명도 슬라이더 Backlog | ⚠️ 낮음 | — | Phase 4 또는 향후 Backlog에 "투명도 커스텀 슬라이더 UI 개선" 추가 |
| ~~FontDialog/ColorDialog Dispose 미호출~~ | ~~⚠️ 낮음~~ | ~~Sprint 1 → Sprint 2에서 개선 권장~~ | ~~MainWindow.xaml.cs FontDialog_Click, ColorDialog_Click에서 `using` 블록 또는 명시적 `Dispose()` 호출로 개선~~ ✅ 해결 (Sprint 2에서 모든 Dialog를 `using` 블록으로 변경) |
| ~~XAML 초기 IsChecked와 settings.json 불일치 가능성~~ | ~~⚠️ 낮음~~ | ~~Sprint 1 → Sprint 2에서 개선 권장~~ | ~~MainWindow.xaml의 `BackgroundTransparentMenuItem IsChecked="True"` 하드코딩. Window_Loaded에서 settings 로드 후 동기화 로직 추가 권장~~ ✅ 해결 (Sprint 2에서 SyncMenuCheckedStates() 구현, Window_Loaded + ContextMenu_Opened에서 호출) |
| null 체크 누락 — BackgroundTransparentMenuItem_Click 등 | ⚠️ 낮음 | Sprint 3 이후에서 개선 권장 | `sender as MenuItem` null 체크 없이 `.IsChecked` 접근. `BackgroundTransparentMenuItem_Click`, `OutlineMenuItem_Click`, `ShadowMenuItem_Click`, `OpacityMenuItem_Click`에서 동일 패턴. 런타임에서 sender가 MenuItem이 아닌 경우 NullReferenceException 가능. Sprint 4 이후 개선 권장 |

---

## 완료 기준 (Phase 전체)

| 항목 | 기준 | 상태 |
|------|------|------|
| 시스템 트레이 아이콘 | 트레이 아이콘 표시, 최소화→트레이 숨김, 더블클릭 복원 동작 | ✅ 완료 (Sprint 1) |
| 트레이 메뉴 | 표시/숨김, Always on Top, 종료 항목 동작 | ✅ 완료 (Sprint 1) |
| 글꼴 변경 | 프리셋 + FontDialog로 글꼴 변경, 텍스트에 즉시 반영 | ✅ 완료 (Sprint 1) |
| 글자 크기 변경 | 프리셋 7단계 + 직접 입력, 텍스트에 즉시 반영 | ✅ 완료 (Sprint 1) |
| 글자 색상 변경 | 프리셋 팔레트 + ColorDialog, 텍스트에 즉시 반영 | ✅ 완료 (Sprint 1) |
| 서식 저장/복원 | 변경된 서식이 settings.json에 저장, 재실행 시 복원 | ✅ 완료 (Sprint 1) |
| 다크 테마 | 글자 #FFFFFF, 배경 #1E1E1E/80%, 테두리 #000000 적용 | ✅ 완료 (Sprint 2) |
| 라이트 테마 | 글자 #000000, 배경 #FFFFFF/80%, 테두리 #FFFFFF 적용 | ✅ 완료 (Sprint 2) |
| 테마 전환 | 토글 시 모든 색상 일괄 변경, 텍스트 내용 유지, 깜빡임 없음 | ✅ 완료 (Sprint 2) |
| 컨텍스트 메뉴 완성 | PRD F11 전체 항목 접근 가능 (Click-Through 제외 — Phase 4) | ✅ 완료 (Sprint 2) |
| 시작 시간 | 2초 이내 (System.Windows.Forms 참조 추가 후에도) | ⬜ 수동 검증 필요 |
| NotifyIcon 정리 | 정상/비정상 종료 시 트레이 아이콘 잔존 없음 | ✅ 완료 (Sprint 1) |
