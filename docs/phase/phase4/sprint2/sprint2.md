# Sprint 2: 최종 마무리 + 배포 준비 (Phase 4)

**Goal:** 전체 기능 통합 테스트, 성능 최적화(메모리 80MB 이하, 유휴 CPU 0%, 시작 2초 이내), DPI 인식, 앱 아이콘, 단일 EXE 배포 확인, 엣지 케이스 수정을 완료하여 배포 가능한 최종 빌드를 만든다.

**Architecture:** Sprint 1에서 구현된 글로벌 핫키 + Click-Through를 포함한 Phase 1~4의 전체 기능을 통합 검증하고, app.manifest에 DPI 인식을 선언하며, 앱 아이콘을 설정하고, Release 빌드로 단일 EXE 배포가 가능한지 최종 확인한다. 성능 병목이 발견되면 불필요한 타이머/폴링 제거, 렌더링 최적화 등을 적용한다.

**Tech Stack:** .NET Framework 4.8, WPF, Win32 API 인터롭, MSBuild

**Sprint 기간:** 2026-04-17 ~ 2026-04-17 (완료)
**상태:** ✅ 완료
**이전 스프린트:** Sprint 1 (글로벌 핫키 + Click-Through)
**브랜치명:** `phase4-sprint2`

---

## 제외 범위

- 핫키 사용자 변경 기능 -- 확정 파라미터에서 MVP 제외 (백로그)
- 멀티 모니터 Per-Monitor DPI -- PRD 범위 밖, System DPI Aware로 충분
- 투명도 슬라이더 UI 개선 -- 백로그
- 테두리 두께/그림자 파라미터 변경 UI -- 백로그
- 새 기능 추가 -- 이 Sprint은 기존 기능의 품질 마무리에만 집중

## 확정 파라미터 (Phase 4 문서 기준)

| 항목 | 확정값 | 근거 |
|------|--------|------|
| 메모리 사용량 | 80MB 이하 (유휴 상태) | PRD 비기능 요구사항 |
| 시작 시간 | 2초 이내 | PRD 비기능 요구사항 |
| 유휴 CPU | 0% | PRD 비기능 요구사항 |
| DPI 인식 | System DPI Aware (app.manifest 선언) | Phase 4 확정 |
| Click-Through 재시작 기본값 | OFF | Phase 4 확정 (안전 설계) |
| 핫키 실패 시 Click-Through | 기능 비활성화 강제 | Phase 4 확정 |

## 실행 플랜

### Phase 1 (순차 -- 인프라)
| Task | 설명 | 대상 | skill |
|------|------|------|-------|
| Task 1 | DPI 인식 + 앱 아이콘 설정 | 데스크톱 | -- |

### Phase 2 (순차 -- 성능)
| Task | 설명 | 대상 | skill |
|------|------|------|-------|
| Task 2 | 성능 최적화 + 불필요 자원 정리 | 데스크톱 | -- |

### Phase 3 (순차 -- 배포 검증)
| Task | 설명 | 대상 | skill |
|------|------|------|-------|
| Task 3 | 단일 EXE 빌드 + Release 설정 확인 | 데스크톱 | -- |

### Phase 4 (순차 -- 통합 검증)
| Task | 설명 | 대상 | skill |
|------|------|------|-------|
| Task 4 | 전체 기능 통합 테스트 + 엣지 케이스 수정 | 데스크톱 | -- |

### Phase 5 (순차 -- 최종 DoD)
| Task | 설명 | 대상 | skill |
|------|------|------|-------|
| Task 5 | Phase 4 DoD 확인 + 프로젝트 전체 DoD 확인 | 데스크톱 | -- |

> **병렬 불가**: 모든 Task가 프로젝트 파일, MainWindow, 빌드 설정을 공유하므로 순차 실행 필수.

---

### Task 1: DPI 인식 + 앱 아이콘 설정

**skill:** --

