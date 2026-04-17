# Sprint 1: 글로벌 핫키 + Click-Through (Phase 4)

**Goal:** RegisterHotKey Win32 API 인터롭으로 글로벌 핫키(표시/숨김, Click-Through 토글)를 등록하고, WS_EX_TRANSPARENT 인터롭으로 Click-Through 모드를 구현하며, 시각 피드백(빨간 점선 테두리 + 상단 바 텍스트 + 트레이 아이콘 변경)과 안전장치(투명도 하한 20%, 핫키 실패 시 기능 비활성화)를 완성한다.

**Architecture:** HotkeyService가 HwndSource.AddHook으로 WM_HOTKEY 메시지를 수신하여 글로벌 핫키를 처리하고, ClickThroughService가 SetWindowLong으로 WS_EX_TRANSPARENT 윈도우 스타일을 토글한다. 두 서비스 모두 MainWindow에서 초기화되며, 기존 TrayIconManager와 컨텍스트 메뉴에 Click-Through 항목을 추가한다. Click-Through 상태는 AppSettings에 저장하되, 재시작 시 항상 OFF로 시작한다(안전 설계).

**Tech Stack:** .NET Framework 4.8, WPF, Win32 API (RegisterHotKey, UnregisterHotKey, SetWindowLong, GetWindowLong), HwndSource

**Sprint 기간:** 2026-04-17 ~ 2026-04-17 (완료)
**상태:** ✅ 완료
**PR:** https://github.com/rynnkitty/OverlayNotepad/pull/ (GitHub 웹에서 수동 생성 필요, phase4-sprint1 → develop)
**이전 스프린트:** Phase 3 Sprint 2 (다크/라이트 테마 + 컨텍스트 메뉴 완성)
**브랜치명:** `phase4-sprint1`

---

## 제외 범위

- 핫키 사용자 변경 기능 -- MVP 제외, Backlog (Phase 4 확정 파라미터)
- 성능 최적화 (메모리/CPU/시작 시간) -- Sprint 2
- DPI 인식 설정 (app.manifest) -- Sprint 2
- 앱 아이콘 설정 -- Sprint 2
- 단일 EXE 빌드 확인 -- Sprint 2
- 전체 기능 통합 테스트 -- Sprint 2
- 엣지 케이스 전체 수정 -- Sprint 2

## 확정 파라미터 (Phase 4 문서 기준)

| 항목 | 값 | 근거 |
|------|-----|------|
| 표시/숨김 핫키 | Ctrl+Shift+N | PRD 기본값, "N"ote 연상 |
| Click-Through 핫키 | Ctrl+Shift+T | PRD 기본값, "T"hrough 연상 |
| Click-Through 시각 피드백 | 빨간 점선 테두리 + 상단 바 "CLICK-THROUGH" 텍스트 + 트레이 아이콘 변경 | UX 확정: 3중 신호 (색맹 고려) |
| Click-Through 시 투명도 하한 | Opacity >= 0.2 (20%) | UX+PO: 완전 투명 + Click-Through = 제어 불가 위험 |
| Click-Through 최초 활성화 | 첫 1회 트레이 벌룬으로 해제 방법 안내 | 사용자 학습 곡선 완화 |
| 핫키 충돌 알림 | 시작 시 트레이 벌룬 1회 표시 | 무반응 방지 |
| 핫키 등록 실패 시 Click-Through | Click-Through 비활성화 강제 | PO: 핫키 없이는 해제 불가, 안전장치 필수 |
| 재시작 시 Click-Through 상태 | 항상 OFF | 안전 기본값 |

## 실행 플랜

### Phase 1 (순차 -- Win32 인터롭 기반)
| Task | 설명 | 대상 | skill |
|------|------|------|-------|
| Task 1 | NativeMethods P/Invoke 선언 + HotkeyService | 데스크톱 | -- |
| Task 2 | ClickThroughService | 데스크톱 | -- |

### Phase 2 (순차 -- MainWindow 통합)
| Task | 설명 | 대상 | skill |
|------|------|------|-------|
| Task 3 | Click-Through 시각 피드백 UI | 데스크톱 | -- |
| Task 4 | MainWindow 통합 + 트레이/메뉴 연동 + 설정 저장 | 데스크톱 | `feature-dev:feature-dev` |

### Phase 3 (순차 -- 검증)
| Task | 설명 | 대상 | skill |
|------|------|------|-------|
| Task 5 | 통합 검증 + 엣지 케이스 + 안전장치 확인 | 데스크톱 | -- |

> **병렬 불가**: 모든 Task가 MainWindow.xaml / MainWindow.xaml.cs를 공유하거나 순차 의존성이 있으므로 순차 실행 필수.

---

### Task 1: NativeMethods P/Invoke 선언 + HotkeyService

**skill:** --

**Files:**
- Create: `src/OverlayNotepad/Interop/NativeMethods.cs`
- Create: `src/OverlayNotepad/Services/HotkeyService.cs`
- Modify: `src/OverlayNotepad/OverlayNotepad.csproj` (새 파일 Compile 항목 추가)

**Step 1: NativeMethods P/Invoke 선언**

