using UnityEngine;

public class PlayerHeavyAttackState : IPlayerState
{
    private float timer;
    private float clipLength;
    private bool hitboxActive;

    public void Enter(PlayerStateMachine player)
    {
        timer = 0f;
        hitboxActive = false;

        var data = player.heavyAttackData;
        clipLength = data.animationClip != null ? data.animationClip.length : 1.2f;

        // 스태미나 소모
        if (player.Stamina != null)
            player.Stamina.Consume(data.staminaCost);

        player.Animator.SetTrigger("HeavyAttack");
        player.CurrentComboIndex = 0;

        if (player.SoundPlayer != null)
            player.SoundPlayer.PlayWhoosh(data);
    }

    public void Update(PlayerStateMachine player)
    {
        var data = player.heavyAttackData;
        timer += Time.deltaTime;
        float normalized = timer / clipLength;

        // 히트박스 ON/OFF
        if (!hitboxActive && normalized >= data.hitboxActivateTime)
        {
            hitboxActive = true;
            if (player.WeaponHitbox != null)
                player.WeaponHitbox.Activate(data, player.baseDamage);
        }
        if (hitboxActive && normalized >= data.hitboxDeactivateTime)
        {
            hitboxActive = false;
            if (player.WeaponHitbox != null)
                player.WeaponHitbox.Deactivate();
        }

        // 공격 종료
        if (timer >= clipLength)
        {
            CleanUp(player);
            player.TransitionTo(player.IdleState);
        }
    }

    public void Exit(PlayerStateMachine player)
    {
        CleanUp(player);
    }

    private void CleanUp(PlayerStateMachine player)
    {
        if (hitboxActive && player.WeaponHitbox != null)
            player.WeaponHitbox.Deactivate();
        hitboxActive = false;
    }
}
