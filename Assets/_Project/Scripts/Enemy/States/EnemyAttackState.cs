using UnityEngine;

public class EnemyAttackState : IEnemyState
{
    private AttackData currentAttack;
    private float timer;
    private float clipLength;
    private bool hitboxActive;

    public void Enter(EnemyStateMachine enemy)
    {
        timer = 0f;
        hitboxActive = false;

        enemy.Agent.isStopped = true;
        enemy.LookAtTarget();

        // 랜덤 공격 패턴 선택 (플레이어와 같은 AttackData 구조)
        currentAttack = enemy.GetRandomAttack();
        clipLength = currentAttack != null && currentAttack.animationClip != null
            ? currentAttack.animationClip.length
            : 1.0f;

        enemy.Animator.SetTrigger("Attack");

        // whoosh
        if (enemy.SoundPlayer != null && currentAttack != null)
            enemy.SoundPlayer.PlayWhoosh(currentAttack);
    }

    public void Update(EnemyStateMachine enemy)
    {
        timer += Time.deltaTime;
        float normalized = timer / clipLength;

        if (currentAttack != null)
        {
            // 히트박스 ON
            if (!hitboxActive && normalized >= currentAttack.hitboxActivateTime)
            {
                hitboxActive = true;
                if (enemy.WeaponHitbox != null)
                    enemy.WeaponHitbox.Activate(currentAttack, enemy.data.baseDamage);
            }

            // 히트박스 OFF
            if (hitboxActive && normalized >= currentAttack.hitboxDeactivateTime)
            {
                hitboxActive = false;
                if (enemy.WeaponHitbox != null)
                    enemy.WeaponHitbox.Deactivate();
            }
        }

        // 공격 종료
        if (timer >= clipLength)
        {
            CleanUp(enemy);
            enemy.AttackCooldownTimer = enemy.data.attackCooldown;
            enemy.TransitionTo(enemy.ChaseState);
        }
    }

    public void Exit(EnemyStateMachine enemy)
    {
        CleanUp(enemy);
    }

    private void CleanUp(EnemyStateMachine enemy)
    {
        if (hitboxActive && enemy.WeaponHitbox != null)
            enemy.WeaponHitbox.Deactivate();
        hitboxActive = false;
    }
}