- `src/OverlayNotepad/Interop/NativeMethods.cs` 생성
- 네임스페이스: `OverlayNotepad.Interop`
- 클래스: `NativeMethods` (internal static)
- P/Invoke 선언:
  - `[DllImport("user32.dll")] static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk)`
  - `[DllImport("user32.dll")] static extern bool UnregisterHotKey(IntPtr hWnd, int id)`
  - `[DllImport("user32.dll")] static extern int GetWindowLong(IntPtr hWnd, int nIndex)`
  - `[DllImport("user32.dll")] static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong)`
- 상수 정의:
  - `MOD_CONTROL = 0x0002`, `MOD_SHIFT = 0x0004`
  - `VK_N = 0x4E`, `VK_T = 0x54`
  - `WM_HOTKEY = 0x0312`
  - `GWL_EXSTYLE = -20`
  - `WS_EX_TRANSPARENT = 0x00000020`
  - `WS_EX_LAYERED = 0x00080000`
- 핫키 ID 상수:
  - `HOTKEY_TOGGLE_VISIBILITY = 1`
  - `HOTKEY_TOGGLE_CLICKTHROUGH = 2`

**Step 2: HotkeyService 클래스 구현**

- `src/OverlayNotepad/Services/HotkeyService.cs` 생성
- 네임스페이스: `OverlayNotepad.Services`
- 클래스: `HotkeyService : IDisposable` (public)
- 필드:
  - `private IntPtr _hwnd` -- 윈도우 핸들
  - `private HwndSource _hwndSource` -- WPF HwndSource
  - `private bool _visibilityHotkeyRegistered` -- 표시/숨김 핫키 등록 성공 여부
  - `private bool _clickThroughHotkeyRegistered` -- Click-Through 핫키 등록 성공 여부
- 이벤트:
  - `public event EventHandler ToggleVisibilityRequested` -- 표시/숨김 토글 요청
  - `public event EventHandler ToggleClickThroughRequested` -- Click-Through 토글 요청
- 프로퍼티:
  - `public bool IsClickThroughHotkeyRegistered => _clickThroughHotkeyRegistered` -- 외부에서 핫키 등록 상태 확인용

**Step 3: Initialize() 메서드 구현**

- `public void Initialize(Window window)` 메서드
- 로직:
  1. `WindowInteropHelper helper = new WindowInteropHelper(window)`
  2. `_hwnd = helper.Handle`
  3. `_hwndSource = HwndSource.FromHwnd(_hwnd)`
  4. `_hwndSource.AddHook(WndProc)` -- WM_HOTKEY 메시지 후킹
  5. 표시/숨김 핫키 등록:
     - `_visibilityHotkeyRegistered = NativeMethods.RegisterHotKey(_hwnd, NativeMethods.HOTKEY_TOGGLE_VISIBILITY, NativeMethods.MOD_CONTROL | NativeMethods.MOD_SHIFT, NativeMethods.VK_N)`
  6. Click-Through 핫키 등록:
     - `_clickThroughHotkeyRegistered = NativeMethods.RegisterHotKey(_hwnd, NativeMethods.HOTKEY_TOGGLE_CLICKTHROUGH, NativeMethods.MOD_CONTROL | NativeMethods.MOD_SHIFT, NativeMethods.VK_T)`
  7. 등록 실패 시 실패한 핫키 정보를 `List<string> failedHotkeys`에 수집하여 반환값 또는 프로퍼티로 노출

**Step 4: WndProc 훅 구현**

- `private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)` 메서드
- 로직:
  - `if (msg == NativeMethods.WM_HOTKEY)`
    - `int hotkeyId = wParam.ToInt32()`
    - `if (hotkeyId == NativeMethods.HOTKEY_TOGGLE_VISIBILITY)` -> `ToggleVisibilityRequested?.Invoke(this, EventArgs.Empty)`
    - `if (hotkeyId == NativeMethods.HOTKEY_TOGGLE_CLICKTHROUGH)` -> `ToggleClickThroughRequested?.Invoke(this, EventArgs.Empty)`
    - `handled = true`
  - `return IntPtr.Zero`

**Step 5: GetFailedHotkeys() 메서드 구현**

- `public List<string> GetFailedHotkeys()` 메서드
- 로직:
  - 등록 실패한 핫키 이름을 수집하여 반환
  - `if (!_visibilityHotkeyRegistered)` -> "Ctrl+Shift+N (표시/숨김)" 추가
  - `if (!_clickThroughHotkeyRegistered)` -> "Ctrl+Shift+T (Click-Through)" 추가
  - 빈 목록이면 모두 성공

**Step 6: Dispose 구현**

- `public void Dispose()` 메서드
- 로직:
  1. `if (_visibilityHotkeyRegistered)` -> `NativeMethods.UnregisterHotKey(_hwnd, NativeMethods.HOTKEY_TOGGLE_VISIBILITY)`
  2. `if (_clickThroughHotkeyRegistered)` -> `NativeMethods.UnregisterHotKey(_hwnd, NativeMethods.HOTKEY_TOGGLE_CLICKTHROUGH)`
  3. `_hwndSource?.RemoveHook(WndProc)`

