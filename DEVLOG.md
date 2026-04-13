# 色狼下山 — 개발 진행 로그

> 이 문서는 `DESIGN.md`를 기준선으로 삼아, 실제로 Unity에서 확인된 구현 상태와 다음 작업 우선순위를 기록한다.
> 설계 의도는 `DESIGN.md`, 현재 구현 현실은 `DEVLOG.md`를 본다.

---

## 현재 상태 요약
- **기준 문서**: `DESIGN.md`
- **현재 페이즈**: `Phase 1` 완료 직전 + `Phase 2` 초입
- **현재 씬 기준**: 최신 Unity Editor 작업 기준 `Assets/Scenes/SampleScene.unity`
- **코드 기준선**: `Assets/_Project/Scripts/`
- **튜닝 에셋 기준선**: `Assets/_Project/ScriptableObjs/`
- **엔진/패키지**: Unity 6000 LTS / URP / New Input System / AI Navigation
- **저장소 추적 상태 주의**: 현재 git에는 스크립트, ScriptableObject, 문서, 일부 설정 파일 위주로 올라가 있으며 `.unity`, `.anim`, `.controller`, `.fbx` 같은 에디터 자산은 저장소만으로 검증되지 않는다.

## 이번 세션에서 실제로 끝난 것
- `Assets/_Project/Scripts` 아키텍처 기준으로 플레이어 시스템을 다시 연결했다.
- `PlayerStateMachine` 기반 이동, 회전, 카메라 추적을 실제 씬에서 동작시켰다.
- `AttackData`, `DodgeData`, `StaminaData`, `CombatTuningData` ScriptableObject 에셋을 만들고 Inspector에 연결했다.
- `PlayerAnimator.controller`를 직접 구성해서 `Idle`, `Locomotion`, `Slash1`, `HeavyAttack`, `Dodge`를 실제 재생되게 만들었다.
- 약공격은 좌클릭, 강공격은 `Shift + 좌클릭`, 회피는 `Space`로 동작하도록 입력 계층을 정리했다.
- 구르기 속도와 `DodgeData`를 엘든링 롤 감각에 맞춰 1차 튜닝했다.
- 현재 체감 기준으로 `이동 / 회피 / 약공격 1타 / 강공격`은 플레이 가능한 상태다.
- 설계 문서 파일명을 `a.md`에서 `DESIGN.md`로 정리하고, 세션 로그는 `ChatLogs/`에 누적하는 운영 방식을 확정했다.
- Git 커밋 및 원격 push 완료: `ce49436`

## 아직 남아 있는 것
- `Slash1 -> Slash2` 콤보 연계가 완전히 안정적이지 않다.
- 구르기 속도 곡선과 회복 구간은 추가 미세 조정이 필요하다.
- 히트박스/적 피격/히트스톱/넉백/사운드 레이어링은 아직 실전 검증 전이다.
- `SampleScene`는 여전히 임시 테스트 씬이다. 최종 `Boot / MainMenu / GamePlay / Hub` 씬 구조는 미구성이다.

---

## 이번 세션 추가 확인

### 저장소 기준 확인 결과
- 작업 트리는 깨끗했고 `main` 브랜치 기준으로 정리되어 있었다.
- `Assets/_Project/Scripts/`와 `Assets/_Project/ScriptableObjs/`는 문서와 일치하게 존재했다.
- 다만 현재 저장소에는 `.unity`, `.anim`, `.controller`, `.fbx` 자산이 잡히지 않았다.
- 따라서 `SampleScene`, Animator 전이, 프리팹 wiring, 실제 배치 상태는 Unity Editor에서 다시 확인해야 한다.

### 플레이어 전투 코드 리뷰 결과
- `PlayerStateMachine` 중심의 입력/상태 전이 구조는 유지되고 있다.
- `Slash1 -> Slash2` 불안정 원인은 코드상으로도 확인된다.
  - 공격 상태가 실제 Animator state가 아니라 `AnimationClip.length`와 로컬 타이머를 기준으로 종료된다.
  - 경공격은 매번 같은 `Attack` 트리거를 사용하고 있어, 콤보 단계가 Animator에 명시적으로 전달되지 않는다.
  - `AttackData.canComboInto` 데이터 구조는 존재하지만 현재 경공격 로직은 `CurrentComboIndex`만 사용한다.
  - 입력 버퍼가 따로 없어서 콤보 입력 타이밍이 조금만 빨라도 누락될 수 있다.
