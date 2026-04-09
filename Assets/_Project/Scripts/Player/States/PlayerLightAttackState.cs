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

        // 콤보 인덱스에 맞는 AttackData 가져오기
        int index = Mathf.Clamp(player.CurrentComboIndex, 0, player.lightAttacks.Length - 1);
        currentAttack = player.lightAttacks[index];

        // 애니메이션 클립 길이 (없으면 기본값)
        clipLength = currentAttack.animationClip != null ? currentAttack.animationClip.length : 0.6f;

        player.Animator.SetTrigger("Attack");

        // whoosh 사운드 (Sound Layering 1층)
        if (player.SoundPlayer != null)
            player.SoundPlayer.PlayWhoosh(currentAttack);
    }

    public void Update(PlayerStateMachine player)
    {
        timer += Time.deltaTime;
        float normalized = timer / clipLength;

        // 히트박스 ON/OFF 타이밍
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

        // 콤보 윈도우 내 입력 감지
        if (normalized >= currentAttack.comboWindowStart && normalized <= currentAttack.comboWindowEnd)
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                comboQueued = true;
        }

        // 구르기 캔슬 (콤보 윈도우에서)
        if (normalized >= currentAttack.comboWindowStart)
        {
            if (player.Input.JumpPressed && player.Stamina != null && player.Stamina.CanConsume(player.dodgeData.staminaCost))
            {
                CleanUp(player);
                player.TransitionTo(player.DodgeState);
                return;
            }
        }

        // 공격 종료
        if (timer >= clipLength)
        {
            CleanUp(player);

            if (comboQueued && player.CurrentComboIndex < player.lightAttacks.Length - 1)
            {
                // 다음 콤보로 연결
                player.CurrentComboIndex++;
                player.TransitionTo(player.LightAttackState);
            }
            else
            {
                // 콤보 끝 → Idle
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