**Files:**
- Modify: `src/OverlayNotepad/app.manifest` (dpiAware 선언 추가)
- Create: `src/OverlayNotepad/Resources/app.ico` (앱 아이콘 파일)
- Modify: `src/OverlayNotepad/OverlayNotepad.csproj` (ApplicationIcon 설정, app.manifest 참조 확인)
- Modify: `src/OverlayNotepad/App.xaml` (아이콘이 앱 리소스로 필요한 경우)

**Step 1: app.manifest에 DPI 인식 선언 추가**

- `src/OverlayNotepad/app.manifest` 파일에 아래 XML 요소 추가 (이미 존재하는 `<application>` 요소에 합침):
  ```xml
  <application xmlns="urn:schemas-microsoft-com:asm.v3">
    <windowsSettings>
      <dpiAware xmlns="http://schemas.microsoft.com/SMI/2005/WindowsSettings">true</dpiAware>
    </windowsSettings>
  </application>
  ```
- app.manifest가 아직 없으면 신규 생성:
  - Visual Studio 기본 app.manifest 템플릿에 위 `<application>` 블록 추가
  - `requestedExecutionLevel level="asInvoker"` 유지
- 이미 Phase 1에서 app.manifest를 만들었다면 `<application>` 블록만 추가

**Step 2: 앱 아이콘 생성**

- `src/OverlayNotepad/Resources/app.ico` 파일 생성
- 투명 메모장 컨셉의 간단한 아이콘 (메모지 + 투명 오버레이 이미지)
- 크기: 16x16, 32x32, 48x48, 256x256 복합 ICO 파일
- 아이콘 생성 방법:
  - WPF 프로젝트에 사용할 .ico 파일을 리소스로 추가
  - 간단한 아이콘이면 기본 메모장 아이콘 형태로 생성 (디자인 정교함은 MVP 이후 개선)

**Step 3: 프로젝트 파일에 아이콘 설정**

- `src/OverlayNotepad/OverlayNotepad.csproj` 수정:
  - `<ApplicationIcon>Resources\app.ico</ApplicationIcon>` 추가 (PropertyGroup 내)
  - `<Resource Include="Resources\app.ico" />` 추가 (ItemGroup 내, 빌드 액션=Resource)
  - app.manifest가 이미 참조되어 있는지 확인: `<ApplicationManifest>app.manifest</ApplicationManifest>`

**Step 4: MainWindow에 아이콘 적용**

- `src/OverlayNotepad/MainWindow.xaml` (또는 MainWindow.xaml.cs)에서:
  - `Icon="pack://application:,,,/Resources/app.ico"` 속성 추가 (XAML)
  - 또는 코드비하인드: `this.Icon = new BitmapImage(new Uri("pack://application:,,,/Resources/app.ico"))`
- 시스템 트레이 아이콘도 동일 아이콘 사용 확인 (Phase 3 Sprint 1의 TrayIconManager에서 이미 설정했을 수 있음)

**Step 5: 빌드 검증**

```bash
"/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "D:/99.실험실/OverlayNotepad/OverlayNotepad.sln" /p:Configuration=Debug /t:Build
```
- 예상: Build succeeded, 0 Errors

수동 검증:
- ✅ 앱 실행 시 작업 표시줄에 아이콘 표시
- ✅ EXE 파일 탐색기에서 아이콘 표시
- ✅ DPI 150% 환경에서 앱 실행 시 UI 요소가 정상 스케일링

**Step 6: 커밋**

```
git add src/OverlayNotepad/app.manifest src/OverlayNotepad/Resources/app.ico src/OverlayNotepad/OverlayNotepad.csproj src/OverlayNotepad/MainWindow.xaml
git commit -m "feat(phase4-sprint2): task1 - DPI 인식 선언 + 앱 아이콘 설정"
```

