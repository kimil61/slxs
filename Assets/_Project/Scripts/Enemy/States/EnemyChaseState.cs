using UnityEngine;

public class EnemyChaseState : IEnemyState
{
    public void Enter(EnemyStateMachine enemy)
    {
        enemy.Agent.speed = enemy.data.moveSpeed;
        enemy.Agent.isStopped = false;
    }

    public void Update(EnemyStateMachine enemy)
    {
        // 타겟 잃음 → Idle
        if (enemy.Target == null)
        {
            enemy.TransitionTo(enemy.IdleState);
            return;
        }

        enemy.Agent.SetDestination(enemy.Target.position);
        enemy.Animator.SetFloat("Speed", enemy.Agent.velocity.magnitude / enemy.data.moveSpeed);

        // 공격 범위 진입 + 쿨다운 완료 → 공격
        float dist = enemy.DistanceToTarget();
        if (dist <= enemy.data.attackRange && enemy.AttackCooldownTimer <= 0f)
        {
            enemy.TransitionTo(enemy.AttackState);
            return;
        }
    }

    public void Exit(EnemyStateMachine enemy) { }
}
