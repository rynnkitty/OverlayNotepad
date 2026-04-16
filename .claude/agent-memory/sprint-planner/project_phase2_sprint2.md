---
name: Phase 2 Sprint 2 계획
description: Phase 2 Sprint 2 - 설정 관리 + 자동 저장 + 윈도우 관리 계획 완료 (2026-04-17)
type: project
---

Phase 2 Sprint 2 계획 수립 완료 (2026-04-17).

**Why:** Phase 2의 두 번째 Sprint로, 사용자 설정 영속화와 자동 저장으로 데이터 안전성 확보.

**How to apply:**
- 5개 Task (순차 실행): AppSettings 모델 -> SettingsManager -> AutoSaveManager -> 윈도우 위치/크기 -> 비정상 종료 대응
- 핵심 파일: Models/AppSettings.cs, Helpers/JsonHelper.cs, Services/SettingsManager.cs, Services/AutoSaveManager.cs, Helpers/ScreenHelper.cs
- JSON 직렬화: DataContractJsonSerializer (NuGet 불필요) 또는 수동 파싱
- 설정 경로: %AppData%/OverlayNotepad/ (settings.json + memo.txt)
- 원자적 파일 쓰기 (temp -> rename)
- Sprint 1 (텍스트 테두리/그림자)이 선행 조건
