# Phase 1: WPF 기본 윈도우 + 투명 배경 + 텍스트 입력 — 실행 계획

> **Status**: 계획 수립 완료 (2026-04-17)
> **ROADMAP 참조**: `ROADMAP.md` Phase 1
> **검토 리포트**: `phase1-po-review.md`, `phase1-user-review.md`, `phase1-ux-review.md`

---

## 개요

WPF 프로젝트를 생성하고, 타이틀바 없는 투명 윈도우에서 텍스트를 입력/편집할 수 있는 동작하는 프로토타입을 완성한다. WPF의 AllowsTransparency + 표준 TextBox 조합으로 투명 배경(F1), Always on Top(F2)의 기초를 확보한다.

```
┌─────────────────────────────────────┐
│  [드래그 영역 12px - 마우스 오버 시  │  <- WindowChrome CaptionHeight=0
│   미세 하이라이트]                   │     + 커스텀 드래그 핸들러
├─────────────────────────────────────┤
│                                     │
│   TextBox (투명 배경)               │  <- AcceptsReturn, AcceptsTab
│   Background=Transparent            │     자동 포커스
│   맑은 고딕 14pt, #FFFFFF           │
│                                     │
│                                     │
└─────────────────────────────────────┘
  WindowChrome ResizeBorderThickness=8
```

### 기술 스택

- .NET Framework 4.8 + WPF (XAML + 코드비하인드)
- WindowChrome (리사이즈/드래그 관리)
- AllowsTransparency=True (투명 배경)
- 외부 NuGet 의존성: 없음

---

## 검토팀 확정 파라미터 (2026-04-17)

> 이기획 (PO), 김수진 (일반 사용자 관점), 한유엑 (UX 전문가) 검토 완료

| 항목 | 원래 설계 | 확정값 | 근거 |
|------|----------|--------|------|
| 기본 윈도우 크기 | 400x300 | 400x300 | PRD 기본값 |
| 최소 윈도우 크기 | 200x150 | 200x150 | UX: 텍스트 영역 실용 최소 크기 |
| 드래그 영역 높이 | 8px | **12px** | UX(한유엑): Fitts' Law + 투명 메모장 공간 효율 균형. PO(이기획) 동의. |
| 리사이즈 방식 | 커스텀 핸들 6px | **WindowChrome ResizeBorderThickness=8** | UX(한유엑): 시스템 관리 리사이즈가 더 안정적 |
| 기본 투명도 | 80% | 80% | PRD 기본값 |
| 기본 폰트 | 맑은 고딕 14pt | 맑은 고딕 14pt | PRD 기본값 |
| 기본 글자색 | #FFFFFF | #FFFFFF | PRD 기본값 (다크 테마) |
| TextBox 자동 포커스 | 미지정 | **필수** | 김수진: 실행 즉시 타이핑 가능해야 함 |
| 투명도 조절 방식 (Sprint 2) | 슬라이더 | **단계별 선택 (20/40/60/80/100%)** | UX(한유엑): ContextMenu 내 슬라이더보다 안정적. Phase 3에서 업그레이드 가능. |
| 드래그 영역 시각적 피드백 | 미지정 | **마우스 오버 시 5% 불투명 하이라이트** | UX(한유엑): 투명 모드에서 어포던스 제공 |

---

## Sprint 분할 계획

| Sprint | 주제 | 주요 작업 | 의존성 |
|--------|------|----------|--------|
| 1 | WPF 프로젝트 구조 + 투명 윈도우 + 기본 텍스트 입력 | 프로젝트 생성, 투명 윈도우, 드래그/리사이즈, TextBox, 최소 종료 수단 | 없음 |
| 2 | 투명도 모드 + Always on Top + 컨텍스트 메뉴 골격 | 배경/전체 투명 모드, 투명도 단계 조절, Topmost 토글, ContextMenu | Sprint 1 |

---

