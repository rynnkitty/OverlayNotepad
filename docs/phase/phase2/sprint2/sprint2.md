# Sprint 2: 설정 관리 + 자동 저장 + 윈도우 관리 (Phase 2)

**Goal:** 사용자 설정을 JSON 파일로 영속화하고, 텍스트를 자동 저장(2초 디바운스)하며, 윈도우 위치/크기를 저장 및 복원한다.

**Architecture:** SettingsManager(싱글톤)가 `%AppData%/OverlayNotepad/settings.json`을 관리하고, AutoSaveManager(DispatcherTimer)가 텍스트 변경을 감지하여 `memo.txt`에 디바운스 저장한다. 파일 쓰기는 원자적(temp -> rename)으로 수행하며, 설정 손상 시 `.bak` 백업 후 기본값을 복원한다. AppDomain.UnhandledException 핸들러로 비정상 종료 시에도 데이터를 보존한다.

**Tech Stack:** .NET Framework 4.8, WPF, DataContractJsonSerializer (또는 수동 JSON 파싱), DispatcherTimer

**Sprint 기간:** 2026-04-17 ~ (사용자 검토 후 구현)
**이전 스프린트:** Sprint 1 (텍스트 테두리/그림자 효과)
**브랜치명:** `phase2-sprint2`

---

## 제외 범위

- 글꼴/크기/색상 변경 UI -- Phase 3 Sprint 1에서 구현 (설정 모델에 필드만 선언)
- 다크/라이트 테마 전환 UI -- Phase 3 Sprint 2에서 구현 (설정 모델에 필드만 선언)
- 설정 손상 시 사용자 알림 UI -- Phase 3 컨텍스트 메뉴 상태 표시로 해결
- 자동 저장 실패 시 사용자 피드백 -- Phase 3 컨텍스트 메뉴 상태 표시로 해결
- 다중 모니터 DPI 개별 처리 -- PRD에서 명시적으로 제외

## 확정 파라미터 (Phase 2 문서 기준)

| 항목 | 값 | 근거 |
|------|-----|------|
| 설정 파일 경로 | `%AppData%/OverlayNotepad/` | PRD 명시 |
| 설정 파일명 | `settings.json` | PRD 명시 |
| 메모 파일명 | `memo.txt` | Phase 2 확정 (설정과 분리) |
| 손상 설정 백업 | `settings.json.bak` | Phase 2 확정 |
| 자동 저장 디바운스 | 2초 | PRD 명시 |
| 기본 윈도우 크기 | 400x300 | PRD 명시 |
| 최소 윈도우 크기 | 200x150 | Phase 2 확정 |
| 텍스트 인코딩 | UTF-8 (BOM 없음) | Phase 2 확정 |
| 기본 폰트 | 맑은 고딕, 14pt | Phase 1 확정 |
| 기본 글자색 | #FFFFFF | Phase 1 확정 |
| 기본 테마 | dark | PRD 기준 |

## 실행 플랜

Task 간 의존성이 명확하므로 순차 실행이 기본이다.

### Phase 1 (순차 -- 기반 모듈)
| Task | 설명 | 대상 | skill |
|------|------|------|-------|
| Task 1 | AppSettings 모델 + JSON 직렬화 유틸리티 | 데스크톱 | -- |
| Task 2 | SettingsManager (로드/저장/손상 복원) | 데스크톱 | -- |

### Phase 2 (순차 -- SettingsManager 의존)
| Task | 설명 | 대상 | skill |
|------|------|------|-------|
| Task 3 | AutoSaveManager (디바운스 자동 저장) | 데스크톱 | -- |
| Task 4 | 윈도우 위치/크기 저장 및 복원 + 화면 범위 보정 | 데스크톱 | -- |

### Phase 3 (순차 -- 통합)
| Task | 설명 | 대상 | skill |
|------|------|------|-------|
| Task 5 | 비정상 종료 대응 + MainWindow 통합 + 최종 검증 | 데스크톱 | -- |

