using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 웨이브 기반 적 스폰. ArenaManager에서 호출.
/// SpawnPoint 배열에서 위치를 뽑아 적 생성.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private SpawnPoint[] spawnPoints;

    private readonly List<GameObject> aliveEnemies = new();

    public int AliveCount => aliveEnemies.Count;
    public bool AllDead => aliveEnemies.Count == 0;

    private void OnEnable()
    {
        EventBus.Subscribe<EnemyDiedEvent>(OnEnemyDied);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EnemyDiedEvent>(OnEnemyDied);
    }

    /// <summary>
    /// 웨이브 하나를 스폰.
    /// </summary>
    public void SpawnWave(SpawnWave wave)
    {
        int spawnIndex = 0;

        foreach (var entry in wave.entries)
        {
            for (int i = 0; i < entry.count; i++)
            {
                if (entry.enemyData == null || entry.enemyData.prefab == null) continue;

                // 스폰 위치 순환
                var point = spawnPoints[spawnIndex % spawnPoints.Length];
                spawnIndex++;

                var enemy = Instantiate(entry.enemyData.prefab, point.Position, point.Rotation);

                // EnemyStateMachine에 데이터 주입
                var sm = enemy.GetComponent<EnemyStateMachine>();
                if (sm != null)
                    sm.data = entry.enemyData;

                aliveEnemies.Add(enemy);
            }
        }
    }

    /// <summary>
    /// 전체 웨이브 시퀀스 실행.
    /// </summary>
    public IEnumerator SpawnAllWaves(SpawnWave[] waves)
    {
        foreach (var wave in waves)
        {
            if (wave.delayBeforeWave > 0f)
                yield return new WaitForSeconds(wave.delayBeforeWave);

            SpawnWave(wave);

            // 이 웨이브의 적이 전부 죽을 때까지 대기
            yield return new WaitUntil(() => AllDead);
        }
    }

    public void ClearAll()
    {
        foreach (var enemy in aliveEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        aliveEnemies.Clear();
    }

    private void OnEnemyDied(EnemyDiedEvent e)
    {
        aliveEnemies.Remove(e.enemy);
    }
}
