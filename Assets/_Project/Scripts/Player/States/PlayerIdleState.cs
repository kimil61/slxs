using UnityEngine;

public class PlayerIdleState : IPlayerState
{
    public void Enter(PlayerStateMachine player)
    {
        player.Animator.SetFloat("Speed", 0f);
        player.CurrentComboIndex = 0;
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

            if (player.ConsumeBufferedAttackInput())
            {
                player.TransitionTo(player.LightAttackState);
                return;
            }
        }

        if (player.Input.MoveInput.sqrMagnitude > 0.01f)
        {
            player.TransitionTo(player.MoveState);
        }
    }

    public void Exit(PlayerStateMachine player) { }
}