> **병렬 불가**: 모든 Task가 SettingsManager에 의존하고, MainWindow.xaml.cs를 공유하므로 순차 실행 필수.

---

### Task 1: AppSettings 모델 + JSON 직렬화 유틸리티

**skill:** --

**Files:**
- Create: `src/OverlayNotepad/Models/AppSettings.cs`
- Create: `src/OverlayNotepad/Helpers/JsonHelper.cs`

**Step 1: AppSettings 모델 클래스 생성**

- `src/OverlayNotepad/Models/AppSettings.cs` 생성
- 네임스페이스: `OverlayNotepad.Models`
- 클래스: `AppSettings` (public)
- 중첩 클래스(또는 별도 클래스)로 섹션 분리:
  - `WindowSettings`: Left(double), Top(double), Width(double, 기본값 400), Height(double, 기본값 300)
  - `TransparencySettings`: Opacity(double, 기본값 0.8), Mode(string, 기본값 "background")
  - `TextEffectSettings`: OutlineEnabled(bool, 기본값 true), OutlineThickness(double, 기본값 1.0), OutlineColor(string, 기본값 "#000000"), ShadowEnabled(bool, 기본값 true), ShadowBlur(double, 기본값 5), ShadowOffset(double, 기본값 1.5), ShadowOpacity(double, 기본값 0.8)
  - `FontSettings`: Family(string, 기본값 "맑은 고딕"), Size(double, 기본값 14), Color(string, 기본값 "#FFFFFF")
  - `Topmost`(bool, 기본값 true)
  - `Theme`(string, 기본값 "dark")
- 정적 메서드 `CreateDefault()` -- 모든 필드가 기본값으로 채워진 인스턴스 반환
- Sprint 1의 `TextEffectSettings`가 이미 있으면 `Models/TextEffectSettings.cs`를 `AppSettings.cs`로 통합 (중복 제거)

**Step 2: JsonHelper 유틸리티 생성**

- `src/OverlayNotepad/Helpers/JsonHelper.cs` 생성
- 네임스페이스: `OverlayNotepad.Helpers`
- 클래스: `JsonHelper` (public static)
- 메서드:
  - `string Serialize(AppSettings settings)` -- AppSettings를 JSON 문자열로 직렬화. 들여쓰기 포함 (가독성)
  - `AppSettings Deserialize(string json)` -- JSON 문자열을 AppSettings로 역직렬화. 실패 시 null 반환
- 구현 방식: `DataContractJsonSerializer` 1차 시도. 불편하면 수동 파싱(문자열 조작)으로 전환
  - `System.Runtime.Serialization` 참조 필요 (csproj에 추가)
  - DataContract/DataMember 어트리뷰트 사용
- JSON 출력 형태 (Phase 2 문서 확정):
  ```json
  {
    "window": { "left": 100, "top": 100, "width": 400, "height": 300 },
    "transparency": { "opacity": 0.8, "mode": "background" },
    "textEffect": { "outlineEnabled": true, "outlineThickness": 1.0, "outlineColor": "#000000", "shadowEnabled": true, "shadowBlur": 5, "shadowOffset": 1.5, "shadowOpacity": 0.8 },
    "font": { "family": "맑은 고딕", "size": 14, "color": "#FFFFFF" },
    "topmost": true,
    "theme": "dark"
  }
  ```

**Step 3: csproj 참조 추가**

- `src/OverlayNotepad/OverlayNotepad.csproj` 수정
- `<Reference Include="System.Runtime.Serialization" />` 추가 (DataContractJsonSerializer 사용 시)
- `<Compile Include="Models\AppSettings.cs" />` 추가
- `<Compile Include="Helpers\JsonHelper.cs" />` 추가

**Step 4: 빌드 검증**

```bash
"/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "D:/99.실험실/OverlayNotepad/OverlayNotepad.sln" /p:Configuration=Debug /t:Build
```
- 예상: Build succeeded, 0 Errors

**Step 5: 커밋**

