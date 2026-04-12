using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpPressed => DodgePressed;
    public bool DodgePressed { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool HeavyAttackPressed { get; private set; }
    public bool InteractPressed { get; private set; }

    private InputActionMap gameplayMap;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction dodgeAction;
    private InputAction attackAction;
    private InputAction interactAction;

    private void Awake()
    {
        gameplayMap = new InputActionMap("GamePlay");

        moveAction = gameplayMap.AddAction("Move", InputActionType.Value);
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        lookAction = gameplayMap.AddAction("Look", InputActionType.Value);
        lookAction.AddBinding("<Mouse>/delta");

        dodgeAction = gameplayMap.AddAction("Dodge", InputActionType.Button);
        dodgeAction.AddBinding("<Keyboard>/space");

        attackAction = gameplayMap.AddAction("Attack", InputActionType.Button);
        attackAction.AddBinding("<Mouse>/leftButton");

        interactAction = gameplayMap.AddAction("Interact", InputActionType.Button);
        interactAction.AddBinding("<Keyboard>/e");
    }

    private void OnEnable()
    {
        gameplayMap.Enable();

        moveAction.performed += OnMovePerformed;
        moveAction.canceled += OnMoveCanceled;

        lookAction.performed += OnLookPerformed;
        lookAction.canceled += OnLookCanceled;

        dodgeAction.performed += OnDodgePerformed;
        attackAction.performed += OnAttackPerformed;
        interactAction.performed += OnInteractPerformed;
    }

    private void OnDisable()
    {
        moveAction.performed -= OnMovePerformed;
        moveAction.canceled -= OnMoveCanceled;

        lookAction.performed -= OnLookPerformed;
        lookAction.canceled -= OnLookCanceled;

        dodgeAction.performed -= OnDodgePerformed;
        attackAction.performed -= OnAttackPerformed;
        interactAction.performed -= OnInteractPerformed;

        gameplayMap.Disable();
    }

    private void LateUpdate()
    {
        DodgePressed = false;
        AttackPressed = false;
        HeavyAttackPressed = false;
        InteractPressed = false;
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        MoveInput = Vector2.zero;
    }

    private void OnLookPerformed(InputAction.CallbackContext ctx)
    {
        LookInput = ctx.ReadValue<Vector2>();
    }

    private void OnLookCanceled(InputAction.CallbackContext ctx)
    {
        LookInput = Vector2.zero;
    }

    private void OnDodgePerformed(InputAction.CallbackContext ctx)
    {
        DodgePressed = true;
    }

    private void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
        bool shiftHeld = Keyboard.current != null &&
            (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed);

        if (shiftHeld)
            HeavyAttackPressed = true;
        else
            AttackPressed = true;
    }

    private void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        InteractPressed = true;
    }
}
