# Sprint 1: WPF 프로젝트 구조 + 투명 윈도우 + 기본 텍스트 입력 (Phase 1)

**Goal:** .NET Framework 4.8 WPF 프로젝트를 생성하고, 타이틀바 없는 투명 윈도우에서 텍스트를 입력할 수 있는 동작하는 프로토타입을 완성한다.

**Architecture:** WPF 솔루션을 생성하고, MainWindow에 AllowsTransparency + WindowChrome 조합으로 투명 윈도우를 구성한다. 상단 12px 드래그 영역과 전체 영역 TextBox를 배치하여 즉시 타이핑 가능한 프로토타입을 만든다.

**Tech Stack:** .NET Framework 4.8, WPF (XAML + 코드비하인드), WindowChrome

**Sprint 기간:** 2026-04-17 ~ (사용자 검토 후 구현)
**이전 스프린트:** 없음 (Phase 0은 인프라 설정, 코드 없음)
**브랜치명:** `phase1-sprint1`

---

## 제외 범위

- 투명도 모드 전환 (배경 투명 / 전체 투명) — Sprint 2
- Always on Top 토글 — Sprint 2
- 컨텍스트 메뉴 — Sprint 2
- 투명도 수치 조절 — Sprint 2
- 텍스트 테두리/그림자 효과 — Phase 2
- 설정 저장/복원 — Phase 2
- 시스템 트레이 — Phase 3

## 실행 플랜

이 Sprint는 모두 같은 솔루션/프로젝트 파일에 의존하므로 순차 실행이 필수이다.

### Phase 1 (순차)
| Task | 설명 | 대상 | skill |
|------|------|------|-------|
| Task 1 | WPF 솔루션 및 프로젝트 생성 | 데스크톱 | — |
| Task 2 | 투명 윈도우 + 드래그/리사이즈 | 데스크톱 | — |
| Task 3 | TextBox 배치 + 텍스트 입력 검증 | 데스크톱 | — |

> **병렬 불가**: 모든 Task가 동일 프로젝트 파일(MainWindow.xaml, MainWindow.xaml.cs)을 수정하므로 순차 실행 필수.

---

### Task 1: WPF 솔루션 및 프로젝트 생성

**skill:** —

**Files:**
- Create: `OverlayNotepad.sln`
- Create: `src/OverlayNotepad/OverlayNotepad.csproj`
- Create: `src/OverlayNotepad/App.xaml`
- Create: `src/OverlayNotepad/App.xaml.cs`
- Create: `src/OverlayNotepad/MainWindow.xaml` (빈 Window 선언)
- Create: `src/OverlayNotepad/MainWindow.xaml.cs` (빈 코드비하인드)
- Create: `src/OverlayNotepad/Properties/AssemblyInfo.cs`

**Step 1: 솔루션 파일 생성**