**완료 기준:**
- ✅ app.manifest에 dpiAware=true 선언 존재
- ✅ EXE 파일 및 작업 표시줄에 앱 아이콘 표시 (16/32/48/256px 복합 ICO)
- ✅ DPI 스케일링 환경에서 UI 깨짐 없음
- ✅ MSBuild 빌드 성공

---

### Task 2: 성능 최적화 + 불필요 자원 정리

**skill:** --

**Files:**
- Modify: `src/OverlayNotepad/MainWindow.xaml.cs` (성능 병목 수정)
- Modify: `src/OverlayNotepad/Services/AutoSaveManager.cs` (타이머 최적화)
- Modify: `src/OverlayNotepad/Services/HotkeyService.cs` (리소스 정리 확인)
- Modify: `src/OverlayNotepad/Services/ClickThroughService.cs` (리소스 정리 확인)
- Modify: `src/OverlayNotepad/App.xaml.cs` (비정상 종료 핸들러 리소스 정리 확인)

**Step 1: 메모리 사용량 측정 (기준선)**

- Debug 빌드로 앱 실행
- 작업 관리자에서 "워킹 셋(메모리)" 확인
  - 유휴 상태: 텍스트 입력 없이 30초 대기 후 측정
  - 텍스트 입력 후: 1000자 정도 입력 후 측정
- 기준: 80MB 이하
- 80MB 초과 시 대응:
  1. GC.Collect() 후 재측정 (관리 메모리 vs 비관리 메모리 구분)
  2. Visual Studio Diagnostic Tools 또는 dotMemory로 메모리 프로파일링
  3. 큰 객체: InstalledFontCollection 캐싱, 불필요한 Brush/Effect 객체 공유

**Step 2: CPU 사용량 측정 (기준선)**

- Debug 빌드로 앱 실행
- 작업 관리자에서 유휴 시 CPU 사용률 확인 (30초 이상 관찰)
- 기준: 0%에 근접 (0.0~0.5% 허용)
- 0% 초과 시 대응:
  1. DispatcherTimer 점검: AutoSaveManager의 디바운스 타이머가 유휴 시 중지되는지 확인
  2. 렌더링 루프 점검: InvalidateVisual()이 불필요하게 반복 호출되지 않는지 확인
  3. WndProc 훅 점검: HotkeyService의 WndProc 훅이 불필요한 메시지를 처리하지 않는지 확인
  4. 정적 프로퍼티 변경 알림이 무한 루프를 유발하지 않는지 확인

**Step 3: 시작 시간 측정**

- MainWindow.xaml.cs의 생성자 또는 Window_Loaded에서 시작 시간 측정 코드 추가:
  ```
  App.xaml.cs의 OnStartup() 시작 시점에서 Stopwatch.StartNew()
  MainWindow.Loaded 이벤트 핸들러에서 Stopwatch.Stop() + Debug.WriteLine("시작 시간: {elapsed.TotalMilliseconds}ms")
  ```
- 기준: 2000ms 이내
- 2초 초과 시 대응:
  1. 설정 파일 로드 최적화 (비동기 로드 검토)
  2. InstalledFontCollection 초기화 지연 로딩 (컨텍스트 메뉴 열 때 로드)
  3. ThemeManager 초기화 경량화
  4. HotkeyService.RegisterHotKey 호출 시점 최적화 (Loaded 이후 비동기)

**Step 4: 리소스 정리 확인**

- 앱 종료 시 리소스 해제가 빠짐없이 수행되는지 확인:
  - `HotkeyService.Dispose()`: UnregisterHotKey 호출, HwndSource.RemoveHook
  - `ClickThroughService`: WS_EX_TRANSPARENT 해제 (필요 시)
  - `TrayIconManager`: NotifyIcon.Dispose() (트레이 아이콘 잔존 방지)
  - `AutoSaveManager`: 타이머 Stop + 마지막 저장 플러시
  - `SettingsManager`: 최종 설정 저장