## Sprint 1 상세 — WPF 프로젝트 구조 + 투명 윈도우 + 기본 텍스트 입력

### 목표
.NET Framework 4.8 WPF 프로젝트를 생성하고, 타이틀바 없는 투명 윈도우에서 텍스트를 입력할 수 있는 동작하는 프로토타입을 완성한다.

### 신규 파일 목록

| 파일 | 설명 |
|------|------|
| `OverlayNotepad.sln` | 솔루션 파일 |
| `src/OverlayNotepad/OverlayNotepad.csproj` | WPF 프로젝트 파일 (.NET Framework 4.8) |
| `src/OverlayNotepad/App.xaml` | Application 정의 (StartupUri=MainWindow.xaml) |
| `src/OverlayNotepad/App.xaml.cs` | Application 코드비하인드 |
| `src/OverlayNotepad/MainWindow.xaml` | 메인 윈도우 XAML (투명 윈도우 + 드래그 영역 + TextBox) |
| `src/OverlayNotepad/MainWindow.xaml.cs` | 메인 윈도우 코드비하인드 (드래그, 포커스, 종료 로직) |
| `src/OverlayNotepad/Properties/AssemblyInfo.cs` | 어셈블리 정보 |

### MainWindow.xaml 핵심 구조

```xml
<Window WindowStyle="None"
        AllowsTransparency="True"
        Background="Transparent"
        MinWidth="200" MinHeight="150"
        Width="400" Height="300"
        FocusManager.FocusedElement="{Binding ElementName=MainTextBox}">

    <WindowChrome.WindowChrome>
        <WindowChrome CaptionHeight="0"
                      ResizeBorderThickness="8"
                      CornerRadius="0"
                      GlassFrameThickness="0"/>
    </WindowChrome.WindowChrome>

    <Grid>
        <!-- 드래그 영역 (12px, 마우스 오버 시 하이라이트) -->
        <Border Height="12" VerticalAlignment="Top"
                Background="Transparent"
                MouseLeftButtonDown="DragArea_MouseLeftButtonDown">
            <Border.Style>
                <!-- IsMouseOver 트리거로 5% 불투명 하이라이트 -->
            </Border.Style>
        </Border>

        <!-- 텍스트 입력 영역 -->
        <TextBox x:Name="MainTextBox"
                 Margin="0,12,0,0"
                 Background="Transparent"
                 BorderThickness="0"
                 AcceptsReturn="True"
                 AcceptsTab="True"
                 TextWrapping="Wrap"
                 VerticalScrollBarVisibility="Auto"
                 FontFamily="맑은 고딕"
                 FontSize="14"
                 Foreground="#FFFFFF"
                 CaretBrush="#FFFFFF"/>
    </Grid>
</Window>
```

### 코드비하인드 핵심 로직

- `DragArea_MouseLeftButtonDown`: `this.DragMove()` 호출
- `Loaded` 이벤트: TextBox 자동 포커스 확인
- Alt+F4: WPF 기본 지원 (별도 구현 불필요)
- Sprint 1에서는 Alt+F4 + 작업표시줄 우클릭 > 닫기가 종료 수단

### 재사용 자산

| 항목 | 출처 | 활용 |
|------|------|------|
| 없음 (Phase 0에 소스 코드 없음) | — | Phase 1이 최초 코드 생성 |

---

## Sprint 2 상세 — 투명도 모드 + Always on Top + 컨텍스트 메뉴 골격

### 목표
배경 투명 / 전체 투명 모드 전환, 투명도 단계 조절, Always on Top 토글, 기본 컨텍스트 메뉴를 구현한다.

### 수정 파일 목록

| 파일 | 변경 내용 |
|------|----------|
| `src/OverlayNotepad/MainWindow.xaml` | ContextMenu 추가, 투명도 관련 UI |
| `src/OverlayNotepad/MainWindow.xaml.cs` | 투명도 모드 전환, Topmost 토글, 메뉴 이벤트 핸들러 |