- `WeaponHitbox`는 `other.GetComponent<IDamageable>()`만 사용하므로 적 루트가 아니라 자식 콜라이더를 맞출 때 프리팹 구조에 따라 적중 누락 가능성이 있다.

### Unity Editor에서 확인할 것
1. `SampleScene` 실제 경로와 저장 여부
2. `PlayerAnimator.controller`의 `Attack` 전이 조건과 `Slash1 -> Slash2` 연결 방식
3. `Slash1`, `Slash2`, `HeavyAttack`, `Dodge` 상태 속도와 transition duration
4. 플레이어 무기 콜라이더와 `WeaponHitbox` owner / layer / trigger 설정
5. 적 프리팹의 `IDamageable` 부착 위치와 자식 콜라이더 구조

---

## 폴더 기준선

### 사용 중인 코드
- `Assets/_Project/Scripts/Core`
- `Assets/_Project/Scripts/Camera`
- `Assets/_Project/Scripts/Combat`
- `Assets/_Project/Scripts/Player`
- `Assets/_Project/Scripts/Enemy`
- `Assets/_Project/Scripts/Level`
- `Assets/_Project/Scripts/UI`
- `Assets/_Project/Scripts/Utils`

### 사용 중인 튜닝 에셋
- `Assets/_Project/ScriptableObjs/`
  - `slash1AttackData.asset`
  - `slash2AttackData.asset`
  - `heavyAttackData.asset`
  - `DodgeData.asset`
  - `StaminaData.asset`
  - `CombatTuningData.asset`

### 주의
- 앞으로도 새 코드는 `Assets/_Project/Scripts` 기준으로 유지한다.
- `Assets/Scripts`를 다시 주력 코드 위치로 쓰지 않는다.
- Unity 참조가 중요하므로 `.asset`와 `.meta`는 항상 같이 커밋한다.

---

## 시스템별 현황

### 1. Player Movement / Camera
**상태**: 플레이 가능

완료:
- WASD 이동
- 카메라 기준 방향 이동
- 이동 방향 회전
- Idle / Running 애니메이션
- Third Person Camera 추적

비고:
- `PlayerStateMachine` 기반으로 실제 동작 확인 완료
- `CharacterController.isGrounded` 기준으로 접지 판정 수정

남은 것:
- `Locomotion`을 추후 `MoveX / MoveZ` 기반 2D Blend Tree로 확장
- 점프는 현재 미구현
- 낙하/착지 연출은 아직 다듬지 않음

### 2. Dodge / Stamina
**상태**: 동작함, 1차 튜닝 완료

완료:
- `Space` 입력으로 회피 동작
- `DodgeData` 기반 `iFrameStart`, `iFrameDuration`, `totalDuration`, `recoveryTime`, `distance`, `speedCurve` 튜닝 가능
- 엘든링 롤 기준으로 1차 속도 세팅

현재 참고값:
- `Roll.anim` 원본 길이: 약 `2.367초`
- Animator `Dodge` 상태 속도는 엘든링 전체 롤 길이 기준으로 빠르게 조정
- `DodgeData.totalDuration`과 `recoveryTime`을 별도로 관리

남은 것:
- 시작 가속 / 후반 감속을 더 자연스럽게 다듬기
- 회피 거리와 무적 시간 밸런스 조정
- 적 공격과 맞물렸을 때 체감 검증

### 3. Light Attack / Heavy Attack
**상태**: 1타 약공격 + 강공격 동작

완료:
- 좌클릭 약공격
- `Shift + 좌클릭` 강공격
- `AttackData` 기반 애니메이션/타이밍 연결
- `Slash1`, `HeavyAttack` 상태 재생 확인

남은 것:
- `Slash1 -> Slash2` 콤보 안정화
- 이후 `Slash3` 확장 여부 판단
- 히트박스 실제 적중 테스트

### 4. Hitbox / Damage / Feedback
**상태**: 코드만 준비, 실제 검증 전

코드상 준비된 것:
- `WeaponHitbox`
- `IDamageable`
- `HitFeedback`
- `CameraShake`
- `CombatSoundPlayer`

남은 것:
- 무기 오브젝트 Collider와 `WeaponHitbox` 실제 연결 검증
- 더미 적 또는 캡슐 적 배치
- 히트스톱 / 넉백 / 셰이크 / 사운드 레이어링 체감 확인