- `App.xaml.cs`의 OnExit 또는 MainWindow.Closing에서 위 Dispose가 호출되는지 확인
- 비정상 종료(AppDomain.UnhandledException)에서도 최소한의 저장 수행 확인

**Step 5: 성능 최적화 적용 (필요 시)**

- 측정 결과에 따라 아래 최적화 항목 적용:
  - 불필요한 DispatcherTimer 중지/해제
  - Brush/Effect 객체 Freeze() 적용 (변경 불필요한 Brush는 Freeze하여 스레드 간 공유 및 성능 개선)
  - SolidColorBrush 재사용 (매번 new 대신 캐시)
  - InstalledFontCollection을 한 번만 로드 후 캐시
  - Debug.WriteLine 등 디버그 출력 정리 (Release 빌드에서 자동 제거되지만 확인)

**Step 6: 빌드 및 재측정**

```bash
"/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "D:/99.실험실/OverlayNotepad/OverlayNotepad.sln" /p:Configuration=Debug /t:Build
```
- 예상: Build succeeded, 0 Errors
- 최적화 적용 후 메모리/CPU/시작시간 재측정하여 기준 충족 확인

**Step 7: 커밋**

```
git add src/OverlayNotepad/MainWindow.xaml.cs src/OverlayNotepad/Services/AutoSaveManager.cs src/OverlayNotepad/Services/HotkeyService.cs src/OverlayNotepad/Services/ClickThroughService.cs src/OverlayNotepad/App.xaml.cs
git commit -m "perf(phase4-sprint2): task2 - 성능 최적화 + 리소스 정리"
```

**완료 기준:**
- ✅ 유휴 메모리 80MB 이하
- ✅ 유휴 CPU 0%에 근접 (0.5% 이하)
- ✅ 시작 시간 2초 이내 (Stopwatch 측정 추가)
- ✅ 앱 종료 시 모든 리소스 정상 해제 (SolidColorBrush.Freeze(), 스트림 using 블록)
- ✅ 트레이 아이콘 잔존 없음
- ✅ MSBuild 빌드 성공

---

### Task 3: 단일 EXE 빌드 + Release 설정 확인

**skill:** --

**Files:**
- Modify: `src/OverlayNotepad/OverlayNotepad.csproj` (Release 빌드 설정 확인/조정)
- Modify: `OverlayNotepad.sln` (필요 시)

**Step 1: Release 빌드 수행**

```bash
"/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "D:/99.실험실/OverlayNotepad/OverlayNotepad.sln" /p:Configuration=Release /t:Rebuild
```
- 예상: Build succeeded, 0 Errors, 0 Warnings (또는 무해한 경고만)

**Step 2: 출력 파일 확인**

```bash
ls -la "D:/99.실험실/OverlayNotepad/src/OverlayNotepad/bin/Release/"
```
- 예상 출력 파일:
  - `OverlayNotepad.exe` -- 메인 실행 파일 (필수)
  - `OverlayNotepad.exe.config` -- 앱 설정 파일 (선택, 없어도 실행 가능)
  - `OverlayNotepad.pdb` -- 디버그 심볼 (선택, 배포 불필요)
- System.Windows.Forms.dll 등 별도 DLL이 출력되지 않는지 확인
  - .NET Framework 4.8 GAC에 포함되므로 별도 배포 불필요
  - 만약 NuGet 패키지 DLL이 포함된 경우, ILMerge 또는 Costura.Fody 검토 (단, MVP에서는 외부 NuGet 최소화 원칙)

**Step 3: 독립 실행 테스트**

- Release 빌드의 `OverlayNotepad.exe`만 별도 폴더에 복사
- 해당 폴더에서 EXE 단독 실행 테스트:
  ```bash
  mkdir -p /tmp/deploy-test
  cp "D:/99.실험실/OverlayNotepad/src/OverlayNotepad/bin/Release/OverlayNotepad.exe" /tmp/deploy-test/
  "/tmp/deploy-test/OverlayNotepad.exe"
  ```
