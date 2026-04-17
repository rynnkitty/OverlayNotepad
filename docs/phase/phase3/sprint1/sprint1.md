# Sprint 1: 시스템 트레이 + 서식 지원 (Phase 3)

**Goal:** WinForms NotifyIcon 인터롭으로 시스템 트레이 기능을 구현하고, 글꼴/크기/색상 서식 변경 기능을 컨텍스트 메뉴에 추가하며, 서식 설정을 settings.json에 저장/복원한다.

**Architecture:** TrayIconManager 클래스가 WinForms NotifyIcon을 래핑하여 트레이 아이콘 생성/해제, 트레이 메뉴 구성, 이벤트 핸들링을 담당한다. 서식 변경(글꼴/크기/색상)은 기존 컨텍스트 메뉴에 서브메뉴를 추가하는 방식으로 구현하며, 프리셋 선택과 시스템 대화상자(FontDialog, ColorDialog) 두 가지 경로를 제공한다. 서식 변경 시 AppSettings의 FontSettings를 업데이트하고 SettingsManager.Save()를 호출하여 즉시 영속화한다.

**Tech Stack:** .NET Framework 4.8, WPF, System.Windows.Forms (NotifyIcon, FontDialog, ColorDialog), System.Drawing

**상태:** ✅ 완료 (2026-04-17)
**Sprint 기간:** 2026-04-17 ~ 2026-04-17
**이전 스프린트:** Phase 2 Sprint 2 (설정 관리 + 자동 저장 + 윈도우 관리)
**브랜치명:** `phase3-sprint1`
**PR:** https://github.com/rynnkitty/OverlayNotepad/compare/develop...phase3-sprint1 (사용자가 GitHub 웹에서 수동 생성 필요)

---

## 제외 범위

- 다크/라이트 테마 전환 -- Sprint 2
- 테마 전환 시 자동 색상 변경 로직 -- Sprint 2
- 컨텍스트 메뉴 완성 (테마 전환, 테두리/그림자 색상 서브메뉴, 자동저장 상태 표시, 최소화) -- Sprint 2
- Click-Through 토글 (트레이 메뉴 포함) -- Phase 4
- 글로벌 핫키 -- Phase 4
- 트레이 메뉴에 Click-Through 항목 -- Phase 4 (Phase 3 확정 파라미터: Phase 4 기능 선반영 금지)
- 투명도 슬라이더 커스텀 UI -- Backlog

## 확정 파라미터 (Phase 3 문서 기준)

| 항목 | 값 | 근거 |
|------|-----|------|
| 글꼴 프리셋 | 맑은 고딕, 나눔고딕, 나눔바른고딕, D2Coding, Consolas, Arial, 굴림 (5~7개) | PO+행정: 빈번 사용 시 빠른 선택 필요 |
| 글꼴 추가 선택 | "더 보기..." -> FontDialog | 프리셋 외 글꼴 선택 경로 |
| 글자 크기 프리셋 | 10/12/14/16/20/24/32pt (7단계) | UX: 자주 쓰는 크기 빠른 선택 |
| 글자 크기 추가 | "직접 입력..." -> InputBox 또는 간단 대화상자 | 프리셋 외 크기 |
| 색상 프리셋 | 흰색, 검정, 빨강, 파랑, 초록, 노랑, 회색, 주황, 보라, 하늘 (10색) | 행정: 반복 작업 빠른 선택 우선 |
| 색상 추가 선택 | "사용자 지정..." -> ColorDialog | 프리셋 외 색상 |
| 서브메뉴 깊이 | 최대 1단계 | UX: 투명 오버레이 위 다단 서브메뉴 조작 불편 |
| 트레이 메뉴 항목 | 표시/숨김, Always on Top, 종료 | PO: Phase 4 기능 선반영 금지 |
| 미설치 글꼴 처리 | InstalledFontCollection으로 확인, 미설치 시 목록 제외 | UX: 없는 글꼴 표시 방지 |

## 실행 플랜

Task 1(TrayIconManager)과 Task 2~3(서식 관련)은 수정 파일이 겹치지 않으므로 병렬 가능하다. Task 4(MainWindow 통합)는 Task 1~3에 의존한다.

### Phase 1 (순차 -- 기반 모듈)
| Task | 설명 | 대상 | skill |
|------|------|------|-------|
| Task 1 | TrayIconManager (시스템 트레이 아이콘 + 트레이 메뉴) | 데스크톱 | -- |

