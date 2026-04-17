# Sprint 2: 투명도 모드 + Always on Top + 컨텍스트 메뉴 골격 (Phase 1)

**Goal:** 배경 투명 / 전체 투명 모드 전환, 투명도 단계 조절, Always on Top 토글, 기본 컨텍스트 메뉴를 구현한다.

**Architecture:** Sprint 1에서 생성한 MainWindow.xaml / MainWindow.xaml.cs에 ContextMenu를 추가하고, 코드비하인드에 투명도 모드 전환 로직과 Topmost 토글을 구현한다. 투명도 모드는 "배경 투명"(Background 속성 제어)과 "전체 투명"(Window.Opacity 제어) 두 가지이며, 컨텍스트 메뉴에서 모드 전환과 투명도 단계 선택을 제공한다.

**Tech Stack:** .NET Framework 4.8, WPF (XAML + 코드비하인드), WindowChrome

**Sprint 기간:** 2026-04-17 ~ 2026-04-17
**상태:** ✅ 완료
**PR:** https://github.com/rynnkitty/OverlayNotepad/compare/develop...phase1-sprint2
**이전 스프린트:** Sprint 1 (WPF 프로젝트 + 투명 윈도우 + TextBox)
**브랜치명:** `phase1-sprint2`

---

## 제외 범위

- 투명도 슬라이더 UI (Phase 3에서 업그레이드 예정, 이 Sprint에서는 단계별 선택만)
- 시스템 트레이 (Phase 3)
- 텍스트 테두리/그림자 효과 (Phase 2)
- 설정 저장/복원 (Phase 2 — 이 Sprint에서 변경한 투명도/모드/Topmost 상태는 메모리에만 유지, 재시작 시 기본값으로 복원)
- 글꼴/색상 변경 메뉴 (Phase 3)

## 실행 플랜

Sprint 2의 모든 작업은 동일한 두 파일(`MainWindow.xaml`, `MainWindow.xaml.cs`)을 수정하므로 순차 실행이 필수이다.

### Phase 1 (순차)
| Task | 설명 | 대상 | skill |
|------|------|------|-------|
| Task 1 | 컨텍스트 메뉴 골격 + 종료 기능 | WPF | -- |
| Task 2 | Always on Top (Topmost) 토글 | WPF | -- |
| Task 3 | 배경 투명 모드 전환 | WPF | -- |
| Task 4 | 전체 투명 모드 + 투명도 단계 조절 | WPF | -- |
| Task 5 | 통합 검증 + 엣지 케이스 확인 | WPF | -- |

> **병렬 불가**: Task 1~5 모두 `MainWindow.xaml` + `MainWindow.xaml.cs` 파일을 수정하므로 순차 실행 필수.

---

### Task 1: 컨텍스트 메뉴 골격 + 종료 기능

**Files:**
- Modify: `src/OverlayNotepad/MainWindow.xaml` (TextBox에 ContextMenu 추가)
- Modify: `src/OverlayNotepad/MainWindow.xaml.cs` (종료 이벤트 핸들러 추가)

**Step 1: MainWindow.xaml에 ContextMenu XAML 추가**

- `MainTextBox`에 `ContextMenu` 속성 추가 (기존 TextBox 기본 컨텍스트 메뉴를 대체)
- 메뉴 구조 (Phase 문서 확정 설계):
  ```
  투명도              >  (서브메뉴: 20% / 40% / 60% / 80% / 100%)
  ────────────────
  [v] 배경 투명 모드    (체크 토글)
  ────────────────
  [v] 항상 위에         (체크 토글, 기본: 활성화)
  ────────────────
  종료
  ```
- 이 Step에서는 메뉴 골격만 XAML로 배치. 이벤트 핸들러는 `종료`만 연결하고, 나머지는 Task 2~4에서 연결.
- 투명도 서브메뉴 항목: `MenuItem` 5개 (Header="20%", "40%", "60%", "80%", "100%")
- 배경 투명 모드: `MenuItem` with `IsCheckable="True"`, `IsChecked="True"` (기본 활성화)
- 항상 위에: `MenuItem` with `IsCheckable="True"`, `IsChecked="True"` (기본 활성화)
- `Separator`로 그룹 구분

**Step 2: 종료 이벤트 핸들러 구현**

- `MainWindow.xaml.cs`에 `ExitMenuItem_Click` 메서드 추가
- 내부: `Application.Current.Shutdown()` 호출
- XAML에서 종료 MenuItem의 `Click` 이벤트에 바인딩

