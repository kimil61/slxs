using UnityEngine;

/// <summary>
/// 런 설정 데이터. 구역 시퀀스, 난이도 배율 등.
/// 런마다 다른 구성을 원하면 여러 에셋 생성.
/// </summary>
[CreateAssetMenu(fileName = "NewRunData", menuName = "색랑하산/Level/RunData")]
public class RunData : ScriptableObject
{
    [Header("구역 구성")]
    public AreaData[] areaSequence;

    [Header("난이도")]
    [Tooltip("적 HP/데미지 배율 (런 차수에 따라 증가)")]
    public float difficultyMultiplier = 1.0f;

    [Header("시드")]
    [Tooltip("0이면 랜덤 시드")]
    public int seed;

    /// <summary>
    /// 유효한 시드 반환. 0이면 랜덤 생성.
    /// </summary>
    public int GetSeed()
    {
        return seed != 0 ? seed : Random.Range(1, int.MaxValue);
    }
}