- 확인 사항:
  - ✅ EXE 단독으로 정상 실행
  - ✅ DLL 누락 오류 없음
  - ✅ 첫 실행 시 settings.json이 %AppData%/OverlayNotepad/에 자동 생성
  - ✅ 모든 기능 정상 동작

**Step 4: .exe.config 불필요 확인**

- .exe.config 파일 없이 EXE만으로 실행 가능한지 확인
- supportedRuntime 등 필수 설정이 있으면 .exe.config도 함께 배포 필요 결정
- .NET Framework 4.8 대상이므로 일반적으로 .exe.config 없이 동작

**Step 5: 빌드 설정 최적화**

- Release 빌드 설정 확인:
  - `<Optimize>true</Optimize>` -- 코드 최적화 활성화
  - `<DebugType>pdbonly</DebugType>` 또는 `<DebugType>none</DebugType>`
  - `<DefineConstants>TRACE</DefineConstants>` -- DEBUG 제거
- EXE 파일 크기 확인 (일반적으로 수백 KB 수준)
- 불필요한 어셈블리 참조 제거 (사용하지 않는 참조가 있으면 정리)

**Step 6: 커밋**

```
git add src/OverlayNotepad/OverlayNotepad.csproj
git commit -m "chore(phase4-sprint2): task3 - Release 빌드 설정 확인 + 단일 EXE 배포 검증"
```

**완료 기준:**
- ✅ Release 빌드 성공 (0 Errors)
- ✅ 단일 EXE 파일로 독립 실행 가능 (56KB)
- ✅ 별도 DLL 배포 불필요
- ✅ EXE 파일에 앱 아이콘 표시
- ✅ 첫 실행 시 설정 디렉토리 자동 생성

---

### Task 4: 전체 기능 통합 테스트 + 엣지 케이스 수정

**skill:** --

**Files:**
- Modify: `src/OverlayNotepad/MainWindow.xaml.cs` (엣지 케이스 수정)
- Modify: `src/OverlayNotepad/Services/HotkeyService.cs` (엣지 케이스 수정)
- Modify: `src/OverlayNotepad/Services/ClickThroughService.cs` (엣지 케이스 수정)
- Modify: `src/OverlayNotepad/Services/SettingsManager.cs` (엣지 케이스 수정)
- (위 파일 중 수정이 실제로 필요한 파일만 변경)

**Step 1: 글로벌 핫키 통합 테스트**

```
시나리오:
  1. 다른 앱(메모장, 브라우저 등) 포커스 상태에서 Ctrl+Shift+N
     - 예상: OverlayNotepad 표시/숨김 토글
  2. 다른 앱 포커스 상태에서 Ctrl+Shift+T
     - 예상: Click-Through 토글 (시각 피드백 확인)
  3. 핫키 충돌 테스트: 다른 앱에서 Ctrl+Shift+N 선점 후 OverlayNotepad 시작
     - 예상: 트레이 벌룬으로 "핫키 등록 실패" 알림
  4. Click-Through 핫키 등록 실패 시
     - 예상: Click-Through 메뉴 항목 비활성화(회색 처리)
  5. 핫키로 숨김 -> 핫키로 표시 -> 포커스가 OverlayNotepad로 이동하는지 확인
```

**Step 2: Click-Through 통합 테스트**

```
시나리오:
  1. Click-Through 활성화 -> 텍스트 영역 클릭
     - 예상: 클릭이 뒤의 앱에 전달
  2. Click-Through 시각 피드백
     - 예상: 빨간 점선 테두리 + "CLICK-THROUGH" 텍스트 + 트레이 아이콘 변경
  3. Click-Through + 전체 투명도 10% 설정
     - 예상: 투명도가 20%로 강제 (하한)
  4. Click-Through 활성 상태에서 우클릭
     - 예상: 컨텍스트 메뉴가 열리지 않음 (클릭이 통과)
  5. Click-Through + 핫키로 해제 -> 정상 클릭 가능 확인
  6. Click-Through 최초 활성화 시
     - 예상: 트레이 벌룬으로 해제 방법 안내 (1회만)
  7. Click-Through 상태에서 트레이 메뉴로 해제
     - 예상: 정상 해제 + 시각 피드백 복원
```

