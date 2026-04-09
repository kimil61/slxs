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
        // 커서 풀려있으면 전투 입력 무시
        bool cursorLocked = player.CursorManager == null || player.CursorManager.IsCursorLocked;

        // 낙하
        if (!player.IsGrounded)
        {
            player.TransitionTo(player.FallState);
            return;
        }

        // 피격/사망은 외부에서 TransitionTo로 전환

        if (cursorLocked)
        {
            // 구르기
            if (player.Input.JumpPressed && player.Stamina != null && player.Stamina.CanConsume(player.dodgeData.staminaCost))
            {
                player.TransitionTo(player.DodgeState);
                return;
            }

            // 공격 (좌클릭) — 추후 InputHandler에 AttackPressed 추가 시 교체
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                player.TransitionTo(player.LightAttackState);
                return;
            }
        }

        // 이동 입력 있으면 MoveState
        if (player.Input.MoveInput.sqrMagnitude > 0.01f)
        {
            player.TransitionTo(player.MoveState);
            return;
        }
    }

    public void Exit(PlayerStateMachine player) { }
}
