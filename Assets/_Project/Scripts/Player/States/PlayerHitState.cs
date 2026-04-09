using UnityEngine;

public class PlayerHitState : IPlayerState
{
    private float timer;
    private float staggerDuration = 0.4f;
    private Vector3 knockbackDir;
    private float knockbackForce;

    public void Setup(Vector3 direction, float force, float duration = 0.4f)
    {
        knockbackDir = direction;
        knockbackForce = force;
        staggerDuration = duration;
    }

    public void Enter(PlayerStateMachine player)
    {
        timer = 0f;
        player.Animator.SetTrigger("Hit");
        player.CurrentComboIndex = 0;
    }

    public void Update(PlayerStateMachine player)
    {
        timer += Time.deltaTime;

        // 넉백 이동 (감쇠)
        if (knockbackForce > 0f)
        {
            float decay = 1f - (timer / staggerDuration);
            if (decay > 0f)
                player.Move(knockbackDir, knockbackForce * decay);
        }

        if (timer >= staggerDuration)
        {
            if (player.Health != null && player.Health.IsDead)
                player.TransitionTo(player.DeathState);
            else
                player.TransitionTo(player.IdleState);
        }
    }

    public void Exit(PlayerStateMachine player) { }
}
