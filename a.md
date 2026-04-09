# 色狼下山 — 기술 아키텍처 & 로드맵 v2

## 게임 개요

- **장르**: 무협 성인 로그라이트 액션
- **엔진**: Unity (URP)
- **시점**: 3인칭 (엘든링식) — 3D 에셋 + 3rd Person Camera
- **타겟 플랫폼**: itch.io (무료 데모) → Steam / DLsite (유료 전환 시)
- **무기**: 단일 무기 (검)
- **목표 플레이타임**: ~2시간 (10~15분/런 × 8런)

## 확정된 의사결정

|항목   |결정                               |비고              |
|-----|---------------------------------|----------------|
|시점   |3인칭 (엘든링식)                       |록온 시스템은 v2      |
|에셋   |풀 3D (Daz 캐릭터)                   |                |
|카메라  |Cinemachine 3rd Person Follow    |장애물 충돌 처리 기구현 완료|
|무기   |단일 (검)                           |무기 교체 없음        |
|배포   |itch.io 먼저 → 유료 전환 시 Steam/DLsite|                |
|플레이타임|2시간 (8런 × 10~15분)                |                |
|성인 연출|전투와 분리된 별도 씬/카메라                 |v2에서 본격 구현      |
|록온   |v2로 미룸                           |MVP는 프리 카메라만    |

-----

## 1. 전체 아키텍처 개요

```
┌─────────────────────────────────────────────────────┐
│                  Game Manager                        │
│  (GameState, RunManager, SceneFlow)                  │
├──────────┬──────────┬───────────┬───────────────────┤
│ Combat   │ Level    │ Character │ Progression       │
│ System   │ System   │ System    │ System            │
├──────────┼──────────┼───────────┼───────────────────┤
│ Input    │ Proc-Gen │ Player    │ Per-Run           │
│ Attack   │ Area     │ Controller│ Upgrades          │
│ Hit Det. │ Layout   │ Enemy AI  │ Permanent         │
│ Damage   │ Spawner  │ NPC/여캐  │ Unlocks           │
│ Feedback │ NavMesh  │ Daz Asset │ Currency          │
├──────────┴──────────┴───────────┴───────────────────┤
│               Foundation Layer                       │
│  Save/Load │ Audio │ UI │ ObjectPool │ EventBus     │
│  3rd Person Camera (Cinemachine)                     │
└─────────────────────────────────────────────────────┘
```

### 양파 구조 (코어 보호 원칙)

```
[코어 — 절대 안 건드림]
│ EventBus, SaveSystem, ObjectPool
│ GameManager 상태 흐름, SceneLoader
│
├─[프레임워크 — 거의 안 건드림]
│  │ IDamageable 인터페이스
│  │ State Machine 베이스 클래스
│  │ ScriptableObject 데이터 구조
│  │ Input System 래퍼
│  │
│  └─[게임플레이 — 자유롭게 튜닝]
│     AttackData SO (공격별 히트스톱/셰이크/넉백/사운드)
│     DodgeData SO (i-frame 시작/지속/거리/속도곡선)
│     StaminaData SO, CombatTuningData SO
│     적 AI 패턴 (SO), 적 스탯 (SO)
│     레벨 배치, 스폰 데이터
│     이펙트, 카메라 연출, Sound Layering
│
│     ← Kimil이 Inspector에서 값 조절
│     ← 코드 수정 없이 게임 느낌 변경 가능
```

-----

## 2. Unity 프로젝트 폴더 구조