**Step 3: 전체 기능 연동 테스트**

```
시나리오:
  1. 투명도 조절 + 테마 전환 + Click-Through 조합
     - 다크 테마 -> 투명도 40% -> Click-Through ON -> 투명도 20% 강제 확인
     - 라이트 테마로 전환 -> Click-Through 시각 피드백 유지
  2. 서식 변경 + Click-Through
     - 글꼴 변경 -> Click-Through ON -> 핫키로 OFF -> 글꼴 유지 확인
  3. 자동 저장 + Click-Through
     - 텍스트 입력 -> Click-Through ON -> 2초 후 자동 저장 실행 확인
  4. 시작 -> 종료 -> 재시작 전체 사이클
     - 모든 설정 변경 -> 정상 종료 -> 재시작 -> 모든 설정 복원 확인
     - Click-Through는 재시작 시 OFF (안전 기본값)
  5. 비정상 종료 복구
     - 텍스트 입력 -> 작업 관리자에서 프로세스 강제 종료
     - 재시작 -> 마지막 자동 저장된 텍스트 복원 확인
  6. 텍스트 테두리 ON + 테마 전환 + 서식 변경
     - 테두리 ON -> 라이트 테마 -> 글꼴 변경 -> 다크 테마 -> 테두리/글꼴/텍스트 정상 확인
  7. 윈도우 관리
     - 윈도우 이동 + 리사이즈 -> 종료 -> 재시작 -> 위치/크기 복원
     - 화면 밖 위치 저장 후 재실행 -> 화면 내 복원
```

**Step 4: 컨텍스트 메뉴 + 트레이 메뉴 통합 테스트**

```
시나리오:
  1. 컨텍스트 메뉴에서 Click-Through 토글 항목 동작 확인
  2. 트레이 메뉴에서 Click-Through 토글 항목 동작 + 상태 체크마크
  3. 트레이 메뉴에서 표시/숨김 토글
  4. 최소화 -> 트레이로 숨김 -> 더블클릭 복원
  5. 모든 컨텍스트 메뉴 항목 한 번씩 클릭 테스트
  6. 모든 트레이 메뉴 항목 한 번씩 클릭 테스트
```

**Step 5: 알려진 엣지 케이스 확인 + 수정**

```
엣지 케이스:
  1. 핫키 등록 실패 + Click-Through 메뉴
     - Click-Through 핫키 등록 실패 시 컨텍스트/트레이 메뉴의 Click-Through 항목이 회색 처리
  2. Click-Through 상태에서 앱 종료 -> 재시작 시 Click-Through OFF
     - settings.json의 IsClickThrough를 true로 수동 편집 -> 재시작 시 OFF 강제
  3. 다중 모니터 -> 단일 모니터 전환
     - Phase 2의 화면 범위 보정 로직 동작 확인
  4. DPI 변경 중 앱 실행
     - WPF 기본 DPI 스케일링으로 처리, UI 깨짐 없음 확인
  5. settings.json 완전 삭제 후 실행
     - 기본값으로 정상 시작, Click-Through OFF, 다크 테마
  6. settings.json 손상 (잘못된 JSON) 후 실행
     - 기본값 복원 후 정상 시작
  7. 매우 긴 텍스트 입력 (10,000자 이상)
     - 메모리 사용량 급증하지 않는지 확인
     - 자동 저장 정상 동작 확인
  8. 빈 텍스트 상태에서 모든 기능 동작
     - 테두리/그림자 효과가 오류 없이 동작 (빈 문자열 처리)
  9. BackgroundTransparentMenuItem_Click null 체크 누락 (Phase 4 문서 미해결 사항)
     - `sender as MenuItem` 패턴에서 null 가드 추가 필요
     - MainWindow.xaml.cs 323행: `MenuItem menuItem = sender as MenuItem;` 뒤에 null 체크 없음
     - 수정: `if (menuItem == null) return;` 추가
```

