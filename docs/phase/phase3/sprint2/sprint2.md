# Sprint 2: 다크/라이트 테마 + 컨텍스트 메뉴 완성 (Phase 3)

**Goal:** ThemeManager로 다크/라이트 테마를 정의하고 전환 기능을 구현하며, 컨텍스트 메뉴에 PRD F11의 모든 항목을 배치하여 MVP 메뉴를 완성한다.

**Architecture:** ThemeManager(싱글톤)가 ThemeDefinition 기반으로 다크/라이트 테마 색상 세트를 관리하고, ApplyTheme()으로 MainWindow의 글자색/배경색/테두리색/드래그바 색상을 일괄 변경한다. 컨텍스트 메뉴는 WPF ContextMenu에 서브메뉴 1단계까지만 사용하여 글꼴/크기/색상/테마/테두리/그림자/자동저장 상태/최소화를 배치한다. Sprint 1에서 추가된 서식 관련 서브메뉴와 통합하여 최종 메뉴 구조를 완성한다.

**Tech Stack:** .NET Framework 4.8, WPF (XAML + 코드비하인드), System.Windows.Forms (FontDialog, ColorDialog)

**Sprint 기간:** 2026-04-17 ~ (사용자 검토 후 구현)
**이전 스프린트:** Sprint 1 (시스템 트레이 + 서식 지원)
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

## 실행 플랜

### Phase 1 (순차 -- 기반 모듈)
| Task | 설명 | 대상 | skill |
|------|------|------|-------|
| Task 1 | ThemeDefinition 모델 + ThemeManager 생성 | 데스크톱 | -- |

### Phase 2 (순차 -- ThemeManager 의존)
| Task | 설명 | 대상 | skill |
|------|------|------|-------|
| Task 2 | 테마 전환 기능 + MainWindow 통합 | 데스크톱 | `feature-dev:feature-dev` |

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
- Modify: `src/OverlayNotepad/Models/AppSettings.cs` (Theme 속성이 아직 문자열만이라면 그대로 유지, ThemeManager가 매핑)
- Modify: `src/OverlayNotepad/OverlayNotepad.csproj` (새 파일 Compile 항목 추가)

**Step 1: ThemeDefinition 모델 생성**

- `src/OverlayNotepad/Models/ThemeDefinition.cs` 생성
- 네임스페이스: `OverlayNotepad.Models`
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
  - `static ThemeDefinition CreateDark()` -- 다크 테마 확정값으로 생성
    - TextColor=#FFFFFF, BackgroundColor=#1E1E1E, BackgroundOpacity=0.8
    - OutlineColor=#000000, ShadowColor=#000000
    - DragBarColor=#333333, DragBarOpacity=0.9
  - `static ThemeDefinition CreateLight()` -- 라이트 테마 확정값으로 생성
    - TextColor=#000000, BackgroundColor=#FFFFFF, BackgroundOpacity=0.8
    - OutlineColor=#FFFFFF, ShadowColor=#808080
    - DragBarColor=#E0E0E0, DragBarOpacity=0.9

**Step 2: ThemeManager 서비스 생성**

- `src/OverlayNotepad/Services/ThemeManager.cs` 생성
- 네임스페이스: `OverlayNotepad.Services`
- 클래스: `ThemeManager` (public)
- 싱글톤: `public static ThemeManager Instance { get; }` (static readonly 초기화)
- 프로퍼티:
  - `ThemeDefinition DarkTheme { get; }` -- 생성자에서 `ThemeDefinition.CreateDark()` 호출
  - `ThemeDefinition LightTheme { get; }` -- 생성자에서 `ThemeDefinition.CreateLight()` 호출
  - `ThemeDefinition CurrentTheme { get; private set; }`
  - `string CurrentThemeName` -- "dark" / "light" (AppSettings와 연동용)
- 메서드:
  - `void SetTheme(string themeName)` -- "dark" / "light" 문자열로 현재 테마 설정
    - `CurrentTheme = themeName == "light" ? LightTheme : DarkTheme`
    - `CurrentThemeName = themeName`
  - `void ToggleTheme()` -- 현재 테마가 dark면 light로, light면 dark로 전환
    - `SetTheme(CurrentThemeName == "dark" ? "light" : "dark")`
  - `ThemeDefinition GetTheme(string name)` -- 이름으로 테마 조회
- 생성자에서 기본 테마를 "dark"로 설정

**Step 3: csproj에 파일 추가**