```
Assets/
├── _Project/
│   ├── Scripts/
│   │   ├── Core/           # GameManager, EventBus, SaveSystem
│   │   ├── Combat/         # 공격, 피격, 데미지, 히트피드백
│   │   ├── Player/         # PlayerController, StateMachine, 3rd Person Movement
│   │   ├── Camera/         # CameraManager (Cinemachine 래퍼)
│   │   ├── Enemy/          # EnemyAI, FSM, NavMeshAgent
│   │   ├── Level/          # AreaGenerator, Spawner, MapGraph
│   │   ├── Progression/    # RunUpgrade, MetaProgression
│   │   ├── UI/             # HUD, Menus, Dialogue
│   │   ├── NPC/            # 여캐 시스템 (v2)
│   │   └── Utils/          # ObjectPool, Timer, Extensions
│   ├── Prefabs/
│   │   ├── Player/
│   │   ├── Enemies/
│   │   ├── Weapons/
│   │   ├── Effects/        # 히트이펙트, 파티클
│   │   └── UI/
│   ├── Art/
│   │   ├── Characters/     # Daz 캐릭터 FBX/텍스처
│   │   ├── Environment/    # 산, 바위, 나무, 터레인
│   │   ├── VFX/            # 파티클 텍스처, 셰이더
│   │   └── UI/
│   ├── Animation/
│   │   ├── Player/         # Animator Controller, Clips
│   │   ├── Enemy/
│   │   └── Retarget/       # Daz→Unity 리타겟 아바타
│   ├── Audio/
│   │   ├── SFX/            # 타격음, 발소리, UI
│   │   └── BGM/
│   ├── Scenes/
│   │   ├── Boot.unity
│   │   ├── MainMenu.unity
│   │   ├── GamePlay.unity  # 메인 런 씬
│   │   └── Hub.unity       # 거점 (산 아래 마을)
│   └── Data/
│       ├── ScriptableObjects/
│       │   ├── WeaponData/
│       │   ├── AttackData/         # 공격별 SO (LightAtk1, LightAtk2, HeavyAtk...)
│       │   ├── DodgeData/          # 구르기 튜닝 (i-frame, 거리, 속도곡선)
│       │   ├── EnemyData/
│       │   ├── UpgradeData/
│       │   ├── AreaData/           # 맵 구역 데이터
│       │   ├── StaminaData/        # 스태미나 수치
│       │   └── CombatTuning/       # 전역 전투 기본값
│       └── Config/
├── Plugins/
└── ThirdParty/
```

-----

## 3. 핵심 시스템별 상세 설계

### 3.1 Game Manager (Core)

**Unity 위치**: 빈 GameObject `[GameManager]` — DontDestroyOnLoad

|컴포넌트       |역할                           |Unity 기능                   |
|-----------|-----------------------------|---------------------------|
|GameManager|전체 게임 상태 (Menu/Run/Hub/Pause)|Singleton MonoBehaviour    |
|RunManager |한 판의 라이프사이클 관리               |ScriptableObject로 런 데이터 관리 |
|SceneLoader|씬 전환, 로딩                     |SceneManager.LoadSceneAsync|
|EventBus   |시스템 간 디커플링 통신                |C# event / Action<T>       |
|SaveSystem |영구 진행도 저장                    |JsonUtility + File I/O     |

**핵심 게임 루프 (8런 × 10~15분 기준)**:

```
Boot → MainMenu → Hub(산 아래 마을) → 산으로 출발 (Run 시작)
                                           ↓
                                  구역 진입 → 전투 → 클리어
                                           ↓        ↓
                                       보상 선택   사망
                                           ↓        ↓
                                      다음 구역    Hub 복귀
                                           ↓       (영구자원 적립)
                                      보스 클리어
                                           ↓
                                   산 정상 도달 → Hub 복귀
```

**런 구조 (1런 = 10~15분)**:

```
산 입구 → 전투구역 1 → 전투구역 2 → (선택: 전투구역 3 or 이벤트) → 보스 → 하산
         ~2분        ~2분         ~2분                           ~3분
```

-----

### 3.2 Camera System (3인칭 — 엘든링식)

**MVP 카메라 구성**:

|요소     |구현                           |비고               |
|-------|-----------------------------|-----------------|
|기본 추적  |Cinemachine 3rd Person Follow|✅ 기구현            |
|장애물 충돌 |Cinemachine Collider         |✅ 기구현 (벽 뒤 처리 완료)|
|카메라 셰이크|Cinemachine Impulse          |타격 피드백용          |
|마우스 회전 |Cinemachine POV              |우클릭 or 항시 회전     |
|록온 시스템 |—                            |❌ v2로 미룸         |
|실내 카메라 |—                            |❌ MVP는 야외(산)만    |