**Step 6: 발견된 엣지 케이스 수정**

- 검증 중 발견된 문제를 수정
- 각 수정 사항에 대해 수정 전/후 동작을 확인

**Step 7: 커밋**

```
git add -A src/OverlayNotepad/
git commit -m "fix(phase4-sprint2): task4 - 전체 기능 통합 테스트 + 엣지 케이스 수정"
```

**완료 기준:**
- ✅ 글로벌 핫키 5가지 시나리오 통과
- ✅ Click-Through 7가지 시나리오 통과
- ✅ 전체 기능 연동 7가지 시나리오 통과
- ✅ 컨텍스트/트레이 메뉴 6가지 시나리오 통과
- ✅ 알려진 엣지 케이스 9가지 확인 + 수정 (null 체크 4개 핸들러: BackgroundTransparentMenuItem_Click, OutlineMenuItem_Click, ShadowMenuItem_Click, OpacityMenuItem_Click)
- ✅ MSBuild 빌드 성공

---

### Task 5: Phase 4 DoD 확인 + 프로젝트 전체 DoD 확인

**skill:** --

**Files:**
- 수정 파일 없음 (검증만 수행, 문제 발견 시 해당 파일 수정)

**Step 1: Phase 4 완료 기준 확인**

Phase 4 전체 DoD (ROADMAP.md 기준):
- ✅ 다른 앱 포커스 상태에서 글로벌 핫키로 메모장 표시/숨김 동작
- ✅ Click-Through 모드에서 마우스 클릭이 뒤의 앱에 전달
- ✅ Click-Through 모드에서 핫키/트레이로만 해제 가능
- ✅ Click-Through 상태가 시각적으로 구분 가능 (빨간 점선 + 텍스트 + 트레이 아이콘)
- ✅ 메모리 80MB 이하, 유휴 CPU 0%, 시작 2초 이내
- ✅ 단일 EXE 파일로 정상 실행 (설치 불필요, 56KB)
- ✅ Windows 10/11에서 정상 동작
- ✅ DPI 인식 (app.manifest dpiAware 선언)
- ✅ 앱 아이콘 표시 (EXE + 작업 표시줄 + 트레이)

**Step 2: 프로젝트 전체 기능 체크리스트 (PRD 기준)**

- ✅ F1: 투명 배경 (배경 투명 모드 + 전체 투명 모드 + 투명도 조절)
- ✅ F2: Always on Top 토글
- ✅ F3: 텍스트 테두리 + 그림자 (토글, 색상 설정)
- ✅ F4: 서식 지원 (글꼴, 크기, 색상 -- 전체 적용)
- ✅ F5: 다크/라이트 테마 전환
- ✅ F6: Click-Through 토글 (핫키/트레이, 시각 피드백, 안전장치)
- ✅ F7: 자동 저장 (2초 디바운스, 비정상 종료 보존)
- ✅ F8: 윈도우 관리 (타이틀바 없음, 드래그 이동, 리사이즈, 위치/크기 기억)
- ✅ F9: 시스템 트레이 (숨김/복원, 트레이 메뉴)
- ✅ F10: 글로벌 핫키 (표시/숨김, Click-Through 토글)
- ✅ F11: 컨텍스트 메뉴 (전체 항목 배치)

**Step 3: 비기능 요구사항 최종 확인**

- ✅ 메모리: 80MB 이하 (유휴 상태)
- ✅ CPU: 유휴 시 0%에 근접
- ✅ 시작 시간: 2초 이내
- ✅ 단일 EXE 배포: 설치 불필요 (56KB)
- ✅ 오프라인 전용: 네트워크 접근 없음
- ✅ Windows 10 (1903+) / Windows 11 호환
- ✅ .NET Framework 4.8 기본 포함

