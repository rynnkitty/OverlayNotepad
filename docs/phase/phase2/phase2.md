# Phase 2: 텍스트 테두리/그림자 + 자동 저장 + 윈도우 관리 -- 실행 계획

> **Status**: 계획 수립 완료 (2026-04-17)
> **ROADMAP 참조**: `ROADMAP.md` Phase 2
> **검토 리포트**: `phase2-product-owner-review.md`, `phase2-ux-specialist-review.md`, `phase2-user-review.md`

---

## 개요

Phase 1에서 구축한 WPF 투명 윈도우 + TextBox 기반 위에 텍스트 가독성(테두리/그림자), 데이터 안전성(자동 저장), 사용 편의성(윈도우 관리)을 추가한다.

```
Phase 1 (선행 조건)
  WPF 투명 윈도우 + TextBox + 투명도 모드 + Always on Top + 컨텍스트 메뉴 골격
    |
    v
Phase 2 Sprint 1: 텍스트 테두리/그림자 효과
  OutlinedTextControl (FormattedText + Geometry)
  DropShadowEffect
  테두리/그림자 토글
    |
    v
Phase 2 Sprint 2: 설정 관리 + 자동 저장 + 윈도우 관리
  SettingsManager (settings.json)
  AutoSaveManager (DispatcherTimer + 디바운스)
  윈도우 위치/크기 저장 및 복원
```

### 핵심 기능 매핑

| PRD 기능 | 우선순위 | Sprint | 설명 |
|----------|---------|--------|------|
| F3. 텍스트 테두리 및 그림자 | P0 필수 | Sprint 1 | FormattedText + Geometry 아웃라인, DropShadowEffect |
| F7. 자동 저장 | P1 높음 | Sprint 2 | 2초 디바운스, 비정상 종료 대응 |
| F8. 윈도우 관리 | P1 높음 | Sprint 2 | 위치/크기 저장, 화면 범위 보정 |

---

## 검토팀 확정 파라미터 (2026-04-17)

> 이기획(PO), 한유엑(UX), 김수진(사용자 관점) 검토 완료

| 항목 | 원래 설계 | 확정값 | 근거 |
|------|----------|--------|------|
| 테두리 두께 기본값 | 미정 | 1.0px | PO: 가독성 확보 최소치, UX: 0.5~3.0px 범위 내 기본값 |
| 테두리 두께 범위 | 미정 | 0.5~3.0px (0.5px 단위) | UX: 4K 디스플레이 대응, 세밀 조절 필요 |
| 그림자 offset (X, Y) | 미정 | 1.5px, 1.5px | UX: 자연스러운 방향, 과도한 offset은 이중 텍스트 효과 |
| 그림자 blur | 미정 | 5px | PO: 가독성/성능 균형, UX: 3~5px 범위에서 5px 채택 |
| 그림자 opacity | 미정 | 0.8 | 가독성 우선, 너무 투명하면 효과 미미 |
| 테두리 색상 기본값 | 미정 | #000000 (다크 테마 기준) | PRD 설정 항목 기준 |
| 테두리 기본 상태 | 활성화 | 활성화 | PRD 명시, 전문가 전원 동의 |
| 자동 저장 디바운스 | 2초 | 2초 | PRD 명시값, 전문가 전원 동의 |
| 기본 윈도우 크기 | 400x300 | 400x300 | PRD 명시값 |
| 최소 윈도우 크기 | 미정 | 200x150 | UX: 이보다 작으면 텍스트 편집 비실용적 |
| 설정 파일 경로 | %AppData%/OverlayNotepad/ | %AppData%/OverlayNotepad/ | PRD 명시 |
| 설정 파일명 | settings.json | settings.json | PRD 명시 |
| 메모 파일명 | 미정 | memo.txt | 설정과 분리하여 별도 저장 (손상 시 독립 복원) |
| 손상 설정 백업 | 미정 | settings.json.bak | UX: 손상 파일 백업 후 기본값 복원 |

---

## Sprint 분할 계획

| Sprint | 주제 | 주요 작업 | 의존성 |
|--------|------|----------|--------|
| 1 | 텍스트 테두리 및 그림자 효과 | OutlinedTextControl, DropShadowEffect, 토글 로직 | Phase 1 완료 (TextBox + 투명 윈도우) |
| 2 | 설정 관리 + 자동 저장 + 윈도우 관리 | SettingsManager, AutoSaveManager, 위치/크기 저장 | Sprint 1 (테두리 설정값 포함) |

---

## Sprint 1 상세 -- 텍스트 테두리 및 그림자 효과

