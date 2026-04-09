using UnityEngine;
using TMPro;

/// <summary>
/// 월드스페이스 데미지 숫자 팝업. 피격 지점에서 위로 떠오르며 사라짐.
/// Prefab으로 만들어 ObjectPool에서 스폰.
/// Canvas(World Space) + TextMeshPro 구성.
/// </summary>
public class DamagePopup : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float floatSpeed = 1.5f;
    [SerializeField] private float lifetime = 0.8f;
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] private float scaleUpAmount = 0.3f;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color criticalColor = Color.yellow;

    private TextMeshProUGUI text;
    private float timer;
    private Vector3 startScale;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        startScale = transform.localScale;
    }

    public void Setup(float damage, bool isCritical = false)
    {
        timer = 0f;
        transform.localScale = startScale;

        if (text != null)
        {
            text.text = Mathf.CeilToInt(damage).ToString();
            text.color = isCritical ? criticalColor : normalColor;

            if (isCritical)
                transform.localScale = startScale * (1f + scaleUpAmount);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // 위로 떠오르기
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // 카메라 쪽을 바라봄
        if (Camera.main != null)
            transform.forward = Camera.main.transform.forward;

        // 페이드아웃
        if (text != null)
        {
            float alpha = 1f - (timer / lifetime);
            var color = text.color;
            color.a = Mathf.Max(0f, alpha);
            text.color = color;
        }

        // 수명 종료
        if (timer >= lifetime)
            gameObject.SetActive(false);
    }
}