**Step 7: csproj에 파일 추가**

- `src/OverlayNotepad/OverlayNotepad.csproj` 수정
- `<Compile Include="Interop\NativeMethods.cs" />` 추가
- `<Compile Include="Services\HotkeyService.cs" />` 추가

**Step 8: 빌드 검증**

```bash
"/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "D:/99.실험실/OverlayNotepad/OverlayNotepad.sln" /p:Configuration=Debug /t:Build
```
- 예상: Build succeeded, 0 Errors

**Step 9: 커밋**

```
git add src/OverlayNotepad/Interop/NativeMethods.cs src/OverlayNotepad/Services/HotkeyService.cs src/OverlayNotepad/OverlayNotepad.csproj
git commit -m "feat(phase4-sprint1): task1 - NativeMethods P/Invoke + HotkeyService 글로벌 핫키"
```

**완료 기준:**
- ✅ NativeMethods에 RegisterHotKey/UnregisterHotKey/SetWindowLong/GetWindowLong 선언
- ✅ HotkeyService.Initialize()로 핫키 등록 + WndProc 후킹
- ✅ 핫키 등록 실패 여부를 GetFailedHotkeys()로 조회 가능
- ✅ IDisposable로 UnregisterHotKey 보장
- ✅ MSBuild 빌드 성공

---

### Task 2: ClickThroughService

**skill:** --

**Files:**
- Create: `src/OverlayNotepad/Services/ClickThroughService.cs`
- Modify: `src/OverlayNotepad/Models/AppSettings.cs` (IsClickThrough 속성 추가)
- Modify: `src/OverlayNotepad/OverlayNotepad.csproj` (새 파일 Compile 항목 추가)

**Step 1: AppSettings에 IsClickThrough 속성 추가**

- `src/OverlayNotepad/Models/AppSettings.cs` 수정
- AppSettings 클래스에 추가:
  - `public bool IsClickThrough { get; set; }` -- 기본값: false
  - 주의: 이 값은 저장되지만 로드 시 항상 false로 강제 (안전 설계)

**Step 2: ClickThroughService 클래스 구현**

- `src/OverlayNotepad/Services/ClickThroughService.cs` 생성
- 네임스페이스: `OverlayNotepad.Services`
- 클래스: `ClickThroughService` (public)
- 필드:
  - `private IntPtr _hwnd` -- 윈도우 핸들
  - `private bool _isEnabled` -- Click-Through 활성 상태
  - `private bool _isFirstActivation = true` -- 최초 활성화 여부 (벌룬 안내용)
  - `private bool _isAvailable = true` -- 기능 사용 가능 여부 (핫키 등록 실패 시 false)
- 이벤트:
  - `public event EventHandler<ClickThroughChangedEventArgs> StateChanged` -- 상태 변경 이벤트
  - ClickThroughChangedEventArgs: `bool IsEnabled` 프로퍼티, `bool IsFirstActivation` 프로퍼티
- 프로퍼티:
  - `public bool IsEnabled => _isEnabled`
  - `public bool IsAvailable => _isAvailable`

**Step 3: Initialize() 메서드**

- `public void Initialize(IntPtr hwnd, bool clickThroughHotkeyAvailable)` 메서드
- 로직:
  1. `_hwnd = hwnd`
  2. `_isAvailable = clickThroughHotkeyAvailable` -- 핫키 등록 성공 시에만 사용 가능
  3. `_isEnabled = false` -- 시작 시 항상 OFF

**Step 4: Toggle() 메서드**

- `public bool Toggle()` 메서드 -- Click-Through 토글, 성공 시 true 반환
- 로직:
  1. `if (!_isAvailable) return false` -- 비활성화 상태면 무시
  2. `_isEnabled = !_isEnabled`
  3. `if (_isEnabled)` -> Enable()
  4. `else` -> Disable()
  5. 이벤트 발행: `StateChanged?.Invoke(this, new ClickThroughChangedEventArgs(_isEnabled, _isFirstActivation))`
  6. `if (_isEnabled && _isFirstActivation)` -> `_isFirstActivation = false`
  7. `return true`

**Step 5: Enable() 내부 메서드**

- `private void Enable()` 메서드
- 로직:
  1. `int exStyle = NativeMethods.GetWindowLong(_hwnd, NativeMethods.GWL_EXSTYLE)`
  2. `NativeMethods.SetWindowLong(_hwnd, NativeMethods.GWL_EXSTYLE, exStyle | NativeMethods.WS_EX_TRANSPARENT)`
  - 주의: WS_EX_LAYERED는 WPF AllowsTransparency=True 시 이미 설정되어 있음, 추가 설정 불필요

**Step 6: Disable() 내부 메서드**

- `private void Disable()` 메서드
- 로직:
  1. `int exStyle = NativeMethods.GetWindowLong(_hwnd, NativeMethods.GWL_EXSTYLE)`
  2. `NativeMethods.SetWindowLong(_hwnd, NativeMethods.GWL_EXSTYLE, exStyle & ~NativeMethods.WS_EX_TRANSPARENT)`

**Step 7: EnforceOpacityFloor() 정적 메서드**