```
git add src/OverlayNotepad/Models/AppSettings.cs src/OverlayNotepad/Helpers/JsonHelper.cs src/OverlayNotepad/OverlayNotepad.csproj
git commit -m "feat(phase2-sprint2): task1 - AppSettings 모델 + JSON 직렬화 유틸리티"
```

**완료 기준:**
- ⬜ AppSettings.CreateDefault()가 모든 기본값을 포함하는 인스턴스 반환
- ⬜ JsonHelper.Serialize/Deserialize 왕복 테스트 성공 (직렬화 -> 역직렬화 -> 값 동일)
- ⬜ MSBuild 빌드 성공

---

### Task 2: SettingsManager (로드/저장/손상 복원)

**skill:** --

**Files:**
- Create: `src/OverlayNotepad/Services/SettingsManager.cs`

**Step 1: SettingsManager 싱글톤 구현**

- `src/OverlayNotepad/Services/SettingsManager.cs` 생성
- 네임스페이스: `OverlayNotepad.Services`
- 클래스: `SettingsManager` (public)
- 싱글톤 패턴: `public static SettingsManager Instance { get; }` (static readonly 초기화 또는 Lazy<T>)
- 프로퍼티:
  - `AppSettings Current { get; private set; }` -- 현재 로드된 설정
  - `string SettingsDirectory` -- `%AppData%/OverlayNotepad/` (Path.Combine(Environment.GetFolderPath(SpecialFolder.ApplicationData), "OverlayNotepad"))
  - `string SettingsFilePath` -- `{SettingsDirectory}/settings.json`
  - `string MemoFilePath` -- `{SettingsDirectory}/memo.txt`
  - `string BackupFilePath` -- `{SettingsDirectory}/settings.json.bak`

**Step 2: Load() 메서드 구현**

- `public void Load()` 메서드
- 로직:
  1. 디렉토리 존재 확인, 없으면 `Directory.CreateDirectory()`
  2. `settings.json` 존재 확인
     - 없으면: `Current = AppSettings.CreateDefault()` -> `Save()` 호출 -> 리턴
  3. `File.ReadAllText(SettingsFilePath, Encoding.UTF8)` 로 읽기
  4. `JsonHelper.Deserialize(json)` 호출
     - 성공: `Current`에 할당
     - 실패(null 반환 또는 예외): 손상 복원 로직 실행
       - 현재 `settings.json`을 `settings.json.bak`으로 복사 (`File.Copy(..., overwrite: true)`)
       - `Current = AppSettings.CreateDefault()` -> `Save()` 호출

**Step 3: Save() 메서드 구현 (원자적 쓰기)**

- `public void Save()` 메서드
- 로직:
  1. 디렉토리 존재 확인, 없으면 생성
  2. `JsonHelper.Serialize(Current)` 로 JSON 문자열 생성
  3. 임시 파일 경로: `{SettingsFilePath}.tmp`
  4. `File.WriteAllText(tempPath, json, Encoding.UTF8)` 로 임시 파일에 쓰기
  5. 기존 `settings.json`이 있으면 `File.Delete(SettingsFilePath)`
  6. `File.Move(tempPath, SettingsFilePath)` 로 원자적 교체
  7. try-catch로 전체 감싸서 쓰기 실패 시 예외를 삼키되 내부 로그 기록 (Debug.WriteLine)

**Step 4: SaveMemo() / LoadMemo() 메서드 구현**

- `public void SaveMemo(string text)` 메서드
  - 원자적 쓰기 패턴 동일 (temp -> rename)
  - 인코딩: UTF-8 (BOM 없음) -- `new UTF8Encoding(false)`
- `public string LoadMemo()` 메서드
  - `memo.txt` 존재 확인 -> 없으면 빈 문자열 반환
  - `File.ReadAllText(MemoFilePath, Encoding.UTF8)` -> 리턴
  - 읽기 실패 시 빈 문자열 반환

**Step 5: csproj에 파일 추가**

- `src/OverlayNotepad/OverlayNotepad.csproj` 수정
- `<Compile Include="Services\SettingsManager.cs" />` 추가

**Step 6: 빌드 검증**

