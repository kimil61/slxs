using UnityEngine;

public class PlayerHeavyAttackState : IPlayerState
{
    private float timer;
    private AttackData currentAttack;
    private bool hitboxActive;

    public void Enter(PlayerStateMachine player)
    {
        timer = 0f;
        hitboxActive = false;

        currentAttack = player.heavyAttackData;
        if (currentAttack == null)
        {
            player.TransitionTo(player.IdleState);
            return;
        }

        // 스태미나 소모
        if (player.Stamina != null)
            player.Stamina.Consume(currentAttack.staminaCost);

        player.Animator.SetTrigger("HeavyAttack");
        player.CurrentComboIndex = 0;

        if (player.SoundPlayer != null)
            player.SoundPlayer.PlayWhoosh(currentAttack);
    }

    public void Update(PlayerStateMachine player)
    {
        if (currentAttack == null)
            return;

        timer += Time.deltaTime;

        // 히트박스 ON/OFF
        if (!hitboxActive && timer >= currentAttack.hitboxStartTime)
        {
            hitboxActive = true;
            if (player.WeaponHitbox != null)
                player.WeaponHitbox.Activate(currentAttack, player.baseDamage);
        }
        if (hitboxActive && timer >= currentAttack.hitboxEndTime)
        {
            hitboxActive = false;
            if (player.WeaponHitbox != null)
                player.WeaponHitbox.Deactivate();
        }

        // 공격 종료
        if (timer >= currentAttack.dodgeCancelTime)
        {
            if (player.Input.DodgePressed && player.Stamina != null && player.Stamina.CanConsume(player.dodgeData.staminaCost))
            {
                CleanUp(player);
                player.TransitionTo(player.DodgeState);
                return;
            }
        }

        if (timer >= currentAttack.moveRecoveryTime && player.Input.MoveInput.sqrMagnitude > 0.01f)
        {
            CleanUp(player);
            player.TransitionTo(player.MoveState);
            return;
        }

        if (timer >= Mathf.Max(currentAttack.totalDuration, 0.01f))
        {
            CleanUp(player);
            player.TransitionTo(player.Input.MoveInput.sqrMagnitude > 0.01f ? player.MoveState : player.IdleState);
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
