# AI-Native 예제 Project

> AI-Native 개발 방법론 예제 — Claude Code 에이전트 및 개발 워크플로우 설정

---

## 📁 프로젝트 구조

```
choiji-guide-big/
├── .claude/
│   ├── agents/
│   │   ├── prd-to-roadmap.md          # PRD → ROADMAP 생성
│   │   ├── phase-planner.md           # Phase 계획 + 전문가 검토
│   │   ├── sprint-planner.md          # Sprint 실행 명세서 생성
│   │   ├── sprint-close.md            # Sprint 마무리 (PR + 문서)
│   │   ├── sprint-review.md           # 코드 리뷰 + 자동 검증
│   │   ├── sprint-pr-fix.md           # PR 이슈 수정 + 재리뷰
│   │   ├── hotfix-close.md            # 핫픽스 마무리
│   │   └── deploy-prod.md             # 프로덕션 배포
│   ├── rules/
│   │   ├── backend.md                 # 백엔드 코딩 규칙
│   │   ├── frontend.md                # 프론트엔드 코딩 규칙
│   │   ├── sprint-workflow.md         # 스프린트/핫픽스 워크플로우 규칙
│   │   └── notion.md                  # Notion 기술 문서 관리
│   ├── commands/
│   │   ├── sprint-dev.md              # Sprint 구현 오케스트레이터
│   │   ├── restart.md                 # 서비스 재시작
│   │   ├── dashboard.md               # 프로젝트 대시보드
│   │   └── context-audit.md           # 컨텍스트 자산 감사
│   ├── agent-memory/
│   │   ├── prd-to-roadmap/
│   │   ├── phase-planner/
│   │   ├── sprint-planner/
│   │   ├── sprint-close/
│   │   ├── sprint-review/
│   │   ├── sprint-pr-fix/
│   │   ├── hotfix-close/
│   │   └── deploy-prod/
│   ├── hooks/
│   │   ├── pretooluse-bash-guard.sh   # Bash 위험 명령 차단
│   │   ├── posttooluse-index-sync.sh  # git commit/checkout 시 index.json 자동 동기화
│   │   ├── stop-doc-checker.sh        # 에이전트 문서 누락 검증
│   │   ├── lib/
│   │   │   ├── doc-rules.json         # 에이전트별 필수 업데이트 규칙
│   │   │   └── audit-rules.json       # context-audit 감사 규칙
│   │   └── test-hooks.sh
│   └── settings.json
├── docs/
│   ├── prd.md                         # 제품 요구사항 문서
│   ├── dev-process.md                 # 개발 프로세스 가이드 (SSOT)
│   ├── ci-policy.md                   # CI/CD 정책
│   ├── setup-guide.md                 # 환경 설정 가이드
│   ├── prompt-guide.md                # 사용자 프롬프트 가이드
│   ├── experts/                       ← 도메인 전문가 프로필 (phase-planner 활용)
│   │   ├── product-owner.md           # 프로젝트 PO
│   │   ├── clinic-admin.md            # 1차의원 행정 전문가
│   │   ├── clinic-physician.md        # 1차의원 진료의
│   │   ├── interface-specialist.md    # 인터페이스 전문가
│   │   └── ux-specialist.md           # Windows Application UX 전문가
│   ├── templates/                     ← 문서 작성 템플릿
│   │   ├── EXAMPLE-prd.md
│   │   ├── EXAMPLE-phase.md
│   │   ├── EXAMPLE-sprint.md
│   │   ├── EXAMPLE-task.md
│   │   ├── EXAMPLE-test-plan.md
│   │   ├── EXAMPLE-test-result.md
│   │   └── EXAMPLE-hotfix.md
│   ├── index.json                     ← 프로젝트 진행 상황 (에이전트가 관리)
│   ├── phase/                         ← Phase/Sprint 문서
│   ├── hotfix/                        ← Hotfix 문서
│   ├── dashboard/                     ← /dashboard 명령 UI
│   └── deploy-history/                ← 배포 기록 아카이브
├── CLAUDE.md
├── ROADMAP.md
└── deploy.md
```

---

## 🤖 Claude 에이전트

8개의 특화 에이전트가 개발 라이프사이클 전체를 커버합니다.

### 1. prd-to-roadmap
**트리거**: PRD 작성 완료 후 ROADMAP 생성 시

`docs/prd.md`를 분석하여 Phase/Sprint 구조의 `ROADMAP.md`를 생성합니다.

```
사용자: "PRD 작성했어. ROADMAP 만들어줘."
→ prd-to-roadmap 에이전트가 Phase 기반 ROADMAP.md 생성
```

### 2. phase-planner
**트리거**: 여러 Sprint에 걸친 기능 계획 시

코드베이스를 분석하고 **도메인 전문가 3~5명**(프로젝트 PO, 1차의원 행정 전문가, 진료의, 인터페이스 전문가, UX 전문가)의 병렬 검토를 거쳐 Phase 문서를 생성합니다.

```
사용자: "요구사항 정리됐어. Phase 문서 만들어줘."
→ phase-planner 에이전트가 전문가 검토 후 docs/phase/phase{N}/phase{N}.md 생성
```

### 3. sprint-planner
**트리거**: 새 Sprint 계획 수립 시

Phase 문서를 참조하여 Task 단위 실행 명세서(`sprint{N}.md`)를 생성합니다.