```bash
"/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "D:/99.실험실/OverlayNotepad/OverlayNotepad.sln" /p:Configuration=Debug /t:Build
```
- 예상: Build succeeded, 0 Errors

**Step 7: 커밋**

```
git add src/OverlayNotepad/Services/SettingsManager.cs src/OverlayNotepad/OverlayNotepad.csproj
git commit -m "feat(phase2-sprint2): task2 - SettingsManager 싱글톤 (로드/저장/손상 복원)"
```

**완료 기준:**
- ⬜ Load()로 기본 설정 파일 생성 확인 (`%AppData%/OverlayNotepad/settings.json` 존재)
- ⬜ Save() 후 파일 내용이 유효한 JSON
- ⬜ 손상된 JSON 파일 로드 시 `.bak` 백업 생성 + 기본값 복원
- ⬜ SaveMemo/LoadMemo 왕복 테스트 성공
- ⬜ MSBuild 빌드 성공

---

### Task 3: AutoSaveManager (디바운스 자동 저장)

**skill:** --

**Files:**
- Create: `src/OverlayNotepad/Services/AutoSaveManager.cs`

**Step 1: AutoSaveManager 클래스 구현**

- `src/OverlayNotepad/Services/AutoSaveManager.cs` 생성
- 네임스페이스: `OverlayNotepad.Services`
- 클래스: `AutoSaveManager` (public)
- 필드:
  - `private DispatcherTimer _timer` -- 100ms 간격 체크 타이머
  - `private DateTime _lastChangeTime` -- 마지막 텍스트 변경 시각
  - `private bool _isDirty` -- 변경 여부 플래그
  - `private readonly Action _saveAction` -- 저장 실행 콜백 (외부에서 주입)
  - `private const int DebounceMs = 2000` -- 디바운스 시간 (2초)
  - `private const int TimerIntervalMs = 100` -- 타이머 체크 간격 (100ms)

**Step 2: 생성자 및 시작/중지**

- 생성자: `AutoSaveManager(Action saveAction)`
  - `_saveAction = saveAction`
  - `_timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(TimerIntervalMs) }`
  - `_timer.Tick += Timer_Tick`
- `public void Start()` -- `_timer.Start()`
- `public void Stop()` -- `_timer.Stop()`

**Step 3: 변경 감지 및 디바운스 로직**

- `public void NotifyChanged()` -- 외부에서 TextBox.TextChanged 시 호출
  - `_lastChangeTime = DateTime.Now`
  - `_isDirty = true`
- `private void Timer_Tick(object sender, EventArgs e)` -- 타이머 핸들러
  - `if (_isDirty && (DateTime.Now - _lastChangeTime).TotalMilliseconds >= DebounceMs)`
    - `_saveAction?.Invoke()`
    - `_isDirty = false`

**Step 4: 즉시 저장 메서드**

- `public void SaveNow()` -- 디바운스 무시하고 즉시 저장 (종료 시, 비정상 종료 시 사용)
  - `if (_isDirty)` -> `_saveAction?.Invoke()` -> `_isDirty = false`

**Step 5: csproj에 파일 추가**

- `src/OverlayNotepad/OverlayNotepad.csproj` 수정
- `<Compile Include="Services\AutoSaveManager.cs" />` 추가

**Step 6: 빌드 검증**

```bash
"/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "D:/99.실험실/OverlayNotepad/OverlayNotepad.sln" /p:Configuration=Debug /t:Build
```
- 예상: Build succeeded, 0 Errors

**Step 7: 커밋**

```
git add src/OverlayNotepad/Services/AutoSaveManager.cs src/OverlayNotepad/OverlayNotepad.csproj
git commit -m "feat(phase2-sprint2): task3 - AutoSaveManager 디바운스 자동 저장"
```

**완료 기준:**
- ⬜ NotifyChanged() 호출 후 2초 뒤 saveAction 콜백 실행
- ⬜ 2초 이내 재변경 시 타이머 리셋 (디바운스 동작)
- ⬜ SaveNow()로 즉시 저장
- ⬜ MSBuild 빌드 성공

