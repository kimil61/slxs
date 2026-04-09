using UnityEngine;

public class EnemyStaggerState : IEnemyState
{
    private float timer;
    private Vector3 knockbackDir;
    private float knockbackForce;
    private float duration;

    public void Setup(Transform attacker, AttackData attackData)
    {
        if (attackData != null)
            knockbackForce = attackData.knockbackForce;
        else
            knockbackForce = 0f;
    }

    public void Enter(EnemyStateMachine enemy)
    {
        timer = 0f;
        duration = enemy.data.staggerDuration;

        enemy.Agent.isStopped = true;
        enemy.Animator.SetTrigger("Hit");

        // 넉백 방향 (공격자 → 적)
        if (enemy.Target != null)
        {
            knockbackDir = (enemy.transform.position - enemy.Target.position).normalized;
            knockbackDir.y = 0f;
        }
    }

    public void Update(EnemyStateMachine enemy)
    {
        timer += Time.deltaTime;

        // 넉백 이동 (NavMesh 위에서)
        if (knockbackForce > 0f)
        {
            float decay = 1f - (timer / duration);
            if (decay > 0f && enemy.Agent.isOnNavMesh)
            {
                Vector3 offset = knockbackDir * knockbackForce * decay * Time.deltaTime;
                enemy.Agent.Move(offset);
            }
        }

        if (timer >= duration)
        {
            enemy.Agent.isStopped = false;
            enemy.TransitionTo(enemy.ChaseState);
        }
    }

    public void Exit(EnemyStateMachine enemy)
    {
        enemy.Agent.isStopped = false;
    }
}
