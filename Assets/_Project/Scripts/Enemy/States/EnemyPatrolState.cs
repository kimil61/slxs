using UnityEngine;

public class EnemyPatrolState : IEnemyState
{
    public void Enter(EnemyStateMachine enemy)
    {
        enemy.Agent.speed = enemy.data.patrolSpeed;
        enemy.Agent.isStopped = false;
        SetNextWaypoint(enemy);
    }

    public void Update(EnemyStateMachine enemy)
    {
        // 플레이어 감지 → 추격
        if (enemy.Target != null)
        {
            enemy.TransitionTo(enemy.ChaseState);
            return;
        }

        enemy.Animator.SetFloat("Speed", enemy.Agent.velocity.magnitude / enemy.data.moveSpeed);

        // 웨이포인트 도착
        if (!enemy.Agent.pathPending && enemy.Agent.remainingDistance < 0.5f)
        {
            enemy.TransitionTo(enemy.IdleState);
        }
    }

    public void Exit(EnemyStateMachine enemy) { }

    private void SetNextWaypoint(EnemyStateMachine enemy)
    {
        if (enemy.waypoints.Length == 0) return;

        enemy.Agent.SetDestination(enemy.waypoints[enemy.CurrentWaypointIndex].position);
        enemy.CurrentWaypointIndex = (enemy.CurrentWaypointIndex + 1) % enemy.waypoints.Length;
    }
}
