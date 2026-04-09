using UnityEngine;

/// <summary>
/// 화면 중심 레이캐스트로 IInteractable 감지 + 하이라이트 + 인터랙션 실행.
/// Player 오브젝트에 붙일 것. CursorManager가 씬에 있어야 함.
/// </summary>
[RequireComponent(typeof(PlayerInputHandler))]
public class InteractionSystem : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("UI (나중에 연결)")]
    [SerializeField] private GameObject promptUI; // 선택. 없으면 무시됨

    private PlayerInputHandler inputHandler;
    private CursorManager cursorManager;
    private Camera mainCamera;
    private IInteractable currentTarget;

    private void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        cursorManager = FindAnyObjectByType<CursorManager>();
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // 커서 풀려있으면 인터랙션 비활성
        if (cursorManager != null && !cursorManager.IsCursorLocked)
        {
            ClearTarget();
            return;
        }

        CheckForInteractable();

        if (inputHandler.InteractPressed && currentTarget != null)
        {
            currentTarget.Interact();
        }
    }

    private void CheckForInteractable()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, interactableLayer))
        {
            var interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                if (interactable != currentTarget)
                {
                    ClearTarget();
                    currentTarget = interactable;
                    currentTarget.OnFocusEnter();
                    SetPromptUI(true, currentTarget.InteractionPrompt);
                }
                return;
            }
        }

        // 아무것도 없으면 포커스 해제
        ClearTarget();
    }

    private void ClearTarget()
    {
        if (currentTarget != null)
        {
            currentTarget.OnFocusExit();
            currentTarget = null;
            SetPromptUI(false, "");
        }
    }

    private void SetPromptUI(bool active, string text)
    {
        if (promptUI == null) return;
        promptUI.SetActive(active);
        // TODO: TextMeshPro 연결 시 텍스트 세팅
    }
}