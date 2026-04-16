# Sprint 1: 텍스트 테두리 및 그림자 효과 (Phase 2)

**Goal:** 투명 배경 위에서 어떤 배경(밝은/어두운)이든 텍스트를 읽을 수 있도록 FormattedText + Geometry 기반 아웃라인 테두리와 DropShadowEffect 그림자를 구현한다.

**Architecture:** 이중 레이어 구조를 채택한다. TextBox는 항상 입력을 담당하고(커서, 선택, IME, Undo/Redo 보존), OutlinedTextControl은 테두리 효과가 적용된 텍스트를 렌더링만 담당한다. 테두리 ON 시 TextBox 글자를 투명으로 만들고 OutlinedTextControl이 같은 위치에 오버레이로 렌더링하며, 테두리 OFF 시 OutlinedTextControl을 숨기고 TextBox가 직접 표시한다. DropShadowEffect는 TextBox에 적용되어 테두리 ON/OFF와 무관하게 동작한다.

**Tech Stack:** .NET Framework 4.8, WPF (XAML + 코드비하인드), FormattedText, Geometry, DropShadowEffect

**Sprint 기간:** 2026-04-17 ~ (사용자 검토 후 구현)
**이전 스프린트:** Phase 1 Sprint 2 (투명도 모드 + Always on Top + 컨텍스트 메뉴 골격)
**브랜치명:** `phase2-sprint1`

---

## 제외 범위

- 테두리/그림자 색상 변경 UI (컨텍스트 메뉴에서 색상 선택) -- Phase 3 Sprint 2
- 테두리 두께 변경 UI -- Phase 3에서 설정 메뉴와 함께 구현
- 그림자 파라미터(blur, offset) 변경 UI -- Phase 3
- 설정 저장/복원 (테두리/그림자 상태 영속화) -- Phase 2 Sprint 2
- 다크/라이트 테마 전환 -- Phase 3
- 성능 최적화 (대량 텍스트에서의 렌더링 성능) -- 필요 시 Phase 4

## 실행 플랜

이 Sprint의 모든 Task는 동일 파일군(`MainWindow.xaml`, `MainWindow.xaml.cs`, 새 컨트롤 파일)을 수정하므로 순차 실행이 필수이다.

### Phase 1 (순차)
| Task | 설명 | 대상 | skill |
|------|------|------|-------|
| Task 1 | TextEffectSettings 모델 + OutlinedTextControl 커스텀 컨트롤 생성 | 데스크톱 | -- |
| Task 2 | DropShadowEffect 적용 + MainWindow 통합 (이중 레이어 구조) | 데스크톱 | -- |
| Task 3 | 테두리/그림자 토글 + 컨텍스트 메뉴 연동 | 데스크톱 | -- |
| Task 4 | 통합 검증 + 가독성 테스트 + 엣지 케이스 | 데스크톱 | -- |

> **병렬 불가**: Task 1~4 모두 동일 프로젝트 파일을 수정하므로 순차 실행 필수.

---

### Task 1: TextEffectSettings 모델 + OutlinedTextControl 커스텀 컨트롤

**Files:**
- Create: `src/OverlayNotepad/Models/TextEffectSettings.cs`
- Create: `src/OverlayNotepad/Controls/OutlinedTextControl.cs`

**Step 1: TextEffectSettings 모델 생성**

