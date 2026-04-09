# 色狼下山 — 개발 진행 로그

> 매 Day 끝에 업데이트. 새 채팅 시작할 때 이 파일을 프로젝트에 올려두면 맥락 유지됨.

---

## 현재 상태 요약
- **Phase**: Phase 2 (전투 MVP) 진입 — 아키텍처 코드 생성 완료
- **에피소드**: EP02 (전투 프로토타입 — 약공격/강공격/구르기)
- **Unity**: 6000 LTS / URP
- **Input**: New Input System (PlayerControls.inputactions)
- **신규 스크립트**: 38개 (Core/Combat/Player/Enemy) — `Assets/_Project/Scripts/` 하위

---

## 완료된 스크립트 목록

### 프로토타입 (Assets/Scripts/) — EP01~EP02

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

### 아키텍처 코드 (Assets/_Project/Scripts/) — Day 4 신규 생성

#### Core (6개)
| 스크립트 | 역할 | 상태 |
|---------|------|------|
| EventBus.cs | 전역 이벤트 버스 (Subscribe/Publish) + 게임 이벤트 정의 | ✅ 완료 |
| GameState.cs | 게임 상태 enum (Boot/MainMenu/Hub/Run/Paused/GameOver) | ✅ 완료 |
| GameManager.cs | Singleton, 상태 전환, 런 관리, 일시정지 | ✅ 완료 |
| SceneLoader.cs | 비동기 씬 전환 유틸리티 | ✅ 완료 |
| SaveSystem.cs | JSON 영구 저장 (통화, 업그레이드 해금) | ✅ 완료 |
| ObjectPool.cs | 범용 오브젝트 풀 + PoolManager Singleton | ✅ 완료 |

#### Combat (9개)
| 스크립트 | 역할 | 상태 |
|---------|------|------|
| AttackData.cs | 공격별 SO (데미지/히트스톱/셰이크/넉백/콤보/3레이어SFX) | ✅ 완료 |
| DodgeData.cs | 구르기 SO (i-frame/거리/속도곡선/캔슬) | ✅ 완료 |
| StaminaData.cs | 스태미나 SO (최대치/소모/회복/고갈페널티) | ✅ 완료 |
| CombatTuningData.cs | 전역 전투 기본값 SO | ✅ 완료 |
| IDamageable.cs | 데미지 인터페이스 (플레이어/적 공용) | ✅ 완료 |
| WeaponHitbox.cs | 무기 Collider ON/OFF + 중복히트 방지 | ✅ 완료 |
| HitFeedback.cs | 히트스톱 + 이펙트 + impact/resonance 사운드 | ✅ 완료 |
| CameraShake.cs | 카메라 위치 오프셋 셰이크 | ✅ 완료 |
| CombatSoundPlayer.cs | Sound Layering 1층 (whoosh) | ✅ 완료 |

#### Player (13개)
| 스크립트 | 역할 | 상태 |
|---------|------|------|
| IPlayerState.cs | 플레이어 상태 인터페이스 | ✅ 완료 |
| PlayerStateMachine.cs | 상태 전환 + 컴포넌트 허브 + 이동/회전 유틸 | ✅ 완료 |
| PlayerHealth.cs | IDamageable, HP, 무적 체크, 넉백→Hit, 사망→Death | ✅ 완료 |
| PlayerStamina.cs | 스태미나 소모/회복/딜레이/고갈 페널티 | ✅ 완료 |
| PlayerAnimator.cs | MoveX/MoveZ/Speed/isGrounded 동기화 | ✅ 완료 |
| PlayerIdleState.cs | 대기 → Move/Attack/Dodge/Fall 전환 | ✅ 완료 |
| PlayerMoveState.cs | 걷기/달리기/스프린트 통합 | ✅ 완료 |
| PlayerDodgeState.cs | i-frame + 속도곡선 + 캔슬윈도우 | ✅ 완료 |
| PlayerLightAttackState.cs | 3타 콤보 + 히트박스 타이밍 + 선입력 | ✅ 완료 |
| PlayerHeavyAttackState.cs | 강공격 + 스태미나 소모 | ✅ 완료 |
| PlayerHitState.cs | 경직 + 넉백 감쇠 | ✅ 완료 |
| PlayerDeathState.cs | 사망 + PlayerDiedEvent | ✅ 완료 |
| PlayerFallState.cs | 낙하 + 공중 방향 제어 | ✅ 완료 |

