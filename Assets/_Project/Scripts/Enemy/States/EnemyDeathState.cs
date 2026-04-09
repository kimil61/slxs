using UnityEngine;

public class EnemyDeathState : IEnemyState
{
    public void Enter(EnemyStateMachine enemy)
    {
        enemy.Agent.isStopped = true;
        enemy.Agent.enabled = false;
        enemy.Animator.SetBool("isDead", true);

        // 히트박스 비활성화
        if (enemy.WeaponHitbox != null)
            enemy.WeaponHitbox.Deactivate();

        // 콜라이더 비활성화 (타격 판정 제거)
        var colliders = enemy.GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
            col.enabled = false;

        // 드롭 처리
        SpawnDrops(enemy);

        // 이벤트 발행
        EventBus.Publish(new EnemyDiedEvent
        {
            enemy = enemy.gameObject,
            position = enemy.transform.position
        });

        // 일정 시간 후 제거
        Object.Destroy(enemy.gameObject, 3f);
    }

    public void Update(EnemyStateMachine enemy)
    {
        // 사망 상태에서는 아무 것도 하지 않음
    }

    public void Exit(EnemyStateMachine enemy) { }

    private void SpawnDrops(EnemyStateMachine enemy)
    {
        if (enemy.data.dropTable == null) return;

        foreach (var drop in enemy.data.dropTable)
        {
            if (Random.value <= drop.chance)
            {
                int count = Random.Range(drop.minCount, drop.maxCount + 1);
                // TODO: 아이템 스폰 시스템 연동
                Debug.Log($"[Drop] {drop.itemId} x{count} at {enemy.transform.position}");
            }
        }
    }
}