### Phase 2 (병렬 가능)
| Task | 설명 | 대상 | skill |
|------|------|------|-------|
| Task 2 | AppSettings 서식 필드 확장 + 글꼴 유효성 헬퍼 | 데스크톱 | -- |
| Task 3 | 서식 변경 컨텍스트 메뉴 서브메뉴 구현 | 데스크톱 | -- |

### Phase 3 (순차 -- 통합)
| Task | 설명 | 대상 | skill |
|------|------|------|-------|
| Task 4 | MainWindow 통합 (트레이 + 서식 연동) + 설정 영속화 | 데스크톱 | `feature-dev:feature-dev` |
| Task 5 | 통합 검증 + 엣지 케이스 | 데스크톱 | -- |

> **병렬 실행**: Task 2와 Task 3은 수정 파일이 겹치지 않으므로 병렬 가능 (Task 2: Models/AppSettings.cs, Helpers/ | Task 3: MainWindow.xaml, MainWindow.xaml.cs). 단, Task 3은 Task 2의 AppSettings 모델을 참조하므로 Task 2가 먼저 완료되어야 안전함. 실질적으로는 순차 권장.

---

### Task 1: TrayIconManager (시스템 트레이 아이콘 + 트레이 메뉴)

**skill:** --

**Files:**
- Create: `src/OverlayNotepad/Services/TrayIconManager.cs`
- Create: `src/OverlayNotepad/Resources/app.ico` (앱 아이콘 리소스)
- Modify: `src/OverlayNotepad/OverlayNotepad.csproj` (System.Windows.Forms 참조 + 파일 추가)

**Step 1: System.Windows.Forms 참조 추가**

- `src/OverlayNotepad/OverlayNotepad.csproj` 수정
- `<Reference>` 섹션에 추가:
  - `<Reference Include="System.Windows.Forms" />`
  - `<Reference Include="System.Drawing" />`
- 참고: .NET Framework 4.8에 기본 포함된 어셈블리이므로 NuGet 불필요

**Step 2: 앱 아이콘 리소스 준비**

- `src/OverlayNotepad/Resources/app.ico` 생성 (간단한 메모장 아이콘)
- 아이콘이 없는 경우: 시스템 기본 아이콘을 코드에서 추출하여 사용하는 폴백 로직 구현
  - `System.Drawing.SystemIcons.Application` 을 폴백으로 사용
- csproj에 아이콘 리소스 추가:
  - `<Resource Include="Resources\app.ico" />`

**Step 3: TrayIconManager 클래스 구현**

- `src/OverlayNotepad/Services/TrayIconManager.cs` 생성
- 네임스페이스: `OverlayNotepad.Services`
- 클래스: `TrayIconManager : IDisposable` (public)
- 필드:
  - `private System.Windows.Forms.NotifyIcon _notifyIcon`
  - `private System.Windows.Forms.ContextMenuStrip _trayMenu`
  - `private System.Windows.Forms.ToolStripMenuItem _alwaysOnTopItem`
- 이벤트:
  - `public event EventHandler ToggleVisibilityRequested` -- 표시/숨김 토글 요청
  - `public event EventHandler AlwaysOnTopToggleRequested` -- Always on Top 토글 요청
  - `public event EventHandler ExitRequested` -- 종료 요청

**Step 4: Initialize() 메서드**

- `public void Initialize(System.Drawing.Icon appIcon)` 메서드
- 로직:
  1. `_notifyIcon = new System.Windows.Forms.NotifyIcon()`
  2. `_notifyIcon.Icon = appIcon ?? System.Drawing.SystemIcons.Application`
  3. `_notifyIcon.Text = "OverlayNotepad"` (툴팁)
  4. `_notifyIcon.Visible = true`
  5. `_notifyIcon.DoubleClick += (s, e) => ToggleVisibilityRequested?.Invoke(this, EventArgs.Empty)`
  6. 트레이 메뉴 구성 (Step 5)
  7. `_notifyIcon.ContextMenuStrip = _trayMenu`

**Step 5: 트레이 메뉴 구성**

- `_trayMenu = new System.Windows.Forms.ContextMenuStrip()`
- 메뉴 항목:
  1. "표시/숨김" -- `Click` -> `ToggleVisibilityRequested?.Invoke(...)`
  2. "항상 위에" -- `_alwaysOnTopItem`, CheckOnClick=true, 초기 Checked=true (기본값)
     - `CheckedChanged` -> `AlwaysOnTopToggleRequested?.Invoke(...)`
  3. `ToolStripSeparator`
  4. "종료" -- `Click` -> `ExitRequested?.Invoke(...)`

**Step 6: 상태 업데이트 메서드**