- `src/OverlayNotepad/OverlayNotepad.csproj` 수정
- `<Compile Include="Models\ThemeDefinition.cs" />` 추가
- `<Compile Include="Services\ThemeManager.cs" />` 추가

**Step 4: 빌드 검증**

```bash
"/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "D:/99.실험실/OverlayNotepad/OverlayNotepad.sln" /p:Configuration=Debug /t:Build
```
- 예상: Build succeeded, 0 Errors

**Step 5: 커밋**

```
git add src/OverlayNotepad/Models/ThemeDefinition.cs src/OverlayNotepad/Services/ThemeManager.cs src/OverlayNotepad/OverlayNotepad.csproj
git commit -m "feat(phase3-sprint2): task1 - ThemeDefinition 모델 + ThemeManager 서비스"
```

**완료 기준:**
- ⬜ ThemeDefinition.CreateDark() / CreateLight()가 확정 파라미터 색상값을 반환
- ⬜ ThemeManager.ToggleTheme()으로 dark <-> light 전환
- ⬜ MSBuild 빌드 성공

---

### Task 2: 테마 전환 기능 + MainWindow 통합

**skill:** `feature-dev:feature-dev`

**Files:**
- Modify: `src/OverlayNotepad/MainWindow.xaml` (배경/드래그바에 x:Name 추가, 테마 바인딩 준비)
- Modify: `src/OverlayNotepad/MainWindow.xaml.cs` (테마 적용 메서드, 테마 초기화, 테마 전환 로직)
- Modify: `src/OverlayNotepad/Services/SettingsManager.cs` (Theme 설정 저장/로드 시 ThemeManager 동기화)

**Step 1: MainWindow.xaml 테마 바인딩 준비**

- 배경색을 적용할 요소에 x:Name 추가 (아직 없다면):
  - 메인 Grid 또는 배경 Border: `x:Name="MainBackground"`
  - 드래그바 Border: `x:Name="DragBar"` (Phase 1에서 이미 있을 수 있음, 확인 후 진행)
- 기존 하드코딩된 색상값을 제거하지 않음 -- 코드비하인드에서 동적으로 덮어씌움

**Step 2: ApplyTheme() 메서드 구현**

- `MainWindow.xaml.cs`에 `private void ApplyTheme(ThemeDefinition theme)` 메서드 추가
- 일괄 변경 패턴 (깜빡임 방지):
  1. `Dispatcher.Invoke(() => { ... })` 블록으로 한 프레임 내 처리
  2. 글자색 변경:
     - `MainTextBox.Foreground = new SolidColorBrush(theme.TextColor)`
     - 테두리 ON 상태면 TextBox.Foreground는 Transparent 유지, OutlinedText.FillBrush만 변경
     - `OutlinedText.FillBrush = new SolidColorBrush(theme.TextColor)`
  3. 배경색 변경:
     - MainBackground의 Background = SolidColorBrush(theme.BackgroundColor) + Opacity = theme.BackgroundOpacity
     - 또는 배경 투명 모드 ON인 경우: Background = Transparent 유지 (테마의 배경색은 배경 투명 모드 OFF 시에만 적용)
  4. 테두리색 변경:
     - `OutlinedText.OutlineBrush = new SolidColorBrush(theme.OutlineColor)`
  5. 그림자색 변경:
     - `TextShadowEffect.Color = theme.ShadowColor`
  6. 드래그바 색상 변경:
     - DragBar.Background = SolidColorBrush(theme.DragBarColor) + Opacity = theme.DragBarOpacity
  7. Sprint 1 서식 설정과 충돌 방지:
     - 사용자가 글자색을 수동으로 변경한 경우, 테마 전환 시 테마 기본 색상으로 리셋됨 (의도된 동작)
     - 글꼴/크기는 테마와 무관 -- 테마 전환 시 변경하지 않음

**Step 3: 테마 초기화 (Window_Loaded에서)**

- Window_Loaded 핸들러에 추가 (SettingsManager.Load() 이후):
  1. `ThemeManager.Instance.SetTheme(SettingsManager.Instance.Current.Theme)` -- 저장된 테마 로드
  2. `ApplyTheme(ThemeManager.Instance.CurrentTheme)` -- 테마 적용

**Step 4: 테마 전환 실행 메서드**

- `private void ToggleTheme()` 메서드:
  1. `ThemeManager.Instance.ToggleTheme()`
  2. `ApplyTheme(ThemeManager.Instance.CurrentTheme)`
  3. `SettingsManager.Instance.Current.Theme = ThemeManager.Instance.CurrentThemeName`
  4. `SettingsManager.Instance.Save()` -- 테마 설정 즉시 저장
  5. Sprint 1에서 추가한 서식 설정도 테마에 맞게 업데이트:
     - `SettingsManager.Instance.Current.Font.Color` = 테마 글자색의 hex 문자열

