using UnityEngine;

/// <summary>
/// 스태미나 관리. 구르기/스프린트/강공격 자원.
/// StaminaData SO에서 수치를 읽어옴.
/// </summary>
public class PlayerStamina : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private StaminaData staminaData;

    public StaminaData Data => staminaData;
    public float Current { get; private set; }
    public float Max => staminaData.maxStamina;
    public bool IsEmpty => Current <= 0f;
    public bool IsPenalized => penaltyTimer > 0f;

    private float regenDelayTimer;
    private float penaltyTimer;

    private void Start()
    {
        Current = staminaData.maxStamina;
    }

    private void Update()
    {
        // 고갈 페널티 중
        if (penaltyTimer > 0f)
        {
            penaltyTimer -= Time.deltaTime;
            return;
        }

        // 회복 딜레이 중
        if (regenDelayTimer > 0f)
        {
            regenDelayTimer -= Time.deltaTime;
            return;
        }

        // 회복
        if (Current < staminaData.maxStamina)
        {
            Current = Mathf.Min(staminaData.maxStamina, Current + staminaData.regenRate * Time.deltaTime);
            PublishEvent();
        }
    }

    /// <summary>
    /// 소모 가능 여부 확인 (고갈 페널티 중이면 불가).
    /// </summary>
    public bool CanConsume(float amount)
    {
        return !IsPenalized && Current >= amount;
    }

    /// <summary>
    /// 스태미나 소모. 0 이하로 떨어지면 고갈 페널티 발동.
    /// </summary>
    public void Consume(float amount)
    {
        Current = Mathf.Max(0f, Current - amount);
        regenDelayTimer = staminaData.regenDelay;

        if (Current <= 0f)
            penaltyTimer = staminaData.emptyPenaltyTime;

        PublishEvent();
    }

    public void ResetStamina()
    {
        Current = staminaData.maxStamina;
        regenDelayTimer = 0f;
        penaltyTimer = 0f;
        PublishEvent();
    }

    private void PublishEvent()
    {
        EventBus.Publish(new StaminaChangedEvent
        {
            current = Current,
            max = staminaData.maxStamina
        });
    }
}