**Step 4: Release 빌드 최종 확인**

```bash
"/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "D:/99.실험실/OverlayNotepad/OverlayNotepad.sln" /p:Configuration=Release /t:Rebuild
```
- 예상: Build succeeded, 0 Errors

**Step 5: 최종 성능 측정 (Release 빌드)**

- Release 빌드의 EXE로 최종 성능 측정:
  - 메모리: 작업 관리자에서 유휴 상태 확인
  - CPU: 30초 관찰
  - 시작 시간: EXE 실행 후 윈도우 표시까지

**Step 6: 문제 발견 시 수정 + 커밋**

```
git add -A src/OverlayNotepad/
git commit -m "fix(phase4-sprint2): task5 - Phase 4 DoD + 프로젝트 전체 DoD 최종 수정"
```
(수정 사항이 없으면 이 커밋은 생략)

**완료 기준:**
- ✅ Phase 4 DoD 전체 항목 충족
- ✅ PRD F1~F11 전체 기능 동작 확인
- ✅ 비기능 요구사항 전체 충족
- ✅ Release 빌드 성공 + 단일 EXE 실행 확인 (56KB)

---

## 최종 검증 계획

| 검증 항목 | 명령/방법 | 예상 결과 |
|-----------|----------|-----------|
| MSBuild Debug 빌드 | `MSBuild OverlayNotepad.sln /p:Configuration=Debug` | Build succeeded, 0 Errors |
| MSBuild Release 빌드 | `MSBuild OverlayNotepad.sln /p:Configuration=Release /t:Rebuild` | Build succeeded, 0 Errors |
| DPI 인식 | app.manifest 내 dpiAware=true 확인 | 선언 존재 |
| 앱 아이콘 | 탐색기에서 EXE 파일 확인 | 아이콘 표시 |
| 메모리 (유휴) | 작업 관리자에서 유휴 30초 후 측정 | 80MB 이하 |
| CPU (유휴) | 작업 관리자에서 30초 관찰 | 0% 근접 |
| 시작 시간 | Stopwatch 또는 체감 측정 | 2초 이내 |
| 단일 EXE 실행 | 별도 폴더에 EXE만 복사 후 실행 | DLL 누락 없이 정상 실행 |
| 글로벌 핫키 | 다른 앱 포커스에서 Ctrl+Shift+N | 표시/숨김 토글 |
| Click-Through | Ctrl+Shift+T 후 클릭 | 클릭이 뒤 앱에 전달 |
| Click-Through 시각 피드백 | Click-Through 활성화 시 육안 확인 | 빨간 점선 + 텍스트 + 트레이 아이콘 |
| Click-Through 안전장치 | 투명도 10% + Click-Through | 20%로 강제 |
| 핫키 충돌 | 핫키 선점 후 앱 시작 | 트레이 벌룬 알림 |
| 전체 설정 사이클 | 설정 변경 -> 종료 -> 재시작 | 모든 설정 복원 (Click-Through는 OFF) |
| 비정상 종료 복구 | 프로세스 kill -> 재시작 | 마지막 저장 텍스트 복원 |
| settings.json 삭제 | 파일 삭제 후 실행 | 기본값으로 정상 시작 |
| 테마 전환 | 컨텍스트 메뉴에서 토글 | 일괄 색상 변경 + 텍스트 유지 |
| 컨텍스트 메뉴 전체 | 우클릭 후 모든 항목 확인 | PRD F11 전체 항목 표시 |
| 트레이 메뉴 | 트레이 우클릭 후 모든 항목 확인 | 표시/숨김, Always on Top, Click-Through, 종료 |
| Windows 10/11 | 양쪽 OS에서 실행 | 전체 기능 정상 동작 |