**Step 5: 빌드 및 수동 검증**

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

**Step 6: 커밋**

```
git add src/OverlayNotepad/MainWindow.xaml src/OverlayNotepad/MainWindow.xaml.cs src/OverlayNotepad/Services/SettingsManager.cs
git commit -m "feat(phase3-sprint2): task2 - 테마 전환 기능 + MainWindow 통합"
```

**완료 기준:**
- ⬜ 다크 -> 라이트 테마 전환 시 모든 색상 일괄 변경
- ⬜ 라이트 -> 다크 테마 전환 시 모든 색상 일괄 변경
- ⬜ 테마 전환 시 텍스트 내용 보존
- ⬜ 테마 설정 영속화 (재실행 시 복원)
- ⬜ 배경 투명 모드와 테마 조합 정상 동작
- ⬜ MSBuild 빌드 성공

---

### Task 3: 컨텍스트 메뉴 최종 구성 (PRD F11 완성)

**skill:** `feature-dev:feature-dev`

**Files:**
- Modify: `src/OverlayNotepad/MainWindow.xaml` (컨텍스트 메뉴 전체 재구성)
- Modify: `src/OverlayNotepad/MainWindow.xaml.cs` (메뉴 이벤트 핸들러 추가/재구성)

**Step 1: 컨텍스트 메뉴 XAML 최종 구성**

Phase 1~3 Sprint 1에서 점진적으로 추가한 메뉴 항목을 Phase 3 확정 구조로 재구성한다.

최종 메뉴 구조:
```
[컨텍스트 메뉴]
+-- 투명도 >  20% / 40% / 60% / (v)80% / 100%     (Phase 1 Sprint 2 기존)
+-- ----구분선----
+-- (v) 배경 투명 모드                                (Phase 1 Sprint 2 기존)
+-- (v) Always on Top                               (Phase 1 Sprint 2 기존)
+-- ----구분선----
+-- 테마: 다크 <-> 라이트                             (신규)
+-- 글꼴 >  (v)맑은 고딕 / 나눔고딕 / ... / 더 보기... (Sprint 1 기존, 재배치)
+-- 글자 크기 >  10 / 12 / (v)14 / 16 / 20 / 24 / 32 / 직접 입력... (Sprint 1 기존, 재배치)
+-- 글자 색상 >  [10색 팔레트] / 사용자 지정...        (Sprint 1 기존, 재배치)
+-- ----구분선----
+-- (v) 텍스트 테두리                                 (Phase 2 Sprint 1 기존)
+-- 테두리 색상 >  [10색 팔레트] / 사용자 지정...       (신규)
+-- ----구분선----
+-- (회색) 자동 저장됨                                (신규, 비활성 상태 표시)
+-- ----구분선----
+-- 최소화                                           (신규)
+-- 종료                                             (Phase 1 Sprint 2 기존)
```

- XAML에서 ContextMenu를 위 순서로 재배치
- 테마 전환 MenuItem:
  - `x:Name="ThemeMenuItem"`
  - Header는 코드비하인드에서 동적 설정: "테마: 다크 -> 라이트" 또는 "테마: 라이트 -> 다크"
  - `Click="ThemeMenuItem_Click"`
- 테두리 색상 서브메뉴:
  - Sprint 1의 글자 색상 서브메뉴와 동일한 패턴 (10색 팔레트 + "사용자 지정...")
  - 색상 선택 시 `OutlinedText.OutlineBrush` 변경 + `SettingsManager.Instance.Current.TextEffect.OutlineColor` 업데이트
- 자동저장 상태 MenuItem:
  - `x:Name="AutoSaveStatusMenuItem"`
  - `IsEnabled="False"` (클릭 불가)
  - Header: "자동 저장됨" (Phase 2의 AutoSaveManager 상태에 따라 업데이트)
  - 저장 중일 때 "저장 중..." 표시 가능 (Optional, 기본은 "자동 저장됨")
- 최소화 MenuItem:
  - `Click="MinimizeMenuItem_Click"`
  - 핸들러: `this.WindowState = WindowState.Minimized` (트레이로 숨김은 Sprint 1의 TrayIconManager가 처리)

**Step 2: 테마 전환 핸들러**