**카메라 설정 권장값 (SO로 튜닝 가능하게)**:

```
CameraSettings (ScriptableObject)
├── followDistance: 4~6m
├── followHeight: 1.5~2m
├── shoulderOffset: (0.5, 0, 0)  // 살짝 오른쪽
├── damping: (0.1, 0.3, 0.1)
├── mouseSensitivity: 2.0
└── collisionRadius: 0.3
```

-----

### 3.3 Combat System (전투)

**MVP 핵심 — 타격감 구현 체크리스트**:

|요소     |구현 방법                                    |Unity 기능                        |튜닝 방식                     |
|-------|-----------------------------------------|--------------------------------|--------------------------|
|입력 처리  |Input Buffer (선입력)                       |New Input System                |코드                        |
|공격 판정  |무기 히트박스 ON/OFF                           |Collider + Trigger              |Collider 크기 조절            |
|히트스톱   |피격 시 3~5프레임 멈춤                           |코루틴 (Time.timeScale X)          |**SO: hitStopDuration**   |
|카메라 셰이크|피격 시 화면 흔들림                              |Cinemachine Impulse             |**SO: shakeIntensity**    |
|넉백     |피격 대상 밀려남                                |CharacterController or Rigidbody|**SO: knockbackForce**    |
|이펙트    |히트 스파크, 피격 플래시                           |Particle System + Material flash|Prefab 교체                 |
|사운드    |3레이어: whoosh(바람)+impact(타격)+resonance(잔향)|AudioSource.PlayOneShot × 3     |**AttackData SO별 개별 클립**  |
|슬로우모션  |강공격/처형 시                                 |Time.timeScale 보간               |**SO: slowScale, slowDur**|

**CombatTuningData (ScriptableObject) — 전역 전투 기본값**:

```
[CreateAssetMenu] CombatTuningData.asset
├── defaultHitStopDuration: 0.05f    // 기본 히트스톱 (AttackData에서 오버라이드 가능)
├── hitFlashDuration: 0.1f           // 피격 흰색 플래시
├── slowMotionScale: 0.3f            // 슬로우모션 배율
├── slowMotionDuration: 0.5f         // 슬로우모션 길이
└── inputBufferWindow: 0.2f          // 선입력 허용 시간
```

**AttackData (ScriptableObject) — 공격별 개별 튜닝**:

```
[CreateAssetMenu] AttackData.asset
├── attackName: "LightAttack_1"
├── animationClip: (참조)
├── damageMultiplier: 1.0f           // 기본 공격력 배율
├── hitStopDuration: 0.03f           // 이 공격의 히트스톱 (경공격=짧게, 중공격=길게)
├── hitStopTimeScale: 0.0f
├── cameraShakeIntensity: 0.2f       // 이 공격의 셰이크 세기
├── cameraShakeDuration: 0.1f
├── knockbackForce: 3.0f             // 이 공격의 넉백
├── staminaCost: 0f                  // 스태미나 소모 (경공격=0, 중공격=30)
├── canComboInto: ["LightAttack_2"]  // 다음 콤보 연결 가능 목록
├── comboWindowStart: 0.4f           // 콤보 입력 허용 시작 (normalized time)
├── comboWindowEnd: 0.8f             // 콤보 입력 허용 끝
├── hitboxActivateTime: 0.2f         // 히트박스 켜지는 타이밍
├── hitboxDeactivateTime: 0.5f       // 히트박스 꺼지는 타이밍
│
│  [Sound Layering — 3레이어 SFX]
├── sfx_whoosh: AudioClip             // 1층: 바람소리 (슥-)
├── sfx_impact: AudioClip             // 2층: 타격음 (퍽-)
├── sfx_resonance: AudioClip          // 3층: 잔향 (금속 울림)
├── sfx_whooshVolume: 0.6f
├── sfx_impactVolume: 1.0f
├── sfx_resonanceVolume: 0.4f
├── sfx_impactDelay: 0.0f             // 타격 시점에 재생
└── sfx_resonanceDelay: 0.05f         // 타격 후 살짝 늦게
```