- `public static double EnforceOpacityFloor(double currentOpacity, bool isClickThroughEnabled)` 메서드
- 로직:
  - `if (isClickThroughEnabled && currentOpacity < 0.2)` -> `return 0.2`
  - `return currentOpacity`
- 이 메서드는 MainWindow에서 투명도 변경 시, 그리고 Click-Through 활성화 시 호출

**Step 8: csproj에 파일 추가**

- `src/OverlayNotepad/OverlayNotepad.csproj` 수정
- `<Compile Include="Services\ClickThroughService.cs" />` 추가

**Step 9: 빌드 검증**

```bash
"/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "D:/99.실험실/OverlayNotepad/OverlayNotepad.sln" /p:Configuration=Debug /t:Build
```
- 예상: Build succeeded, 0 Errors

**Step 10: 커밋**

```
git add src/OverlayNotepad/Services/ClickThroughService.cs src/OverlayNotepad/Models/AppSettings.cs src/OverlayNotepad/OverlayNotepad.csproj
git commit -m "feat(phase4-sprint1): task2 - ClickThroughService (WS_EX_TRANSPARENT 토글 + 안전장치)"
```

**완료 기준:**
- ✅ ClickThroughService.Toggle()로 WS_EX_TRANSPARENT 플래그 토글
- ✅ IsAvailable이 핫키 등록 상태에 연동
- ✅ EnforceOpacityFloor()가 Click-Through 시 최소 20% 보장
- ✅ StateChanged 이벤트 발행 (IsEnabled, IsFirstActivation 포함)
- ✅ AppSettings.IsClickThrough 속성 추가
- ✅ MSBuild 빌드 성공

---

### Task 3: Click-Through 시각 피드백 UI

**skill:** --

**Files:**
- Modify: `src/OverlayNotepad/MainWindow.xaml` (Click-Through 시각 피드백 UI 요소 추가)

**Step 1: Click-Through 테두리 스타일 준비**

- `src/OverlayNotepad/MainWindow.xaml` 수정
- Window.Resources 섹션에 Click-Through 모드용 리소스 추가:
  - 빨간색 점선 테두리 Brush:
    ```xml
    <VisualBrush x:Key="ClickThroughBorderBrush" TileMode="Tile" 
                 Viewport="0,0,8,8" ViewportUnits="Absolute"
                 Viewbox="0,0,8,8" ViewboxUnits="Absolute">
      <VisualBrush.Visual>
        <Rectangle Width="8" Height="8" Fill="#FF4444"/>
      </VisualBrush.Visual>
    </VisualBrush>
    ```
  - 또는 더 간단한 방식: Border의 BorderBrush를 코드비하인드에서 직접 설정
    - 점선 효과는 Border에서 직접 지원하지 않으므로 대안 사용:
    - **권장**: 전체 윈도우를 감싸는 Border 요소에 `x:Name="ClickThroughBorder"` 추가
    - 기본 상태: `BorderThickness="0"`, `BorderBrush="Transparent"`
    - Click-Through 상태: `BorderThickness="3"`, `BorderBrush="#FF4444"` (빨간색)
    - 점선 효과: Rectangle 기반 커스텀 또는 단순 실선 빨간 테두리로 대체 (WPF Border는 DashStyle 미지원)
    - **대안**: Grid의 최상위에 Rectangle 오버레이를 추가하여 점선 표현
      ```xml
      <Rectangle x:Name="ClickThroughIndicator" 
                 Stroke="#FF4444" StrokeThickness="3"
                 StrokeDashArray="4 2" 
                 IsHitTestVisible="False" Visibility="Collapsed"/>
      ```

**Step 2: Click-Through 상태 텍스트 영역**

- MainWindow.xaml의 드래그 영역(상단 바) 내에 TextBlock 추가:
  ```xml
  <TextBlock x:Name="ClickThroughLabel"
             Text="CLICK-THROUGH"
             Foreground="#FF4444"
             FontSize="10" FontWeight="Bold"
             HorizontalAlignment="Center"
             VerticalAlignment="Center"
             Visibility="Collapsed"/>
  ```
- Click-Through 모드에서는 드래그 기능이 불필요하므로 상단 바를 상태 표시 영역으로 재활용
- 기본 상태: Visibility="Collapsed" (숨김)
- Click-Through 상태: Visibility="Visible" (표시)

**Step 3: 빌드 검증**

```bash
"/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "D:/99.실험실/OverlayNotepad/OverlayNotepad.sln" /p:Configuration=Debug /t:Build
```
- 예상: Build succeeded, 0 Errors

**Step 4: 커밋**

```
git add src/OverlayNotepad/MainWindow.xaml
git commit -m "feat(phase4-sprint1): task3 - Click-Through 시각 피드백 UI 요소 (점선 테두리 + 상태 텍스트)"
```

**완료 기준:**
- ✅ ClickThroughIndicator Rectangle (점선 빨간 테두리) 요소 추가
- ✅ ClickThroughLabel TextBlock (상단 바 "CLICK-THROUGH" 텍스트) 요소 추가
- ✅ 두 요소 모두 기본 Collapsed 상태
- ✅ MSBuild 빌드 성공

