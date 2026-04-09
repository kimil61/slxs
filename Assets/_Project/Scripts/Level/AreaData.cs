using UnityEngine;

/// <summary>
/// 구역(Arena) 데이터. 적 배치, 보상, 다음 구역 등.
/// Assets/_Project/Data/ScriptableObjects/AreaData/ 에 에셋 생성.
/// </summary>
[CreateAssetMenu(fileName = "NewAreaData", menuName = "색랑하산/Level/AreaData")]
public class AreaData : ScriptableObject
{
    [Header("기본 정보")]
    public string areaName;
    [TextArea] public string description;

    [Header("지형")]
    [Tooltip("이 구역의 Arena Prefab")]
    public GameObject arenaPrefab;

    [Header("적 스폰")]
    public SpawnWave[] waves;

    [Header("클리어 보상")]
    public int currencyReward;
    public bool showUpgradeSelect = true;

    [Header("구역 연결")]
    public bool isBossArea;
}

/// <summary>
/// 스폰 웨이브. 한 웨이브 내 적 구성.
/// </summary>
[System.Serializable]
public struct SpawnWave
{
    public SpawnEntry[] entries;
    [Tooltip("이전 웨이브 클리어 후 대기 시간")]
    public float delayBeforeWave;
}

[System.Serializable]
public struct SpawnEntry
{
    public EnemyData enemyData;
    public int count;
}
