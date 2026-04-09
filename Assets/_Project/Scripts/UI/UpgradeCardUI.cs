using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 업그레이드 카드 1장. UpgradeSelectScreen에서 3개 생성.
/// </summary>
public class UpgradeCardUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button selectButton;

    private Action onSelected;

    public void Setup(string title, string description, Sprite icon, Action onSelect)
    {
        if (titleText != null) titleText.text = title;
        if (descriptionText != null) descriptionText.text = description;
        if (iconImage != null && icon != null) iconImage.sprite = icon;
        onSelected = onSelect;

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => onSelected?.Invoke());
        }
    }
}

/// <summary>
/// 전투 후 업그레이드 3택 화면.
/// </summary>
public class UpgradeSelectScreen : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject screenRoot;
    [SerializeField] private UpgradeCardUI[] cards;   // 3장

    public void Show(UpgradeOption[] options)
    {
        if (screenRoot != null)
            screenRoot.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        int count = Mathf.Min(options.Length, cards.Length);
        for (int i = 0; i < cards.Length; i++)
        {
            if (i < count)
            {
                cards[i].gameObject.SetActive(true);
                var option = options[i];
                cards[i].Setup(option.title, option.description, option.icon, () => OnSelected(option));
            }
            else
            {
                cards[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnSelected(UpgradeOption option)
    {
        // TODO: 실제 업그레이드 적용 (RunManager 연동)
        Debug.Log($"[Upgrade] 선택: {option.title}");
        Hide();
    }

    private void Hide()
    {
        if (screenRoot != null)
            screenRoot.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}

[Serializable]
public struct UpgradeOption
{
    public string title;
    public string description;
    public Sprite icon;
    // TODO: 실제 효과 데이터 (RunUpgrade SO 참조)
}