### 목표
투명 배경 위에서 어떤 배경(밝은/어두운)이든 텍스트를 읽을 수 있도록 아웃라인 테두리와 그림자 효과를 구현한다.

### 아키텍처 설계

```
MainWindow
  |
  +-- DragBar (상단, Phase 1에서 구현)
  |
  +-- Grid (텍스트 영역)
       |
       +-- OutlinedTextControl (테두리 ON 시 표시, 렌더링 전용)
       |     - FormattedText로 텍스트 Geometry 생성
       |     - Geometry.GetWidenedPathGeometry()로 아웃라인 생성
       |     - DrawGeometry()로 아웃라인 + 채움 렌더링
       |
       +-- TextBox (항상 존재, 입력 담당)
       |     - 테두리 ON: 글자색 투명 (입력만 담당, 표시는 OutlinedTextControl)
       |     - 테두리 OFF: 글자색 활성 (일반 TextBox로 동작)
       |     - DropShadowEffect 적용 (테두리 ON/OFF 무관)
       |
       +-- 동기화: TextBox.Text <-> OutlinedTextControl.Text 바인딩
```

**핵심 설계 결정: 이중 레이어 구조**
- TextBox는 항상 입력을 담당 (커서, 선택, IME, Undo/Redo 보존)
- OutlinedTextControl은 테두리 효과를 가진 텍스트를 렌더링만 담당
- 테두리 ON 시: TextBox 글자를 투명으로 만들고 OutlinedTextControl이 같은 위치에 렌더링
- 테두리 OFF 시: OutlinedTextControl 숨기고 TextBox가 직접 표시
- 이 구조로 PO/UX 리뷰에서 지적된 "편집 기능 호환성" 문제를 해결

### 백엔드 (코드비하인드)

| 파일 | 유형 | 내용 |
|------|------|------|
| `Controls/OutlinedTextControl.cs` | 신규 | FormattedText + Geometry 기반 커스텀 UserControl. OnRender()에서 아웃라인 렌더링 |
| `Models/TextEffectSettings.cs` | 신규 | 테두리/그림자 설정 모델 (두께, 색상, blur, offset, 활성화 여부) |
| `MainWindow.xaml.cs` | 수정 | 테두리 토글 로직, TextBox-OutlinedTextControl 동기화 |

### 프론트엔드 (XAML)

| 파일 | 유형 | 내용 |
|------|------|------|
| `Controls/OutlinedTextControl.xaml` | 신규 | 커스텀 컨트롤 XAML (크기/위치 바인딩) |
| `MainWindow.xaml` | 수정 | OutlinedTextControl 배치, DropShadowEffect, 컨텍스트 메뉴에 테두리 토글 추가 |

### 주요 구현 사항

