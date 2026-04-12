using UnityEngine;

public class PlayerLightAttackState : IPlayerState
{
    private float timer;
    private float clipLength;
    private AttackData currentAttack;
    private bool hitboxActive;
    private bool comboQueued;

    public void Enter(PlayerStateMachine player)
    {
        timer = 0f;
        hitboxActive = false;
        comboQueued = false;

        int index = Mathf.Clamp(player.CurrentComboIndex, 0, player.lightAttacks.Length - 1);
        currentAttack = player.lightAttacks[index];
        clipLength = currentAttack.animationClip != null ? currentAttack.animationClip.length : 0.6f;

        player.Animator.SetTrigger("Attack");

        if (player.SoundPlayer != null)
            player.SoundPlayer.PlayWhoosh(currentAttack);
    }

    public void Update(PlayerStateMachine player)
    {
        timer += Time.deltaTime;
        float normalized = timer / clipLength;

        if (!hitboxActive && normalized >= currentAttack.hitboxActivateTime)
        {
            hitboxActive = true;
            if (player.WeaponHitbox != null)
                player.WeaponHitbox.Activate(currentAttack, player.baseDamage);
        }

        if (hitboxActive && normalized >= currentAttack.hitboxDeactivateTime)
        {
            hitboxActive = false;
            if (player.WeaponHitbox != null)
                player.WeaponHitbox.Deactivate();
        }

        if (normalized >= currentAttack.comboWindowStart && normalized <= currentAttack.comboWindowEnd)
        {
            if (player.Input.AttackPressed)
                comboQueued = true;
        }

        if (normalized >= currentAttack.comboWindowStart)
        {
            if (player.Input.DodgePressed && player.Stamina != null && player.Stamina.CanConsume(player.dodgeData.staminaCost))
            {
                CleanUp(player);
                player.TransitionTo(player.DodgeState);
                return;
            }
        }

        if (timer >= clipLength)
        {
            CleanUp(player);

            if (comboQueued && player.CurrentComboIndex < player.lightAttacks.Length - 1)
            {
                player.CurrentComboIndex++;
                player.TransitionTo(player.LightAttackState);
            }
            else
            {
                player.CurrentComboIndex = 0;
                player.TransitionTo(player.IdleState);
            }
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