---

### Task 4: MainWindow 통합 + 트레이/메뉴 연동 + 설정 저장

**skill:** `feature-dev:feature-dev`

**Files:**
- Modify: `src/OverlayNotepad/MainWindow.xaml` (컨텍스트 메뉴에 Click-Through 항목 추가)
- Modify: `src/OverlayNotepad/MainWindow.xaml.cs` (HotkeyService, ClickThroughService 초기화 및 이벤트 처리, 시각 피드백 제어)
- Modify: `src/OverlayNotepad/Services/TrayIconManager.cs` (Click-Through 메뉴 항목 추가, 벌룬 알림 메서드, 아이콘 변경)

**Step 1: TrayIconManager에 Click-Through 관련 기능 추가**

- `src/OverlayNotepad/Services/TrayIconManager.cs` 수정
- 필드 추가:
  - `private System.Windows.Forms.ToolStripMenuItem _clickThroughItem` -- Click-Through 토글 메뉴 항목
- 이벤트 추가:
  - `public event EventHandler ClickThroughToggleRequested` -- Click-Through 토글 요청
- 트레이 메뉴 구성에 항목 추가 (Step 5 부분 수정):
  - 기존 "항상 위에" 항목 아래에 추가:
    1. "Click-Through" -- `_clickThroughItem`, CheckOnClick=true, 초기 Checked=false
       - `CheckedChanged` -> `ClickThroughToggleRequested?.Invoke(...)`
    2. ToolStripSeparator (기존 구분선 앞에)
- 메서드 추가:
  - `public void UpdateClickThroughState(bool isEnabled)` -- 외부에서 Click-Through 상태 변경 시 트레이 메뉴 동기화
    - `_clickThroughItem.Checked = isEnabled`
  - `public void SetClickThroughAvailable(bool isAvailable)` -- Click-Through 핫키 등록 실패 시 메뉴 비활성화
    - `_clickThroughItem.Enabled = isAvailable`
    - 비활성화 시 `_clickThroughItem.Text = "Click-Through (핫키 없음)"` 으로 변경
  - `public void ShowBalloonTip(string title, string text, int timeoutMs = 3000)` -- 트레이 벌룬 알림 표시
    - `_notifyIcon.ShowBalloonTip(timeoutMs, title, text, System.Windows.Forms.ToolTipIcon.Info)`
  - `public void SetClickThroughIcon(bool isClickThrough)` -- Click-Through 상태에 따라 트레이 아이콘 변경
    - Click-Through ON: 아이콘 위에 빨간 오버레이 표시 (System.Drawing으로 동적 생성)
    - Click-Through OFF: 원래 아이콘 복원
    - 구현 방법: `System.Drawing.Graphics`로 기본 아이콘 위에 빨간 원(Ellipse)을 그린 새 아이콘 생성
    - 또는 단순히 ToolTipText를 변경: "OverlayNotepad (Click-Through)" / "OverlayNotepad"

**Step 2: 컨텍스트 메뉴에 Click-Through 항목 추가**

- `src/OverlayNotepad/MainWindow.xaml` 수정
- 기존 컨텍스트 메뉴 구조에 Click-Through 항목 추가:
  - Phase 3 Sprint 2에서 완성한 메뉴의 "최소화" 항목 바로 위에 추가:
    ```
    ... (기존 항목)
    ────────────────
    [v] Click-Through    (신규)
    최소화
    종료
    ```
  - XAML:
    ```xml
    <Separator />
    <MenuItem x:Name="ClickThroughMenuItem" Header="Click-Through" 
              IsCheckable="True" IsChecked="False"
              Click="ClickThroughMenuItem_Click" />
    ```

**Step 3: MainWindow.xaml.cs HotkeyService 초기화**

- `MainWindow.xaml.cs` 수정
- 필드 추가:
  - `private HotkeyService _hotkeyService;`
  - `private ClickThroughService _clickThroughService;`
- Window_Loaded 핸들러에 추가 (TrayIconManager 초기화 이후):
  1. HotkeyService 초기화:
     - `_hotkeyService = new HotkeyService()`
     - `_hotkeyService.Initialize(this)` -- 주의: Window.Loaded 이후에야 Handle이 유효
     - `_hotkeyService.ToggleVisibilityRequested += OnHotkeyToggleVisibility`
     - `_hotkeyService.ToggleClickThroughRequested += OnHotkeyToggleClickThrough`
  2. ClickThroughService 초기화:
     - `_clickThroughService = new ClickThroughService()`
     - `var hwnd = new WindowInteropHelper(this).Handle`
     - `_clickThroughService.Initialize(hwnd, _hotkeyService.IsClickThroughHotkeyRegistered)`
     - `_clickThroughService.StateChanged += OnClickThroughStateChanged`
  3. 핫키 등록 실패 처리:
     - `var failedHotkeys = _hotkeyService.GetFailedHotkeys()`
     - `if (failedHotkeys.Count > 0)`:
       - 트레이 벌룬 알림: `_trayIconManager.ShowBalloonTip("핫키 등록 실패", "다음 핫키를 등록하지 못했습니다:\n" + string.Join("\n", failedHotkeys) + "\n다른 프로그램이 사용 중일 수 있습니다.")`
       - Click-Through 핫키 실패 시: `_trayIconManager.SetClickThroughAvailable(false)`
       - 컨텍스트 메뉴의 Click-Through 항목도 비활성화: `ClickThroughMenuItem.IsEnabled = false`