- `public void UpdateAlwaysOnTopState(bool isTopmost)` -- 외부에서 Topmost 상태 변경 시 트레이 메뉴 동기화
  - `_alwaysOnTopItem.Checked = isTopmost`

**Step 7: Dispose 구현**

- `public void Dispose()` 메서드
- 로직:
  1. `_notifyIcon.Visible = false` (아이콘 숨김 먼저)
  2. `_notifyIcon.Dispose()`
  3. `_trayMenu?.Dispose()`
- 중요: 트레이 아이콘 잔존 방지를 위해 반드시 Visible=false 후 Dispose

**Step 8: csproj에 파일 추가**

- `src/OverlayNotepad/OverlayNotepad.csproj` 수정
- `<Compile Include="Services\TrayIconManager.cs" />` 추가

**Step 9: 빌드 검증**

```bash
"/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "D:/99.실험실/OverlayNotepad/OverlayNotepad.sln" /p:Configuration=Debug /t:Build
```
- 예상: Build succeeded, 0 Errors

**Step 10: 커밋**

```
git add src/OverlayNotepad/Services/TrayIconManager.cs src/OverlayNotepad/Resources/app.ico src/OverlayNotepad/OverlayNotepad.csproj
git commit -m "feat(phase3-sprint1): task1 - TrayIconManager 시스템 트레이 아이콘 + 트레이 메뉴"
```

**완료 기준:**
- ✅ TrayIconManager 클래스 빌드 성공
- ✅ NotifyIcon 이벤트 (DoubleClick, 메뉴 항목) 핸들링 구조 완성
- ✅ IDisposable 패턴 구현 (Visible=false -> Dispose 순서)

---

### Task 2: AppSettings 서식 필드 확장 + 글꼴 유효성 헬퍼

**skill:** --

**Files:**
- Modify: `src/OverlayNotepad/Models/AppSettings.cs` (FontSettings 섹션에 실제 저장/복원 연동 확인)
- Create: `src/OverlayNotepad/Helpers/FontHelper.cs` (설치된 글꼴 확인 유틸리티)

**Step 1: AppSettings FontSettings 확인 및 보완**

- `src/OverlayNotepad/Models/AppSettings.cs`의 FontSettings 확인
- Phase 2 Sprint 2 Task 1에서 이미 선언된 필드:
  - `Family` (string, 기본값 "맑은 고딕")
  - `Size` (double, 기본값 14)
  - `Color` (string, 기본값 "#FFFFFF")
- 이 필드들이 정상적으로 직렬화/역직렬화되는지 확인
- 기존 settings.json에 font 섹션이 누락된 경우 기본값 폴백 처리가 되어 있는지 확인

**Step 2: FontHelper 유틸리티 생성**

- `src/OverlayNotepad/Helpers/FontHelper.cs` 생성
- 네임스페이스: `OverlayNotepad.Helpers`
- 클래스: `FontHelper` (public static)
- 메서드:
  - `public static List<string> GetPresetFonts()` 
    - 프리셋 목록: "맑은 고딕", "나눔고딕", "나눔바른고딕", "D2Coding", "Consolas", "Arial", "굴림"
    - 각 글꼴이 시스템에 설치되어 있는지 `IsInstalled()` 로 확인
    - 설치된 글꼴만 필터링하여 반환
  - `public static bool IsInstalled(string fontName)`
    - `System.Drawing.Text.InstalledFontCollection` 으로 시스템 글꼴 목록 조회
    - 글꼴명이 목록에 존재하면 true
    - InstalledFontCollection 인스턴스는 static 캐싱 (매번 생성 비용 방지)
  - `public static List<double> GetPresetSizes()`
    - 반환: `[10, 12, 14, 16, 20, 24, 32]`
  - `public static List<(string Name, string Hex)> GetPresetColors()`
    - 반환: `[("흰색","#FFFFFF"), ("검정","#000000"), ("빨강","#FF0000"), ("파랑","#0000FF"), ("초록","#00FF00"), ("노랑","#FFFF00"), ("회색","#808080"), ("주황","#FFA500"), ("보라","#800080"), ("하늘","#87CEEB")]`

**Step 3: csproj에 파일 추가**

- `src/OverlayNotepad/OverlayNotepad.csproj` 수정
- `<Compile Include="Helpers\FontHelper.cs" />` 추가

**Step 4: 빌드 검증**

```bash
"/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "D:/99.실험실/OverlayNotepad/OverlayNotepad.sln" /p:Configuration=Debug /t:Build
```
- 예상: Build succeeded, 0 Errors

