public class PlayerDeathState : IPlayerState
{
    public void Enter(PlayerStateMachine player)
    {
        player.Animator.SetBool("isDead", true);
        player.Controller.enabled = false;

        EventBus.Publish(new PlayerDiedEvent());
    }

    public void Update(PlayerStateMachine player)
    {
        // 사망 상태에서는 입력 무시. UI에서 Hub 복귀 처리.
    }

    public void Exit(PlayerStateMachine player)
    {
        player.Animator.SetBool("isDead", false);
        player.Controller.enabled = true;
    }
}