### 5. Enemy AI
**상태**: 아키텍처 코드만 존재, 씬 실장 전

준비된 코드:
- `EnemyStateMachine`
- `EnemyHealth`
- `EnemyData`
- `Idle / Patrol / Chase / Attack / Stagger / Death`

남은 것:
- 테스트용 적 프리팹
- NavMesh Bake
- 플레이어 감지 및 전투 연결
- 적 공격용 `AttackData` 에셋 생성

### 6. Level / Run Loop / UI
**상태**: 대부분 뼈대만 있음

준비된 코드:
- `RunManager`
- `ArenaManager`
- `EnemySpawner`
- `AreaData`
- UI 관련 스크립트 다수

남은 것:
- 실제 씬에 배치 및 연결
- 전투 구역 / 보상 / 결산 루프 연결
- HUD / 보스 HP / 사망 화면 시각 구현

---

## DESIGN.md 기준 체크

### Phase 0: 환경 세팅
- [x] Unity 프로젝트 생성 (URP)
- [x] Input System / URP / AI Navigation 포함 기본 패키지 상태 확인
- [x] Git 저장소 및 원격 push
- [x] 아키텍처 폴더 구조 생성
- [ ] Boot / MainMenu / Gameplay 씬 플로우 분리
- [ ] Daz 캐릭터 1개 임포트 정식 테스트

### Phase 1: 3인칭 코어 무브먼트
- [x] WASD 입력
- [x] 카메라 기준 방향 이동
- [x] 캐릭터 회전
- [x] 걷기 / 달리기 애니메이션
- [x] 구르기 동작
- [x] 스태미나 연결
- [x] DodgeData 기반 튜닝 구조
- [ ] 점프 / 낙하 / 착지 정리
- [ ] Locomotion 2D Blend Tree 정리

### Phase 2: 전투 MVP
- [x] AttackData / DodgeData / StaminaData / CombatTuningData 생성
- [x] 약공격 1타 동작
- [x] 강공격 동작
- [x] ScriptableObject 기반 튜닝 구조 확립
- [ ] 경공격 2타 콤보 안정화
- [ ] 히트박스 실제 적중 검증
- [ ] 적 데미지 시스템 연결
- [ ] 히트스톱 / 넉백 / 셰이크 체감 검증
- [ ] 사운드 레이어링 연결 검증

### Phase 3: Enemy AI
- [ ] 테스트 적 프리팹
- [ ] NavMesh Bake
- [ ] 플레이어 추적
- [ ] 적 공격 연결
- [ ] 적 사망 / 드롭

### Phase 4 이후
- 아직 본격 착수 전

---

## 다음 우선순위

### 바로 다음
1. `Slash1 -> Slash2` 콤보 안정화
2. 테스트용 적 1종 배치
3. 무기 히트박스와 적 피격 확인
4. 히트스톱 / 넉백 / 카메라 셰이크 / 사운드 체감 조정

### 그 다음
1. NavMesh Bake
2. 적 FSM 실제 구동
3. 사망 / 결산 / HUD 최소 연결
4. `GamePlay` 씬 분리

### 아직 건드리지 말 것
- 록온 시스템
- 여캐/NPC 시스템
- Hub / Meta Progression 대규모 확장
- 성인 연출용 별도 씬

---

## 현재 운영 원칙
- 설계는 `DESIGN.md`, 현실 구현은 `DEVLOG.md`
- 시스템은 코드보다 `ScriptableObject` 튜닝 우선
- 플레이어 감각은 먼저 `회피 / 약공격 / 강공격 / 카메라`를 안정화
- 적, UI, 로그라이트 루프는 그 다음
- `SampleScene`은 계속 테스트 씬으로 써도 되지만, 일정 시점에 `GamePlay` 씬으로 분리 필요

---

## 메모
- 현재 Animator 파라미터 이름 오타 하나로도 바로 동작이 깨진다.
- `HeavyAttack`처럼 코드와 Animator 파라미터 이름을 반드시 정확히 맞춘다.
- 회피 애니메이션 길이와 `DodgeData.totalDuration`은 반드시 분리해서 생각한다.
- 게임 체감은 코드보다 `AttackData`, `DodgeData`, `CombatTuningData` 값이 훨씬 크게 좌우한다.