### 투명도 모드 설계

```
[배경 투명 모드] (기본값)
  - AllowsTransparency=True + Background=Transparent
  - 배경만 투명, 텍스트는 불투명
  - 배경 위 콘텐츠가 보임

[전체 투명 모드]
  - Window.Opacity 조절 (0.0 ~ 1.0)
  - 배경 + 텍스트 모두 투명도 적용
  - 투명도 단계: 20% / 40% / 60% / 80% / 100%
```

### 컨텍스트 메뉴 골격 (Sprint 2)

```
┌──────────────────────────┐
│ 투명도                   >│  -> 20% / 40% / 60% / 80% / 100%
│ ─────────────────────── │
│ [v] 배경 투명 모드       │  체크 토글
│ ─────────────────────── │
│ [v] 항상 위에            │  체크 토글 (기본: 활성화)
│ ─────────────────────── │
│ 종료                     │
└──────────────────────────┘
```

### 핵심 구현 로직

1. **배경 투명 모드**: `Window.Background` 를 `Transparent` <-> `#33000000` (반투명 배경) 전환
2. **전체 투명 모드**: `Window.Opacity` 값 조절 (0.2 / 0.4 / 0.6 / 0.8 / 1.0)
3. **Always on Top**: `Window.Topmost` 토글 (기본값: true)
4. **종료**: `Application.Current.Shutdown()` 호출

---

## 미해결 사항 / 리스크

| 항목 | 심각도 | 설명 | 대응 방안 |
|------|--------|------|----------|
| TextBox 교체 용이성 | 중간 | Phase 2에서 텍스트 테두리 구현 시 TextBox -> 커스텀 컨트롤 전환 필요 | TextBox를 쉽게 교체 가능한 구조로 설계. 이벤트 핸들러를 최소화하고, 데이터 바인딩 패턴 활용 권고 (PO, UX 공통 권고) |
| 소프트웨어 렌더링 성능 | 낮음 | AllowsTransparency=True 시 하드웨어 가속 비활성화 | 메모장 용도에서는 영향 미미. Phase 1 완료 후 대량 텍스트(수천 줄) 입력 시 스크롤 성능 확인 (UX 권고) |
| Sprint 1 종료 수단 제한 | 낮음 | Sprint 1에서 종료 방법이 Alt+F4/작업표시줄뿐 | Sprint 2에서 컨텍스트 메뉴로 해결. Sprint 1은 프로토타입 단계이므로 수용 가능 (김수진 제기, PO 수용) |
| 드래그 영역 인지성 | 낮음 | 투명 모드에서 드래그 영역이 안 보일 수 있음 | 마우스 오버 시 5% 불투명 하이라이트로 해결 (확정 파라미터 반영 완료) |

---

## 완료 기준 (Phase 전체)

| 항목 | 기준 | 상태 |
|------|------|------|
| 투명 윈도우 | 타이틀바 없는 투명 윈도우에서 드래그 이동 및 리사이즈 동작 | ⬜ |
| 텍스트 입력 | 투명 배경 위에서 한글/영문 텍스트 입력 정상 동작 | ⬜ |
| 표준 편집 | 복사/붙여넣기, Undo/Redo 등 표준 편집 기능 동작 | ⬜ |
| 자동 포커스 | 실행 즉시 TextBox에 포커스, 바로 타이핑 가능 | ⬜ |
| 투명 모드 | 배경 투명 / 전체 투명 모드 전환 동작 | ⬜ |
| 투명도 조절 | 컨텍스트 메뉴에서 투명도 단계별 조절 동작 | ⬜ |
| Always on Top | Topmost 토글 동작 (기본값: 활성화) | ⬜ |
| 컨텍스트 메뉴 | 우클릭 메뉴에서 투명도, 모드 전환, Always on Top, 종료 동작 | ⬜ |
| 메모리 | 유휴 상태 메모리 사용량 80MB 이하 | ⬜ |