**Step 4: 핫키 이벤트 핸들러 구현**

- `private void OnHotkeyToggleVisibility(object sender, EventArgs e)`:
  - 기존 TrayIconManager의 OnToggleVisibility와 동일한 로직 호출
  - 숨겨져 있으면: Show() -> WindowState=Normal -> ShowInTaskbar=true -> Activate()
  - 보이는 상태면: Hide() -> ShowInTaskbar=false

- `private void OnHotkeyToggleClickThrough(object sender, EventArgs e)`:
  - `_clickThroughService.Toggle()` 호출

**Step 5: Click-Through 상태 변경 핸들러 구현**

- `private void OnClickThroughStateChanged(object sender, ClickThroughChangedEventArgs e)`:
  - Dispatcher.Invoke로 UI 스레드에서 실행:
  1. 시각 피드백 업데이트:
     - `ClickThroughIndicator.Visibility = e.IsEnabled ? Visibility.Visible : Visibility.Collapsed`
     - `ClickThroughLabel.Visibility = e.IsEnabled ? Visibility.Visible : Visibility.Collapsed`
  2. 투명도 하한 강제:
     - `if (e.IsEnabled)`:
       - `this.Opacity = ClickThroughService.EnforceOpacityFloor(this.Opacity, true)`
  3. 트레이 상태 동기화:
     - `_trayIconManager.UpdateClickThroughState(e.IsEnabled)`
     - `_trayIconManager.SetClickThroughIcon(e.IsEnabled)`
  4. 컨텍스트 메뉴 동기화:
     - `ClickThroughMenuItem.IsChecked = e.IsEnabled`
  5. 최초 활성화 시 벌룬 안내:
     - `if (e.IsEnabled && e.IsFirstActivation)`:
       - `_trayIconManager.ShowBalloonTip("Click-Through 모드 활성화", "마우스 클릭이 뒤의 앱에 전달됩니다.\n해제: Ctrl+Shift+T 또는 트레이 메뉴")`
  6. 설정 저장 (현재 세션 상태만, 재시작 시 OFF):
     - `SettingsManager.Instance.Current.IsClickThrough = e.IsEnabled`
     - 주의: Save()는 호출하되, Load() 시 IsClickThrough를 강제 false로 리셋하는 로직이 필요
     - SettingsManager.Load() 메서드 끝에 `Current.IsClickThrough = false` 추가 (안전 설계)

**Step 6: 컨텍스트 메뉴 Click-Through 핸들러**

- `private void ClickThroughMenuItem_Click(object sender, RoutedEventArgs e)`:
  - `_clickThroughService.Toggle()` 호출
  - 주의: Click-Through 활성 상태에서는 컨텍스트 메뉴 자체가 열리지 않음 (마우스 클릭이 통과하므로)
  - 따라서 이 핸들러는 Click-Through 비활성 상태에서만 호출됨 (활성화 방향으로만 동작)
  - 비활성화는 핫키(Ctrl+Shift+T) 또는 트레이 메뉴에서만 가능

**Step 7: 트레이 메뉴 Click-Through 핸들러 연동**

- TrayIconManager의 ClickThroughToggleRequested 이벤트 연결:
  - `_trayIconManager.ClickThroughToggleRequested += OnTrayClickThroughToggle`
- `private void OnTrayClickThroughToggle(object sender, EventArgs e)`:
  - `_clickThroughService.Toggle()` 호출

**Step 8: 투명도 변경 시 Click-Through 하한 강제**

- 기존 투명도 변경 로직(컨텍스트 메뉴의 투명도 프리셋 핸들러)에 추가:
  - 투명도를 변경할 때:
    ```csharp
    double newOpacity = ClickThroughService.EnforceOpacityFloor(requestedOpacity, _clickThroughService.IsEnabled);
    this.Opacity = newOpacity;
    ```

**Step 9: SettingsManager.Load()에 Click-Through 안전 리셋 추가**

- `src/OverlayNotepad/Services/SettingsManager.cs` 수정 (필요한 경우)
- Load() 메서드 끝에 추가:
  - `Current.IsClickThrough = false;` -- 재시작 시 항상 Click-Through OFF

**Step 10: Dispose 연동**

- Window_Closing 핸들러에 추가:
  - `_hotkeyService?.Dispose()` -- UnregisterHotKey 호출
  - Click-Through 활성 상태면 비활성화: `if (_clickThroughService.IsEnabled) _clickThroughService.Toggle()`

**Step 11: 빌드 및 수동 검증**

```bash
"/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "D:/99.실험실/OverlayNotepad/OverlayNotepad.sln" /p:Configuration=Debug /t:Build
```
- 예상: Build succeeded, 0 Errors

