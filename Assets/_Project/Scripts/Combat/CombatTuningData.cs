using UnityEngine;

/// <summary>
/// 전역 전투 기본값. AttackData에서 오버라이드하지 않은 경우 이 값을 사용.
/// </summary>
[CreateAssetMenu(fileName = "CombatTuningData", menuName = "색랑하산/Combat/CombatTuningData")]
public class CombatTuningData : ScriptableObject
{
    [Header("히트스톱")]
    [Tooltip("기본 히트스톱 (AttackData에서 오버라이드 가능)")]
    public float defaultHitStopDuration = 0.05f;

    [Header("피격 플래시")]
    public float hitFlashDuration = 0.1f;

    [Header("슬로우모션 (강공격/처형)")]
    [Range(0.01f, 1f)]
    public float slowMotionScale = 0.3f;
    public float slowMotionDuration = 0.5f;

    [Header("선입력")]
    [Tooltip("선입력 허용 시간 (초)")]
    public float inputBufferWindow = 0.2f;

    [Header("기본 데미지")]
    public float basePlayerDamage = 10f;
}