**Step 5: 커밋**

```
git add src/OverlayNotepad/Models/AppSettings.cs src/OverlayNotepad/Helpers/FontHelper.cs src/OverlayNotepad/OverlayNotepad.csproj
git commit -m "feat(phase3-sprint1): task2 - AppSettings 서식 필드 확인 + FontHelper 유효성 헬퍼"
```

**완료 기준:**
- ✅ FontHelper.GetPresetFonts()가 설치된 글꼴만 반환
- ✅ 프리셋 크기/색상 목록 정상 반환
- ✅ MSBuild 빌드 성공

---

### Task 3: 서식 변경 컨텍스트 메뉴 서브메뉴 구현

**skill:** --

**Files:**
- Modify: `src/OverlayNotepad/MainWindow.xaml` (컨텍스트 메뉴에 글꼴/크기/색상 서브메뉴 추가)
- Modify: `src/OverlayNotepad/MainWindow.xaml.cs` (서식 변경 이벤트 핸들러 추가)

**Step 1: 컨텍스트 메뉴에 서식 서브메뉴 추가**

- Phase 2 Sprint 1에서 구현된 기존 ContextMenu에 항목 추가
- 추가 위치: 기존 "텍스트 그림자" 항목 아래, "항상 위에" 항목 위
- 메뉴 구조 (추가 부분):
  ```
  ... (기존 항목)
  [v] 텍스트 테두리
  [v] 텍스트 그림자
  ────────────────
  글꼴 >  (서브메뉴)
  글자 크기 >  (서브메뉴)
  글자 색상 >  (서브메뉴)
  ────────────────
  [v] 항상 위에
  ... (기존 항목)
  ```

**Step 2: 글꼴 서브메뉴 구현 (XAML)**

- `MenuItem Header="글꼴"` 서브메뉴
- 서브메뉴 항목은 **코드비하인드에서 동적 생성** (설치된 글꼴만 표시해야 하므로)
- XAML에는 빈 서브메뉴 컨테이너만 선언:
  ```xml
  <MenuItem x:Name="FontFamilyMenu" Header="글꼴" SubmenuOpened="FontFamilyMenu_SubmenuOpened">
    <MenuItem Header="로딩 중..." IsEnabled="False" />
  </MenuItem>
  ```
- `FontFamilyMenu_SubmenuOpened` 핸들러에서 동적으로 항목 생성 (최초 1회)

**Step 3: 글꼴 서브메뉴 코드비하인드**

- `MainWindow.xaml.cs`에 `FontFamilyMenu_SubmenuOpened` 메서드 추가:
  - 최초 호출 시만 서브메뉴 재구성 (`_fontMenuInitialized` 플래그)
  - `FontHelper.GetPresetFonts()` 로 프리셋 목록 가져오기
  - 각 글꼴에 대해 `MenuItem` 생성:
    - `Header = fontName` (해당 글꼴로 표시하면 좋지만, WPF MenuItem에서는 어려우므로 텍스트만)
    - `IsCheckable = true`, 현재 글꼴과 일치하면 `IsChecked = true`
    - `Click += FontPreset_Click` (태그에 글꼴명 저장: `Tag = fontName`)
  - Separator 추가
  - "더 보기..." MenuItem 추가 -- `Click += FontDialog_Click`
- `FontPreset_Click` 핸들러:
  - `string fontName = (sender as MenuItem).Tag as string`
  - `ApplyFontFamily(fontName)` 호출
- `FontDialog_Click` 핸들러:
  - `var dialog = new System.Windows.Forms.FontDialog()`
  - `dialog.Font = new System.Drawing.Font(현재 글꼴, (float)현재 크기)`
  - `if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)`
    - `ApplyFontFamily(dialog.Font.FontFamily.Name)`
    - `ApplyFontSize(dialog.Font.Size)` (FontDialog는 글꼴+크기를 함께 변경)

**Step 4: 글자 크기 서브메뉴 구현**

- XAML:
  ```xml
  <MenuItem x:Name="FontSizeMenu" Header="글자 크기">
    <!-- 프리셋 7단계: 코드비하인드에서 생성 또는 XAML에 직접 선언 -->
  </MenuItem>
  ```