---

### Task 4: 윈도우 위치/크기 저장 및 복원 + 화면 범위 보정

**skill:** --

**Files:**
- Create: `src/OverlayNotepad/Helpers/ScreenHelper.cs`
- Modify: `src/OverlayNotepad/MainWindow.xaml` (MinWidth/MinHeight 설정)
- Modify: `src/OverlayNotepad/MainWindow.xaml.cs` (위치/크기 저장 및 복원 로직 추가)

**Step 1: ScreenHelper 유틸리티 생성**

- `src/OverlayNotepad/Helpers/ScreenHelper.cs` 생성
- 네임스페이스: `OverlayNotepad.Helpers`
- 클래스: `ScreenHelper` (public static)
- 메서드:
  - `public static bool IsWithinScreenBounds(double left, double top, double width, double height)` 
    - `SystemParameters.VirtualScreenLeft`, `VirtualScreenTop`, `VirtualScreenWidth`, `VirtualScreenHeight` 와 비교
    - 윈도우의 최소 50%가 화면 내에 있으면 true 반환 (완전히 밖이 아니면 허용)
  - `public static (double left, double top) GetDefaultPosition(double width, double height)`
    - 화면 중앙 좌표 계산
    - `left = (SystemParameters.VirtualScreenWidth - width) / 2 + SystemParameters.VirtualScreenLeft`
    - `top = (SystemParameters.VirtualScreenHeight - height) / 2 + SystemParameters.VirtualScreenTop`

**Step 2: MainWindow.xaml MinWidth/MinHeight 설정**

- MainWindow.xaml의 Window 요소에 추가 (아직 없다면):
  - `MinWidth="200"` (Phase 2 확정 파라미터)
  - `MinHeight="150"` (Phase 2 확정 파라미터)

**Step 3: MainWindow.xaml.cs 윈도우 복원 로직**

- `MainWindow.xaml.cs`의 `Window_Loaded` (또는 Loaded 이벤트 핸들러)에 추가:
  1. `SettingsManager.Instance.Load()` 호출 (아직 연동 안 되어 있다면)
  2. `var ws = SettingsManager.Instance.Current.Window` (WindowSettings 가져오기)
  3. `if (ScreenHelper.IsWithinScreenBounds(ws.Left, ws.Top, ws.Width, ws.Height))` -> 저장된 위치/크기 적용
  4. `else` -> `var (left, top) = ScreenHelper.GetDefaultPosition(ws.Width, ws.Height)` -> 기본 위치 적용
  5. `this.Left = ...; this.Top = ...; this.Width = ...; this.Height = ...;`

**Step 4: MainWindow.xaml.cs 윈도우 저장 로직**

- `MainWindow.xaml.cs`에 `Window_Closing` 이벤트 핸들러 추가 (또는 기존에 추가):
  1. `SettingsManager.Instance.Current.Window.Left = this.Left;` (Top, Width, Height 동일)
  2. `SettingsManager.Instance.Save()` 호출

**Step 5: csproj에 파일 추가**

- `src/OverlayNotepad/OverlayNotepad.csproj` 수정
- `<Compile Include="Helpers\ScreenHelper.cs" />` 추가

**Step 6: 빌드 검증**

```bash
"/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "D:/99.실험실/OverlayNotepad/OverlayNotepad.sln" /p:Configuration=Debug /t:Build
```
- 예상: Build succeeded, 0 Errors

수동 검증 항목:
- ⬜ 프로그램 실행 -> 윈도우 이동/크기 변경 -> 종료 -> 재실행 시 이전 위치/크기 복원
- ⬜ settings.json의 window 섹션에 좌표/크기가 저장됨
- ⬜ settings.json에서 window.left를 -9999로 수정 후 실행 -> 화면 중앙에 표시
- ⬜ MinWidth 200, MinHeight 150 이하로 축소 불가

**Step 7: 커밋**

