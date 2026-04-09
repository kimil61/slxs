using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ESC로 커서 잠금/해제 토글.
/// PlayerMovement.Start()에 있던 커서 잠금을 여기로 이관.
/// </summary>
public class CursorManager : MonoBehaviour
{
    public bool IsCursorLocked { get; private set; }

    private void Start()
    {
        LockCursor();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (IsCursorLocked)
                UnlockCursor();
            else
                LockCursor();
        }
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        IsCursorLocked = true;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        IsCursorLocked = false;
    }
}