# 色狼下山 — 개발 진행 로그

> 매 Day 끝에 업데이트. 새 채팅 시작할 때 이 파일을 프로젝트에 올려두면 맥락 유지됨.

---

## 현재 상태 요약
- **에피소드**: EP02 (전투 프로토타입 — 약공격/강공격/구르기)
- **Unity**: 6000 LTS / URP
- **Input**: New Input System (PlayerControls.inputactions)

---

## 완료된 스크립트 목록

| 스크립트 | 역할 | 상태 |
|---------|------|------|
| PlayerControls.cs | Input Actions 자동생성 (Gameplay 맵) | ✅ 완료 |
| PlayerInputHandler.cs | 입력 수집 (Move/Look/Jump/Interact) | ✅ 완료 |
| PlayerMovement.cs | WASD 이동 + 카메라 기준 회전 + 중력 + 전투 이동잠금 | ✅ 완료 (v2 - LockMovement 추가) |
| ThirdPersonCamera.cs | 마우스 오빗 카메라 + 벽 충돌 줌인 | ✅ 완료 (v2 - Lerp 제거) |
| CursorManager.cs | ESC 커서 잠금/해제 토글 | ✅ 완료 |
| IInteractable.cs | 상호작용 인터페이스 | ✅ 완료 |
| InteractionSystem.cs | 뷰포트 레이캐스트 + 포커스 관리 | ✅ 완료 |
| SampleInteractable.cs | 하이라이트 테스트용 | ✅ 완료 |
| PlayerCombat.cs | 약공격/강공격(홀드)/구르기 + 이동잠금 연동 | ✅ 완료 |

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

---

## Day 2 (3/30) — 인터랙션 시스템 + 커서 관리 (진행중 미완료)

### 미완료
- CursorManager.cs (ESC 커서 잠금 토글)
- IInteractable.cs (인터페이스)
- InteractionSystem.cs (뷰포트 레이캐스트 + 포커스 관리)
- SampleInteractable.cs (하이라이트 테스트)
- PlayerMovement.cs에서 커서 잠금 로직 제거 (CursorManager로 이관)

---

## Day 3 (4/2) — EP02 전투 프로토타입 시작

### 완료
- Mixamo 캐릭터 + 애니메이션 임포트 (Idle/Run/Slash1/Slash2/HeavyAtk/Roll)
- Animator Controller 세팅 (Speed, Attack, HeavyAttack, Roll 파라미터)
- PlayerCombat.cs 신규 작성 (New Input System 기반)
  - 좌클릭 탭 → 약공격 (Trigger: Attack)
  - 좌클릭 홀드(0.4초+) → 강공격 (Trigger: HeavyAttack)
  - 스페이스 → 구르기 (Trigger: Roll)
  - 전투 중 이동잠금 (PlayerMovement.LockMovement 연동)
  - 커서 풀려있으면 전투 입력 무시 (CursorManager 연동)
- PlayerMovement.cs 수정 — LockMovement()/UnlockMovement() 추가
- PlayerController.cs 삭제 (EP02 가이드 스크립트, 구버전 Input 사용 + PlayerMovement와 중복)

### 이슈 해결
- EP02 가이드 스크립트가 구버전 Input 클래스 사용 → New Input System으로 전환 (Mouse.current / Keyboard.current)
- PlayerController.cs가 Rigidbody 기반이라 기존 CharacterController 아키텍처와 충돌 → 삭제, PlayerMovement에 통합

### 알려진 이슈
- 좌클릭이 Interact(EP01)와 Attack(EP02) 양쪽에 바인딩됨 → 전투 테스트 시 InteractionSystem 비활성화 필요. 추후 게임 상태 머신으로 분리 예정.

### 다음
- Animator 트랜지션 타이밍 미세 조정
- 칼 오브젝트 + Collider (타격 판정)
- 적 캡슐 + 데미지 시스템
- 히트스탑 + 카메라 쉐이크

---

## 기술 부채 / 나중에 할 것
- [ ] 좌클릭 입력 충돌 해소 (Interact vs Attack → 게임 상태 머신)
- [ ] Daz3D 캐릭터로 Capsule 교체 (EP06)
- [ ] 애니메이션 이벤트로 Invoke 타이밍 교체 (현재 하드코딩된 duration)
- [ ] 카메라 락온 모드 (전투 시스템 때)