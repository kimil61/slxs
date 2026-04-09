using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 일시정지 메뉴. ESC 또는 GameManager.TogglePause()로 토글.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject menuRoot;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;

    private void Start()
    {
        if (menuRoot != null)
            menuRoot.SetActive(false);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResume);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenu);
    }

    public void Show()
    {
        if (menuRoot != null)
            menuRoot.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Hide()
    {
        if (menuRoot != null)
            menuRoot.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnResume()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.TogglePause();
    }

    private void OnMainMenu()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
            GameManager.Instance.ChangeState(GameState.MainMenu);
    }
}