수동 검증:
- ⬜ Ctrl+Shift+N으로 표시/숨김 토글
- ⬜ Ctrl+Shift+T로 Click-Through 토글
- ⬜ Click-Through 시 빨간 점선 테두리 + "CLICK-THROUGH" 텍스트 표시
- ⬜ Click-Through 시 트레이 메뉴 상태 동기화
- ⬜ Click-Through 시 마우스 클릭이 뒤 앱에 전달
- ⬜ Click-Through 최초 활성화 시 벌룬 안내 표시
- ⬜ 투명도 10%에서 Click-Through 시 20%로 강제

**Step 12: 커밋**

```
git add src/OverlayNotepad/MainWindow.xaml src/OverlayNotepad/MainWindow.xaml.cs src/OverlayNotepad/Services/TrayIconManager.cs src/OverlayNotepad/Services/SettingsManager.cs
git commit -m "feat(phase4-sprint1): task4 - MainWindow 통합 (핫키 + Click-Through + 트레이/메뉴 연동)"
```

**완료 기준:**
- ✅ 글로벌 핫키로 표시/숨김 토글 (다른 앱 포커스 상태에서)
- ✅ 글로벌 핫키로 Click-Through 토글
- ✅ Click-Through 시 3중 시각 피드백 (점선 테두리 + 상단 텍스트 + 트레이 아이콘)
- ✅ 트레이 메뉴에서 Click-Through 토글
- ✅ 컨텍스트 메뉴에 Click-Through 항목 (비활성화 방향은 핫키/트레이만)
- ✅ 핫키 등록 실패 시 벌룬 알림 + Click-Through 비활성화
- ✅ 투명도 하한 20% 강제
- ✅ 재시작 시 Click-Through OFF
- ✅ 정상 종료 시 UnregisterHotKey 호출
- ✅ MSBuild 빌드 성공

---

### Task 5: 통합 검증 + 엣지 케이스 + 안전장치 확인

**skill:** --

**Files:**
- Modify: `src/OverlayNotepad/MainWindow.xaml` (필요 시 수정)
- Modify: `src/OverlayNotepad/MainWindow.xaml.cs` (필요 시 수정)
- Modify: `src/OverlayNotepad/Services/ClickThroughService.cs` (필요 시 수정)
- Modify: `src/OverlayNotepad/Services/HotkeyService.cs` (필요 시 수정)
- Modify: `src/OverlayNotepad/Services/TrayIconManager.cs` (필요 시 수정)

**Step 1: 글로벌 핫키 검증**

```
검증 시나리오:
  1. 메모장이 포커스 상태에서 Ctrl+Shift+N -> 숨김
  2. 다른 앱(메모장, 브라우저 등) 포커스 상태에서 Ctrl+Shift+N -> 메모장 표시 + 포커스 획득
  3. 트레이 숨김 상태에서 Ctrl+Shift+N -> 메모장 복원
  4. 다른 앱 포커스 상태에서 Ctrl+Shift+T -> Click-Through 활성화
  5. Click-Through 상태에서 Ctrl+Shift+T -> Click-Through 비활성화
  6. 빠른 연속 핫키 입력(5회 이상) -> 상태 정상 전환, 깜빡임 없음
```

**Step 2: Click-Through 기능 검증**

```
검증 시나리오:
  1. Click-Through ON -> 메모장 위에서 마우스 클릭 -> 뒤의 앱이 클릭됨
  2. Click-Through ON -> 메모장 위에서 마우스 스크롤 -> 뒤의 앱이 스크롤됨
  3. Click-Through ON -> 빨간 점선 테두리 표시 확인
  4. Click-Through ON -> 상단 바에 "CLICK-THROUGH" 텍스트 표시 확인
  5. Click-Through ON -> 트레이 아이콘/툴팁 변경 확인
  6. Click-Through ON -> Ctrl+Shift+T -> 즉시 비활성화, 테두리/텍스트 사라짐
  7. Click-Through ON -> 트레이 메뉴에서 "Click-Through" 체크 해제 -> 비활성화
  8. Click-Through ON -> 우클릭 컨텍스트 메뉴 -> 열리지 않음 (클릭 통과)
```

**Step 3: 안전장치 검증**

```
검증 시나리오:
  1. 투명도 10% 상태에서 Click-Through ON -> 자동으로 20%로 조정
  2. Click-Through ON 상태에서 투명도를 10%로 변경 시도 -> 20%로 강제
  3. Click-Through ON 상태에서 앱 종료(Alt+F4) -> 핫키는 직접 동작하지 않으므로 트레이 메뉴 "종료" 사용
     - 정상 종료 + 트레이 아이콘 사라짐 + UnregisterHotKey 확인
  4. Click-Through ON 상태에서 앱 종료 후 재시작 -> Click-Through OFF로 시작
  5. 핫키 등록 실패 시뮬레이션 (다른 앱에서 Ctrl+Shift+T 선점):
     - Click-Through 메뉴 항목이 회색 비활성화
     - 트레이 벌룬으로 등록 실패 알림
  6. 최초 Click-Through 활성화 -> 벌룬 안내 표시 ("해제: Ctrl+Shift+T 또는 트레이 메뉴")
  7. 두 번째 이후 Click-Through 활성화 -> 벌룬 안내 미표시
```

