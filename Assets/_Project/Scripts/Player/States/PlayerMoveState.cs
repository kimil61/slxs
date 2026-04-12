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

        if (!player.IsGrounded)
        {
            player.TransitionTo(player.FallState);
            return;
        }

        if (cursorLocked)
        {
            if (player.Input.DodgePressed && player.Stamina != null && player.Stamina.CanConsume(player.dodgeData.staminaCost))
            {
                player.TransitionTo(player.DodgeState);
                return;
            }

            if (player.Input.HeavyAttackPressed && player.Stamina != null && player.heavyAttackData != null
                && player.Stamina.CanConsume(player.heavyAttackData.staminaCost))
            {
                player.TransitionTo(player.HeavyAttackState);
                return;
            }

            if (player.Input.AttackPressed)
            {
                player.TransitionTo(player.LightAttackState);
                return;
            }
        }

        Vector3 moveDir = player.GetCameraRelativeMoveDir();
        if (moveDir.sqrMagnitude < 0.01f)
        {
            player.TransitionTo(player.IdleState);
            return;
        }

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
        player.Animator.SetFloat("Speed", isSprinting ? 2f : 1f);
    }

    public void Exit(PlayerStateMachine player)
    {
        player.Animator.SetFloat("Speed", 0f);
    }
}