- XAML에 7단계 프리셋을 직접 선언 가능:
  ```xml
  <MenuItem Header="10" Tag="10" Click="FontSizePreset_Click" />
  <MenuItem Header="12" Tag="12" Click="FontSizePreset_Click" />
  <MenuItem Header="14" Tag="14" IsChecked="True" Click="FontSizePreset_Click" />
  <MenuItem Header="16" Tag="16" Click="FontSizePreset_Click" />
  <MenuItem Header="20" Tag="20" Click="FontSizePreset_Click" />
  <MenuItem Header="24" Tag="24" Click="FontSizePreset_Click" />
  <MenuItem Header="32" Tag="32" Click="FontSizePreset_Click" />
  <Separator />
  <MenuItem Header="직접 입력..." Click="FontSizeCustom_Click" />
  ```
- `FontSizePreset_Click` 핸들러:
  - `double size = double.Parse((sender as MenuItem).Tag as string)`
  - `ApplyFontSize(size)`
  - 서브메뉴 내 모든 항목의 IsChecked 업데이트 (선택된 크기만 체크)
- `FontSizeCustom_Click` 핸들러:
  - 간단한 InputBox 구현 (WPF에 내장 InputBox가 없으므로):
    - `Microsoft.VisualBasic.Interaction.InputBox` 사용 (Microsoft.VisualBasic 참조 추가)
    - 또는 간단한 커스텀 대화상자 Window 구현
  - 입력값을 double로 파싱, 유효 범위(6~72pt) 확인 후 `ApplyFontSize(size)`

**Step 5: 글자 색상 서브메뉴 구현**

- XAML:
  ```xml
  <MenuItem x:Name="FontColorMenu" Header="글자 색상">
    <!-- 프리셋 10색 + 사용자 지정: 코드비하인드에서 동적 생성 -->
  </MenuItem>
  ```
- 코드비하인드에서 프리셋 색상 메뉴 동적 생성:
  - `FontHelper.GetPresetColors()` 반복
  - 각 색상에 대해 MenuItem 생성:
    - `Header` = 색상 이름
    - `Tag` = hex 값 (예: "#FFFFFF")
    - `Icon` = 작은 Rectangle (해당 색상으로 채운 사각형) -- 색상 미리보기
    - `Click += ColorPreset_Click`
  - Separator + "사용자 지정..." MenuItem 추가 -- `Click += ColorDialog_Click`
- `ColorPreset_Click` 핸들러:
  - `string hex = (sender as MenuItem).Tag as string`
  - `ApplyFontColor(hex)` 호출
- `ColorDialog_Click` 핸들러:
  - `var dialog = new System.Windows.Forms.ColorDialog()`
  - 현재 색상을 초기값으로 설정
  - `if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)`
    - `string hex = ColorToHex(dialog.Color)` (System.Drawing.Color -> "#RRGGBB")
    - `ApplyFontColor(hex)`

**Step 6: 서식 적용 메서드 구현**

- `private void ApplyFontFamily(string fontName)` 메서드:
  - `MainTextBox.FontFamily = new FontFamily(fontName)`
  - OutlinedTextControl이 있으면 동기화: `OutlinedText.FontFamily = MainTextBox.FontFamily`
  - (설정 저장은 Task 4에서 통합)
- `private void ApplyFontSize(double size)` 메서드:
  - `MainTextBox.FontSize = size`
  - OutlinedTextControl 동기화: `OutlinedText.FontSize = size`
- `private void ApplyFontColor(string hex)` 메서드:
  - `Color color = (Color)ColorConverter.ConvertFromString(hex)`
  - `Brush brush = new SolidColorBrush(color)`
  - 테두리 ON 상태: `OutlinedText.FillBrush = brush` (TextBox Foreground는 투명 유지)
  - 테두리 OFF 상태: `MainTextBox.Foreground = brush`
  - `MainTextBox.CaretBrush = brush` (캐럿도 동일 색상)
  - 설정값 임시 저장: `_currentFontColor = hex` (Task 4에서 settings.json 연동)

**Step 7: 빌드 검증**

```bash
"/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "D:/99.실험실/OverlayNotepad/OverlayNotepad.sln" /p:Configuration=Debug /t:Build
```
- 예상: Build succeeded, 0 Errors

**Step 8: 커밋**

```
git add src/OverlayNotepad/MainWindow.xaml src/OverlayNotepad/MainWindow.xaml.cs src/OverlayNotepad/OverlayNotepad.csproj
git commit -m "feat(phase3-sprint1): task3 - 서식 변경 컨텍스트 메뉴 서브메뉴 (글꼴/크기/색상)"
```

**완료 기준:**
- ✅ 글꼴 서브메뉴에 설치된 프리셋 글꼴 표시 + "더 보기..." 항목
- ✅ 크기 서브메뉴에 7단계 프리셋 + "직접 입력..." 항목
- ✅ 색상 서브메뉴에 10색 프리셋 + "사용자 지정..." 항목
- ✅ ApplyFontFamily/Size/Color 메서드로 즉시 적용
- ✅ MSBuild 빌드 성공