- `ThemeMenuItem_Click` 이벤트 핸들러:
  1. `ToggleTheme()` 호출 (Task 2에서 구현)
  2. 메뉴 Header 텍스트 업데이트:
     - dark인 경우: "테마: 다크 -> 라이트"
     - light인 경우: "테마: 라이트 -> 다크"

**Step 3: 테두리 색상 서브메뉴 핸들러**

- Sprint 1의 글자 색상 팔레트 구현 패턴을 재사용
- 10색 프리셋 + "사용자 지정..." (ColorDialog)
- 색상 선택 시:
  1. `OutlinedText.OutlineBrush = new SolidColorBrush(selectedColor)`
  2. TextEffectSettings의 OutlineColor 업데이트
  3. AppSettings의 TextEffect.OutlineColor를 hex 문자열로 업데이트
  4. `SettingsManager.Instance.Save()`

**Step 4: 자동저장 상태 표시 연동**

- ContextMenu가 열릴 때(ContextMenu.Opened 이벤트) AutoSaveStatusMenuItem의 Header를 업데이트
- AutoSaveManager의 _isDirty 상태를 외부에서 조회할 수 있도록 `bool HasUnsavedChanges` 프로퍼티 추가 (SettingsManager 또는 AutoSaveManager에)
- Header 텍스트:
  - 미변경 상태: "자동 저장됨"
  - 변경 대기 중: "저장 대기 중..."

**Step 5: 최소화 핸들러**

- `MinimizeMenuItem_Click` 이벤트 핸들러:
  - `this.WindowState = WindowState.Minimized`
  - Sprint 1의 TrayIconManager가 StateChanged 이벤트를 감지하여 트레이로 숨김 처리

**Step 6: 메뉴 상태 동기화**

- ContextMenu.Opened 이벤트 핸들러에서:
  1. 투명도 서브메뉴: 현재 투명도에 체크마크 (기존)
  2. 배경 투명 모드: 현재 상태에 체크 (기존)
  3. Always on Top: 현재 상태에 체크 (기존)
  4. 테마 MenuItem Header 업데이트
  5. 글꼴 서브메뉴: 현재 글꼴에 체크마크 (Sprint 1)
  6. 글자 크기 서브메뉴: 현재 크기에 체크마크 (Sprint 1)
  7. 텍스트 테두리: 현재 상태에 체크 (Phase 2)
  8. 자동저장 상태 업데이트

**Step 7: 빌드 및 수동 검증**

```bash
"/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "D:/99.실험실/OverlayNotepad/OverlayNotepad.sln" /p:Configuration=Debug /t:Build
```
- 예상: Build succeeded, 0 Errors

수동 검증:
- ⬜ 컨텍스트 메뉴에 PRD F11의 모든 항목이 표시됨 (Click-Through 제외)
- ⬜ 테마 전환 메뉴에서 테마 토글 동작
- ⬜ 테두리 색상 서브메뉴에서 색상 변경 동작
- ⬜ 테두리 색상 변경이 settings.json에 저장
- ⬜ 자동저장 상태 표시 동작 ("자동 저장됨" / "저장 대기 중...")
- ⬜ 최소화 메뉴 클릭 시 트레이로 숨김
- ⬜ 서브메뉴 깊이가 1단계를 초과하지 않음
- ⬜ 각 서브메뉴에서 현재 선택값에 체크마크 표시

**Step 8: 커밋**

```
git add src/OverlayNotepad/MainWindow.xaml src/OverlayNotepad/MainWindow.xaml.cs
git commit -m "feat(phase3-sprint2): task3 - 컨텍스트 메뉴 최종 구성 (PRD F11 완성)"
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
- ⬜ 시스템 트레이 아이콘 표시 및 최소화/복원 동작 (Sprint 1)
- ⬜ 글꼴/크기/색상 변경이 전체 텍스트에 즉시 반영 (Sprint 1)
- ⬜ 변경된 서식이 저장되어 재실행 시 유지 (Sprint 1)
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
  - 글꼴 프리셋 확인 시 InstalledFontCollection 캐싱
  - 테마 전환 최적화 (불필요한 재렌더링 방지)

**Step 7: 커밋**

```
git add src/OverlayNotepad/MainWindow.xaml src/OverlayNotepad/MainWindow.xaml.cs src/OverlayNotepad/Services/ThemeManager.cs
git commit -m "fix(phase3-sprint2): task4 - 통합 검증 + 엣지 케이스 수정"
```

**완료 기준:**
- ⬜ 테마 전환 7가지 시나리오 모두 통과
- ⬜ 메뉴 항목 전체 클릭 테스트 통과
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
