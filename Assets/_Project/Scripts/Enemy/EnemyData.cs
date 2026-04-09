using UnityEngine;

/// <summary>
/// 적 데이터 SO. 산적/궁수/두목 등 적 타입별로 에셋 생성.
/// Assets/_Project/Data/ScriptableObjects/EnemyData/ 에 배치.
/// </summary>
[CreateAssetMenu(fileName = "NewEnemyData", menuName = "색랑하산/Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("기본 스탯")]
    public string enemyName;
    public float maxHP = 100f;
    public float moveSpeed = 3.5f;

    [Header("감지")]
    public float detectRange = 10f;
    public float attackRange = 2f;
    public float loseTargetRange = 15f;

    [Header("전투")]
    public float attackCooldown = 2.0f;
    public float staggerDuration = 0.3f;
    [Tooltip("이 적이 사용하는 공격 패턴 (AttackData SO)")]
    public AttackData[] attackPatterns;
    public float baseDamage = 10f;

    [Header("패트롤")]
    public float patrolSpeed = 1.5f;
    public float patrolWaitTime = 2f;

    [Header("드롭")]
    public DropEntry[] dropTable;

    [Header("프리팹")]
    public GameObject prefab;
}

[System.Serializable]
public struct DropEntry
{
    public string itemId;
    [Range(0f, 1f)] public float chance;
    public int minCount;
    public int maxCount;
}