---

### Task 4: MainWindow 통합 (트레이 + 서식 연동) + 설정 영속화

**skill:** `feature-dev:feature-dev`

**Files:**
- Modify: `src/OverlayNotepad/MainWindow.xaml.cs` (TrayIconManager 초기화, 최소화->트레이 숨김, 서식 설정 저장/복원 연동)
- Modify: `src/OverlayNotepad/App.xaml.cs` (TrayIconManager Dispose 보장)

**Step 1: TrayIconManager 초기화**

- `MainWindow.xaml.cs`에 필드 추가:
  - `private TrayIconManager _trayIconManager;`
- `Window_Loaded` 핸들러에 추가:
  1. `_trayIconManager = new TrayIconManager()`
  2. 아이콘 로드:
     - 리소스에서 `app.ico` 로드: `var iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/Resources/app.ico"))` -> `new System.Drawing.Icon(iconStream.Stream)`
     - 실패 시 폴백: `System.Drawing.SystemIcons.Application`
  3. `_trayIconManager.Initialize(icon)`
  4. 이벤트 연결:
     - `_trayIconManager.ToggleVisibilityRequested += OnToggleVisibility`
     - `_trayIconManager.AlwaysOnTopToggleRequested += OnTrayAlwaysOnTopToggle`
     - `_trayIconManager.ExitRequested += OnTrayExit`

**Step 2: 최소화 -> 트레이 숨김 로직**

- `MainWindow.xaml.cs`에 `StateChanged` 이벤트 핸들러 등록:
  - Window가 Minimized 상태가 되면:
    - `this.Hide()` (윈도우 숨김)
    - `this.ShowInTaskbar = false` (작업표시줄에서도 숨김)
- `OnToggleVisibility` 핸들러:
  - 숨겨져 있으면 (`this.IsVisible == false` 또는 `this.WindowState == Minimized`):
    - `this.Show()`
    - `this.WindowState = WindowState.Normal`
    - `this.ShowInTaskbar = true`
    - `this.Activate()` (포커스 가져오기)
  - 보이는 상태면:
    - `this.WindowState = WindowState.Minimized` (StateChanged에서 숨김 처리)

**Step 3: 트레이 이벤트 핸들러**

- `OnTrayAlwaysOnTopToggle` 핸들러:
  - `this.Topmost = !this.Topmost`
  - `_trayIconManager.UpdateAlwaysOnTopState(this.Topmost)`
  - 기존 컨텍스트 메뉴의 "항상 위에" 항목도 동기화
  - `SettingsManager.Instance.Current.Topmost = this.Topmost`
  - `SettingsManager.Instance.Save()`
- `OnTrayExit` 핸들러:
  - `Application.Current.Shutdown()`

**Step 4: 서식 설정 저장 연동**

- 기존 `ApplyFontFamily()`, `ApplyFontSize()`, `ApplyFontColor()` 메서드에 설정 저장 로직 추가:
  - `ApplyFontFamily(string fontName)` 끝에:
    - `SettingsManager.Instance.Current.Font.Family = fontName`
    - `SettingsManager.Instance.Save()`
  - `ApplyFontSize(double size)` 끝에:
    - `SettingsManager.Instance.Current.Font.Size = size`
    - `SettingsManager.Instance.Save()`
  - `ApplyFontColor(string hex)` 끝에:
    - `SettingsManager.Instance.Current.Font.Color = hex`
    - `SettingsManager.Instance.Save()`

**Step 5: 서식 설정 복원 (앱 시작 시)**

- `Window_Loaded` 핸들러에 서식 복원 로직 추가 (SettingsManager.Load() 이후):
  - `var font = SettingsManager.Instance.Current.Font`
  - `ApplyFontFamily(font.Family)` -- 저장된 글꼴 적용
  - `ApplyFontSize(font.Size)` -- 저장된 크기 적용
  - `ApplyFontColor(font.Color)` -- 저장된 색상 적용
  - 유효성 검증: `FontHelper.IsInstalled(font.Family)` -> false면 기본값 "맑은 고딕" 사용

**Step 6: TrayIconManager Dispose 보장**

- `MainWindow.xaml.cs`의 `Window_Closing` 핸들러에 추가:
  - `_trayIconManager?.Dispose()`
- `App.xaml.cs`의 예외 핸들러에 추가:
  - MainWindow에 접근하여 `_trayIconManager?.Dispose()` 호출
  - try-catch로 감싸서 Dispose 실패해도 추가 예외 방지