```
git add src/OverlayNotepad/Helpers/ScreenHelper.cs src/OverlayNotepad/MainWindow.xaml src/OverlayNotepad/MainWindow.xaml.cs src/OverlayNotepad/OverlayNotepad.csproj
git commit -m "feat(phase2-sprint2): task4 - 윈도우 위치/크기 저장 및 복원 + 화면 범위 보정"
```

**완료 기준:**
- ⬜ 윈도우 위치/크기가 settings.json에 저장
- ⬜ 재실행 시 이전 위치/크기 복원
- ⬜ 화면 밖 좌표 시 화면 중앙으로 보정
- ⬜ MSBuild 빌드 성공

---

### Task 5: 비정상 종료 대응 + MainWindow 통합 + 최종 검증

**skill:** --

**Files:**
- Modify: `src/OverlayNotepad/App.xaml.cs` (전역 예외 핸들러 등록)
- Modify: `src/OverlayNotepad/MainWindow.xaml.cs` (AutoSaveManager 연동, 설정 로드/저장 통합, 메모 로드/저장)

**Step 1: App.xaml.cs 전역 예외 핸들러**

- `src/OverlayNotepad/App.xaml.cs` 수정
- 생성자 또는 `OnStartup` 오버라이드에서 등록:
  - `AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;`
  - `this.DispatcherUnhandledException += OnDispatcherUnhandledException;`
- `OnUnhandledException` 핸들러:
  - `SettingsManager.Instance.Save()` (설정 긴급 저장)
  - MainWindow의 AutoSaveManager에 접근하여 `SaveNow()` 호출
  - try-catch로 감싸서 저장 실패해도 추가 예외 발생 방지
- `OnDispatcherUnhandledException` 핸들러:
  - 동일한 긴급 저장 로직
  - `e.Handled = true` (가능하면 프로그램 종료 방지, 심각한 오류는 종료 허용)

**Step 2: MainWindow.xaml.cs AutoSaveManager 연동**

- MainWindow에 AutoSaveManager 필드 추가:
  - `private AutoSaveManager _autoSaveManager;`
- Window_Loaded 핸들러에 추가:
  1. `SettingsManager.Instance.Load()` (이미 Task 4에서 추가)
  2. 메모 로드: `MainTextBox.Text = SettingsManager.Instance.LoadMemo();`
  3. AutoSaveManager 초기화:
     ```
     _autoSaveManager = new AutoSaveManager(() => {
         SettingsManager.Instance.SaveMemo(MainTextBox.Text);
     });
     _autoSaveManager.Start();
     ```
  4. TextBox.TextChanged 이벤트 연결:
     - `MainTextBox.TextChanged += (s, e) => _autoSaveManager.NotifyChanged();`

**Step 3: MainWindow.xaml.cs 종료 시 저장**

- Window_Closing 핸들러에 추가 (Task 4에서 윈도우 위치 저장 이후):
  1. `_autoSaveManager?.SaveNow()` -- 디바운스 무시하고 즉시 메모 저장
  2. `_autoSaveManager?.Stop()` -- 타이머 정지
  3. `SettingsManager.Instance.SaveMemo(MainTextBox.Text)` -- 최종 메모 저장
  4. `SettingsManager.Instance.Save()` -- 최종 설정 저장 (윈도우 위치 포함)

**Step 4: Sprint 1 설정값 연동 (테두리/그림자/투명도/Topmost)**

- Window_Loaded에서 설정값 적용:
  - 투명도 모드/값: `SettingsManager.Instance.Current.Transparency` -> 기존 투명도 로직에 적용
  - Topmost: `this.Topmost = SettingsManager.Instance.Current.Topmost`
  - 테두리/그림자: `SettingsManager.Instance.Current.TextEffect` -> OutlinedTextControl/DropShadowEffect에 적용
- 설정 변경 시 (컨텍스트 메뉴 등에서 투명도/Topmost/테두리 변경 시):
  - `SettingsManager.Instance.Current.{해당 필드}` 업데이트
  - (자동 저장이 settings.json을 주기적으로 저장하지는 않으므로, 설정 변경 시 즉시 `SettingsManager.Instance.Save()` 호출)

