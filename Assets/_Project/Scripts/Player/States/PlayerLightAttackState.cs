using UnityEngine;

public class PlayerLightAttackState : IPlayerState
{
    private float timer;
    private AttackData currentAttack;
    private bool hitboxActive;
    private bool comboQueued;
    private bool comboTriggered;

    public void Enter(PlayerStateMachine player)
    {
        timer = 0f;
        hitboxActive = false;
        comboQueued = false;
        comboTriggered = false;

        if (player.lightAttacks == null || player.lightAttacks.Length == 0)
        {
            player.TransitionTo(player.IdleState);
            return;
        }

        int index = Mathf.Clamp(player.CurrentComboIndex, 0, player.lightAttacks.Length - 1);
        currentAttack = player.lightAttacks[index];
        if (currentAttack == null)
        {
            player.TransitionTo(player.IdleState);
            return;
        }

        player.Animator.SetTrigger("Attack");

        if (player.Stamina != null)
            player.Stamina.Consume(currentAttack.staminaCost);

        if (player.SoundPlayer != null)
            player.SoundPlayer.PlayWhoosh(currentAttack);
    }

    public void Update(PlayerStateMachine player)
    {
        if (currentAttack == null)
            return;

        timer += Time.deltaTime;
        float totalDuration = Mathf.Max(currentAttack.totalDuration, 0.01f);

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

        if (timer >= currentAttack.comboWindowStartTime && timer <= currentAttack.comboWindowEndTime)
        {
            if (player.ConsumeBufferedAttackInput())
                comboQueued = true;
        }

        if (comboQueued && !comboTriggered && timer >= currentAttack.comboWindowEndTime)
        {
            StartNextCombo(player);
            return;
        }

        if (timer >= currentAttack.dodgeCancelTime)
        {
            if (player.Input.DodgePressed && player.Stamina != null && player.Stamina.CanConsume(player.dodgeData.staminaCost))
            {
                CleanUp(player);
                player.TransitionTo(player.DodgeState);
                return;
            }
        }

        if (!comboQueued && timer >= currentAttack.moveRecoveryTime && player.Input.MoveInput.sqrMagnitude > 0.01f)
        {
            CleanUp(player);
            player.CurrentComboIndex = 0;
            player.TransitionTo(player.MoveState);
            return;
        }

        if (timer >= totalDuration)
        {
            if (comboQueued && player.CurrentComboIndex < player.lightAttacks.Length - 1)
            {
                StartNextCombo(player);
            }
            else
            {
                CleanUp(player);
                player.CurrentComboIndex = 0;
                player.TransitionTo(player.Input.MoveInput.sqrMagnitude > 0.01f ? player.MoveState : player.IdleState);
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

    private void StartNextCombo(PlayerStateMachine player)
    {
        if (comboTriggered || player.CurrentComboIndex >= player.lightAttacks.Length - 1)
            return;

        comboTriggered = true;
        CleanUp(player);
        player.CurrentComboIndex++;
        player.TransitionTo(player.LightAttackState);
    }
}