> **핵심: 플레이어 공격과 적 공격 모두 AttackData SO를 공유함.**
> 산적의 횡베기도, 플레이어의 경공격 1타도 같은 AttackData 구조.
> → 코드 한 벌로 양쪽 전투를 처리. 적 추가 시 SO 에셋만 새로 만들면 됨.

**Sound Layering 작동 방식**:

```
공격 시작 → sfx_whoosh 재생 (선동작에 바람소리)
    ↓
히트 판정 → sfx_impact 재생 (타격 순간)
    ↓
딜레이 후 → sfx_resonance 재생 (잔향/울림)

※ 빗나가면 whoosh만 재생, impact/resonance는 스킵
※ 이 3개 클립을 공격마다 다르게 설정 가능 (경공격=가벼운 소리, 중공격=묵직한 소리)
```

**Animator Controller 구조 (Player — 3인칭)**:

```
Animator Controller: Player_AC
├── Base Layer (이동 — 3D 전방향)
│   ├── Idle
│   ├── Walk (Blend Tree: 전후좌우)
│   ├── Run  (Blend Tree: 전후좌우)
│   ├── Sprint
│   └── Fall / Land
├── Combat Layer (weight=1, Override)
│   ├── Idle_Combat
│   ├── LightAttack_1 → LightAttack_2 → LightAttack_3 (콤보)
│   ├── HeavyAttack
│   ├── Dodge (구르기 — 엘든링식 i-frame)
│   └── Hit / Death
└── Parameters
    ├── MoveX (float) — 좌우 이동 블렌드
    ├── MoveZ (float) — 전후 이동 블렌드
    ├── Speed (float)
    ├── isGrounded (bool)
    ├── isSprinting (bool)
    ├── Attack (trigger)
    ├── HeavyAttack (trigger)
    ├── Dodge (trigger)
    ├── Hit (trigger)
    └── isDead (bool)
```

**3인칭 이동 핵심 (엘든링식)**:

```
입력 (WASD) → 카메라 기준 방향 변환 → 캐릭터 회전 + 이동
                                        ↓
                               카메라 forward/right 기준으로
                               moveDir = cam.forward * inputZ + cam.right * inputX
                               캐릭터가 moveDir 방향으로 부드럽게 회전
```

-----

### 3.4 Player Controller (3인칭)

**State Machine 패턴**:

```
IPlayerState (interface)
├── IdleState
├── MoveState (Walk/Run/Sprint 통합, Blend Tree 연동)
├── DodgeState (구르기, i-frame, 스태미나 소모)
├── LightAttackState (3타 콤보, 루트모션 선택적)
├── HeavyAttackState (차지 가능)
├── HitState (경직, 슈퍼아머 판정)
├── DeathState
└── FallState
```

**Unity 컴포넌트 구성 (3인칭 특화)**:

|컴포넌트              |역할                              |
|------------------|--------------------------------|
|PlayerStateMachine|상태 전환 관리                        |
|PlayerMovement    |**카메라 기준 이동**, 회전, 스프린트, 중력     |
|PlayerCombat      |공격 입력, 콤보, 데미지 처리               |
|PlayerHealth      |HP, 스태미나, 무적 프레임, 사망            |
|PlayerAnimator    |Animator 파라미터 동기화               |
|PlayerInput       |New Input System 래퍼 (WASD + 마우스)|
|PlayerStamina     |구르기/스프린트/강공격 자원 관리              |

**스태미나 시스템 (엘든링 핵심 메카닉)**:

```
StaminaData (ScriptableObject)
├── maxStamina: 100
├── sprintCostPerSec: 15
├── regenRate: 30/sec
├── regenDelay: 1.0f    // 소모 후 회복 시작까지 딜레이
└── emptyPenaltyTime: 2.0f  // 0 되면 잠시 행동 불가
```

**DodgeData (ScriptableObject) — 구르기 전용 튜닝**:

