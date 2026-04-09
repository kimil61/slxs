public class PlayerFallState : IPlayerState
{
    public void Enter(PlayerStateMachine player)
    {
        player.Animator.SetBool("isGrounded", false);
    }

    public void Update(PlayerStateMachine player)
    {
        // 공중에서도 약간의 방향 제어
        var moveDir = player.GetCameraRelativeMoveDir();
        if (moveDir.sqrMagnitude > 0.01f)
        {
            player.Move(moveDir, player.walkSpeed * 0.5f);
            player.RotateTowards(moveDir);
        }

        // 착지
        if (player.IsGrounded)
        {
            player.Animator.SetBool("isGrounded", true);
            player.TransitionTo(player.IdleState);
        }
    }

    public void Exit(PlayerStateMachine player)
    {
        player.Animator.SetBool("isGrounded", true);
    }
}
