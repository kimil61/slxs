using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 플레이어 HP바. 메인 바 + 피격 시 지연 감소하는 서브 바.
/// </summary>
public class PlayerHPBar : MonoBehaviour
{
    [Header("Bars")]
    [SerializeField] private Image fillBar;       // 실제 HP (빨강)
    [SerializeField] private Image trailBar;      // 지연 감소 바 (주황/흰색)

    [Header("Text (선택)")]
    [SerializeField] private TextMeshProUGUI hpText;

    [Header("Settings")]
    [SerializeField] private float trailSpeed = 2f;
    [SerializeField] private float trailDelay = 0.5f;

    private float targetFill;
    private float trailFill;
    private float trailDelayTimer;

    private void Start()
    {
        targetFill = 1f;
        trailFill = 1f;
        SetFill(1f);
    }

    private void Update()
    {
        // 지연 바 서서히 감소
        if (trailBar != null && trailFill > targetFill)
        {
            if (trailDelayTimer > 0f)
            {
                trailDelayTimer -= Time.deltaTime;
            }
            else
            {
                trailFill = Mathf.MoveTowards(trailFill, targetFill, trailSpeed * Time.deltaTime);
                trailBar.fillAmount = trailFill;
            }
        }
    }

    public void UpdateHP(float current, float max)
    {
        if (max <= 0f) return;

        targetFill = current / max;
        trailDelayTimer = trailDelay;

        if (fillBar != null)
            fillBar.fillAmount = targetFill;

        // 회복 시 트레일도 즉시 올림
        if (trailFill < targetFill)
        {
            trailFill = targetFill;
            if (trailBar != null)
                trailBar.fillAmount = trailFill;
        }

        if (hpText != null)
            hpText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }

    private void SetFill(float value)
    {
        if (fillBar != null) fillBar.fillAmount = value;
        if (trailBar != null) trailBar.fillAmount = value;
    }
}
