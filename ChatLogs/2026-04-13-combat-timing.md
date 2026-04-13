# 2026-04-13 Combat Timing Follow-up

## 작업 배경
- 플레이 테스트 중 `Slash1` 이후 WASD가 다시 먹는 시점이 애니메이션 체감과 어긋나는 문제가 있었다.
- 원인은 공격 상태 종료가 Animator state 실제 재생 종료가 아니라 `AnimationClip.length`와 로컬 타이머에 묶여 있던 구조였다.

## 오늘 바꾼 것
- `AttackData`가 공격 타이밍의 기준 데이터가 되도록 구조를 정리했다.
- 아래 타이밍이 `AttackData`에서 직접 조절되도록 변경했다.
  - `totalDuration`
  - `moveRecoveryTime`
  - `dodgeCancelTime`
  - `comboWindowStartTime`
  - `comboWindowEndTime`
  - `hitboxStartTime`
  - `hitboxEndTime`
- `PlayerLightAttackState`, `PlayerHeavyAttackState`, `EnemyAttackState`가 이제 위 값을 직접 읽는다.
- `slash1AttackData`, `slash2AttackData`, `heavyAttackData`에 1차 기본값을 넣었다.

## Unity Editor에서 확인한 포인트
- `Slash1` 애니메이션 원본 길이는 길지만, Animator state speed를 올리면 실제 체감 재생 시간은 더 짧아진다.
- 이때 `AttackData.totalDuration`과 `moveRecoveryTime`을 실제 재생 시간에 맞춰 주면 이동 복귀 타이밍이 자연스럽게 맞는다.
- 이번 확인 기준으로는 `Slash1` state speed를 `2`로 두고 실제 재생 체감을 약 `0.75초`로 봤을 때, `totalDuration = 0.75`, `moveRecoveryTime = 0.75`로 맞추니 깔끔하게 동작했다.

## 현재 해석
- 문제는 공격 코드가 잘못됐다기보다는, 애니메이션 시간과 게임플레이 시간을 같은 값으로 간주한 구조 문제였다.
- 이제는 애니메이션 speed를 바꾸더라도 판정 시간과 복귀 시간을 별도로 맞출 수 있다.

## 남은 것
- `Slash1 -> Slash2` 콤보 안정성은 Animator 전이와 입력 타이밍까지 포함해서 계속 검증해야 한다.
- `DodgeData`도 공격처럼 전부 초 단위 기준으로 더 일관되게 바꿀지 판단이 필요하다.
- 적 피격/히트스톱/넉백/카메라 셰이크는 아직 실전 검증 전이다.

## 다음 세션 시작 포인트
1. `Slash1`, `Slash2`, `HeavyAttack` 각각 실제 재생 시간과 `AttackData` 값 다시 맞추기
2. `Slash1 -> Slash2` 입력 체감 확인
3. `DodgeData`도 공격과 같은 타이밍 설계로 확장할지 결정
