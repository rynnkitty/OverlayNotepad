# 프롬프트 가이드

> 이 문서는 **사용자 참고용**입니다. Claude Code가 자동으로 읽지 않습니다.
> 바이브코딩이 처음이라면 "시작하기 전에" 섹션부터 읽어주세요.

---

## 시작하기 전에

### 바이브코딩이란?

자연어로 AI에게 개발 작업을 지시하는 방식입니다. 코드를 직접 쓰지 않아도 됩니다.
이 프로젝트에서는 **Claude Code**를 사용하며, 터미널에서 `claude` 명령으로 실행합니다.

> 📖 [Claude Code 개요](https://code.claude.com/docs/ko/overview) · [Quickstart](https://code.claude.com/docs/ko/quickstart)

### 용어 정리

| 용어 | 의미 |
|------|------|
| **PRD** | 제품 요구사항 문서. 무엇을 만들지 정의 (`docs/prd.md`) |
| **ROADMAP** | 프로젝트 전체 계획. Phase와 Sprint로 구성 |
| **Phase** | 여러 Sprint에 걸친 대규모 기능 단위. 전문가 검토 포함 |
| **Sprint** | 1~2일 단위의 개발 작업. `sprint{N}.md`가 실행 명세서 |
| **Hotfix** | 프로덕션 긴급 패치. 파일 3개, 50줄 이하 |
| **에이전트** | Claude Code가 특정 역할을 수행하는 자동화 도구 |
| **develop** | 스테이징 브랜치 (로컬 검증용) |
| **main** | 프로덕션 브랜치 (merge 시 자동 배포) |

### 처음 이 프로젝트에 참여할 때

1. 저장소 클론 및 환경 설정: `docs/setup-guide.md` 참조
2. 현재 진행 상황 파악: `ROADMAP.md` 및 `docs/index.json` 확인
3. 진행 중인 Sprint가 있다면: `docs/phase/phase{P}/sprint{N}/sprint{N}.md` 확인

### 에이전트 파이프라인 전체 그림

> 📖 [커스텀 에이전트 생성](https://code.claude.com/docs/ko/sub-agents) · 각 단계를 독립 에이전트로 분리하여 컨텍스트 윈도우를 절약하고, 역할별 모델을 최적화합니다.

```
PRD ──→ prd-to-roadmap ──→ ROADMAP.md
                              │
                     👤 ROADMAP 검토/수정/확정
                              │
                    Phase별로 반복:
                              │
              요구사항 인터뷰 ──→ phase-planner ──→ phase{N}/phase{N}.md
                                  (전문가 5명 병렬 검토)
                                                      │
                                             👤 Phase 문서 검토/파라미터 확정
                                                      │
                                          Sprint별로 반복:
                                                      │
                                        sprint-planner ──→ sprint{N}/sprint{N}.md
                                                              │
                                                     👤 Sprint 계획 검토/승인
                                                              │
                                                        /sprint-dev {P}-{N} ──→ 구현
                                                              │
                                                        sprint-close ──→ develop PR
                                                              │
                                                     👤 PR 검토
                                                              │
                                                        sprint-review ──→ 코드 리뷰 + 검증
                                                              │
                                                     👤 수동 검증 수행
                                                              │
                                                        deploy-prod ──→ main 배포
                                                              │
                                                     👤 PR 머지 + 실서버 확인
```

### 핵심 규칙 3가지

1. **`sprint{N}.md`가 실행의 기준**: 계획 문서에 적힌 대로 Task를 순서대로 구현합니다.
2. **계획과 다르면 물어본다**: Claude가 임의로 변경하지 않고 사용자에게 확인합니다.
3. **검증 명령을 반드시 실행**: 각 Task에 적힌 검증 명령으로 결과를 확인합니다.

### Hook 자동 안전장치

사용자가 신경 쓰지 않아도 **세 가지 hook**이 자동으로 규칙을 강제합니다:

- **bash-guard** (PreToolUse): 위험한 Bash 명령을 즉시 차단합니다.
  - `cd /path && ...` 체이닝, `git push origin main/develop`, `git push --force`, `git reset --hard`, 잘못된 브랜치명

- **index-sync** (PostToolUse): git commit/checkout 시 `docs/index.json`을 자동 업데이트합니다.

- **doc-checker** (Stop): 에이전트가 작업을 끝내려 할 때, 빠뜨린 문서 업데이트를 자동 감지합니다.

> 📖 [Hooks 가이드](https://code.claude.com/docs/ko/hooks-guide) — 규칙 확장: `.claude/hooks/lib/doc-rules.json` 수정

### 모델 선택 가이드

| 단계 | 모델 | 비고 |
|------|------|------|
| 요구사항 인터뷰 / brainstorming | Opus 권장 | 깊은 추론 필요 |
| PRD → ROADMAP / Phase / Sprint 계획 | **Opus** (에이전트 자동) | 에이전트가 처리 |
| **Sprint 구현** | **Sonnet** (기본) | 복잡하면 `/model opus` |
| Sprint 마무리 / 리뷰 / 배포 | **Sonnet** (에이전트 자동) | 에이전트가 처리 |
| Hotfix | **Sonnet** (기본) | 범위가 좁은 수정 |

---

## 어떤 경로를 선택해야 하나?

```
"이거 하고 싶은데" →  규모가 얼마나 큰가?
                    │
                    ├─ 새 기능/시스템 (여러 Sprint) → B. 메이저 업데이트
                    ├─ 파일 몇 개만 수정            → C. 간단한 수정
                    └─ 프로덕션 버그다!             → D. 긴급 버그 수정
```

| 경로 | 기준 | 프로세스 |
|------|------|---------|
| **A. 프로젝트 시작** | PRD가 준비됨 | PRD → ROADMAP → Phase → Sprint |
| **B. 메이저 업데이트** | 신규 기능, 반드시 Phase 경유 | Phase → Sprint → 배포 |
| **C. 간단한 수정** | 파일 3개 이하, 50줄 이하 | 바로 수정 |
| **D. 긴급 버그 수정** | 프로덕션 장애 | Hotfix → main 배포 |

---

## A. 프로젝트 시작

### A-1. PRD에서 ROADMAP 생성

```
docs/prd.md 작성했어. ROADMAP 만들어줘.
```

### A-2. ROADMAP 검토/수정

```
ROADMAP 확인했어.
- Phase 2의 청구 연동 Sprint 범위를 조정해줘. EDI 연동은 Phase 3으로 옮기자.
- Phase 1 완료 기준에 "접수 화면 응답 2초 이내" 추가해줘.
```

### A-3. 첫 Phase 시작

```
ROADMAP 확정. Phase 1부터 시작하자.
```

> 이후 B 경로와 동일하게 진행

---

## B. 메이저 업데이트 (Phase → Sprint)

> **모델**: 인터뷰 시 `/model opus` 권장, 구현은 Sonnet(기본)으로 충분

### B-1. 요구사항 인터뷰

무엇을 만들고 싶은지 설명합니다. 구체적일수록 좋습니다.

```
처방 입력 화면을 새로 만들고 싶어.
- 약품 즐겨찾기 및 세트 처방 기능
- 상병코드 자동완성
- DUR 체크 연동
- Tab/Enter 키보드 중심 조작
```

```
건강보험 청구 EDI 연동 기능을 추가하고 싶어.
- 심평원 EDI 파일 생성 및 전송
- 청구 결과 수신 및 상태 추적
- 삭감 내역 조회
```

> brainstorming이 시작됩니다. 목표, 제약조건, 기존 기능 관계 등을 인터뷰합니다.

**대화 중 유용한 표현들:**

```
나는 심평원 청구 규격을 잘 몰라서 전문가 의견이 필요해.
```

```
기존 처방 기능과 충돌하는 부분이 있을까?
```

```
이 연동에 리스크가 있을까? 실패 처리는 어떻게 해야 해?
```

### B-2. Phase 문서 생성

```
요구사항 정리됐어. Phase 문서 만들어줘.
```

> phase-planner 에이전트가:
> 1. ROADMAP + 코드베이스 분석
> 2. 도메인 전문가 3~5명 병렬 검토 (프로젝트 PO, 행정 전문가, 진료의, 인터페이스 전문가, UX 전문가)
> 3. 검토 결과 통합 (파라미터 충돌 시 보수적 방향 채택)
> 4. `docs/phase/phase{N}/phase{N}.md` + 전문가 리뷰 파일 생성

### B-3. Phase 문서 검토/수정

> **"검토팀 확정 파라미터"** 섹션이 핵심입니다. 전문가가 검토한 값이므로 모르는 파라미터는 그대로 두어도 됩니다.

```
docs/phase/phase1/phase1.md 확인했어.
- DUR 체크 타임아웃을 3초 → 5초로 변경해줘.
- Sprint 분할은 좋은데, Sprint 3에 키보드 단축키 설계도 넣어줘.
- 나머지는 확정.
```

### B-4. Sprint 계획

```
phase1의 sprint 1 계획 세워줘.
```

### B-5. Sprint 계획 검토 후 구현

```
docs/phase/phase1/sprint1/sprint1.md 검토 완료. /sprint-dev 1-1
```

수정이 필요하면:

```
docs/phase/phase1/sprint1/sprint1.md 확인했어.
- Task 3의 약품 검색에 성분명 검색도 추가해줘.
- Task 6은 다음 Sprint으로 넘기자.
수정 후 /sprint-dev 1-1
```

### B-6. Sprint 마무리

```
phase1-sprint1 구현 끝났어. 마무리 해줘.
```

### B-7. Sprint 리뷰

PR을 검토한 후:

```
PR 확인했어. 스프린트 리뷰 해줘.
```

Critical 이슈가 발견되면:

```
수정 완료. 다시 스프린트 리뷰 해줘.
```

### B-8. 다음 Sprint 계속

```
phase1의 sprint 2 계획 세워줘.
```

### B-9. Phase 완료 후 배포

```
develop 검증 완료됐어. 프로덕션 배포 해줘.
```

---

## C. 간단한 수정

> 파일 3개/50줄 이하 + DB 변경 없음 + 새 의존성 없음이면 Phase/Sprint 없이 바로 수정합니다.

```
접수 화면에서 환자 이름 검색 시 초성 검색이 안 돼. 수정해줘.
```

```
처방전 출력 시 의원 도장 이미지가 잘려. 확인해줘.
```

```
수납 화면의 본인부담금 계산이 비급여 항목을 누락하고 있어. 수정해줘.
```

수정 후:

```
수정 확인했어. 커밋해줘.
```

---

## D. 긴급 버그 수정 (Hotfix)

> **모델: Sonnet** — 빠른 대응이 중요

### D-1. 증상 설명

```
프로덕션에서 청구 EDI 전송이 실패하고 있어. 긴급 수정해줘.
```

```
접수 화면이 특정 환자에서 오류 나고 있어. 빨리 확인해줘.
```

```
오늘 심평원 수가 코드가 바뀌었는데 처방 화면에서 못 찾겠다는 민원이 왔어.
```

> Hotfix vs Sprint 자동 판단 → hotfix 브랜치 생성 → 수정

### D-2. Hotfix 마무리

```
hotfix 구현 끝났어. 마무리해줘.
```

> hotfix-close 에이전트 → main PR, 경량 리뷰, 타겟 검증, develop 역머지

---

## F. 구현 중 유용한 프롬프트

### 중간부터 재개

```
docs/phase/phase1/sprint1/sprint1.md의 Task 4부터 이어서 구현해줘. Task 1~3은 완료됐어.
```

### 계획 변경이 필요할 때

```
Task 3 구현하다 보니 LIS 응답 포맷이 명세와 달라.
Task 3을 수정하고 이어서 진행해줘.
```

### 막혔을 때

```
Task 4에서 심평원 EDI 응답 파싱이 안 돼.
실제 응답은 이거야: {...}
어떻게 처리할지 같이 봐줘.
```

### 구현 도중 다른 버그 발견

```
Task 5 구현 중에 기존 처방 이력 조회에서 버그 발견했어.
이건 나중에 별도로 고치자. 일단 phase1-sprint1 계속 진행해.
```

### 병렬 실행

```
docs/phase/phase1/sprint2/sprint2.md의 Phase 2를 팀으로 병렬 실행해줘.
Task 2는 백엔드(DB/API), Task 3은 프론트엔드(화면)로 분리 가능해.
```

---

## G. 배포 관련

### 프로덕션 배포

```
develop 검증 완료됐어. 프로덕션 배포 해줘.
```

### 배포 후 문제 발생

```
방금 배포했는데 헬스체크가 실패해. 확인해줘.
```

```
롤백해야 할 것 같아. 이전 버전으로 돌려줘.
```

---

## H. 문서/관리

### Notion 업데이트

```
이번 Sprint에서 처방 DB 스키마 변경했으니 Notion 데이터 모델 업데이트해줘.
```

```
Phase 1 완료됐어. Notion 릴리즈 노트 업데이트해줘.
```

### 현재 상태 확인

```
지금 프로젝트 어디까지 진행됐어?
```

```
Phase 1 남은 작업이 뭐야?
```

### 컨텍스트 감사

```
/context-audit
```

---

## 팁

### 좋은 프롬프트의 공통점

1. **목적을 먼저 말한다**: "처방 입력을 빠르게 하고 싶어" → 왜 하는지 맥락 제공
2. **제약조건을 같이 말한다**: "단, 기존 세트처방 데이터가 유지되어야 해"
3. **기존 기능을 언급한다**: "기존 접수 화면처럼"
4. **모르는 건 모른다고 한다**: "EDI 규격은 잘 몰라서 전문가 의견 필요해"

### Claude가 엉뚱하게 갈 때

```
아니야, 그게 아니라 [원래 의도 설명]. sprint{N}.md 다시 읽고 Task 3부터 해줘.
```

```
지금 하는 건 sprint{N}.md 범위 밖이야. Task 목록에 집중해줘.
```

### 세션이 길어져서 컨텍스트가 부족할 때

```
sprint{N}.md 다시 읽고 현재 진행 상황 정리해줘.
```

```
새 세션이야. docs/phase/phase1/sprint1/sprint1.md의 Task 5부터 이어서 구현해줘.
```

---

## 참고 문서

### 프로젝트 내부

| 문서 | 용도 |
|------|------|
| `CLAUDE.md` | 프로젝트 규칙, 에이전트 라우팅 (매 세션 자동 로드) |
| `docs/dev-process.md` | 검증 매트릭스, 코드 리뷰 체크리스트, 문서 관리 규칙 |
| `docs/ci-policy.md` | Git 브랜치 전략, CI/CD 파이프라인 |
| `docs/setup-guide.md` | 초기 환경 설정 |
| `.claude/agents/*.md` | 8개 에이전트 정의 |
| `.claude/rules/*.md` | 경로별 조건부 규칙 |
| `.claude/hooks/lib/doc-rules.json` | 에이전트별 필수 업데이트 규칙 |

### Claude Code 공식 문서

| 주제 | URL |
|------|-----|
| 개요 | https://code.claude.com/docs/ko/overview |
| 커스텀 에이전트 | https://code.claude.com/docs/ko/sub-agents |
| Hooks | https://code.claude.com/docs/ko/hooks-guide |
| CLAUDE.md | https://code.claude.com/docs/ko/memory |
| 에이전트 팀 | https://code.claude.com/docs/ko/agent-teams |
