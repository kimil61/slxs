using System.Collections;
using UnityEngine;

/// <summary>
/// Arena 하나를 관리. 진입 → 웨이브 스폰 → 클리어 → 출구 열림.
/// Arena Prefab 루트에 붙일 것.
/// </summary>
public class ArenaManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private AreaData areaData;

    [Header("References")]
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private GameObject exitBlocker;     // 클리어 전 출구 막는 오브젝트
    [SerializeField] private GameObject exitPath;        // 클리어 후 열리는 길

    [Header("Entry Trigger")]
    [SerializeField] private Collider entryTrigger;      // 플레이어 진입 감지용

    private bool isActive;
    private bool isCleared;

    public bool IsCleared => isCleared;

    private void Start()
    {
        // 초기 상태: 출구 잠김
        if (exitBlocker != null) exitBlocker.SetActive(true);
        if (exitPath != null) exitPath.SetActive(false);
    }

    /// <summary>
    /// 플레이어가 Arena에 진입하면 호출 (Trigger 또는 RunManager에서).
    /// </summary>
    public void Activate()
    {
        if (isActive || isCleared) return;
        isActive = true;

        StartCoroutine(ArenaSequence());
    }

    private IEnumerator ArenaSequence()
    {
        // 진입 트리거 비활성화
        if (entryTrigger != null)
            entryTrigger.enabled = false;

        // 웨이브 스폰 + 클리어 대기
        if (spawner != null && areaData.waves != null)
            yield return spawner.SpawnAllWaves(areaData.waves);

        // 구역 클리어!
        OnCleared();
    }

    private void OnCleared()
    {
        isCleared = true;
        isActive = false;

        // 출구 열기
        if (exitBlocker != null) exitBlocker.SetActive(false);
        if (exitPath != null) exitPath.SetActive(true);

        EventBus.Publish(new AreaClearedEvent { areaIndex = -1 });

        Debug.Log($"[Arena] {areaData.areaName} 클리어! 보상: {areaData.currencyReward}");
    }

    // 플레이어 진입 감지 (Trigger 방식)
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Activate();
    }
}