**Step 3: 검증**

```
검증: 빌드 후 실행, TextBox 우클릭 시 컨텍스트 메뉴 표시 확인
  - 메뉴 항목 5개 그룹 표시 (투명도 서브메뉴, 배경 투명 모드, 항상 위에, 종료)
  - "종료" 클릭 시 앱 종료 확인
  - 나머지 항목은 클릭 가능하지만 아직 기능 미연결 (Task 2~4에서 구현)
예상: 컨텍스트 메뉴 정상 표시, 종료 동작
```

**Step 4: 커밋**

```
git add src/OverlayNotepad/MainWindow.xaml src/OverlayNotepad/MainWindow.xaml.cs
git commit -m "feat(phase1-sprint2): task1 -- 컨텍스트 메뉴 골격 + 종료 기능"
```

**완료 기준:**
- ✅ TextBox 우클릭 시 컨텍스트 메뉴 표시
- ✅ 메뉴에 투명도/배경투명모드/항상위에/종료 항목 존재
- ✅ 종료 클릭 시 앱 정상 종료

---

### Task 2: Always on Top (Topmost) 토글

**Files:**
- Modify: `src/OverlayNotepad/MainWindow.xaml` (Window에 Topmost="True" 기본값 설정)
- Modify: `src/OverlayNotepad/MainWindow.xaml.cs` ("항상 위에" MenuItem 클릭 핸들러 추가)

**Step 1: Window 기본값 설정**

- `MainWindow.xaml`의 `<Window>` 태그에 `Topmost="True"` 추가 (기본값: 항상 위에 활성화)
- Sprint 1에서 이미 설정되어 있을 수 있으므로 확인 후 추가

**Step 2: Topmost 토글 이벤트 핸들러 구현**

- `MainWindow.xaml.cs`에 `AlwaysOnTopMenuItem_Click` 메서드 추가
- 로직:
  ```
  MenuItem menuItem = sender as MenuItem
  this.Topmost = menuItem.IsChecked
  ```
- `IsCheckable="True"`로 설정된 MenuItem이므로 WPF가 자동으로 `IsChecked` 토글 처리
- XAML에서 "항상 위에" MenuItem의 `Click` 이벤트에 바인딩

**Step 3: 검증**

```
검증: 빌드 후 실행
  1. 기본 상태에서 다른 창 위에 표시되는지 확인 (Topmost=True)
  2. 우클릭 메뉴 > "항상 위에" 체크 해제
  3. 다른 창 클릭 시 메모장이 뒤로 가는지 확인
  4. 다시 우클릭 > "항상 위에" 체크
  5. 다른 창 위에 다시 표시되는지 확인
예상: Topmost 토글 정상 동작
```

**Step 4: 커밋**

```
git add src/OverlayNotepad/MainWindow.xaml src/OverlayNotepad/MainWindow.xaml.cs
git commit -m "feat(phase1-sprint2): task2 -- Always on Top 토글 구현"
```

**완료 기준:**
- ✅ 기본 상태에서 항상 위에 활성화
- ✅ 컨텍스트 메뉴에서 Topmost 토글 동작
- ✅ 토글 해제 시 다른 창 뒤로 이동 확인

---

### Task 3: 배경 투명 모드 전환

**Files:**
- Modify: `src/OverlayNotepad/MainWindow.xaml.cs` ("배경 투명 모드" MenuItem 클릭 핸들러 추가)

**Step 1: 배경 투명 모드 전환 로직 구현**

- `MainWindow.xaml.cs`에 `BackgroundTransparentMenuItem_Click` 메서드 추가
- 로직:
  ```
  MenuItem menuItem = sender as MenuItem
  if (menuItem.IsChecked)
  {
      // 배경 투명 모드 ON: 배경을 완전 투명으로
      this.Background = Brushes.Transparent;
  }
  else
  {
      // 배경 투명 모드 OFF: 반투명 배경으로 전환
      // #33000000 = 약 20% 불투명 검정 배경
      this.Background = new SolidColorBrush(Color.FromArgb(0x33, 0x00, 0x00, 0x00));
  }
  ```
- 기본값: 배경 투명 모드 ON (Sprint 1 기본 상태 = Background="Transparent")
- 배경 투명 OFF 시 반투명 검정 배경(`#33000000`)으로 텍스트 영역의 시각적 경계를 제공

**Step 2: 검증**

