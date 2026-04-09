using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveState : IPlayerState
{
    public void Enter(PlayerStateMachine player)
    {
    }

    public void Update(PlayerStateMachine player)
    {
        bool cursorLocked = player.CursorManager == null || player.CursorManager.IsCursorLocked;

        // 낙하
        if (!player.IsGrounded)
        {
            player.TransitionTo(player.FallState);
            return;
        }

        if (cursorLocked)
        {
            // 구르기
            if (player.Input.JumpPressed && player.Stamina != null && player.Stamina.CanConsume(player.dodgeData.staminaCost))
            {
                player.TransitionTo(player.DodgeState);
                return;
            }

            // 공격
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                player.TransitionTo(player.LightAttackState);
                return;
            }
        }

        // 이동 입력 없으면 Idle
        Vector3 moveDir = player.GetCameraRelativeMoveDir();
        if (moveDir.sqrMagnitude < 0.01f)
        {
            player.TransitionTo(player.IdleState);
            return;
        }

        // 스프린트 체크 (Shift 홀드 + 스태미나 있음)
        bool isSprinting = Keyboard.current != null
            && Keyboard.current.leftShiftKey.isPressed
            && player.Stamina != null
            && player.Stamina.Current > 0f;

        float speed;
        if (isSprinting)
        {
            speed = player.sprintSpeed;
            if (player.Stamina != null)
                player.Stamina.Consume(player.Stamina.Data.sprintCostPerSec * Time.deltaTime);
        }
        else
        {
            speed = player.runSpeed;
        }

        player.Move(moveDir, speed);
        player.RotateTowards(moveDir);

        // Animator
        player.Animator.SetFloat("Speed", isSprinting ? 2f : 1f);
    }

    public void Exit(PlayerStateMachine player)
    {
        player.Animator.SetFloat("Speed", 0f);
    }
}