```
DodgeData (ScriptableObject)
├── staminaCost: 25                // 구르기 스태미나 소모
├── iFrameStart: 0.05f            // 무적 시작 (초) — 구르기 시작 후
├── iFrameDuration: 0.3f          // 무적 지속 시간 (이 값이 게임 난이도를 결정)
├── totalDuration: 0.6f           // 구르기 전체 길이
├── recoveryTime: 0.15f           // 구르기 후 행동 불가 시간
├── distance: 3.5f                // 구르기 이동 거리
├── speedCurve: AnimationCurve    // 구르기 속도 곡선 (빠르게 시작→느리게 끝)
└── canCancelInto: ["Attack"]     // 구르기 후반부에서 캔슬 가능한 액션

※ iFrameStart + iFrameDuration 조합이 핵심.
※ 엘든링 기준 참고값: 경장 구르기 ~0.43초 무적, 중장 ~0.33초 무적
※ SO이므로 Inspector에서 0.01초 단위로 튜닝 가능
```

-----

### 3.5 Enemy AI (3D NavMesh 기반)

**FSM 기반 (MVP)**:

```
EnemyStateMachine
├── IdleState       — 제자리 대기
├── PatrolState     — NavMeshAgent 웨이포인트 순회
├── ChaseState      — NavMeshAgent.SetDestination(player.position)
├── AttackState     — 공격 범위 진입 → 정지 → 공격 애니메이션
├── StaggerState    — 피격 경직 (NavAgent 정지)
├── DeathState      — 사망 연출 + 드롭
└── 감지: Physics.OverlapSphere (3D)
```

**Unity 컴포넌트**:

|컴포넌트             |역할                 |
|-----------------|-------------------|
|NavMeshAgent     |3D 경로탐색 (Unity 내장) |
|EnemyStateMachine|상태 관리              |
|EnemyData (SO)   |HP, 공격력, 감지범위, 드롭 등|

**ScriptableObject 적 데이터**:

```
[CreateAssetMenu] EnemyData.asset
├── HP: 100, 이동속도: 3.5
├── 감지 범위: 10m, 공격 범위: 2m
├── 공격 쿨다운: 2.0f
├── 공격 패턴: [AttackData SO 참조]  // ← 플레이어와 같은 AttackData 구조
├── 드롭 테이블: [{item, weight, count}]
├── 경직 시간: 0.3f
└── Prefab 참조
```

**MVP 적 타입 (3종)**:

|타입      |행동            |난이도|
|--------|--------------|---|
|산적 (근접) |접근 → 횡베기      |★☆☆|
|궁수 (원거리)|거리 유지 → 화살    |★★☆|
|두목 (돌진) |돌진 → 강공격, 슈퍼아머|★★★|

-----

### 3.6 Level System (산 등반 로그라이트)

**맵 컨셉: 산을 올라가는 구조**

```
산 정상 (보스) ──── 8런 차
    ↑
  구역 3 ────────── 5~8런 차에 도달
    ↑
  구역 2 ────────── 3~5런 차에 도달  
    ↑
  구역 1 ────────── 1~2런 차에 도달
    ↑
  산 입구 (시작점)
```

**1런의 흐름 (10~15분)**:

```
산 입구 → 전투 구역 A → 보상 선택 → 전투 구역 B → 보상 선택 → 보스 → 하산/사망
```

**구역 = 야외 3D 공간 (Arena 방식)**:

- 방(Room)이 아니라 **열린 공간** — 엘든링처럼 바위, 나무, 언덕이 있는 야외
- 적을 전부 처치 → 다음 구역으로 가는 길 열림
- NavMesh Bake: Terrain 또는 ProBuilder 지형에 한 번만

**Unity 구현**:

|요소     |방법                                 |
|-------|-----------------------------------|
|구역 레이아웃|Prefab Arena 세트 (야외 지형)            |
|지형     |Unity Terrain 또는 ProBuilder + 무료 에셋|
|적 배치   |SpawnPoint + EnemyData SO          |
|구역 연결  |길/계단 트리거 → 다음 Arena 활성화            |
|NavMesh|지형 Bake (구역별)                      |
|시드 관리  |System.Random(seed)                |