```
검증: 빌드 후 실행
  1. 기본 상태: 배경이 완전 투명, 뒤의 창/데스크탑이 보임
  2. 우클릭 > "배경 투명 모드" 체크 해제
  3. 배경이 반투명 검정으로 변경되어 텍스트 영역이 시각적으로 구분됨
  4. 다시 체크하면 배경이 투명으로 복원
  5. 배경 투명 모드 전환 시 텍스트 내용이 유지되는지 확인
예상: 배경 투명/반투명 전환 정상 동작, 텍스트 유지
```

**Step 3: 커밋**

```
git add src/OverlayNotepad/MainWindow.xaml.cs
git commit -m "feat(phase1-sprint2): task3 -- 배경 투명 모드 전환 구현"
```

**완료 기준:**
- ✅ 배경 투명 모드 ON: 배경 완전 투명
- ✅ 배경 투명 모드 OFF: 반투명 검정 배경 표시
- ✅ 모드 전환 시 텍스트 내용 유지

---

### Task 4: 전체 투명 모드 + 투명도 단계 조절

**Files:**
- Modify: `src/OverlayNotepad/MainWindow.xaml.cs` (투명도 서브메뉴 클릭 핸들러 + 현재 선택 표시 로직)
- Modify: `src/OverlayNotepad/MainWindow.xaml` (투명도 서브메뉴 항목에 클릭 이벤트 연결)

**Step 1: 투명도 단계 클릭 핸들러 구현**

- `MainWindow.xaml.cs`에 `OpacityMenuItem_Click` 메서드 추가
- 투명도 단계: 20% / 40% / 60% / 80% / 100%
  - 20% → `Window.Opacity = 0.2`
  - 40% → `Window.Opacity = 0.4`
  - 60% → `Window.Opacity = 0.6`
  - 80% → `Window.Opacity = 0.8` (기본값)
  - 100% → `Window.Opacity = 1.0`
- 각 MenuItem의 `Tag` 속성에 Opacity 값을 저장 (예: `Tag="0.2"`, `Tag="0.4"` ...)
- 클릭 시:
  ```
  MenuItem clicked = sender as MenuItem
  double opacity = double.Parse(clicked.Tag.ToString())
  this.Opacity = opacity
  // 현재 선택 항목에 체크 표시, 나머지 해제
  UpdateOpacityMenuChecks(clicked)
  ```

**Step 2: 현재 선택 표시 로직 구현**

- `UpdateOpacityMenuChecks` 헬퍼 메서드:
  - 투명도 서브메뉴의 모든 자식 MenuItem을 순회
  - 클릭된 항목만 `IsChecked = true`, 나머지는 `IsChecked = false`
- 기본값: 80% 항목에 체크 표시 (`Window.Opacity = 0.8` — Phase 문서 확정 파라미터 "기본 투명도 80%")

**Step 3: XAML 이벤트 연결**

- Task 1에서 생성한 투명도 서브메뉴 5개 항목에 `Click="OpacityMenuItem_Click"` 추가
- 각 항목에 `Tag` 값 설정
- 80% 항목에 `IsChecked="True"` 기본 설정

**Step 4: Window 기본 Opacity 설정**

- `MainWindow.xaml`의 `<Window>` 태그에 `Opacity="0.8"` 추가 (기본 투명도 80%)

**Step 5: 검증**

```
검증: 빌드 후 실행
  1. 기본 상태: 윈도우 전체가 80% 불투명도로 표시
  2. 우클릭 > 투명도 > 20% 선택
  3. 윈도우 전체(텍스트 포함)가 매우 투명해짐
  4. 투명도 > 100% 선택
  5. 윈도우가 완전 불투명으로 표시
  6. 투명도 > 60% 선택 후, 메뉴를 다시 열어 60%에 체크 표시 확인
  7. 투명도 20% 상태에서 텍스트 입력이 정상 동작하는지 확인
  8. 투명도 변경 + 배경 투명 모드 조합 테스트:
     - 배경 투명 ON + 투명도 60%: 배경 투명 + 텍스트도 60% 투명
     - 배경 투명 OFF + 투명도 100%: 반투명 배경 + 텍스트 완전 불투명
예상: 투명도 단계별 조절 정상 동작, 선택 표시 정확
```

**Step 6: 커밋**

```
git add src/OverlayNotepad/MainWindow.xaml src/OverlayNotepad/MainWindow.xaml.cs
git commit -m "feat(phase1-sprint2): task4 -- 전체 투명 모드 + 투명도 단계 조절"
```