#### Enemy (10개)
| 스크립트 | 역할 | 상태 |
|---------|------|------|
| EnemyData.cs | 적 SO (스탯/감지/공격패턴/드롭) | ✅ 완료 |
| IEnemyState.cs | 적 상태 인터페이스 | ✅ 완료 |
| EnemyStateMachine.cs | NavMeshAgent + FSM + OverlapSphere 감지 | ✅ 완료 |
| EnemyHealth.cs | IDamageable, 피격→Stagger, 사망→Death | ✅ 완료 |
| EnemyIdleState.cs | 대기, 감지→Chase, 웨이포인트→Patrol | ✅ 완료 |
| EnemyPatrolState.cs | 웨이포인트 순회 | ✅ 완료 |
| EnemyChaseState.cs | NavMesh 추격, 범위 진입→Attack | ✅ 완료 |
| EnemyAttackState.cs | AttackData SO 재사용, 히트박스 타이밍 | ✅ 완료 |
| EnemyStaggerState.cs | 경직 + 넉백(NavMesh) | ✅ 완료 |
| EnemyDeathState.cs | 사망 + 드롭 + EnemyDiedEvent | ✅ 완료 |

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

## Day 4 (4/9) — 아키텍처 코드 일괄 생성

### 완료
- `Assets/_Project/Scripts/` 폴더 구조 생성 (Core/Combat/Player/Enemy + 기타)
- **Core 6개**: EventBus, GameState, GameManager, SceneLoader, SaveSystem, ObjectPool(+PoolManager)
- **Combat SO 4개**: AttackData, DodgeData, StaminaData, CombatTuningData (CreateAssetMenu: 색랑하산/Combat/)
- **Combat 시스템 5개**: IDamageable, WeaponHitbox, HitFeedback(+Runner), CameraShake, CombatSoundPlayer
- **Player SM 10개**: IPlayerState, PlayerStateMachine + 8개 States (Idle/Move/Dodge/LightAttack/HeavyAttack/Hit/Death/Fall)
- **Player 3개**: PlayerHealth(IDamageable), PlayerStamina(SO기반), PlayerAnimator(해시 최적화)
- **Enemy 10개**: EnemyData SO, IEnemyState, EnemyStateMachine(NavMesh+Gizmos), EnemyHealth + 6개 States (Idle/Patrol/Chase/Attack/Stagger/Death)

### 핵심 설계
- 플레이어/적 모두 **AttackData SO 공유** → 코드 한 벌로 양쪽 전투 처리
- 구르기 i-frame은 **DodgeData SO**에서 0.01초 단위 튜닝
- **Sound Layering 3레이어**: whoosh(CombatSoundPlayer) → impact+resonance(HitFeedback)
- **EventBus** 기반 시스템 간 디커플링 (PlayerDied, EnemyDied, StaminaChanged 등)

### 다음
- 기존 프로토타입 코드(Assets/Scripts/) → 새 구조로 마이그레이션
- UI 시스템 (HUD, HP바, 스태미나바)
- Level 시스템 (Arena, Spawner)

---

## 기술 부채 / 나중에 할 것
- [ ] 좌클릭 입력 충돌 해소 (Interact vs Attack → 게임 상태 머신)
- [ ] Daz3D 캐릭터로 Capsule 교체 (EP06)
- [ ] 애니메이션 이벤트로 Invoke 타이밍 교체 (현재 하드코딩된 duration)
- [ ] 카메라 락온 모드 (전투 시스템 때)