- `src/OverlayNotepad/Models/TextEffectSettings.cs` 생성
- 네임스페이스: `OverlayNotepad.Models`
- 프로퍼티 목록 (Phase 2 확정 파라미터 기반):
  - `bool OutlineEnabled` -- 테두리 활성화 여부 (기본값: `true`)
  - `double OutlineThickness` -- 테두리 두께 (기본값: `1.0`, 범위: 0.5~3.0)
  - `Color OutlineColor` -- 테두리 색상 (기본값: `Colors.Black`, #000000)
  - `bool ShadowEnabled` -- 그림자 활성화 여부 (기본값: `true`)
  - `double ShadowBlurRadius` -- 그림자 블러 반경 (기본값: `5.0`)
  - `double ShadowOffsetX` -- 그림자 X 오프셋 (기본값: `1.5`)
  - `double ShadowOffsetY` -- 그림자 Y 오프셋 (기본값: `1.5`)
  - `double ShadowOpacity` -- 그림자 불투명도 (기본값: `0.8`)
- 기본 생성자에서 위 기본값으로 초기화

**Step 2: OutlinedTextControl 생성**

- `src/OverlayNotepad/Controls/OutlinedTextControl.cs` 생성
- 네임스페이스: `OverlayNotepad.Controls`
- `FrameworkElement`를 상속하는 클래스 (XAML 파일 없이 코드 전용 컨트롤)
- DependencyProperty 정의:
  - `Text` (string) -- 표시할 텍스트
  - `FontFamily` (FontFamily) -- 글꼴
  - `FontSize` (double) -- 글자 크기
  - `FillBrush` (Brush) -- 글자 채움 색상 (기본: `Brushes.White`)
  - `OutlineThickness` (double) -- 테두리 두께 (기본: `1.0`)
  - `OutlineBrush` (Brush) -- 테두리 색상 (기본: `Brushes.Black`)
  - `VerticalOffset` (double) -- 스크롤 동기화용 세로 오프셋
  - `HorizontalOffset` (double) -- 가로 오프셋
  - `TextPadding` (Thickness) -- TextBox와 일치시키기 위한 패딩
- `OnRender(DrawingContext dc)` 오버라이드:
  1. 텍스트가 비어있으면 리턴
  2. 텍스트를 줄 단위(`\n`)로 분할
  3. 각 줄마다:
     a. `FormattedText` 생성 (글꼴, 크기, 채움 브러시 설정)
     b. `FormattedText.BuildGeometry()` 호출로 텍스트 Geometry 획득
     c. `Pen` 생성 (OutlineBrush, OutlineThickness)
     d. `TranslateTransform`으로 줄 위치 + 오프셋 적용
     e. `dc.DrawGeometry(FillBrush, outlinePen, geometry)` 호출
  4. DependencyProperty 변경 시 `InvalidateVisual()` 콜백 등록
- `IsHitTestVisible = false` 설정 (마우스 이벤트를 아래 TextBox로 통과)

**Step 3: 프로젝트 파일 업데이트**

- `src/OverlayNotepad/OverlayNotepad.csproj`에 새 파일 2개의 Compile 항목 추가:
  - `Models\TextEffectSettings.cs`
  - `Controls\OutlinedTextControl.cs`

**Step 4: 빌드 검증**

```bash
"/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "D:/99.실험실/OverlayNotepad/OverlayNotepad.sln" /p:Configuration=Debug /t:Build
```
- 예상: Build succeeded, 0 Errors

**Step 5: 커밋**

```
git add src/OverlayNotepad/Models/TextEffectSettings.cs src/OverlayNotepad/Controls/OutlinedTextControl.cs src/OverlayNotepad/OverlayNotepad.csproj
git commit -m "feat(phase2-sprint1): task1 -- TextEffectSettings 모델 + OutlinedTextControl 커스텀 컨트롤"
```

**완료 기준:**
- ⬜ MSBuild 빌드 성공 (0 Errors)
- ⬜ TextEffectSettings 모델에 확정 파라미터 기본값 반영
- ⬜ OutlinedTextControl의 DependencyProperty 정의 완료

---

### Task 2: DropShadowEffect 적용 + MainWindow 통합 (이중 레이어 구조)

**skill:** `feature-dev:feature-dev`

**Files:**
- Modify: `src/OverlayNotepad/MainWindow.xaml` (OutlinedTextControl 배치 + DropShadowEffect + 레이아웃 조정)
- Modify: `src/OverlayNotepad/MainWindow.xaml.cs` (텍스트 동기화 + 스크롤 동기화 + 효과 초기화)

**Step 1: MainWindow.xaml에 이중 레이어 구조 배치**

- MainWindow.xaml의 기존 Grid(텍스트 영역) 내에 OutlinedTextControl 추가
- 레이아웃 구조:
  ```
  <Grid>
    <!-- 상단 드래그 영역 (기존 Sprint 1에서 구현) -->
    <Border Height="12" ... />

    <!-- 텍스트 영역 (드래그 영역 아래) -->
    <Grid Margin="0,12,0,0">
      <!-- 아웃라인 텍스트 렌더링 레이어 (테두리 ON 시만 표시) -->
      <local:OutlinedTextControl x:Name="OutlinedText"
                                  IsHitTestVisible="False"
                                  Visibility="Visible" />

      <!-- TextBox (항상 존재, 입력 담당) -->
      <TextBox x:Name="MainTextBox" ... (기존 속성 유지) />
    </Grid>
  </Grid>
  ```
- OutlinedTextControl이 TextBox 위에 겹쳐지도록 같은 Grid 셀에 배치
- OutlinedTextControl의 `IsHitTestVisible="False"` -- 모든 마우스/키보드 이벤트가 TextBox로 전달
- XAML 네임스페이스 추가: `xmlns:local="clr-namespace:OverlayNotepad.Controls"`

**Step 2: DropShadowEffect 적용**

- TextBox에 DropShadowEffect 설정 (XAML):
  ```xml
  <TextBox.Effect>
    <DropShadowEffect x:Name="TextShadowEffect"
                      ShadowDepth="1.5"
                      BlurRadius="5"
                      Opacity="0.8"
                      Color="Black"
                      Direction="315" />
  </TextBox.Effect>
  ```
- ShadowDepth: `1.5` (확정 파라미터의 offset X/Y=1.5 -> ShadowDepth=1.5, Direction으로 방향 제어)
- BlurRadius: `5` (확정 파라미터)
- Opacity: `0.8` (확정 파라미터)
- Direction: `315` (오른쪽 아래 방향, offset X=1.5 Y=1.5에 대응)

**Step 3: 코드비하인드 -- 텍스트 동기화**

- `MainWindow.xaml.cs`에 다음 로직 추가:
- `_textEffectSettings` 필드 (TextEffectSettings 인스턴스, 기본값으로 초기화)
- `MainTextBox.TextChanged` 이벤트 핸들러:
  - `OutlinedText.Text = MainTextBox.Text` (텍스트 동기화)
- `MainTextBox.SelectionChanged` 이벤트 핸들러 또는 `ScrollViewer.ScrollChanged`:
  - TextBox 내부의 ScrollViewer를 VisualTreeHelper로 탐색
  - `OutlinedText.VerticalOffset = scrollViewer.VerticalOffset` (스크롤 동기화)
  - `OutlinedText.HorizontalOffset = scrollViewer.HorizontalOffset`
- 글꼴 동기화 (Phase 3에서 글꼴 변경 시 대응):
  - OutlinedText.FontFamily = MainTextBox.FontFamily
  - OutlinedText.FontSize = MainTextBox.FontSize

**Step 4: 코드비하인드 -- 초기 상태 설정**

- Window_Loaded 이벤트 (기존 핸들러 확장):
  - TextEffectSettings 기본값으로 초기화
  - `ApplyTextEffects()` 메서드 호출
- `ApplyTextEffects()` 메서드:
  - `_textEffectSettings.OutlineEnabled` 확인
  - ON: OutlinedText.Visibility = Visible, MainTextBox.Foreground = Transparent
  - OFF: OutlinedText.Visibility = Collapsed, MainTextBox.Foreground = 설정된 색상 (#FFFFFF)
  - OutlinedText의 OutlineThickness, OutlineBrush, FillBrush 설정
  - DropShadowEffect 활성화/비활성화: `TextShadowEffect.Opacity = _textEffectSettings.ShadowEnabled ? 0.8 : 0`

**Step 5: ScrollViewer 탐색 헬퍼**

- `FindScrollViewer(DependencyObject obj)` 헬퍼 메서드:
  - VisualTreeHelper.GetChildrenCount / GetChild 를 재귀 탐색
  - ScrollViewer 타입을 찾아 반환
- TextBox Loaded 이벤트에서 ScrollViewer 참조를 캐싱

**Step 6: 빌드 및 수동 검증**

```bash
"/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "D:/99.실험실/OverlayNotepad/OverlayNotepad.sln" /p:Configuration=Debug /t:Build
```
- 예상: Build succeeded, 0 Errors

수동 검증:
- ⬜ 실행 시 텍스트에 검정 테두리(아웃라인)가 보임 (흰색 글자 + 검정 테두리)
- ⬜ 텍스트에 그림자 효과가 적용됨
- ⬜ 텍스트 입력 시 OutlinedTextControl이 실시간 동기화
- ⬜ 여러 줄 입력 후 스크롤 시 OutlinedTextControl이 함께 스크롤
- ⬜ 한글 IME 조합 중에도 정상 표시
- ⬜ 기존 기능(드래그, 리사이즈, 투명도, Topmost, 컨텍스트 메뉴) 정상 동작

**Step 7: 커밋**

```
git add src/OverlayNotepad/MainWindow.xaml src/OverlayNotepad/MainWindow.xaml.cs
git commit -m "feat(phase2-sprint1): task2 -- DropShadowEffect + OutlinedTextControl 이중 레이어 통합"
```

**완료 기준:**
- ⬜ 텍스트에 검정 아웃라인 테두리 표시
- ⬜ DropShadowEffect 그림자 표시
- ⬜ 텍스트 동기화 (입력 즉시 반영)
- ⬜ 스크롤 동기화
- ⬜ 기존 Phase 1 기능 회귀 없음

---

### Task 3: 테두리/그림자 토글 + 컨텍스트 메뉴 연동

**Files:**
- Modify: `src/OverlayNotepad/MainWindow.xaml` (컨텍스트 메뉴에 테두리/그림자 토글 항목 추가)
- Modify: `src/OverlayNotepad/MainWindow.xaml.cs` (토글 이벤트 핸들러 추가)

**Step 1: 컨텍스트 메뉴에 테두리/그림자 토글 항목 추가**

- Phase 1 Sprint 2에서 생성한 기존 ContextMenu에 항목 추가
- 추가 위치: "배경 투명 모드" 항목 아래, "항상 위에" 항목 위에 Separator 구분
- 메뉴 구조 (추가 부분):
  ```
  ... (기존 투명도 서브메뉴)
  ────────────────
  [v] 배경 투명 모드
  ────────────────
  [v] 텍스트 테두리        (새로 추가, IsCheckable, 기본: 체크됨)
  [v] 텍스트 그림자        (새로 추가, IsCheckable, 기본: 체크됨)
  ────────────────
  [v] 항상 위에
  ────────────────
  종료
  ```
- 테두리 MenuItem: `x:Name="OutlineMenuItem"`, `IsCheckable="True"`, `IsChecked="True"`, `Click="OutlineMenuItem_Click"`
- 그림자 MenuItem: `x:Name="ShadowMenuItem"`, `IsCheckable="True"`, `IsChecked="True"`, `Click="ShadowMenuItem_Click"`

**Step 2: 테두리 토글 핸들러**

- `MainWindow.xaml.cs`에 `OutlineMenuItem_Click` 메서드 추가:
  ```
  _textEffectSettings.OutlineEnabled = outlineMenuItem.IsChecked
  ApplyTextEffects()  // Task 2에서 구현한 메서드 재사용
  ```
- 토글 시 텍스트 내용/커서 위치 보존:
  - TextBox.Text, TextBox.SelectionStart, TextBox.SelectionLength를 변경하지 않음
  - Foreground 색상만 투명/흰색으로 전환

**Step 3: 그림자 토글 핸들러**

- `MainWindow.xaml.cs`에 `ShadowMenuItem_Click` 메서드 추가:
  ```
  _textEffectSettings.ShadowEnabled = shadowMenuItem.IsChecked
  ApplyTextEffects()
  ```
- 그림자 ON: TextShadowEffect.Opacity = 0.8 (확정 파라미터)
- 그림자 OFF: TextShadowEffect.Opacity = 0

**Step 4: 빌드 및 수동 검증**

```bash
"/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "D:/99.실험실/OverlayNotepad/OverlayNotepad.sln" /p:Configuration=Debug /t:Build
```
- 예상: Build succeeded, 0 Errors

수동 검증:
- ⬜ 컨텍스트 메뉴에 "텍스트 테두리", "텍스트 그림자" 항목 표시
- ⬜ 기본 상태: 테두리 ON, 그림자 ON
- ⬜ 테두리 OFF 시: 아웃라인 사라지고 TextBox 글자색이 흰색으로 변경, 텍스트 내용 유지
- ⬜ 테두리 ON 시: 아웃라인 다시 표시, 텍스트 내용 유지
- ⬜ 그림자 OFF 시: 그림자 사라짐
- ⬜ 그림자 ON 시: 그림자 다시 표시
- ⬜ 테두리 OFF + 그림자 OFF: 일반 TextBox 상태 (Phase 1과 동일)
- ⬜ 테두리 OFF + 그림자 ON: 그림자만 적용된 TextBox
- ⬜ 토글 전후 커서 위치 보존

**Step 5: 커밋**

```
git add src/OverlayNotepad/MainWindow.xaml src/OverlayNotepad/MainWindow.xaml.cs
git commit -m "feat(phase2-sprint1): task3 -- 테두리/그림자 토글 + 컨텍스트 메뉴 연동"
```

**완료 기준:**
- ⬜ 컨텍스트 메뉴에서 테두리/그림자 독립 토글
- ⬜ 4가지 조합 모두 정상 동작 (둘 다 ON / 테두리만 / 그림자만 / 둘 다 OFF)
- ⬜ 토글 시 텍스트 내용/커서 위치 보존

---

### Task 4: 통합 검증 + 가독성 테스트 + 엣지 케이스

**Files:**
- Modify: `src/OverlayNotepad/MainWindow.xaml.cs` (필요시 엣지 케이스 수정)
- Modify: `src/OverlayNotepad/Controls/OutlinedTextControl.cs` (필요시 렌더링 수정)

**Step 1: 가독성 검증 (핵심 검증)**

이 Task의 핵심은 투명 배경 위에서 다양한 배경에 대한 가독성 검증이다.

```
가독성 검증 시나리오:
  1. 밝은 배경 테스트
     - 메모장 뒤에 흰색 배경 웹페이지/문서를 배치
     - 배경 투명 모드 ON 상태에서 텍스트 가독성 확인
     - 기대: 검정 아웃라인 + 그림자 덕분에 흰색 글자가 밝은 배경에서도 읽힘
  2. 어두운 배경 테스트
     - 메모장 뒤에 검정 배경(터미널/IDE 등)을 배치
     - 배경 투명 모드 ON 상태에서 텍스트 가독성 확인
     - 기대: 흰색 글자가 어두운 배경에서 자연스럽게 읽힘
  3. 혼합 배경 테스트
     - 메모장 뒤에 밝은/어두운 영역이 혼합된 화면을 배치
     - 테두리가 양쪽 배경 모두에서 가독성을 제공하는지 확인
  4. 고대비 배경 테스트
     - 바탕화면 배경이 다양한 색상인 상태에서 확인
```

**Step 2: 기능 통합 검증**

```
통합 검증 시나리오:
  1. 앱 실행 -> 기본 상태 확인
     - 테두리 ON, 그림자 ON, Topmost ON, 배경 투명, 투명도 80%
     - 텍스트에 검정 아웃라인 + 그림자 표시
  2. 텍스트 입력 -> 한글/영문 입력 시 아웃라인이 실시간 동기화
  3. Ctrl+Z/Ctrl+Y -> Undo/Redo 동작, 아웃라인 동기화
  4. Ctrl+A -> 전체 선택 동작 (선택 영역이 TextBox에서 정상 표시)
  5. 투명도 조합:
     - 투명도 20% + 테두리 ON -> 전체가 투명해지지만 텍스트/테두리 보임
     - 투명도 100% + 테두리 ON -> 불투명 상태에서 테두리 표시
  6. 배경 투명 모드 조합:
     - 배경 투명 OFF + 테두리 ON -> 반투명 배경 위에 테두리 텍스트
     - 배경 투명 ON + 테두리 OFF -> Phase 1 상태와 동일
  7. 여러 줄 입력 -> 스크롤 시 아웃라인 동기화 확인
  8. 윈도우 리사이즈 -> 텍스트 리플로우 시 아웃라인 업데이트
```

**Step 3: 엣지 케이스 확인**

```
엣지 케이스:
  1. 빈 텍스트 상태에서 테두리 토글 -> 오류 없이 동작
  2. 매우 긴 텍스트 (100줄 이상) 입력 시 아웃라인 렌더링 성능
     - 눈에 띄는 지연이 있으면 폴백 검토 필요
  3. 탭 문자 포함 텍스트에서 아웃라인 위치가 TextBox와 일치하는지
  4. 줄바꿈 포함 텍스트에서 줄 높이(LineHeight)가 TextBox와 일치하는지
  5. 텍스트 선택 중 테두리 토글 -> 선택 범위 유지
  6. IME 조합 중(한글 입력 중간 상태) 아웃라인이 깜빡이거나 이중으로 표시되지 않는지
  7. Alt+F4 종료 시 오류 없음
```

**Step 4: 수정 사항 반영**

- 가독성 검증에서 문제가 발견되면:
  - 테두리 두께 조정 (1.0px -> 1.5px 등)
  - 그림자 blur 조정 (5px -> 3px 등)
- 동기화 문제가 발견되면:
  - OutlinedTextControl 렌더링 로직 수정
  - 패딩/마진 오프셋 미세 조정
- **폴백 시나리오**: FormattedText 렌더링이 TextBox와 심각하게 불일치하는 경우
  - Phase 문서의 폴백 A: DropShadowEffect만 사용
  - Phase 문서의 폴백 B: 다중 DropShadowEffect (4방향 그림자로 테두리 시뮬레이션)

**Step 5: 커밋**

```
git add src/OverlayNotepad/MainWindow.xaml src/OverlayNotepad/MainWindow.xaml.cs src/OverlayNotepad/Controls/OutlinedTextControl.cs
git commit -m "fix(phase2-sprint1): task4 -- 통합 검증 + 가독성 테스트 후 수정"
```

**완료 기준:**
- ⬜ 밝은 배경 위에서 텍스트 가독성 확보
- ⬜ 어두운 배경 위에서 텍스트 가독성 확보
- ⬜ 혼합 배경 위에서 텍스트 가독성 확보
- ⬜ 모든 투명도/배경 모드 조합에서 테두리/그림자 정상 동작
- ⬜ 스크롤 동기화 정상
- ⬜ IME 조합 중 아웃라인 정상 표시
- ⬜ 100줄 이상 텍스트에서 눈에 띄는 렌더링 지연 없음
- ⬜ Phase 1 기능 회귀 없음

---

## 최종 검증 계획

| 검증 항목 | 명령/방법 | 예상 결과 |
|-----------|----------|-----------|
| 빌드 성공 | `MSBuild OverlayNotepad.sln /p:Configuration=Debug` | Build succeeded, 0 Errors |
| 기본 상태 | 앱 실행 후 확인 | 텍스트에 검정 아웃라인 + 그림자, Topmost=True, 배경 투명, 투명도 80% |
| 테두리 효과 | 텍스트 입력 후 육안 확인 | 흰색 글자 주위에 검정 아웃라인 (두께 1.0px) |
| 그림자 효과 | 텍스트 입력 후 육안 확인 | 오른쪽 아래 방향 그림자 (blur 5px, opacity 0.8) |
| 테두리 토글 | 우클릭 > "텍스트 테두리" 체크 해제/체크 | ON: 아웃라인 표시 / OFF: 아웃라인 제거, TextBox 직접 표시 |
| 그림자 토글 | 우클릭 > "텍스트 그림자" 체크 해제/체크 | ON: 그림자 표시 / OFF: 그림자 제거 |
| 토글 시 편집 연속성 | 토글 전후 텍스트/커서 확인 | 텍스트 내용 + 커서 위치 보존 |
| 밝은 배경 가독성 | 뒤에 흰색 배경 배치 후 확인 | 검정 아웃라인 덕분에 텍스트 가독 |
| 어두운 배경 가독성 | 뒤에 검정 배경 배치 후 확인 | 흰색 글자가 자연스럽게 가독 |
| 텍스트 동기화 | 빠르게 텍스트 입력 | 아웃라인이 실시간 동기화 |
| 스크롤 동기화 | 여러 줄 입력 후 스크롤 | 아웃라인이 TextBox와 함께 스크롤 |
| IME 입력 | 한글 입력 (조합 중 상태) | 조합 중에도 아웃라인 정상 |
| 투명도 조합 | 투명도 20% + 테두리 ON | 전체 투명 + 테두리 보임 |
| 메모리 | 작업 관리자에서 확인 | 80MB 이하 |
| 기존 기능 회귀 | 드래그/리사이즈/투명도/Topmost/종료 | 모두 정상 동작 |