**Step 7: 기존 Topmost 동기화 보완**

- 기존 컨텍스트 메뉴의 "항상 위에" 토글 핸들러에서 트레이 메뉴 상태도 동기화:
  - `_trayIconManager?.UpdateAlwaysOnTopState(this.Topmost)`

**Step 8: 빌드 검증**

```bash
"/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "D:/99.실험실/OverlayNotepad/OverlayNotepad.sln" /p:Configuration=Debug /t:Build
```
- 예상: Build succeeded, 0 Errors

**Step 9: 커밋**

```
git add src/OverlayNotepad/MainWindow.xaml.cs src/OverlayNotepad/App.xaml.cs
git commit -m "feat(phase3-sprint1): task4 - MainWindow 통합 (트레이 + 서식 연동 + 설정 영속화)"
```

**완료 기준:**
- ✅ 시스템 트레이에 아이콘 표시
- ✅ 최소화 시 트레이로 숨김, 더블클릭/메뉴로 복원
- ✅ 트레이 메뉴에서 표시/숨김, Always on Top, 종료 동작
- ✅ 글꼴/크기/색상 변경 시 settings.json에 즉시 저장
- ✅ 프로그램 재실행 시 서식 설정 복원
- ✅ 정상/비정상 종료 시 트레이 아이콘 잔존 없음
- ✅ MSBuild 빌드 성공

---

### Task 5: 통합 검증 + 엣지 케이스

**skill:** --

**Files:**
- Modify: `src/OverlayNotepad/MainWindow.xaml.cs` (필요시 엣지 케이스 수정)
- Modify: `src/OverlayNotepad/Services/TrayIconManager.cs` (필요시 수정)

**Step 1: 트레이 아이콘 기능 검증**

```
검증 시나리오:
  1. 앱 실행 -> 시스템 트레이에 아이콘 표시 확인
  2. 트레이 아이콘에 마우스 호버 -> "OverlayNotepad" 툴팁 표시
  3. 윈도우 최소화 (Alt+F9 또는 최소화 동작) -> 작업표시줄에서 사라짐, 트레이 아이콘만 남음
  4. 트레이 아이콘 더블클릭 -> 윈도우 복원 + 포커스 획득
  5. 트레이 우클릭 -> 메뉴 표시 (표시/숨김, 항상 위에, 종료)
  6. "표시/숨김" 클릭 -> 윈도우 숨김/복원 토글
  7. "항상 위에" 토글 -> Topmost 변경 확인 (다른 창 위/아래 전환)
  8. 컨텍스트 메뉴의 "항상 위에" 토글 -> 트레이 메뉴의 "항상 위에" 상태 동기화 확인
  9. "종료" 클릭 -> 프로그램 종료 + 트레이 아이콘 즉시 사라짐
```

**Step 2: 서식 변경 기능 검증**

```
검증 시나리오:
  1. 우클릭 -> "글꼴" 서브메뉴 -> 설치된 글꼴만 표시 확인
  2. 글꼴 프리셋 선택 -> 전체 텍스트에 즉시 반영
  3. "더 보기..." -> FontDialog 표시 -> 글꼴 선택 -> 반영
  4. 우클릭 -> "글자 크기" -> 프리셋 선택 -> 즉시 반영
  5. "직접 입력..." -> 입력 대화상자 -> 유효 크기 입력 -> 반영
  6. 우클릭 -> "글자 색상" -> 프리셋 색상 선택 -> 즉시 반영
  7. "사용자 지정..." -> ColorDialog -> 색상 선택 -> 반영
  8. 테두리 ON 상태에서 글꼴 변경 -> OutlinedTextControl 동기화 확인
  9. 테두리 ON 상태에서 크기 변경 -> OutlinedTextControl 동기화 확인
  10. 테두리 ON 상태에서 색상 변경 -> OutlinedTextControl FillBrush 변경 확인
```

**Step 3: 설정 영속화 검증**

```
검증 시나리오:
  1. 글꼴을 Consolas로 변경 -> 종료 -> 재실행 -> Consolas 복원 확인
  2. 크기를 24로 변경 -> 종료 -> 재실행 -> 24pt 복원 확인
  3. 색상을 빨강으로 변경 -> 종료 -> 재실행 -> 빨강 복원 확인
  4. settings.json의 font 섹션 확인 -> 변경된 값이 저장되어 있는지
  5. settings.json에서 font.family를 시스템에 없는 글꼴로 수정 -> 실행 -> 기본값(맑은 고딕) 폴백 확인
```

