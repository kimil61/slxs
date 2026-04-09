using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 보스 HP바. 화면 하단 (엘든링식).
/// 보스 전투 시작 시 Show(), 종료 시 Hide().
/// </summary>
public class BossHPBar : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject barRoot;
    [SerializeField] private Image fillBar;
    [SerializeField] private Image trailBar;
    [SerializeField] private TextMeshProUGUI bossNameText;

    [Header("Settings")]
    [SerializeField] private float trailSpeed = 1.5f;
    [SerializeField] private float trailDelay = 0.6f;

    private float targetFill;
    private float trailFill;
    private float trailDelayTimer;

    private void Start()
    {
        if (barRoot != null)
            barRoot.SetActive(false);
    }

    private void Update()
    {
        if (trailBar != null && trailFill > targetFill)
        {
            if (trailDelayTimer > 0f)
                trailDelayTimer -= Time.deltaTime;
            else
            {
                trailFill = Mathf.MoveTowards(trailFill, targetFill, trailSpeed * Time.deltaTime);
                trailBar.fillAmount = trailFill;
            }
        }
    }

    public void Show(string bossName, float current, float max)
    {
        if (barRoot != null)
            barRoot.SetActive(true);

        if (bossNameText != null)
            bossNameText.text = bossName;

        targetFill = 1f;
        trailFill = 1f;
        if (fillBar != null) fillBar.fillAmount = 1f;
        if (trailBar != null) trailBar.fillAmount = 1f;

        UpdateHP(current, max);
    }

    public void UpdateHP(float current, float max)
    {
        if (max <= 0f) return;

        targetFill = current / max;
        trailDelayTimer = trailDelay;

        if (fillBar != null)
            fillBar.fillAmount = targetFill;
    }

    public void Hide()
    {
        if (barRoot != null)
            barRoot.SetActive(false);
    }
}
