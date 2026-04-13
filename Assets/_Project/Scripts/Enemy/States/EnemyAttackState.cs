using UnityEngine;

public class EnemyAttackState : IEnemyState
{
    private AttackData currentAttack;
    private float timer;
    private bool hitboxActive;

    public void Enter(EnemyStateMachine enemy)
    {
        timer = 0f;
        hitboxActive = false;

        enemy.Agent.isStopped = true;
        enemy.LookAtTarget();

        // 랜덤 공격 패턴 선택 (플레이어와 같은 AttackData 구조)
        currentAttack = enemy.GetRandomAttack();

        enemy.Animator.SetTrigger("Attack");

        // whoosh
        if (enemy.SoundPlayer != null && currentAttack != null)
            enemy.SoundPlayer.PlayWhoosh(currentAttack);
    }

    public void Update(EnemyStateMachine enemy)
    {
        timer += Time.deltaTime;

        if (currentAttack != null)
        {
            // 히트박스 ON
            if (!hitboxActive && timer >= currentAttack.hitboxStartTime)
            {
                hitboxActive = true;
                if (enemy.WeaponHitbox != null)
                    enemy.WeaponHitbox.Activate(currentAttack, enemy.data.baseDamage);
            }

            // 히트박스 OFF
            if (hitboxActive && timer >= currentAttack.hitboxEndTime)
            {
                hitboxActive = false;
                if (enemy.WeaponHitbox != null)
                    enemy.WeaponHitbox.Deactivate();
            }
        }

        // 공격 종료
        if (timer >= Mathf.Max(currentAttack != null ? currentAttack.totalDuration : 1f, 0.01f))
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