**Step 4: 엣지 케이스 확인**

```
엣지 케이스:
  1. 트레이 아이콘 더블클릭 여러 번 빠르게 -> 윈도우 상태 정상 (깜빡임/다중 표시 없음)
  2. 윈도우 숨김 상태에서 프로그램 종료 (트레이 메뉴 "종료") -> 정상 종료 + 트레이 아이콘 사라짐
  3. Alt+F4로 종료 -> 트레이 아이콘 사라짐
  4. 매우 작은 크기(6pt) 직접 입력 -> 반영되지만 최소 제한 확인
  5. 매우 큰 크기(72pt) 직접 입력 -> 반영 확인
  6. 잘못된 크기 입력 (문자, 음수, 0) -> 무시하고 현재 값 유지
  7. 글꼴 변경 후 한글 입력 -> 해당 글꼴로 정상 표시
  8. 색상 변경 후 캐럿 색상도 함께 변경 확인
  9. 서식 변경 중 텍스트 내용/커서 위치 보존 확인
  10. 작업 관리자에서 프로세스 kill -> 트레이 아이콘 잔존 여부 (Windows 동작에 의존, 마우스 호버 시 사라짐)
```

**Step 5: 수정 사항 반영**

- 검증에서 발견된 문제 수정
- 주의: 트레이 아이콘 잔존은 Windows OS의 알려진 동작이므로, 정상 종료 시 Dispose만 보장하면 충분

**Step 6: 커밋**

```
git add src/OverlayNotepad/
git commit -m "fix(phase3-sprint1): task5 - 통합 검증 후 수정"
```

**완료 기준:**
- ✅ 트레이 아이콘 모든 기능 정상 동작
- ✅ 서식 변경 전체 텍스트 즉시 반영
- ✅ 서식 설정 저장/복원 정상
- ✅ 테두리 ON 상태에서 서식 변경 시 OutlinedTextControl 동기화
- ✅ 엣지 케이스 처리 (잘못된 입력, 빠른 더블클릭, 미설치 글꼴)
- ✅ Phase 1-2 기능 회귀 없음

---

## 최종 검증 계획

| 검증 항목 | 명령 / 방법 | 예상 결과 |
|-----------|-------------|-----------|
| MSBuild 빌드 | `MSBuild OverlayNotepad.sln /p:Configuration=Debug` | Build succeeded, 0 Errors |
| 트레이 아이콘 표시 | 앱 실행 후 시스템 트레이 확인 | 아이콘 표시, 툴팁 "OverlayNotepad" |
| 최소화 -> 트레이 | 윈도우 최소화 | 작업표시줄에서 사라짐, 트레이 아이콘 유지 |
| 트레이 더블클릭 | 트레이 아이콘 더블클릭 | 윈도우 복원 + 포커스 |
| 트레이 메뉴 | 트레이 우클릭 | "표시/숨김", "항상 위에", "종료" 표시 |
| 글꼴 변경 | 우클릭 > 글꼴 > Consolas | 전체 텍스트 글꼴 변경 |
| 글꼴 대화상자 | 우클릭 > 글꼴 > "더 보기..." | FontDialog 표시, 선택 시 적용 |
| 크기 변경 | 우클릭 > 글자 크기 > 24 | 전체 텍스트 크기 변경 |
| 크기 직접 입력 | 우클릭 > 글자 크기 > "직접 입력..." > 18 | 18pt 적용 |
| 색상 변경 | 우클릭 > 글자 색상 > 빨강 | 전체 텍스트 빨강 |
| 색상 대화상자 | 우클릭 > 글자 색상 > "사용자 지정..." | ColorDialog 표시, 선택 시 적용 |
| 서식 저장 | 글꼴 변경 -> 종료 -> 재실행 | 변경된 글꼴 복원 |
| 설정 JSON 확인 | `%AppData%/OverlayNotepad/settings.json` 확인 | font 섹션에 변경된 값 |
| 미설치 글꼴 폴백 | settings.json font.family를 "없는글꼴"로 수정 -> 실행 | "맑은 고딕" 기본값 사용 |
| 트레이 아이콘 정리 | Alt+F4 종료 후 트레이 확인 | 아이콘 즉시 사라짐 |
| 테두리+서식 연동 | 테두리 ON + 글꼴 변경 | OutlinedTextControl 글꼴 동기화 |
| 기존 기능 회귀 | 드래그/리사이즈/투명도/테두리/자동저장 | 모두 정상 동작 |
| 메모리 | 작업 관리자에서 확인 | 80MB 이하 |
