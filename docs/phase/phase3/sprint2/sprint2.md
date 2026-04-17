# Sprint 2: 다크/라이트 테마 + 컨텍스트 메뉴 완성 (Phase 3)

**Goal:** ThemeManager로 다크/라이트 테마를 정의하고 전환 기능을 구현하며, 컨텍스트 메뉴에 PRD F11의 모든 항목을 배치하여 MVP 메뉴를 완성한다.

**Architecture:** ThemeManager(싱글톤)가 ThemeDefinition 기반으로 다크/라이트 테마 색상 세트를 관리하고, ApplyTheme()으로 MainWindow의 글자색/배경색/테두리색/드래그바 색상을 일괄 변경한다. 컨텍스트 메뉴는 WPF ContextMenu에 서브메뉴 1단계까지만 사용하여 글꼴/크기/색상/테마/테두리/그림자/자동저장 상태/최소화를 배치한다. Sprint 1에서 추가된 서식 관련 서브메뉴와 통합하여 최종 메뉴 구조를 완성한다.

**Tech Stack:** .NET Framework 4.8, WPF (XAML + 코드비하인드), System.Windows.Forms (FontDialog, ColorDialog)

**Sprint 기간:** 2026-04-17 ~ (사용자 검토 후 구현)
**이전 스프린트:** Sprint 1 (시스템 트레이 + 서식 지원, phase3-sprint1 -> develop 머지 완료)
**브랜치명:** `phase3-sprint2`

---

## 제외 범위

- Click-Through 토글 메뉴 항목 -- Phase 4 Sprint 1에서 구현
- 투명도 슬라이더 UI -- 확정 파라미터에 따라 5단계 선택으로 대체 (이미 Phase 1에서 구현된 형태)
- 테두리 두께 변경 UI -- 향후 Backlog
- 그림자 파라미터(blur, offset) 변경 UI -- 향후 Backlog
- 글로벌 핫키 -- Phase 4

## 확정 파라미터 (Phase 3 문서 기준)

| 항목 | 값 | 근거 |
|------|-----|------|
| 다크 테마 글자색 | #FFFFFF | PO 확정 |
| 다크 테마 배경색 | #1E1E1E (80% 투명) | PO 확정 |
| 다크 테마 테두리색 | #000000 | PO 확정 |
| 다크 테마 그림자색 | #000000 | PO 확정 |
| 다크 테마 드래그바 | #333333 (90% 불투명) | PO 확정 |
| 라이트 테마 글자색 | #000000 | PO 확정 |
| 라이트 테마 배경색 | #FFFFFF (80% 투명) | PO 확정 |
| 라이트 테마 테두리색 | #FFFFFF | PO 확정 |
| 라이트 테마 그림자색 | #808080 | PO 확정 |
| 라이트 테마 드래그바 | #E0E0E0 (90% 불투명) | PO 확정 |
| 서브메뉴 최대 깊이 | 1단계 | UX 확정 |
| 테마 전환 방식 | 일괄 변경 (깜빡임 방지) | UX 확정 |
| 글꼴 프리셋 | 맑은 고딕, 나눔고딕, 나눔바른고딕, D2Coding, Consolas, Arial, 굴림 (미설치 시 제외) | 행정+UX 확정 |
| 글꼴 크기 프리셋 | 10/12/14/16/20/24/32pt + 직접 입력 | UX 확정 |
| 색상 프리셋 | 흰색, 검정, 빨강, 파랑, 초록, 노랑, 회색, 주황, 보라, 하늘 (10색) | 행정 확정 |

## 현재 코드베이스 주요 현황 (Sprint 1 완료 시점)

| 파일 | 핵심 내용 |
|------|----------|
| `src/OverlayNotepad/Models/AppSettings.cs` | `Theme` 프로퍼티 이미 존재 (string, 기본값 "dark"), `Font` 설정 포함 |
| `src/OverlayNotepad/Services/SettingsManager.cs` | 싱글톤, Load/Save/SaveMemo/LoadMemo 구현 완료 |
| `src/OverlayNotepad/Services/AutoSaveManager.cs` | `_isDirty` 필드가 private, 외부 조회 불가 -- `HasUnsavedChanges` 프로퍼티 추가 필요 |
| `src/OverlayNotepad/Services/TrayIconManager.cs` | 트레이 아이콘 + 메뉴(표시/숨김, Always on Top, 종료) 구현 완료 |
| `src/OverlayNotepad/Controls/OutlinedTextControl.cs` | FillBrush, OutlineBrush, OutlineThickness 등 DependencyProperty로 구현 |
| `src/OverlayNotepad/MainWindow.xaml` | ContextMenu에 투명도/배경투명/테두리/그림자/글꼴/크기/색상/Always on Top/종료 이미 존재 |
| `src/OverlayNotepad/MainWindow.xaml.cs` | ApplyFontFamily/ApplyFontSize/ApplyFontColor, ApplyTextEffects, SyncOutlinedTextProperties 구현 완료 |