**Step 5: 빌드 및 수동 검증**

```bash
"/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "D:/99.실험실/OverlayNotepad/OverlayNotepad.sln" /p:Configuration=Debug /t:Build
```
- 예상: Build succeeded, 0 Errors

수동 검증 항목 (전체 기능 확인):
- ⬜ 프로그램 실행 -> 텍스트 입력 -> 2초 대기 -> `%AppData%/OverlayNotepad/memo.txt` 내용 확인
- ⬜ 프로그램 종료 -> 재실행 -> 이전 텍스트 복원 확인
- ⬜ 윈도우 이동 -> 종료 -> 재실행 -> 위치 복원 확인
- ⬜ settings.json 삭제 -> 프로그램 실행 -> 기본값으로 정상 시작 (400x300, 화면 중앙)
- ⬜ settings.json 내용을 `{invalid json` 으로 수정 -> 실행 -> settings.json.bak 생성 + 기본값 복원
- ⬜ 투명도/Topmost 설정이 재실행 시 유지
- ⬜ 테두리/그림자 설정이 재실행 시 유지

**Step 6: 커밋**

```
git add src/OverlayNotepad/App.xaml.cs src/OverlayNotepad/MainWindow.xaml.cs
git commit -m "feat(phase2-sprint2): task5 - 비정상 종료 대응 + MainWindow 통합"
```

**완료 기준:**
- ⬜ 자동 저장 동작 (텍스트 변경 후 2초 뒤 memo.txt 갱신)
- ⬜ 정상 종료 시 설정 + 메모 저장
- ⬜ 비정상 종료 핸들러 등록 확인
- ⬜ Sprint 1 설정값 (투명도, Topmost, 테두리/그림자) 영속화
- ⬜ MSBuild 빌드 성공

---

## 최종 검증 계획

| 검증 항목 | 명령 / 방법 | 예상 결과 |
|-----------|-------------|-----------|
| MSBuild 빌드 | `MSBuild OverlayNotepad.sln /p:Configuration=Debug` | Build succeeded, 0 Errors |
| 설정 파일 생성 | 최초 실행 후 `%AppData%/OverlayNotepad/settings.json` 확인 | 유효한 JSON, 기본값 포함 |
| 자동 저장 | 텍스트 입력 -> 2초 대기 -> `memo.txt` 확인 | 입력한 텍스트와 동일 |
| 디바운스 | 텍스트 연속 입력(2초 이내 반복) -> memo.txt 갱신 시점 확인 | 마지막 입력 후 2초 뒤 1회만 저장 |
| 메모 복원 | 텍스트 입력 -> 종료 -> 재실행 | 이전 텍스트 복원 |
| 윈도우 위치 복원 | 윈도우 이동 -> 종료 -> 재실행 | 이전 위치/크기 유지 |
| 윈도우 크기 복원 | 윈도우 리사이즈 -> 종료 -> 재실행 | 이전 크기 유지 |
| 화면 범위 보정 | settings.json window.left=-9999 -> 실행 | 화면 중앙에 표시 |
| 설정 손상 복원 | settings.json을 `{broken` 으로 수정 -> 실행 | .bak 백업 생성, 기본값으로 시작 |
| 설정 삭제 복원 | settings.json 삭제 -> 실행 | 기본값으로 새 파일 생성 |
| 최소 크기 제한 | 윈도우를 200x150 이하로 축소 시도 | 축소 불가 |
| 투명도 영속화 | 투명도 변경 -> 종료 -> 재실행 | 이전 투명도 유지 |
| Topmost 영속화 | Topmost 토글 -> 종료 -> 재실행 | 이전 Topmost 상태 유지 |
| 테두리/그림자 영속화 | 테두리 토글 -> 종료 -> 재실행 | 이전 테두리 상태 유지 |
| 종료 시 저장 | 텍스트 입력 직후(2초 미만) Alt+F4 -> 재실행 | 텍스트 복원 (즉시 저장) |
