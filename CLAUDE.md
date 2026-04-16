## 언어 및 커뮤니케이션 규칙

- 기본 응답 언어: 한국어
- 코드 주석/커밋 메시지/문서: 한국어
- 변수명/함수명: 영어

## 주요 명령어

```bash
/sprint-dev {P}-{N}    # Phase P의 Sprint N 구현 실행
/restart [service]     # 서비스 재시작
/dashboard             # 프로젝트 대시보드 열기
/context-audit         # 컨텍스트 자산 감사 (중복/상충/고아 파일 검출)
```

## Bash 명령 실행 규칙

bash-guard hook(`.claude/hooks/pretooluse-bash-guard.sh`)이 자동 차단:
- `cd /path &&` 체이닝, main/develop 직접 push, force push, `git reset --hard`, 비정상 브랜치명

## 개발 프로세스

프로세스 상세는 `docs/dev-process.md` 참조. 스프린트/핫픽스 워크플로우 규칙은 `.claude/rules/sprint-workflow.md` 참조.

### 프로젝트 라이프사이클

```
PRD → prd-to-roadmap → ROADMAP.md (Phase 구조)
  → phase-planner → docs/phase/phase{N}/phase{N}.md (전문가 검토 + 확정 파라미터)
    → sprint-planner → docs/phase/phase{P}/sprint{N}/sprint{N}.md (실행 명세서)
      → 구현 → sprint-close → sprint-review → deploy-prod
```

### 핵심 원칙

- **수정사항 → Hotfix vs Sprint 의사결정 먼저**: `docs/dev-process.md` 섹션 2 기준
- **검증 원칙**: `docs/dev-process.md` 섹션 5 참조
- 배포 후 수동 작업: `deploy.md` 참조 (완료 기록은 `docs/deploy-history/` 아카이브)
- 브랜치/워크플로우 상세 규칙: `.claude/rules/sprint-workflow.md` 참조

## 에이전트 사용 규칙

다음 요청에는 반드시 **Agent 도구**(`subagent_type` 파라미터)로 해당 에이전트를 호출한다. **Skill 도구로 호출하지 않는다** — 이들은 `.claude/agents/` 디렉토리의 커스텀 에이전트이며 스킬이 아니다. 직접 탐색/계획하지 않는다.

| 요청 | Agent subagent_type | 모델 |
|------|---------------------|------|
| PRD → 로드맵 | `prd-to-roadmap` | Opus |
| Phase 계획 | `phase-planner` | Opus |
| 스프린트 계획 | `sprint-planner` | Opus |
| 스프린트 마무리 (PR 생성) | `sprint-close` | Sonnet |
| 스프린트 리뷰 (코드 리뷰 + 검증) | `sprint-review` | Sonnet |
| PR 이슈 수정 + 재리뷰 | `sprint-pr-fix` | Sonnet |
| 프로덕션 배포 | `deploy-prod` | Sonnet |
| 핫픽스 마무리 | `hotfix-close` | Sonnet |

## 경로별 상세 규칙

- 백엔드: `.claude/rules/backend.md`
- 프론트엔드: `.claude/rules/frontend.md`
- 스프린트 워크플로우: `.claude/rules/sprint-workflow.md`
- Notion 문서 관리: `.claude/rules/notion.md` (페이지 ID는 Notion 설정 후 채울 것)

## 하네스 피드백 수집

- **피드백 파일**: `.claude/feedback.md` — skill/agent/hook/rule 개선 백로그
- 세션 중 "피드백 기록해줘: ~~" 라고 하면 해당 파일 `미반영` 섹션에 추가
- 반영은 사용자가 수동으로 진행, 반영 후 `반영 완료` 섹션으로 이동

## 훅 시스템

- **PreToolUse (bash-guard)**: 위험 명령 차단 (force push, hard reset, 잘못된 브랜치명 등)
- **PostToolUse (index-sync)**: git checkout/commit 감지 시 `docs/index.json` 자동 동기화
- **Stop (doc-checker)**: 에이전트 완료 전 필수 파일 업데이트 검증
- 검증 규칙 상세: `.claude/hooks/lib/doc-rules.json`

## 체크리스트 작성 형식

- 완료 항목: `- ✅ 항목 내용`
- 미완료 항목: `- ⬜ 항목 내용`
- GFM `[x]`/`[ ]` 대신 이모지 사용

## Notion 기술 문서 관리

업데이트 트리거: `docs/dev-process.md` 섹션 8.5 참조. 상세 규칙: `.claude/rules/notion.md`.

## UI 작성 규칙
- **Designer.cs 분리 필수**: 모든 폼은 `FormName.cs`와 `FormName.Designer.cs`로 분리하여 작성합니다.
- UI 코드는 반드시 `Designer.cs`의 `InitializeComponent()` 메서드 내에 작성합니다.
- `Designer.cs` 파일 상단에 `partial class` 선언을 포함합니다.
### Designer.cs 절대 금지 사항 (WinForms Designer 호환성)
> **이 규칙을 위반하면 Visual Studio Designer에서 폼을 열 수 없게 됩니다.**
1. **`InitializeComponent()` 내 람다 표현식 금지**
   - ❌ `this.btn.Click += (s, e) => { ... };`
   - ✅ `this.btn.Click += new System.EventHandler(this.btn_Click);`
   - 이유: Designer가 `InitializeComponent()`를 파싱할 때 람다를 처리하지 못함

2. **`#region Windows Form Designer generated code` 블록 내 헬퍼 메서드 정의 금지**
   - ❌ `InitializeComponent()` 또는 `#region` 블록 내에 `private void AddColumn(...)` 등 커스텀 메서드 정의
   - ✅ 헬퍼 메서드는 `#endregion` 아래(partial class 내부)에 정의
   - 이유: Designer가 `#region` 내부 메서드를 `Form` 기본 클래스 메서드로 탐색하여 `'Form.AddColumn' 메서드를 찾을 수 없습니다` 오류 발생

3. **헬퍼 메서드 배치 패턴** (컬럼 추가, GroupBox 설정 등)
   ```csharp
   #endregion

   // Designer에서 생성한 코드와 분리된 헬퍼 메서드 (Designer가 재생성하지 않음)
   private DataGridViewTextBoxColumn MakeColumn(string name, ...) { return new ...; }
   private void SetupGroupBox(GroupBox grp, ...) { ... }
   ```
   - `InitializeComponent()` 내에서는 `this.dgv.Columns.Add(MakeColumn(...));` 형태로 호출

4. **이벤트 핸들러 등록 형식**
   - ❌ `this.timer.Tick += (s, e) => Refresh();`
   - ✅ `this.timer.Tick += new System.EventHandler(this.timer_Tick);`
   - 마우스 이벤트: `new System.Windows.Forms.MouseEventHandler(this.ctrl_MouseDown)`