-----

### 3.7 Progression System (성장)

**Per-Run (한 판 내 성장)**:

```
RunUpgrade (ScriptableObject)
├── 무공 강화 — 공격력, 공속, 콤보 확장
├── 경공 — 구르기 비용 감소, 이동속도
├── 내공 — HP, 스태미나, 회복
├── 기공 — 히트스톱 강화, 넉백 강화 (타격감 변화!)
└── 전투 후 3택 1 선택
```

**Meta / Permanent (영구 성장 — 8런 분량)**:

```
MetaProgression
├── 통화: 武功秘笈 (런에서 획득 → Hub에서 소비)
├── 영구 업그레이드 (8런에 걸쳐 점진적 해금)
│   ├── Tier 1: 기본 스탯 (HP+, 공격력+, 스태미나+)
│   ├── Tier 2: 새 시작 아이템
│   ├── Tier 3: 구역 2 직행 가능
│   ├── Tier 4: 강공격 차지 해금
│   └── Tier 5+: (v2 여캐 관련)
└── 저장: JSON 파일
```

-----

### 3.8 UI System

|화면      |주요 요소              |Unity 기능                     |
|--------|-------------------|-----------------------------|
|HUD     |HP바, 스태미나바, 미니맵    |Canvas (Screen Space Overlay)|
|보스 HP   |화면 하단 보스 HP바 (엘든링식)|Canvas                       |
|일시정지    |설정, 스탯             |Canvas Group                 |
|업그레이드 선택|3장 카드 UI           |DOTween 애니메이션                |
|사망 화면   |“落山 (하산)” + 획득 자원  |엘든링 “YOU DIED” 스타일           |
|메인 메뉴   |시작, 설정, 종료         |별도 씬                         |
|Hub UI  |업그레이드, NPC         |별도 씬                         |

-----

### 3.9 Foundation Layer

|시스템          |역할                     |구현                                   |
|-------------|-----------------------|-------------------------------------|
|Object Pool  |이펙트, 투사체 재활용           |Queue<GameObject>                    |
|Audio Manager|BGM/SFX 통합 관리          |Singleton + AudioMixer               |
|Save System  |영구 진행도                 |JSON 직렬화                             |
|Event Bus    |시스템 간 통신               |static event / ScriptableObject Event|
|Camera       |3rd Person Follow + 셰이크|Cinemachine (✅ 기구현)                  |

-----

## 4. 필수 Unity 패키지 & 에셋

### 무료/내장

|패키지                            |용도                           |
|-------------------------------|-----------------------------|
|Universal Render Pipeline (URP)|렌더링                          |
|Input System                   |New Input System (WASD + 마우스)|
|Cinemachine                    |3rd Person Camera, 셰이크       |
|TextMeshPro                    |UI 텍스트                       |
|AI Navigation                  |NavMesh (적 AI 길찾기)           |
|Terrain Tools                  |산 지형 제작                      |
|ProBuilder                     |빠른 레벨 프로토타이핑                 |

### 추천 에셋 (유료/무료)

|에셋            |용도                 |필수도|
|--------------|-------------------|---|
|DOTween       |UI/연출 애니메이션        |★★★|
|Animancer     |Animator 대안 (코드 기반)|★★☆|
|Odin Inspector|에디터 SO 관리          |★★☆|


> **Animancer 참고**: Animator Controller GUI 대신 코드로 애니메이션 직접 제어.
> 에디터 조작 약하고 코드 선호하는 스타일에 적합. 유료 (검토 추천).

-----

## 5. Daz → Unity 파이프라인

```
Daz Studio
  ↓ Export (FBX — DAZ to Unity Bridge 또는 수동 FBX)
Unity Import
  ↓ Rig: Humanoid로 설정 (Avatar 매핑)
  ↓ Material: URP Lit 셰이더로 리매핑
  ↓ Animation Retarget: Humanoid Avatar 기반
  ↓ 최적화: Polygon 수 확인 (Decimate if needed)
  ↓ (v2) Cloth/Jiggle: Dynamic Bone 또는 Magica Cloth 2
```

