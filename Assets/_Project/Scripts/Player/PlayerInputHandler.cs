using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool InteractPressed { get; private set; }

    private PlayerControls controls;


    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Enable();

        controls.GamePlay.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        controls.GamePlay.Move.canceled += ctx => MoveInput = Vector2.zero;

        controls.GamePlay.Look.performed += ctx => LookInput = ctx.ReadValue<Vector2>();
        controls.GamePlay.Look.canceled += ctx => LookInput = Vector2.zero;

        controls.GamePlay.Jump.performed += ctx => JumpPressed = true;

        controls.GamePlay.Interact.performed += ctx => InteractPressed = true;
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void LateUpdate()
    {
        JumpPressed = false;
        InteractPressed = false;
    }
}
