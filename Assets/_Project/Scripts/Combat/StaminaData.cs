using UnityEngine;

/// <summary>
/// 스태미나 수치 데이터. 구르기/스프린트/강공격 자원 관리.
/// </summary>
[CreateAssetMenu(fileName = "NewStaminaData", menuName = "색랑하산/Combat/StaminaData")]
public class StaminaData : ScriptableObject
{
    [Header("최대치")]
    public float maxStamina = 100f;

    [Header("소모")]
    public float sprintCostPerSec = 15f;

    [Header("회복")]
    [Tooltip("초당 회복량")]
    public float regenRate = 30f;

    [Tooltip("소모 후 회복 시작까지 딜레이 (초)")]
    public float regenDelay = 1.0f;

    [Header("고갈 페널티")]
    [Tooltip("스태미나 0 시 행동 불가 시간 (초)")]
    public float emptyPenaltyTime = 2.0f;
}