- `OverlayNotepad.sln` 을 프로젝트 루트(`D:\99.실험실\OverlayNotepad\`)에 생성
- 솔루션 내 프로젝트 참조: `src\OverlayNotepad\OverlayNotepad.csproj`

**Step 2: WPF 프로젝트 파일 생성**

- `src/OverlayNotepad/OverlayNotepad.csproj` 생성
- TargetFrameworkVersion: `v4.8`
- OutputType: `WinExe`
- RootNamespace: `OverlayNotepad`
- AssemblyName: `OverlayNotepad`
- 참조 어셈블리: `PresentationCore`, `PresentationFramework`, `WindowsBase`, `System.Xaml`, `System`
- ApplicationDefinition: `App.xaml`
- Page: `MainWindow.xaml`
- Compile: `App.xaml.cs`, `MainWindow.xaml.cs`, `Properties\AssemblyInfo.cs`

**Step 3: App.xaml / App.xaml.cs 생성**

- `App.xaml`: Application 루트 요소, `StartupUri="MainWindow.xaml"`
- `App.xaml.cs`: 빈 partial class `App : Application`

**Step 4: MainWindow.xaml / MainWindow.xaml.cs 생성 (최소 구조)**

- `MainWindow.xaml`: 기본 Window 선언 (Title="OverlayNotepad", Width=400, Height=300)
- `MainWindow.xaml.cs`: 빈 partial class `MainWindow : Window`, 생성자에 `InitializeComponent()` 호출

**Step 5: AssemblyInfo.cs 생성**

- 어셈블리 제목: "OverlayNotepad"
- 버전: 0.1.0.0

**Step 6: 빌드 검증**

```bash
# MSBuild로 빌드 (Visual Studio 또는 Developer Command Prompt)
# MSBuild 경로는 환경에 따라 다를 수 있음
"/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "D:/99.실험실/OverlayNotepad/OverlayNotepad.sln" /p:Configuration=Debug /t:Build
```
- 예상: Build succeeded, 0 Errors
- 출력: `src/OverlayNotepad/bin/Debug/OverlayNotepad.exe` 생성

**Step 7: 커밋**

```
git add OverlayNotepad.sln src/OverlayNotepad/
git commit -m "feat(phase1-sprint1): task1 - WPF 솔루션 및 프로젝트 구조 생성"
```

**완료 기준:**
- ⬜ MSBuild 빌드 성공 (0 Errors)
- ⬜ OverlayNotepad.exe 실행 시 기본 윈도우 표시

---

### Task 2: 투명 윈도우 + 드래그/리사이즈

**skill:** —

**Files:**
- Modify: `src/OverlayNotepad/MainWindow.xaml` (투명 윈도우 속성 + WindowChrome + 드래그 영역 Border 추가)
- Modify: `src/OverlayNotepad/MainWindow.xaml.cs` (드래그 이벤트 핸들러 추가)

**Step 1: MainWindow.xaml 투명 윈도우 설정**

Window 요소에 다음 속성 추가:
- `WindowStyle="None"` — 타이틀바 제거
- `AllowsTransparency="True"` — 투명 허용
- `Background="Transparent"` — 배경 투명
- `MinWidth="200"` `MinHeight="150"` — 최소 크기 (Phase 1 확정 파라미터)
- `Width="400"` `Height="300"` — 기본 크기 (Phase 1 확정 파라미터)

**Step 2: WindowChrome 설정**

Window.WindowChrome 첨부 속성 추가:
- `CaptionHeight="0"` — 시스템 캡션 비활성화 (커스텀 드래그 영역 사용)
- `ResizeBorderThickness="8"` — 리사이즈 테두리 8px (Phase 1 확정 파라미터)
- `CornerRadius="0"` — 둥근 모서리 없음
- `GlassFrameThickness="0"` — Aero Glass 효과 없음

**Step 3: 드래그 영역 구현**

Grid 내 상단 Border 요소 추가:
- `Height="12"` — 드래그 영역 높이 12px (Phase 1 확정 파라미터)
- `VerticalAlignment="Top"`
- `Background="Transparent"`
- `MouseLeftButtonDown` 이벤트 연결 → `DragArea_MouseLeftButtonDown` 핸들러
- XAML Style 트리거: `IsMouseOver=True` 시 `Background` 를 `#0DFFFFFF` (5% 불투명 흰색 하이라이트, Phase 1 확정 파라미터)

**Step 4: 코드비하인드 드래그 핸들러**

`MainWindow.xaml.cs`에 추가:
- `DragArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)` 메서드
- 내부: `this.DragMove()` 호출

**Step 5: 빌드 및 수동 검증**

```bash
"/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "D:/99.실험실/OverlayNotepad/OverlayNotepad.sln" /p:Configuration=Debug /t:Build
```
- 예상: Build succeeded, 0 Errors

수동 검증 항목:
- ⬜ 실행 시 타이틀바 없는 투명 윈도우 표시 (배경이 투명해서 뒤가 보임)
- ⬜ 상단 12px 영역에 마우스 오버 시 미세한 하이라이트 표시
- ⬜ 상단 12px 영역 드래그로 윈도우 이동 가능
- ⬜ 윈도우 가장자리(8px 영역)에서 리사이즈 가능
- ⬜ 최소 크기(200x150) 이하로 줄어들지 않음
- ⬜ Alt+F4로 종료 가능

**Step 6: 커밋**

```
git add src/OverlayNotepad/MainWindow.xaml src/OverlayNotepad/MainWindow.xaml.cs
git commit -m "feat(phase1-sprint1): task2 - 투명 윈도우 + WindowChrome 드래그/리사이즈"
```

**완료 기준:**
- ⬜ 투명 윈도우 표시 (뒤 화면이 보임)
- ⬜ 드래그로 윈도우 이동
- ⬜ 가장자리 리사이즈 동작
- ⬜ 마우스 오버 시 드래그 영역 하이라이트

---

### Task 3: TextBox 배치 + 텍스트 입력 검증

**skill:** —

**Files:**
- Modify: `src/OverlayNotepad/MainWindow.xaml` (TextBox 요소 추가)
- Modify: `src/OverlayNotepad/MainWindow.xaml.cs` (Loaded 이벤트에서 포커스 확인 로직 추가)

**Step 1: TextBox 배치**

MainWindow.xaml의 Grid 내에 TextBox 추가 (드래그 영역 Border 아래):
- `x:Name="MainTextBox"`
- `Margin="0,12,0,0"` — 상단 12px 드래그 영역 아래 배치
- `Background="Transparent"` — 투명 배경
- `BorderThickness="0"` — 테두리 없음
- `AcceptsReturn="True"` — 엔터키로 줄바꿈
- `AcceptsTab="True"` — 탭 입력 허용
- `TextWrapping="Wrap"` — 자동 줄바꿈
- `VerticalScrollBarVisibility="Auto"` — 스크롤바 자동 표시
- `FontFamily="맑은 고딕"` — 기본 폰트 (Phase 1 확정 파라미터)
- `FontSize="14"` — 14pt (Phase 1 확정 파라미터)
- `Foreground="#FFFFFF"` — 흰색 글자 (Phase 1 확정 파라미터)
- `CaretBrush="#FFFFFF"` — 흰색 캐럿

**Step 2: Window에 FocusManager 설정**

Window 요소에 추가:
- `FocusManager.FocusedElement="{Binding ElementName=MainTextBox}"` — 실행 즉시 TextBox에 포커스 (Phase 1 확정 파라미터: TextBox 자동 포커스 필수)

**Step 3: Loaded 이벤트 포커스 보장**

`MainWindow.xaml.cs`에 추가:
- Window `Loaded` 이벤트 핸들러 등록 (XAML에서 `Loaded="Window_Loaded"` 또는 생성자에서 `this.Loaded += ...`)
- 핸들러 내부: `MainTextBox.Focus()` 호출 (FocusManager 실패 시 폴백)

**Step 4: 빌드 및 수동 검증**

```bash
"/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "D:/99.실험실/OverlayNotepad/OverlayNotepad.sln" /p:Configuration=Debug /t:Build
```
- 예상: Build succeeded, 0 Errors

수동 검증 항목 (전체 기능 확인):
- ⬜ 실행 즉시 TextBox에 포커스 (캐럿 깜빡임 확인)
- ⬜ 영문 입력 정상 동작
- ⬜ 한글 입력 정상 동작 (IME 조합 중 글자가 정상 표시)
- ⬜ 엔터키로 줄바꿈 동작
- ⬜ 탭 입력 동작
- ⬜ Ctrl+C / Ctrl+V (복사/붙여넣기) 동작
- ⬜ Ctrl+Z / Ctrl+Y (Undo/Redo) 동작
- ⬜ Ctrl+A (전체 선택) 동작
- ⬜ 텍스트가 많아지면 스크롤바 자동 표시
- ⬜ 투명 배경 위에서 흰색 텍스트가 잘 보임

**Step 5: 커밋**

```
git add src/OverlayNotepad/MainWindow.xaml src/OverlayNotepad/MainWindow.xaml.cs
git commit -m "feat(phase1-sprint1): task3 - TextBox 배치 및 텍스트 입력 기능"
```

**완료 기준:**
- ⬜ 실행 즉시 TextBox 자동 포커스
- ⬜ 한글/영문 입력 정상
- ⬜ 복사/붙여넣기, Undo/Redo 동작
- ⬜ 투명 배경 위 텍스트 가독성 확인

---

## 최종 검증 계획

| 검증 항목 | 명령 / 방법 | 예상 결과 |
|-----------|-------------|-----------|
| MSBuild 빌드 | `MSBuild OverlayNotepad.sln /p:Configuration=Debug` | Build succeeded, 0 Errors |
| 투명 윈도우 | EXE 실행 후 육안 확인 | 타이틀바 없는 투명 윈도우, 뒤 화면이 보임 |
| 드래그 이동 | 상단 12px 영역 드래그 | 윈도우 이동 |
| 리사이즈 | 가장자리 드래그 | 윈도우 크기 변경, 최소 200x150 제한 |
| 자동 포커스 | EXE 실행 직후 | 캐럿 깜빡임, 바로 타이핑 가능 |
| 한글 입력 | 한/영 전환 후 한글 입력 | IME 조합 정상, 한글 표시 |
| 영문 입력 | 영문 타이핑 | 정상 표시 |
| 표준 편집 | Ctrl+C/V/Z/Y/A | 복사/붙여넣기/Undo/Redo/전체선택 동작 |
| 줄바꿈/탭 | Enter, Tab 키 | 줄바꿈, 탭 삽입 |
| 스크롤 | 여러 줄 입력 | 세로 스크롤바 자동 표시 |
| 종료 | Alt+F4 | 프로그램 정상 종료 |
| 드래그 영역 피드백 | 상단 12px에 마우스 오버 | 미세한 하이라이트 표시 |