**MVP**: Daz 캐릭터 1개 임포트 → Humanoid 리그 → 전투 애니메이션 재생 확인
**v2**: 의상, 천 물리, 신체 물리

-----

## 6. 개발 로드맵 (Phase 기반)

### Phase 0: 환경 세팅 (1주)

- [ ] Unity 프로젝트 생성 (URP)
- [ ] 폴더 구조 세팅
- [ ] Git 저장소 초기화
- [ ] 필수 패키지 설치 (Input System, Cinemachine, TMP, DOTween, AI Navigation)
- [ ] Boot → MainMenu → Gameplay 씬 플로우 확인
- [ ] Daz 캐릭터 1개 임포트 테스트 (Humanoid 리그 + 기본 애니메이션)
- [ ] Cinemachine 3rd Person Follow 카메라 확인 (✅ 기존 작업 재활용)

### Phase 1: 3인칭 코어 무브먼트 (1~2주)

- [ ] WASD 입력 → 카메라 기준 방향 변환
- [ ] 캐릭터 이동 방향으로 부드러운 회전
- [ ] 걷기 / 달리기 / 스프린트
- [ ] Blend Tree (MoveX, MoveZ 기반 전후좌우)
- [ ] 구르기 (DodgeData SO 기반: i-frame 시작/지속/거리/속도곡선 전부 SO 튜닝)
- [ ] 중력 / 낙하
- [ ] 스태미나 시스템 (StaminaData SO)
- [ ] 임시 테스트 지형 (ProBuilder 평지 + 경사)

### Phase 2: 전투 MVP (2~3주)

- [ ] AttackData SO 구조 생성 (공격별 개별 튜닝: 데미지/히트스톱/셰이크/넉백)
- [ ] 경공격 3타 콤보 (AttackData SO × 3: LightAttack_1/2/3)
- [ ] 중공격 (AttackData SO × 1: HeavyAttack, 스태미나 소모)
- [ ] 히트 판정 (3D Collider Trigger, hitboxActivate/Deactivate 타이밍 SO에서 제어)
- [ ] 데미지 시스템 (IDamageable 인터페이스 — 플레이어/적 공용)
- [ ] CombatTuningData SO (전역 기본값: 슬로우모션, 선입력 윈도우)
- [ ] **타격감**: 히트스톱, 카메라셰이크, 넉백, 피격 플래시 (전부 AttackData SO에서 읽음)
- [ ] 히트 이펙트 파티클
- [ ] **Sound Layering 구현** (공격별 3레이어: whoosh→impact→resonance)
- [ ] 테스트용 허수아비 적
- [ ] 적 공격도 AttackData SO 재사용 확인 (같은 코드로 양쪽 처리)
- [ ] 사망 시 “落山” 연출

### Phase 3: Enemy AI (2주)

- [ ] NavMesh Bake (테스트 지형)
- [ ] 기본 FSM (Idle → Chase → Attack → Stagger → Death)
- [ ] 적 타입 3종 (산적/궁수/두목)
- [ ] EnemyData SO 3개 생성
- [ ] 적 사망 드롭 (경험치/무공비급)
- [ ] 적 스폰 시스템

### Phase 4: 로그라이트 런 루프 (2~3주)

- [ ] Arena 구역 3개 (Prefab 야외 지형)
- [ ] 구역 클리어 → 다음 구역 길 열림
- [ ] 전투 후 업그레이드 3택 1 (RunUpgrade SO)
- [ ] 보스 1종 (두목 강화 버전)
- [ ] 보스 HP바 (화면 하단)
- [ ] 사망 → 결산 → Hub 복귀
- [ ] Run 완료 → 결산 → Hub 복귀
- [ ] 1런 10~15분 타이머 체크

### Phase 5: Hub & Meta Progression (1~2주)

- [ ] Hub 씬 (산 아래 마을)
- [ ] 영구 통화 (武功秘笈) 시스템
- [ ] 영구 업그레이드 상점 UI (Tier 1~4)
- [ ] Hub → 산 출발 (Run 시작) 플로우
- [ ] 8런 걸쳐 완주 가능한 밸런스 1차