```
사용자: "phase1의 sprint 1 계획 세워줘."
→ sprint-planner 에이전트가 docs/phase/phase1/sprint1/sprint1.md 생성
```

### 4. sprint-close
**트리거**: Sprint 구현 완료 후

1. ROADMAP.md 상태 업데이트
2. `develop` 브랜치로 PR 생성
3. deploy.md 아카이빙
4. sprint-planner 메모리 업데이트

```
사용자: "sprint 1 구현 끝났어. 마무리 해줘."
→ sprint-close 에이전트가 develop PR 생성 + 문서 정리
```

### 5. sprint-review
**트리거**: sprint-close 후 PR 검토 완료 후

코드 리뷰 + 자동 검증 + deploy.md 결과 기록을 수행합니다.

```
사용자: "PR 확인했어. 스프린트 리뷰 해줘."
→ sprint-review 에이전트가 코드 리뷰 + 검증 수행
```

### 6. sprint-pr-fix
**트리거**: sprint-review에서 이슈 발견 시

이슈별 수정을 안내하고 재리뷰를 실행합니다.

### 7. hotfix-close
**트리거**: 핫픽스 구현 완료 후

`main` 브랜치 PR 생성 → 경량 코드 리뷰 → 타겟 검증 → `develop` 역머지 PR까지 자동 처리합니다.

```
사용자: "hotfix 마무리 해줘."
→ hotfix-close 에이전트가 main PR + develop 역머지
```

### 8. deploy-prod
**트리거**: develop 브랜치 QA 완료 후 프로덕션 배포 시

`develop → main` PR 생성, 사전 점검, 배포 후 실서버 검증을 수행합니다.

```
사용자: "develop 검증 완료됐어. 프로덕션 배포 해줘."
→ deploy-prod 에이전트가 PR 생성 + 실서버 헬스체크
```

---

## 🔒 Hook 시스템

### PreToolUse: bash-guard (위험 명령 차단)

| 차단 패턴 | 이유 |
|----------|------|
| `cd ... &&` 체이닝 | 프로젝트 루트에서 직접 실행 |
| `git push origin main` | PR만 허용 |
| `git push origin develop` | PR만 허용 |
| `git push --force` | 이력 파괴 방지 |
| `git reset --hard` | 변경사항 손실 방지 |
| 잘못된 브랜치명 | `phase{P}-sprint{N}` 또는 `hotfix/*` 형식만 허용 |

### PostToolUse: index-sync (진행 상황 자동 동기화)

git commit/checkout 감지 시 `docs/index.json`을 자동 업데이트합니다.

### Stop: doc-checker (문서 누락 검증)

에이전트가 응답을 마치려 할 때 필수 파일 업데이트 여부를 체크합니다.

| 에이전트 | 필수 업데이트 체크 |
|---------|-----------------|
| prd-to-roadmap | index.json (`project` 필드) |
| phase-planner | ROADMAP, index.json, MEMORY, 전문가 리뷰 파일 ≥1 |
| sprint-planner | ROADMAP, index.json, MEMORY |
| sprint-close | deploy.md, index.json, MEMORY, 아카이빙, 체크박스, PR→develop |
| sprint-review | Critical 미해결 경고 |
| hotfix-close | deploy.md, index.json, 아카이빙, hotfix 문서, PR→main |
| deploy-prod | deploy.md, index.json, 아카이빙 |

> 규칙 추가/변경: `.claude/hooks/lib/doc-rules.json`만 수정하면 됩니다.

---

## 🔄 개발 워크플로우

### Sprint 흐름

```
1. prd-to-roadmap → ROADMAP.md 생성 (최초 1회)
2. phase-planner → docs/phase/phase{N}/phase{N}.md 생성 (Phase 시작 시)
3. sprint-planner → docs/phase/phase{P}/sprint{N}/sprint{N}.md 생성
4. git checkout -b phase{P}-sprint{N}
5. /sprint-dev {P}-{N} → Task 순서대로 구현
6. sprint-close → develop PR + 문서 정리
7. sprint-review → 코드 리뷰 + 검증
8. deploy-prod → main 배포
```

### Hotfix 흐름

```
1. git checkout -b hotfix/{설명} (main 기반)
2. 긴급 수정...
3. hotfix-close → main PR + 타겟 검증 + develop 역머지
```

자세한 내용은 `docs/dev-process.md` 참조.

---

## 📋 슬래시 커맨드

| 커맨드 | 설명 |
|--------|------|
| `/sprint-dev {P}-{N}` | Sprint 구현 오케스트레이터 |
| `/restart [service]` | 서비스 재시작 |
| `/dashboard` | 프로젝트 대시보드 열기 |
| `/context-audit` | 컨텍스트 자산 감사 |

---

## 📚 참고 문서

| 문서 | 용도 |
|------|------|
| `docs/prd.md` | 제품 요구사항 문서 |
| `docs/dev-process.md` | 개발 프로세스 전체 가이드 (SSOT) |
| `docs/ci-policy.md` | CI/CD 정책 |
| `docs/setup-guide.md` | 환경 설정 가이드 |
| `docs/prompt-guide.md` | 사용자 프롬프트 가이드 |
| `ROADMAP.md` | 프로젝트 로드맵 |
| `deploy.md` | 현재 수동 검증 항목 |
