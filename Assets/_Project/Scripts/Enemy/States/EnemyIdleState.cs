using UnityEngine;

public class EnemyIdleState : IEnemyState
{
    private float waitTimer;

    public void Enter(EnemyStateMachine enemy)
    {
        enemy.Agent.ResetPath();
        enemy.Animator.SetFloat("Speed", 0f);
        waitTimer = 0f;
    }

    public void Update(EnemyStateMachine enemy)
    {
        // 플레이어 감지 → 추격
        if (enemy.Target != null)
        {
            enemy.TransitionTo(enemy.ChaseState);
            return;
        }

        // 웨이포인트 있으면 일정 시간 후 패트롤
        if (enemy.waypoints.Length > 0)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= enemy.data.patrolWaitTime)
            {
                enemy.TransitionTo(enemy.PatrolState);
                return;
            }
        }
    }

    public void Exit(EnemyStateMachine enemy) { }
}