1. **OutlinedTextControl 렌더링 파이프라인**
   - `FormattedText` 생성 (글꼴, 크기, 텍스트 내용)
   - `BuildGeometry()` -> 텍스트의 Geometry 획득
   - `Pen`으로 아웃라인 그리기 (두께: 확정값 1.0px, 색상: #000000)
   - `DrawingContext.DrawGeometry(fillBrush, outlinePen, geometry)`

2. **DropShadowEffect 적용**
   - TextBox의 `Effect` 속성에 DropShadowEffect 설정
   - ShadowDepth: 1.5, BlurRadius: 5, Opacity: 0.8, Color: Black
   - 테두리 ON/OFF와 독립적으로 동작

3. **텍스트 동기화**
   - TextBox.TextChanged -> OutlinedTextControl.Text 업데이트
   - TextBox.ScrollChanged -> OutlinedTextControl 스크롤 동기화
   - TextBox.FontFamily/FontSize 변경 -> OutlinedTextControl 반영

4. **토글 로직**
   - 컨텍스트 메뉴 "텍스트 테두리" 체크박스
   - ON: OutlinedTextControl.Visibility = Visible, TextBox.Foreground = Transparent
   - OFF: OutlinedTextControl.Visibility = Collapsed, TextBox.Foreground = 설정 색상

### 폴백 계획
FormattedText + Geometry 방식이 성능 문제나 편집 동기화 문제를 일으킬 경우:
- **폴백 A**: DropShadowEffect만 사용 (그림자만으로 가독성 80% 이상 확보)
- **폴백 B**: 다중 DropShadowEffect (4방향 그림자로 테두리 시뮬레이션)

---

## Sprint 2 상세 -- 설정 관리 + 자동 저장 + 윈도우 관리

### 목표
사용자 설정을 JSON 파일로 영속화하고, 텍스트를 자동 저장하며, 윈도우 위치/크기를 복원한다.

### 아키텍처 설계

```
SettingsManager (싱글톤)
  |
  +-- AppSettings 모델
  |     - 윈도우: Left, Top, Width, Height
  |     - 투명도: Opacity, TransparencyMode
  |     - 텍스트 효과: OutlineEnabled, OutlineThickness, OutlineColor, ShadowEnabled
  |     - 서식: FontFamily, FontSize, ForegroundColor (Phase 3에서 UI 추가)
  |     - 테마: Theme (Phase 3에서 구현)
  |     - Always on Top: Topmost
  |
  +-- Load() -> JSON 역직렬화, 실패 시 .bak 백업 + 기본값
  +-- Save() -> JSON 직렬화, 원자적 쓰기 (temp -> rename)
  |
AutoSaveManager
  |
  +-- DispatcherTimer (100ms 간격 체크)
  +-- 디바운스 로직: 마지막 변경 후 2초 경과 시 저장
  +-- 저장 대상: memo.txt (텍스트 내용) + settings.json (설정)
  |
MainWindow
  +-- Loaded: SettingsManager.Load() -> 윈도우/설정 복원
  +-- Closing: 즉시 저장 (디바운스 무시)
  +-- AppDomain.UnhandledException: 긴급 저장
```

### 백엔드 (코드비하인드)

| 파일 | 유형 | 내용 |
|------|------|------|
| `Models/AppSettings.cs` | 신규 | 전체 설정 모델 클래스. 기본값 포함 |
| `Services/SettingsManager.cs` | 신규 | JSON 직렬화/역직렬화, 원자적 파일 쓰기, 손상 복원 |
| `Services/AutoSaveManager.cs` | 신규 | DispatcherTimer 기반 디바운스 자동 저장 |
| `MainWindow.xaml.cs` | 수정 | 설정 로드/저장 연동, 윈도우 위치/크기 복원, 비정상 종료 핸들러 |
| `App.xaml.cs` | 수정 | AppDomain.UnhandledException 등록, 전역 예외 처리 |

### 프론트엔드 (XAML)

| 파일 | 유형 | 내용 |
|------|------|------|
| `MainWindow.xaml` | 수정 | MinWidth/MinHeight 설정 (200x150) |

### 주요 구현 사항

1. **설정 파일 구조 (settings.json)**
   ```json
   {
     "window": {
       "left": 100,
       "top": 100,
       "width": 400,
       "height": 300
     },
     "transparency": {
       "opacity": 0.8,
       "mode": "background"
     },
     "textEffect": {
       "outlineEnabled": true,
       "outlineThickness": 1.0,
       "outlineColor": "#000000",
       "shadowEnabled": true,
       "shadowBlur": 5,
       "shadowOffset": 1.5,
       "shadowOpacity": 0.8
     },
     "font": {
       "family": "맑은 고딕",
       "size": 14,
       "color": "#FFFFFF"
     },
     "topmost": true,
     "theme": "dark"
   }
   ```

2. **메모 저장 (memo.txt)**
   - 설정(settings.json)과 메모 내용(memo.txt)을 분리
   - 한쪽 파일 손상이 다른 쪽에 영향 없음
   - 텍스트 인코딩: UTF-8 (BOM 없음)

3. **원자적 파일 쓰기**
   - 임시 파일에 먼저 쓰기 -> File.Move()로 덮어쓰기
   - 쓰기 도중 크래시 시 원본 파일 보존

4. **설정 손상 복원**
   - JSON 파싱 실패 시: 현재 파일을 `.bak`으로 복사 -> 기본값으로 새 파일 생성
   - 파일 없음: 기본값으로 새 파일 생성
   - `.bak` 파일은 최대 1개만 유지 (이전 .bak 덮어쓰기)

5. **자동 저장 디바운스 로직**
   ```
   TextBox.TextChanged 발생
     -> _lastChangeTime = DateTime.Now
     -> _isDirty = true

   DispatcherTimer (100ms마다)
     -> if (_isDirty && DateTime.Now - _lastChangeTime >= 2초)
       -> SaveMemo()
       -> _isDirty = false
   ```

6. **윈도우 위치/크기 복원**
   - 저장: Window.Closing 시 Left, Top, Width, Height 저장
   - 복원: Window.Loaded 시 저장값 적용
   - 화면 범위 보정: SystemParameters.VirtualScreenLeft/Top/Width/Height와 비교
   - 범위 밖이면 기본 위치(화면 중앙)로 복원

7. **비정상 종료 대응**
   - `AppDomain.CurrentDomain.UnhandledException` 핸들러
   - `Application.Current.DispatcherUnhandledException` 핸들러
   - 두 핸들러 모두에서 즉시 저장 (디바운스 무시)

### 재사용 자산 (Phase 1에서 가져옴)

| 자산 | 출처 | 용도 |
|------|------|------|
| MainWindow.xaml 레이아웃 | Phase 1 Sprint 1 | 텍스트 영역에 OutlinedTextControl 추가 |
| 컨텍스트 메뉴 골격 | Phase 1 Sprint 2 | 테두리 토글 항목 추가 |
| 투명도 모드 로직 | Phase 1 Sprint 2 | 설정값으로 영속화 |
| Always on Top 로직 | Phase 1 Sprint 2 | 설정값으로 영속화 |

---

## JSON 직렬화 기술 선택

| 선택지 | 장점 | 단점 | 결정 |
|--------|------|------|------|
| System.Text.Json | .NET 표준, NuGet 불필요 | .NET Framework 4.8에 미포함 (NuGet 필요) | X |
| Newtonsoft.Json | 가장 널리 사용 | NuGet 의존성 추가 | X |
| JavaScriptSerializer | .NET Framework 내장 | 기능 제한, deprecated 성격 | X |
| DataContractJsonSerializer | .NET Framework 내장 | 설정 번거로움 | X |
| **수동 파싱** | NuGet 불필요, 완전 제어 | 구현 비용 | **채택** |

**결정: 수동 JSON 파싱/생성**
- PRD "외부 라이브러리 NuGet 의존성 최소화" 원칙 준수
- settings.json 구조가 단순하고 고정적이므로 수동 파싱 가능
- `System.Runtime.Serialization.Json.DataContractJsonSerializer`를 1차 시도, 불편하면 간단한 수동 파싱으로 전환
- Sprint 계획 시 구체적 방식 확정

---

## 미해결 사항 / 리스크

### 기술 리스크

| 리스크 | 영향 | 완화 방안 | Sprint |
|--------|------|----------|--------|
| FormattedText 렌더링과 TextBox 동기화 복잡도 | 중간 | 이중 레이어 구조로 입력/표시 분리, 폴백으로 DropShadowEffect만 사용 | Sprint 1 |
| 스크롤 동기화 | 낮음 | TextBox.ScrollChanged 이벤트로 OutlinedTextControl 오프셋 동기화 | Sprint 1 |
| 원자적 파일 쓰기 실패 (디스크 용량) | 낮음 | try-catch로 저장 실패 감지, 내부 로그 기록 | Sprint 2 |
| 다중 모니터 DPI 차이 | 낮음 | Phase 2에서는 처리하지 않음 (PRD: "별도 멀티 모니터 DPI 처리 없음") | 제외 |

### UX 리스크 (전문가 검토 반영)

| 리스크 | 출처 | 완화 방안 |
|--------|------|----------|
| 테두리 ON/OFF 전환 시 시각적 불연속 | UX 검토 | TextBox 항상 유지 + 오버레이 구조로 해결 |
| 설정 파일 손상 시 사용자 인지 부재 | UX 검토 | .bak 백업 + 기본값 복원, Phase 3에서 알림 UI 추가 |
| 자동 저장 실패 시 피드백 없음 | 사용자 검토 | 내부 로그 기록, Phase 3 컨텍스트 메뉴 상태 표시로 해결 |

---

## 완료 기준 (Phase 전체)

| 항목 | 기준 | 상태 |
|------|------|------|
| 텍스트 테두리 효과 | 밝은/어두운 배경 위에서 텍스트 가독성 확보 | ⬜ |
| 그림자 효과 | DropShadowEffect 적용, 가독성 보조 | ⬜ |
| 테두리 토글 | 컨텍스트 메뉴에서 ON/OFF 전환 동작 | ⬜ |
| 토글 시 편집 연속성 | 텍스트 내용/커서 위치 보존 | ⬜ |
| 자동 저장 | 텍스트 변경 후 2초 뒤 자동 저장 | ⬜ |
| 데이터 복원 | 프로그램 재실행 시 이전 텍스트 복원 | ⬜ |
| 비정상 종료 대응 | 프로세스 kill 후 마지막 저장 데이터 복원 | ⬜ |
| 설정 기본값 복원 | settings.json 삭제/손상 시 기본값으로 정상 시작 | ⬜ |
| 윈도우 위치/크기 복원 | 다음 실행 시 이전 위치/크기 유지 | ⬜ |
| 화면 범위 보정 | 모니터 분리 후 재실행 시 화면 내 복원 | ⬜ |
| 최소 윈도우 크기 | 200x150 이하로 축소 불가 | ⬜ |
