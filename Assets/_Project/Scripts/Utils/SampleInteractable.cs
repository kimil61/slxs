using UnityEngine;

/// <summary>
/// 테스트용 상호작용 오브젝트.
/// Collider 있는 오브젝트에 붙이고, interactableLayer에 해당 레이어 설정할 것.
/// </summary>
public class SampleInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt = "조사하기 [E / 클릭]";
    [SerializeField] private Color highlightColor = Color.yellow;

    private Renderer objectRenderer;
    private Color originalColor;

    public string InteractionPrompt => prompt;

    private void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
            originalColor = objectRenderer.material.color;
    }

    public void Interact()
    {
        Debug.Log($"[Interact] {gameObject.name} 상호작용 실행!");
        // TODO: 실제 로직 (아이템 획득, 대화 시작 등)
    }

    public void OnFocusEnter()
    {
        if (objectRenderer != null)
            objectRenderer.material.color = highlightColor;
    }

    public void OnFocusExit()
    {
        if (objectRenderer != null)
            objectRenderer.material.color = originalColor;
    }
}