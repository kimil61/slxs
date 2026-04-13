# 色狼下山 (Saek-Nang-Ha-San)

무협풍 3인칭 3D 액션 게임 프로젝트입니다. Unity 6000 LTS / URP / C# 기반으로 개발 중입니다.

## 문서 역할
- `a.md`: 설계 원본, 아키텍처, 시스템 방향, 로드맵
- `DEVLOG.md`: 실제 구현 상태와 다음 작업 우선순위
- `AGENTS.md`: 프로젝트 작업 규칙과 기준선

## 현재 구현 상태
- `Assets/_Project/Scripts/`가 현재 주력 코드 위치입니다.
- `Assets/_Project/ScriptableObjs/`에 전투/회피/스태미나 튜닝 에셋이 있습니다.
- 현재 `SampleScene` 기준으로 아래가 동작합니다.
  - WASD 이동
  - 3인칭 카메라 추적
  - Idle / Running 애니메이션
  - 회피(`Space`)
  - 약공격(좌클릭)
  - 강공격(`Shift + 좌클릭`)

## 현재 우선 작업
1. `Slash1 -> Slash2` 콤보 안정화
2. 테스트용 적 1종 배치
3. 히트박스 / 피격 / 히트스톱 / 넉백 검증
4. NavMesh 기반 적 AI 연결

## 폴더 기준
```text
Assets/
├── Animations/
├── Characters/
├── Scenes/
├── Settings/
├── _Project/
│   ├── Scripts/
│   └── ScriptableObjs/
├── PlayerAnimator.controller
└── PlayerControls.inputactions
```

## 개발 원칙
- 설계는 `a.md`, 현실 구현은 `DEVLOG.md`를 따른다.
- 플레이 감각은 코드보다 `ScriptableObject` 튜닝을 우선한다.
- Unity 자산은 `.meta`와 함께 관리한다.