### Phase 6: UI & Polish (1~2주)

- [ ] 전투 HUD (HP바, 스태미나바)
- [ ] 메인 메뉴
- [ ] 일시정지 메뉴
- [ ] 업그레이드 카드 UI 연출 (DOTween)
- [ ] 사운드/BGM 통합
- [ ] 사망 화면 연출

### Phase 7: Demo Build (1주)

- [ ] 전체 플로우 통합 테스트 (8런 완주)
- [ ] 밸런싱 1차
- [ ] 버그 수정
- [ ] 빌드 & 패키징 (Windows)
- [ ] itch.io 페이지 작성 & 업로드
- [ ] **← 여기까지가 Demo / MVP**

-----

### Phase 8~: v2 확장 (Demo 이후)

- [ ] **록온 시스템** (Cinemachine Target Group)
- [ ] 여캐 조우 이벤트 (EventRoom)
- [ ] 4단계 여캐 컨트롤 (조우→정착→의존→지배)
- [ ] 충성/복종 루트 분기
- [ ] 箱娘식 시드 기반 랜덤 여캐 (외모/체형/성격/욕구)
- [ ] Daz 캐릭터 의상 시스템
- [ ] 천 물리 / 신체 물리 (Magica Cloth 2)
- [ ] 성인 컨텐츠 별도 씬/카메라 연출
- [ ] 랜덤 산맥 맵 확장
- [ ] Steam / DLsite 유료 빌드 & 스토어 페이지

-----

## 7. 기술 리스크 & 대응

|리스크                    |영향        |대응                                   |
|-----------------------|----------|-------------------------------------|
|3인칭 카메라 조작감            |플레이 느낌 전체 |✅ 기구현됨, SO로 미세 튜닝                    |
|Daz→Unity 파이프라인        |캐릭터 비주얼   |Phase 0에서 사전 검증                      |
|타격감 튜닝 시간 초과           |Phase 2 일정|CombatTuningData SO로 빠른 반복, 엘든링 수치 참고|
|3D NavMesh 적 AI        |적이 지형에 끼임 |야외 평지 위주로 시작, 복잡한 지형은 후순위            |
|Animator Controller 숙련도|전체 일정     |유튜브 학습 + Animancer 대안                |
|8런 밸런스                 |재미 곡선     |Phase 7에서 직접 플레이 테스트, 수치는 SO로 빠른 조절  |
|스코프 크리프 (v2 침투)        |MVP 지연    |Phase 구분 엄격, v2는 TODO 주석만            |
|1인 개발 번아웃              |전체        |7~14일 스프린트, 런닝 루틴 유지                 |

-----

## 8. 의사결정 로그

|날짜     |결정                              |이유                                 |
|-------|--------------------------------|-----------------------------------|
|2025-04|엘든링식 3인칭 3D                     |만들고 싶은 걸 만들어야 끝까지 간다               |
|2025-04|록온 v2 미룸                        |MVP 카메라 공수 절감, 프리카메라로 충분           |
|2025-04|야외(산) 맵 한정                      |실내 없으면 카메라 충돌 문제 최소화               |
|2025-04|단일 무기 (검)                       |전투 복잡도 제한                          |
|2025-04|itch.io 우선 배포                   |가볍게 데모 올리고, 유료화 시 Steam/DLsite     |
|2025-04|2시간/8런 분량                       |极品采花郎 참고, MVP에 적절한 볼륨              |
|2025-04|양파 구조 (코어 보호)                   |AI 코드 생성 + 본인 튜닝 워크플로우 보장          |
|2025-04|AttackData SO 도입 (Gemini 피드백)   |공격별 개별 튜닝, 플레이어/적 공용 구조            |
|2025-04|DodgeData SO 분리 (Gemini 피드백)    |i-frame 0.01초 단위 튜닝, 난이도 핵심 변수     |
|2025-04|Sound Layering 3레이어 (Gemini 피드백)|whoosh→impact→resonance 합성으로 타격감 강화|