**Step 4: 기존 기능 회귀 검증**

```
회귀 시나리오:
  1. 투명도 조절 (컨텍스트 메뉴) -- 정상 동작
  2. 배경 투명 모드 토글 -- 정상 동작
  3. Always on Top 토글 -- 정상 동작
  4. 테마 전환 (다크/라이트) -- 정상 동작
  5. 글꼴/크기/색상 변경 -- 정상 동작
  6. 텍스트 테두리/그림자 토글 -- 정상 동작
  7. 자동 저장 -- 텍스트 변경 후 2초 뒤 memo.txt 갱신
  8. 윈도우 드래그/리사이즈 -- 정상 동작
  9. 시스템 트레이 -- 최소화/복원/종료 정상
  10. 설정 저장/복원 -- 종료 후 재시작 시 설정 유지
```

**Step 5: Click-Through + 테마/투명도 조합 검증**

```
조합 시나리오:
  1. 다크 테마 + Click-Through ON -> 빨간 점선 테두리가 어두운 배경 위에서 잘 보임
  2. 라이트 테마 + Click-Through ON -> 빨간 점선 테두리가 밝은 배경 위에서 잘 보임
  3. 배경 투명 모드 ON + Click-Through ON -> 빨간 점선 테두리 + "CLICK-THROUGH" 텍스트 보임
  4. 전체 투명도 20% + Click-Through ON -> 최소 20% 유지, 테두리와 텍스트 어렴풋이 보임
  5. Click-Through ON 상태에서 테마 전환 (핫키/트레이로 불가하므로, 이전에 Click-Through 해제 -> 테마 전환 -> Click-Through 재활성화 시나리오)
```

**Step 6: 수정 사항 반영**

- 검증 중 발견된 문제 수정
- 주요 리스크:
  - WS_EX_TRANSPARENT와 WPF AllowsTransparency 조합 시 예상치 못한 동작
  - Click-Through 시 트레이 메뉴 접근성 (트레이 아이콘은 별도 프로세스 UI이므로 정상 동작해야 함)
  - 핫키 등록 타이밍 (Window.Loaded 이후 Handle이 유효한지 확인)

**Step 7: 커밋**

```
git add src/OverlayNotepad/
git commit -m "fix(phase4-sprint1): task5 - 통합 검증 후 엣지 케이스 수정"
```

**완료 기준:**
- ✅ 글로벌 핫키 6가지 시나리오 모두 통과
- ✅ Click-Through 기능 8가지 시나리오 모두 통과
- ✅ 안전장치 7가지 시나리오 모두 통과
- ✅ 기존 기능 회귀 10가지 시나리오 모두 통과
- ✅ 조합 테스트 5가지 시나리오 모두 통과
- ✅ MSBuild 빌드 성공 (Debug + Release)

---

## 최종 검증 계획

| 검증 항목 | 명령 / 방법 | 예상 결과 |
|-----------|-------------|-----------|
| MSBuild 빌드 | `MSBuild OverlayNotepad.sln /p:Configuration=Debug` | Build succeeded, 0 Errors |
| 글로벌 핫키: 표시/숨김 | 다른 앱 포커스 + Ctrl+Shift+N | 메모장 표시/숨김 토글 |
| 글로벌 핫키: Click-Through | 다른 앱 포커스 + Ctrl+Shift+T | Click-Through 토글 |
| Click-Through 마우스 통과 | Click-Through ON + 메모장 위 클릭 | 뒤 앱이 클릭됨 |
| Click-Through 해제 | Click-Through ON + Ctrl+Shift+T | 즉시 비활성화 |
| 시각 피드백: 테두리 | Click-Through ON 시 확인 | 빨간 점선 테두리 표시 |
| 시각 피드백: 상단 텍스트 | Click-Through ON 시 확인 | "CLICK-THROUGH" 텍스트 표시 |
| 시각 피드백: 트레이 | Click-Through ON 시 트레이 확인 | 트레이 아이콘/툴팁 변경 |
| 트레이 메뉴: Click-Through | 트레이 우클릭 > Click-Through | 토글 동작, 체크마크 동기화 |
| 투명도 하한 | 투명도 10% + Click-Through ON | 자동 20% 조정 |
| 핫키 충돌 알림 | 핫키 선점 상태에서 앱 시작 | 트레이 벌룬 알림 |
| 핫키 실패 시 CT 비활성화 | CT 핫키 실패 시 메뉴 확인 | Click-Through 메뉴 회색 처리 |
| 최초 CT 활성화 안내 | 첫 Click-Through ON | 벌룬 "해제: Ctrl+Shift+T 또는 트레이 메뉴" |
| 재시작 시 CT OFF | CT ON -> 종료 -> 재시작 | Click-Through OFF로 시작 |
| 정상 종료 | Alt+F4 또는 트레이 > 종료 | 트레이 아이콘 사라짐, 핫키 해제 |
| 기존 기능 회귀 | 투명도/테마/서식/자동저장/트레이 | 모두 정상 동작 |
| 메모리 | 작업 관리자에서 확인 | 80MB 이하 (참고, 최적화는 Sprint 2) |