## Phase 3 미해결 사항 반영 (phase3.md 기준)

| 미해결 사항 | 반영 Task | 대응 |
|------------|----------|------|
| FontDialog/ColorDialog Dispose 미호출 | Task 2 | FontDialog_Click, ColorDialog_Click에서 `using` 블록으로 감싸기 |
| XAML 초기 IsChecked와 settings.json 불일치 | Task 2 | Window_Loaded에서 settings 로드 후 모든 CheckBox 메뉴 아이템 동기화 |
| 테마 전환 시 깜빡임 | Task 2 | Dispatcher.Invoke로 한 프레임 내 일괄 처리 |

## 실행 플랜

### Phase 1 (순차 -- 기반 모듈)
| Task | 설명 | 대상 | skill |
|------|------|------|-------|
| Task 1 | ThemeDefinition 모델 + ThemeManager 생성 | 데스크톱 | -- |

### Phase 2 (순차 -- ThemeManager 의존)
| Task | 설명 | 대상 | skill |
|------|------|------|-------|
| Task 2 | 테마 전환 기능 + MainWindow 통합 + Sprint 1 미해결 개선 | 데스크톱 | `feature-dev:feature-dev` |

### Phase 3 (순차 -- 테마 + 서식 통합)
| Task | 설명 | 대상 | skill |
|------|------|------|-------|
| Task 3 | 컨텍스트 메뉴 최종 구성 (PRD F11 완성) | 데스크톱 | `feature-dev:feature-dev` |

### Phase 4 (순차 -- 통합 검증)
| Task | 설명 | 대상 | skill |
|------|------|------|-------|
| Task 4 | 통합 검증 + 테마/메뉴 엣지 케이스 + Phase 3 DoD 확인 | 데스크톱 | -- |

> **병렬 불가**: 모든 Task가 MainWindow.xaml / MainWindow.xaml.cs를 공유하므로 순차 실행 필수.

---

### Task 1: ThemeDefinition 모델 + ThemeManager 생성

**skill:** --

**Files:**
- Create: `src/OverlayNotepad/Models/ThemeDefinition.cs`
- Create: `src/OverlayNotepad/Services/ThemeManager.cs`
- Modify: `src/OverlayNotepad/OverlayNotepad.csproj` (새 파일 Compile 항목 추가)

> AppSettings.cs는 수정 불필요 -- `Theme` 프로퍼티(string "dark"/"light")가 이미 존재하며 ThemeManager가 매핑.

**Step 1: ThemeDefinition 모델 생성**

- `src/OverlayNotepad/Models/ThemeDefinition.cs` 생성
- 네임스페이스: `OverlayNotepad.Models`
- using: `System.Windows.Media` (Color 타입)
- 클래스: `ThemeDefinition` (public)
- 프로퍼티:
  - `string Name` -- "dark" / "light"
  - `Color TextColor` -- 글자색
  - `Color BackgroundColor` -- 배경색 (불투명 기준)
  - `double BackgroundOpacity` -- 배경 투명도 (0.0~1.0, 기본 0.8)
  - `Color OutlineColor` -- 텍스트 테두리색
  - `Color ShadowColor` -- 텍스트 그림자색
  - `Color DragBarColor` -- 드래그바 색상
  - `double DragBarOpacity` -- 드래그바 불투명도 (기본 0.9)
