using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 사망 화면. 엘든링 "YOU DIED" 스타일 → "落山 (하산)".
/// 페이드인 → 텍스트 등장 → 획득 자원 표시 → Hub 복귀 버튼.
/// </summary>
public class DeathScreen : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject screenRoot;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private Button returnButton;

    [Header("Settings")]
    [SerializeField] private float fadeInDuration = 1.5f;
    [SerializeField] private float textDelay = 0.8f;
    [SerializeField] private string deathTitle = "落  山";

    private void Start()
    {
        if (screenRoot != null)
            screenRoot.SetActive(false);

        if (returnButton != null)
            returnButton.onClick.AddListener(OnReturnToHub);
    }

    public void Show(int earnedCurrency = 0)
    {
        if (screenRoot != null)
            screenRoot.SetActive(true);

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        if (titleText != null)
        {
            titleText.text = deathTitle;
            titleText.gameObject.SetActive(false);
        }

        if (currencyText != null)
        {
            currencyText.text = earnedCurrency > 0 ? $"武功秘笈  +{earnedCurrency}" : "";
            currencyText.gameObject.SetActive(false);
        }

        if (returnButton != null)
            returnButton.gameObject.SetActive(false);

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // 1. 배경 페이드인
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            if (canvasGroup != null)
                canvasGroup.alpha = elapsed / fadeInDuration;
            yield return null;
        }

        // 2. 타이틀 등장
        yield return new WaitForSecondsRealtime(textDelay);
        if (titleText != null)
            titleText.gameObject.SetActive(true);

        // 3. 통화 표시
        yield return new WaitForSecondsRealtime(0.5f);
        if (currencyText != null)
            currencyText.gameObject.SetActive(true);

        // 4. 복귀 버튼
        yield return new WaitForSecondsRealtime(1.0f);
        if (returnButton != null)
            returnButton.gameObject.SetActive(true);

        // 커서 해제
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnReturnToHub()
    {
        if (screenRoot != null)
            screenRoot.SetActive(false);

        if (GameManager.Instance != null)
            GameManager.Instance.EndRun(false, 0);
    }
}