**완료 기준:**
- ✅ 5단계 투명도 선택 동작 (20/40/60/80/100%)
- ✅ 기본 투명도 80%
- ✅ 현재 선택된 투명도에 체크 표시
- ✅ 배경 투명 모드와 전체 투명도 조합 정상 동작

---

### Task 5: 통합 검증 + 엣지 케이스 확인

**Files:**
- Modify: `src/OverlayNotepad/MainWindow.xaml.cs` (필요시 엣지 케이스 수정)

**Step 1: 전체 기능 통합 검증**

```
검증 시나리오:
  1. 앱 실행 → 기본 상태 확인
     - Topmost=True, 배경 투명, 투명도 80%
     - TextBox에 자동 포커스
  2. 텍스트 입력 → 한글/영문 입력 정상 동작
  3. 우클릭 → 컨텍스트 메뉴 표시
     - "항상 위에" 체크됨, "배경 투명 모드" 체크됨
     - 투명도 서브메뉴의 80%에 체크됨
  4. 투명도 변경 → 20% → 텍스트 입력 가능 확인
  5. 배경 투명 OFF → 반투명 배경 표시, 텍스트 입력 가능 확인
  6. Always on Top OFF → 다른 창 클릭 시 메모장 뒤로 이동
  7. Always on Top ON → 다시 최상위 표시
  8. 종료 → 앱 정상 종료
```

**Step 2: 엣지 케이스 확인**

```
엣지 케이스:
  1. 투명도 20% 상태에서 컨텍스트 메뉴가 정상 표시되는지
     - 주의: Window.Opacity는 자식 요소(ContextMenu 포함)에도 적용될 수 있음
     - WPF ContextMenu는 별도 Popup 윈도우이므로 Window.Opacity와 독립 — 정상 표시 예상
     - 만약 메뉴도 투명해진다면: ContextMenu에 별도 Opacity=1.0 설정 필요
  2. 드래그 영역(상단 12px)에서 우클릭 시에도 컨텍스트 메뉴 표시
     - TextBox 영역이 아닌 드래그 영역에서도 메뉴 접근 가능해야 함
     - 필요 시 Window 레벨에 ContextMenu 바인딩 추가 (TextBox 전용이 아닌 전체 윈도우용)
  3. 투명도 변경 중 텍스트 입력 중이던 한글 조합이 깨지지 않는지
  4. 윈도우 리사이즈 후에도 컨텍스트 메뉴 정상 표시
```

**Step 3: 수정 사항 반영 및 커밋**

- 엣지 케이스에서 발견된 문제가 있으면 수정
- 특히 엣지 케이스 2(드래그 영역 우클릭)는 높은 확률로 수정 필요

```
git add src/OverlayNotepad/MainWindow.xaml src/OverlayNotepad/MainWindow.xaml.cs
git commit -m "fix(phase1-sprint2): task5 -- 통합 검증 후 엣지 케이스 수정"
```

**완료 기준:**
- ✅ 전체 기능 통합 시나리오 통과
- ✅ 투명도 20% 상태에서 컨텍스트 메뉴 정상 표시
- ✅ 드래그 영역에서도 컨텍스트 메뉴 접근 가능
- ✅ 모든 모드 조합에서 텍스트 입력 정상 동작

---

## 최종 검증 계획

| 검증 항목 | 명령/방법 | 예상 결과 |
|-----------|----------|-----------|
| 빌드 성공 | Visual Studio 또는 `msbuild OverlayNotepad.sln /p:Configuration=Release` | Build succeeded, 0 errors |
| 기본 상태 | 앱 실행 후 확인 | Topmost=True, 배경 투명, 투명도 80%, TextBox 자동 포커스 |
| 컨텍스트 메뉴 | TextBox 우클릭 | 투명도/배경투명모드/항상위에/종료 메뉴 표시 |
| 투명도 조절 | 메뉴에서 20%/40%/60%/80%/100% 선택 | 윈도우 전체 투명도 변경, 선택 항목 체크 표시 |
| 배경 투명 전환 | 메뉴에서 "배경 투명 모드" 토글 | ON: 배경 투명 / OFF: 반투명 배경 |
| Always on Top | 메뉴에서 "항상 위에" 토글 | ON: 최상위 / OFF: 다른 창 뒤로 이동 가능 |
| 종료 | 메뉴에서 "종료" 클릭 | 앱 정상 종료 |
| 메모리 | 작업 관리자에서 확인 | 80MB 이하 |
| 모드 조합 | 배경 투명 OFF + 투명도 40% | 반투명 배경 + 전체 40% 불투명 |