- 정적 팩토리 메서드:
  - `static ThemeDefinition CreateDark()`:
    - Name="dark"
    - TextColor=Colors.White (#FFFFFF)
    - BackgroundColor=Color.FromRgb(0x1E, 0x1E, 0x1E) (#1E1E1E)
    - BackgroundOpacity=0.8
    - OutlineColor=Colors.Black (#000000)
    - ShadowColor=Colors.Black (#000000)
    - DragBarColor=Color.FromRgb(0x33, 0x33, 0x33) (#333333)
    - DragBarOpacity=0.9
  - `static ThemeDefinition CreateLight()`:
    - Name="light"
    - TextColor=Colors.Black (#000000)
    - BackgroundColor=Colors.White (#FFFFFF)
    - BackgroundOpacity=0.8
    - OutlineColor=Colors.White (#FFFFFF)
    - ShadowColor=Color.FromRgb(0x80, 0x80, 0x80) (#808080)
    - DragBarColor=Color.FromRgb(0xE0, 0xE0, 0xE0) (#E0E0E0)
    - DragBarOpacity=0.9

**Step 2: ThemeManager 서비스 생성**

- `src/OverlayNotepad/Services/ThemeManager.cs` 생성
- 네임스페이스: `OverlayNotepad.Services`
- using: `OverlayNotepad.Models`
- 클래스: `ThemeManager` (public)
- 싱글톤 패턴: `public static readonly ThemeManager Instance = new ThemeManager();`
- private 생성자에서:
  - `DarkTheme = ThemeDefinition.CreateDark()`
  - `LightTheme = ThemeDefinition.CreateLight()`
  - `CurrentTheme = DarkTheme` (기본 다크)
  - `CurrentThemeName = "dark"`
- 프로퍼티:
  - `ThemeDefinition DarkTheme { get; }` (readonly)
  - `ThemeDefinition LightTheme { get; }` (readonly)
  - `ThemeDefinition CurrentTheme { get; private set; }`
  - `string CurrentThemeName { get; private set; }` -- "dark" / "light"
- 메서드:
  - `void SetTheme(string themeName)`:
    - `CurrentThemeName = (themeName == "light") ? "light" : "dark"` (잘못된 값은 dark 폴백)
    - `CurrentTheme = (CurrentThemeName == "light") ? LightTheme : DarkTheme`
  - `void ToggleTheme()`:
    - `SetTheme(CurrentThemeName == "dark" ? "light" : "dark")`
  - `ThemeDefinition GetTheme(string name)`:
    - `return (name == "light") ? LightTheme : DarkTheme`

**Step 3: csproj에 파일 추가**

- `src/OverlayNotepad/OverlayNotepad.csproj`의 `<ItemGroup>` (Compile 항목 포함) 섹션에 추가:
  - `<Compile Include="Models\ThemeDefinition.cs" />`
  - `<Compile Include="Services\ThemeManager.cs" />`
- 기존 `<Compile Include="Services\TrayIconManager.cs" />` 뒤에 배치

**Step 4: 빌드 검증**

```bash
"/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "D:/99.실험실/OverlayNotepad/OverlayNotepad.sln" /p:Configuration=Debug /t:Build
```
- 예상: Build succeeded, 0 Errors

**Step 5: 커밋**

```
git add src/OverlayNotepad/Models/ThemeDefinition.cs src/OverlayNotepad/Services/ThemeManager.cs src/OverlayNotepad/OverlayNotepad.csproj
git commit -m "feat(phase3-sprint2): task1 -- ThemeDefinition 모델 + ThemeManager 서비스"
```

**완료 기준:**
- ⬜ ThemeDefinition.CreateDark() / CreateLight()가 확정 파라미터 색상값을 반환
- ⬜ ThemeManager.ToggleTheme()으로 dark <-> light 전환
- ⬜ ThemeManager.SetTheme("invalid")가 dark로 폴백
- ⬜ MSBuild 빌드 성공

---

### Task 2: 테마 전환 기능 + MainWindow 통합 + Sprint 1 미해결 개선

**skill:** `feature-dev:feature-dev`

**Files:**
- Modify: `src/OverlayNotepad/MainWindow.xaml` (드래그바에 x:Name 추가, 배경 레이어 구조 변경)
- Modify: `src/OverlayNotepad/MainWindow.xaml.cs` (테마 적용 메서드, 테마 초기화, 메뉴 동기화, Dialog Dispose 개선)
- Modify: `src/OverlayNotepad/Services/AutoSaveManager.cs` (HasUnsavedChanges 프로퍼티 추가)

**Step 1: MainWindow.xaml 테마 바인딩 준비**

현재 XAML 구조 분석:
- 드래그바 Border: `Height="12"` -- x:Name이 없음 -> `x:Name="DragBar"` 추가 필요
- 메인 Grid: Background 없음 (Window 자체가 Transparent) -> 배경색을 적용할 Border 또는 Grid 추가 필요
  - 방법: 기존 `<Grid>` 전체를 `<Border x:Name="MainBackground" ... >` 으로 감싸기
  - MainBackground.Background = 테마 배경색 (투명 모드 OFF 시), Transparent (투명 모드 ON 시)
  - MainBackground.Opacity = 테마 BackgroundOpacity

수정 내용:
1. 최상위 Grid를 Border로 감싸기:
   ```xml
   <Border x:Name="MainBackground" Background="Transparent" CornerRadius="0">
     <Grid>
       <!-- 기존 드래그바 + 텍스트 영역 -->
     </Grid>
   </Border>
   ```
2. 드래그바 Border에 `x:Name="DragBar"` 추가
3. 드래그바 Style의 Trigger에서 IsMouseOver 시 색상을 테마에 맞게 변경 -- 코드비하인드에서 동적으로 처리하므로 XAML에서는 기본값만 유지

**Step 2: ApplyTheme() 메서드 구현**

`MainWindow.xaml.cs`에 추가:

```
private void ApplyTheme(ThemeDefinition theme)
```

일괄 변경 패턴 (깜빡임 방지 -- Dispatcher.Invoke 내에서 모든 변경 수행):

1. **글자색**:
   - `OutlinedText.FillBrush = new SolidColorBrush(theme.TextColor)`
   - `MainTextBox.CaretBrush = new SolidColorBrush(theme.TextColor)`
   - 테두리 OFF 상태면: `MainTextBox.Foreground = new SolidColorBrush(theme.TextColor)`
   - 테두리 ON 상태면: `MainTextBox.Foreground = Brushes.Transparent` (유지)

2. **배경색**:
   - `SettingsManager.Instance.Current.Transparency.Mode`가 "background"(투명모드 ON)이면:
     - `MainBackground.Background = Brushes.Transparent` (배경 투명 유지)
   - "solid"(투명모드 OFF)이면:
     - `var bgColor = theme.BackgroundColor; bgColor.A = (byte)(theme.BackgroundOpacity * 255);`
     - `MainBackground.Background = new SolidColorBrush(bgColor)`

3. **테두리색**:
   - `OutlinedText.OutlineBrush = new SolidColorBrush(theme.OutlineColor)`
   - `SettingsManager.Instance.Current.TextEffect.OutlineColor = theme.OutlineColor hex 문자열`

4. **그림자색**:
   - `TextShadowEffect.Color = theme.ShadowColor`

5. **드래그바**:
   - `var dragColor = theme.DragBarColor; dragColor.A = (byte)(theme.DragBarOpacity * 255);`
   - `DragBar.Background = new SolidColorBrush(dragColor)`

6. **서식 설정 동기화**:
   - `SettingsManager.Instance.Current.Font.Color = ColorToHex(theme.TextColor)`
   - 글꼴/크기는 변경하지 않음 (테마와 무관)

**Step 3: ColorToHex 헬퍼 메서드**

`MainWindow.xaml.cs`에 추가:
```
private static string ColorToHex(Color c) => string.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B);
```

**Step 4: 테마 초기화 (Window_Loaded 수정)**

기존 Window_Loaded 핸들러에서, "서식 설정 복원" 블록 뒤에 추가:

```
// 테마 초기화
ThemeManager.Instance.SetTheme(settings.Theme ?? "dark");
ApplyTheme(ThemeManager.Instance.CurrentTheme);

// 메뉴 체크 상태 동기화 (Phase 3 미해결 사항 개선)
SyncMenuCheckedStates(settings);
```

**Step 5: SyncMenuCheckedStates 메서드**

XAML 하드코딩된 IsChecked와 실제 settings 불일치 해결:
```
private void SyncMenuCheckedStates(AppSettings settings)
{
    // 배경 투명 모드
    BackgroundTransparentMenuItem.IsChecked = (settings.Transparency.Mode == "background");
    // Always on Top
    AlwaysOnTopMenuItem.IsChecked = settings.Topmost;
    // 텍스트 테두리
    OutlineMenuItem.IsChecked = settings.TextEffect.OutlineEnabled;
    // 텍스트 그림자
    ShadowMenuItem.IsChecked = settings.TextEffect.ShadowEnabled;
    // 투명도 서브메뉴 체크
    foreach (object item in OpacityMenu.Items)
    {
        if (item is MenuItem mi && mi.Tag != null)
            mi.IsChecked = (double.Parse(mi.Tag.ToString()) == settings.Transparency.Opacity);
    }
}
```

> 참고: BackgroundTransparentMenuItem에 x:Name이 현재 XAML에 없음. `x:Name="BackgroundTransparentMenuItem"` 추가 필요.

**Step 6: ToggleTheme 메서드**

```
private void ToggleTheme()
{
    ThemeManager.Instance.ToggleTheme();
    ApplyTheme(ThemeManager.Instance.CurrentTheme);
    SettingsManager.Instance.Current.Theme = ThemeManager.Instance.CurrentThemeName;
    SettingsManager.Instance.Save();
}
```

**Step 7: AutoSaveManager.HasUnsavedChanges 프로퍼티 추가**

`src/OverlayNotepad/Services/AutoSaveManager.cs` 수정:
- `public bool HasUnsavedChanges => _isDirty;` 프로퍼티 추가 (기존 `_isDirty` 필드를 외부에서 조회 가능)

**Step 8: FontDialog/ColorDialog Dispose 개선**

`MainWindow.xaml.cs`의 기존 메서드 수정:

- `FontDialog_Click`: `var dialog = new FontDialog()` -> `using (var dialog = new FontDialog()) { ... }` 패턴으로 변경
- `ColorDialog_Click`: `var dialog = new ColorDialog()` -> `using (var dialog = new ColorDialog()) { ... }` 패턴으로 변경

**Step 9: 빌드 및 수동 검증**

```bash
"/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "D:/99.실험실/OverlayNotepad/OverlayNotepad.sln" /p:Configuration=Debug /t:Build
```
- 예상: Build succeeded, 0 Errors

수동 검증:
- ⬜ 앱 실행 시 기본 다크 테마 적용 (흰색 글자, #1E1E1E 배경, 검정 테두리)
- ⬜ 테마 전환 시 글자색/배경색/테두리색/드래그바 색상 일괄 변경
- ⬜ 테마 전환 시 깜빡임 없음
- ⬜ 테마 전환 시 텍스트 내용 유지
- ⬜ 테마 설정이 settings.json에 저장, 재실행 시 복원
- ⬜ 배경 투명 모드 ON 상태에서 테마 전환 시 배경이 투명 유지
- ⬜ 메뉴 체크 상태가 실제 settings 값과 일치

**Step 10: 커밋**

```
git add src/OverlayNotepad/MainWindow.xaml src/OverlayNotepad/MainWindow.xaml.cs src/OverlayNotepad/Services/AutoSaveManager.cs
git commit -m "feat(phase3-sprint2): task2 -- 테마 전환 기능 + MainWindow 통합 + Dialog Dispose 개선"
```

**완료 기준:**
- ⬜ 다크 -> 라이트 테마 전환 시 모든 색상 일괄 변경
- ⬜ 라이트 -> 다크 테마 전환 시 모든 색상 일괄 변경
- ⬜ 테마 전환 시 텍스트 내용 보존
- ⬜ 테마 설정 영속화 (재실행 시 복원)
- ⬜ 배경 투명 모드와 테마 조합 정상 동작
- ⬜ FontDialog/ColorDialog using 블록으로 Dispose 보장
- ⬜ 메뉴 IsChecked가 settings.json 로드 후 동기화
- ⬜ MSBuild 빌드 성공

---

### Task 3: 컨텍스트 메뉴 최종 구성 (PRD F11 완성)

**skill:** `feature-dev:feature-dev`

**Files:**
- Modify: `src/OverlayNotepad/MainWindow.xaml` (컨텍스트 메뉴 전체 재구성)
- Modify: `src/OverlayNotepad/MainWindow.xaml.cs` (메뉴 이벤트 핸들러 추가/재구성)

**Step 1: 컨텍스트 메뉴 XAML 최종 구성**

Phase 1~3 Sprint 1에서 점진적으로 추가한 메뉴 항목을 Phase 3 확정 구조로 재구성한다.

최종 메뉴 구조 (PRD F11 기준, Phase 3 phase3.md 확정):
```
[컨텍스트 메뉴]
+-- 투명도 >  20% / 40% / 60% / (v)80% / 100%     (Phase 1 Sprint 2 기존)
+-- ----구분선----
+-- (v) 배경 투명 모드                                (Phase 1 Sprint 2 기존)
+-- (v) Always on Top                               (Phase 1 Sprint 2 기존)
+-- ----구분선----
+-- 테마: 다크 -> 라이트                              (신규)
+-- 글꼴 >  (v)맑은 고딕 / 나눔고딕 / ... / 더 보기... (Sprint 1 기존, 재배치)
+-- 글자 크기 >  10 / 12 / (v)14 / 16 / 20 / 24 / 32 / 직접 입력... (Sprint 1 기존, 재배치)
+-- 글자 색상 >  [10색 팔레트] / 사용자 지정...        (Sprint 1 기존, 재배치)
+-- ----구분선----
+-- (v) 텍스트 테두리                                 (Phase 2 Sprint 1 기존)
+-- 테두리 색상 >  [10색 팔레트] / 사용자 지정...       (신규)
+-- (v) 텍스트 그림자                                 (Phase 2 Sprint 1 기존)
+-- ----구분선----
+-- (회색) 자동 저장됨                                (신규, 비활성 상태 표시)
+-- ----구분선----
+-- 최소화                                           (신규)
+-- 종료                                             (Phase 1 Sprint 2 기존)
```

XAML 수정 내용:
- 기존 ContextMenu를 위 순서로 재배치 (기존 항목은 순서만 변경, 신규 항목 추가)
- 테마 전환 MenuItem 추가:
  - `<MenuItem x:Name="ThemeMenuItem" Header="테마: 다크 -> 라이트" Click="ThemeMenuItem_Click" />`
  - Header는 코드비하인드에서 ContextMenu.Opened 이벤트 시 동적 업데이트
- 테두리 색상 서브메뉴 추가:
  - `<MenuItem x:Name="OutlineColorMenu" Header="테두리 색상" />`
  - 동적 생성 (Sprint 1의 글자 색상 팔레트와 동일 패턴)
- 자동저장 상태 추가:
  - `<MenuItem x:Name="AutoSaveStatusMenuItem" Header="자동 저장됨" IsEnabled="False" />`
- 최소화 추가:
  - `<MenuItem Header="최소화" Click="MinimizeMenuItem_Click" />`

**Step 2: 테마 전환 핸들러**

```
private void ThemeMenuItem_Click(object sender, RoutedEventArgs e)
{
    ToggleTheme();  // Task 2에서 구현
    UpdateThemeMenuHeader();
}

private void UpdateThemeMenuHeader()
{
    if (ThemeManager.Instance.CurrentThemeName == "dark")
        ThemeMenuItem.Header = "테마: 다크 -> 라이트";
    else
        ThemeMenuItem.Header = "테마: 라이트 -> 다크";
}
```

**Step 3: 테두리 색상 서브메뉴 초기화**

Sprint 1의 `InitializeFontColorMenu()` 패턴을 재사용:

```
private void InitializeOutlineColorMenu()
```

- FontHelper.GetPresetColors()로 10색 팔레트 생성 (동일 패턴)
- 각 항목 클릭 시 OutlineColorPreset_Click 핸들러
- "사용자 지정..." 항목 클릭 시 OutlineColorDialog_Click 핸들러
- Window_Loaded에서 호출 (InitializeFontColorMenu 다음)

색상 변경 핸들러:
```
private void ApplyOutlineColor(string hex)
{
    var color = (Color)ColorConverter.ConvertFromString(hex);
    OutlinedText.OutlineBrush = new SolidColorBrush(color);
    SettingsManager.Instance.Current.TextEffect.OutlineColor = hex;
    SettingsManager.Instance.Save();
}
```

OutlineColorPreset_Click: MenuItem.Tag에서 hex 추출 -> ApplyOutlineColor(hex) 호출
OutlineColorDialog_Click: `using (var dialog = new ColorDialog()) { ... }` -> ApplyOutlineColor(hex) 호출

**Step 4: 자동저장 상태 표시 연동**

ContextMenu.Opened 이벤트 핸들러에서:
```
AutoSaveStatusMenuItem.Header = _autoSaveManager?.HasUnsavedChanges == true
    ? "저장 대기 중..."
    : "자동 저장됨";
```

ContextMenu.Opened 이벤트 바인딩 (XAML):
```xml
<ContextMenu Opened="ContextMenu_Opened">
```

ContextMenu_Opened 핸들러에서 모든 메뉴 상태 동기화:
1. 투명도 체크마크 업데이트
2. 배경 투명 모드 체크 업데이트
3. Always on Top 체크 업데이트
4. 테마 MenuItem Header 업데이트 (`UpdateThemeMenuHeader()`)
5. 글꼴 크기 서브메뉴 체크마크 업데이트
6. 텍스트 테두리 체크 업데이트
7. 텍스트 그림자 체크 업데이트
8. 자동저장 상태 업데이트

**Step 5: 최소화 핸들러**

```
private void MinimizeMenuItem_Click(object sender, RoutedEventArgs e)
{
    this.WindowState = WindowState.Minimized;
    // Sprint 1의 Window_StateChanged가 트레이로 숨김 처리
}
```

**Step 6: 빌드 및 수동 검증**

```bash
"/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "D:/99.실험실/OverlayNotepad/OverlayNotepad.sln" /p:Configuration=Debug /t:Build
```
- 예상: Build succeeded, 0 Errors

수동 검증:
- ⬜ 컨텍스트 메뉴에 PRD F11의 모든 항목이 표시됨 (Click-Through 제외)
- ⬜ 테마 전환 메뉴에서 테마 토글 동작
- ⬜ 테마 전환 시 메뉴 Header 텍스트 업데이트
- ⬜ 테두리 색상 서브메뉴에서 색상 변경 동작
- ⬜ 테두리 색상 변경이 settings.json에 저장
- ⬜ 테두리 색상 ColorDialog에서 using 블록 Dispose 확인
- ⬜ 자동저장 상태 표시 동작 ("자동 저장됨" / "저장 대기 중...")
- ⬜ 최소화 메뉴 클릭 시 트레이로 숨김
- ⬜ 서브메뉴 깊이가 1단계를 초과하지 않음
- ⬜ ContextMenu 열 때 각 서브메뉴에서 현재 선택값에 체크마크 표시

**Step 7: 커밋**

```
git add src/OverlayNotepad/MainWindow.xaml src/OverlayNotepad/MainWindow.xaml.cs
git commit -m "feat(phase3-sprint2): task3 -- 컨텍스트 메뉴 최종 구성 (PRD F11 완성)"
```

**완료 기준:**
- ⬜ PRD F11 메뉴 항목 전체 배치 (Click-Through 제외)
- ⬜ 테마 전환 토글 동작
- ⬜ 테두리 색상 변경 + 저장
- ⬜ 자동저장 상태 표시
- ⬜ 최소화 -> 트레이 숨김 동작
- ⬜ 서브메뉴 깊이 1단계 이내
- ⬜ MSBuild 빌드 성공

---

### Task 4: 통합 검증 + 테마/메뉴 엣지 케이스 + Phase 3 DoD 확인

**skill:** --

**Files:**
- Modify: `src/OverlayNotepad/MainWindow.xaml` (필요 시 수정)
- Modify: `src/OverlayNotepad/MainWindow.xaml.cs` (필요 시 수정)
- Modify: `src/OverlayNotepad/Services/ThemeManager.cs` (필요 시 수정)

**Step 1: 테마 전환 통합 검증**

```
테마 전환 시나리오:
  1. 다크 -> 라이트 전환
     - 글자색: #FFFFFF -> #000000
     - 배경: #1E1E1E/80% -> #FFFFFF/80%
     - 테두리: #000000 -> #FFFFFF
     - 그림자: #000000 -> #808080
     - 드래그바: #333333/90% -> #E0E0E0/90%
     - 텍스트 내용 유지, 커서 위치 유지
  2. 라이트 -> 다크 전환
     - 위와 반대 색상으로 복원
  3. 빠른 연속 전환 (5회 이상 빠르게 토글)
     - 깜빡임 없이 정상 전환
     - 최종 상태가 올바른 테마
  4. 배경 투명 모드 ON + 테마 전환
     - 배경이 투명 유지, 글자색/테두리/그림자만 변경
  5. 배경 투명 모드 OFF + 테마 전환
     - 배경색도 함께 변경
  6. 테두리 OFF 상태에서 테마 전환
     - TextBox.Foreground가 테마 글자색으로 변경
  7. 사용자 지정 글자색 상태에서 테마 전환
     - 테마 기본 색상으로 리셋 (의도된 동작)
```

**Step 2: 컨텍스트 메뉴 통합 검증**

```
메뉴 검증 시나리오:
  1. 모든 메뉴 항목 클릭 테스트 (하나씩)
  2. 서브메뉴 열기/닫기 테스트
  3. 글꼴 변경 -> 테마 전환 -> 글꼴 유지 확인
  4. 글자 크기 변경 -> 테마 전환 -> 크기 유지 확인
  5. 테두리 색상 변경 -> 테마 전환 -> 테마 기본 테두리 색상으로 리셋 확인
  6. 미설치 글꼴이 프리셋에서 제외되는지 확인
  7. "직접 입력..." 메뉴에서 InputBox 취소 시 크기 유지
  8. "사용자 지정..." ColorDialog에서 취소 시 색상 유지
  9. 투명도 서브메뉴에서 현재 투명도에 체크마크 표시 확인
  10. ContextMenu 열 때마다 자동저장 상태 갱신 확인
```

**Step 3: 설정 영속화 검증**

```
영속화 시나리오:
  1. 테마 변경 -> 종료 -> 재실행 -> 변경된 테마 유지
  2. 글꼴 변경 -> 종료 -> 재실행 -> 변경된 글꼴 유지
  3. 테두리 색상 변경 -> 종료 -> 재실행 -> 변경된 색상 유지
  4. settings.json 삭제 -> 실행 -> 기본값(다크 테마) 정상 시작
  5. settings.json에서 theme을 "light"로 수동 편집 -> 실행 -> 라이트 테마 적용
  6. settings.json에서 theme을 "invalid"로 수동 편집 -> 실행 -> 다크 테마(기본) 적용
```

**Step 4: Phase 3 완료 기준 확인**

Phase 3 전체 DoD (ROADMAP.md 기준):
- ⬜ 시스템 트레이 아이콘 표시 및 최소화/복원 동작 (Sprint 1 완료 -- 재확인)
- ⬜ 글꼴/크기/색상 변경이 전체 텍스트에 즉시 반영 (Sprint 1 완료 -- 재확인)
- ⬜ 변경된 서식이 저장되어 재실행 시 유지 (Sprint 1 완료 -- 재확인)
- ⬜ 다크/라이트 테마 전환 시 모든 색상이 일괄 변경 (이번 Sprint)
- ⬜ 컨텍스트 메뉴에서 PRD F11의 모든 항목 접근 가능 (이번 Sprint)
- ⬜ 테마 전환 시 텍스트 내용 유지 (이번 Sprint)
- ⬜ 시작 시간 2초 이내 (이번 Sprint에서 측정)

**Step 5: 성능 측정**

- 앱 시작 시간 측정 (System.Windows.Forms 참조 + ThemeManager 초기화 포함)
  - Stopwatch로 MainWindow.Loaded 이벤트까지 시간 측정
  - 기준: 2초 이내
- 메모리 사용량 확인 (작업 관리자)
  - 기준: 80MB 이하

**Step 6: 엣지 케이스 수정 반영**

- 검증 중 발견된 문제 수정
- 성능 기준 초과 시 대응:
  - 글꼴 프리셋 확인 시 InstalledFontCollection 캐싱 (이미 FontHelper에서 캐싱됨 -- 확인)
  - 테마 전환 최적화 (불필요한 재렌더링 방지)

**Step 7: 커밋 (수정사항이 있을 경우에만)**

```
git add -u
git commit -m "fix(phase3-sprint2): task4 -- 통합 검증 + 엣지 케이스 수정"
```

**완료 기준:**
- ⬜ 테마 전환 7가지 시나리오 모두 통과
- ⬜ 메뉴 항목 전체 클릭 테스트 통과 (10개 시나리오)
- ⬜ 설정 영속화 6가지 시나리오 모두 통과
- ⬜ Phase 3 DoD 전체 항목 확인
- ⬜ 시작 시간 2초 이내
- ⬜ 메모리 80MB 이하
- ⬜ MSBuild 빌드 성공

---

## 최종 검증 계획

| 검증 항목 | 명령/방법 | 예상 결과 |
|-----------|----------|-----------|
| MSBuild 빌드 | `MSBuild OverlayNotepad.sln /p:Configuration=Debug` | Build succeeded, 0 Errors |
| 다크 테마 적용 | 앱 실행 (기본) 후 색상 확인 | 글자 #FFFFFF, 배경 #1E1E1E/80%, 테두리 #000000 |
| 라이트 테마 적용 | 메뉴에서 테마 전환 후 색상 확인 | 글자 #000000, 배경 #FFFFFF/80%, 테두리 #FFFFFF |
| 테마 전환 깜빡임 | 테마 토글 시 육안 확인 | 깜빡임 없이 일괄 변경 |
| 테마 + 텍스트 보존 | 테마 전환 전후 텍스트 비교 | 텍스트 내용 동일 |
| 테마 영속화 | 테마 변경 -> 종료 -> 재실행 | 변경된 테마 유지 |
| 컨텍스트 메뉴 전체 | 우클릭 후 메뉴 항목 확인 | PRD F11 항목 전체 표시 (Click-Through 제외) |
| 테두리 색상 변경 | 메뉴 > 테두리 색상 > 색상 선택 | 테두리 색상 즉시 변경 |
| 자동저장 상태 | 텍스트 미변경 시 메뉴 확인 | "자동 저장됨" 표시 |
| 자동저장 대기 | 텍스트 변경 직후 메뉴 확인 | "저장 대기 중..." 표시 |
| 최소화 | 메뉴 > 최소화 | 트레이로 숨김 |
| 서브메뉴 깊이 | 모든 서브메뉴 확인 | 1단계 이내 |
| 시작 시간 | Stopwatch 측정 | 2초 이내 |
| 메모리 | 작업 관리자 확인 | 80MB 이하 |
| 서식 + 테마 조합 | 글꼴 변경 -> 테마 전환 | 글꼴 유지, 색상만 변경 |
| 투명 모드 + 테마 | 배경 투명 ON + 테마 전환 | 배경 투명 유지, 글자색 변경 |
| settings.json 손상 | theme을 "invalid"로 수정 후 실행 | 다크 테마 기본 적용 |
| Dialog Dispose | FontDialog/ColorDialog 사용 후 Task Manager | 메모리 누수 없음 |
| 메뉴 IsChecked 동기화 | settings.json 수동 편집 후 실행 | 메뉴 체크 상태가 settings 값과 일치 |
