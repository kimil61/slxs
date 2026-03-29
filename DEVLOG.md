# 色狼下山 — 개발 진행 로그

> 매 Day 끝에 업데이트. 새 채팅 시작할 때 이 파일을 프로젝트에 올려두면 맥락 유지됨.

---

## 현재 상태 요약
- **에피소드**: EP01 (WASD + 3인칭 카메라 + 클릭 인터랙션)
- **Unity**: 6000 LTS / URP
- **Input**: New Input System (PlayerControls.inputactions)

---

## 완료된 스크립트 목록

| 스크립트 | 역할 | 상태 |
|---------|------|------|
| PlayerControls.cs | Input Actions 자동생성 (Gameplay 맵) | ✅ 완료 |
| PlayerInputHandler.cs | 입력 수집 (Move/Look/Jump/Interact) | ✅ 완료 |
| PlayerMovement.cs | WASD 이동 + 카메라 기준 회전 + 중력 | ✅ 완료 |
| ThirdPersonCamera.cs | 마우스 오빗 카메라 + 벽 충돌 줌인 | ✅ 완료 (v2 - Lerp 제거) |

---

## Day 1 (3/29) — 프로젝트 세팅 + 이동 + 카메라

### 완료
- URP 프로젝트 생성
- New Input System 세팅 (Gameplay 맵: Move/Look/Jump/Interact)
- WASD 이동 (CharacterController 기반, 카메라 방향 기준)
- 3인칭 오빗 카메라 (직접 구현, Cinemachine 미사용)
- 벽 충돌 시 카메라 자동 줌인
- 카메라 이중보간 버그 수정 (Lerp+Slerp → 즉시배치+LookAt)

### 결정 사항
- 카메라: 직접 구현 (Cinemachine 미사용) → 커스텀 자유도 확보
- 입력: New Input System only
- 물리: CharacterController (Rigidbody 아님)
- sensitivity 기본값 2f (Inspector에서 조절 가능)

### 다음 (Day 2)
- 클릭 인터랙션 (레이캐스트 + 하이라이트)
- ESC 커서 해제/잠금 토글

---

## 기술 부채 / 나중에 할 것
- [ ] Daz3D 캐릭터로 Capsule 교체 (EP06)
- [ ] 애니메이션 컨트롤러 연결 (EP03부터)
- [ ] 카메라 락온 모드 (전투 시스템 때)
