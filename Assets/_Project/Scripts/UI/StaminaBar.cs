using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 스태미나바. 고갈 시 빨간 깜빡임 연출.
/// </summary>
public class StaminaBar : MonoBehaviour
{
    [Header("Bar")]
    [SerializeField] private Image fillBar;

    [Header("Colors")]
    [SerializeField] private Color normalColor = new Color(0.2f, 0.8f, 0.2f);    // 초록
    [SerializeField] private Color lowColor = new Color(0.9f, 0.6f, 0.1f);       // 주황
    [SerializeField] private Color emptyColor = new Color(0.9f, 0.2f, 0.2f);     // 빨강

    [Header("Settings")]
    [SerializeField] private float lowThreshold = 0.25f;
    [SerializeField] private float blinkSpeed = 4f;

    private float targetFill;
    private bool isEmpty;

    public void UpdateStamina(float current, float max)
    {
        if (max <= 0f) return;

        targetFill = current / max;
        isEmpty = current <= 0f;

        if (fillBar != null)
            fillBar.fillAmount = targetFill;
    }

    private void Update()
    {
        if (fillBar == null) return;

        // 부드러운 바 이동
        fillBar.fillAmount = Mathf.Lerp(fillBar.fillAmount, targetFill, 10f * Time.deltaTime);

        // 색상 처리
        if (isEmpty)
        {
            // 고갈: 빨간색 깜빡임
            float blink = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            fillBar.color = Color.Lerp(emptyColor, Color.clear, blink * 0.5f);
        }
        else if (targetFill <= lowThreshold)
        {
            fillBar.color = lowColor;
        }
        else
        {
            fillBar.color = normalColor;
        }
    }
}
