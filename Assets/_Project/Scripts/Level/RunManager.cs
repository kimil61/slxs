using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1런 전체 라이프사이클 관리.
/// 산 입구 → 전투구역 1 → 전투구역 2 → (선택) → 보스 → 하산.
/// GamePlay 씬에 배치.
/// </summary>
public class RunManager : MonoBehaviour
{
    [Header("구역 시퀀스")]
    [SerializeField] private AreaData[] areaSequence;   // 순서대로 진행할 구역들

    [Header("References")]
    [SerializeField] private Transform playerSpawn;
    [SerializeField] private HUDManager hudManager;

    [Header("Runtime")]
    private int currentAreaIndex;
    private int earnedCurrency;
    private float runTimer;
    private GameObject currentArenaInstance;

    private readonly List<ArenaManager> arenas = new();

    private void Start()
    {
        currentAreaIndex = 0;
        earnedCurrency = 0;
        runTimer = 0f;

        EventBus.Subscribe<AreaClearedEvent>(OnAreaCleared);
        EventBus.Subscribe<PlayerDiedEvent>(OnPlayerDied);

        // 첫 구역 로드
        LoadArea(currentAreaIndex);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<AreaClearedEvent>(OnAreaCleared);
        EventBus.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
    }

    private void Update()
    {
        runTimer += Time.deltaTime;
    }

    private void LoadArea(int index)
    {
        if (index >= areaSequence.Length)
        {
            // 모든 구역 클리어 = 런 성공
            RunComplete(true);
            return;
        }

        var areaData = areaSequence[index];
        if (areaData.arenaPrefab == null)
        {
            Debug.LogError($"[RunManager] Area {index} ({areaData.areaName}) has no arena prefab!");
            return;
        }

        // 이전 Arena 정리
        if (currentArenaInstance != null)
            Destroy(currentArenaInstance);

        // Arena Prefab 생성
        currentArenaInstance = Instantiate(areaData.arenaPrefab);
        var arena = currentArenaInstance.GetComponent<ArenaManager>();
        if (arena != null)
            arenas.Add(arena);

        Debug.Log($"[RunManager] 구역 {index + 1}/{areaSequence.Length}: {areaData.areaName}");
    }

    private void OnAreaCleared(AreaClearedEvent e)
    {
        var areaData = areaSequence[currentAreaIndex];
        earnedCurrency += areaData.currencyReward;

        // 보스 구역이었으면 런 성공
        if (areaData.isBossArea)
        {
            RunComplete(true);
            return;
        }

        // 업그레이드 선택 표시 (TODO: UpgradeSelectScreen 연동)
        if (areaData.showUpgradeSelect)
        {
            Debug.Log("[RunManager] 업그레이드 선택 화면 표시 (TODO)");
        }

        // 다음 구역으로
        currentAreaIndex++;
        LoadArea(currentAreaIndex);
    }

    private void OnPlayerDied(PlayerDiedEvent e)
    {
        RunComplete(false);
    }

    private void RunComplete(bool victory)
    {
        float minutes = runTimer / 60f;
        Debug.Log($"[RunManager] 런 {(victory ? "성공" : "실패")} — {minutes:F1}분, 통화 {earnedCurrency}");

        // 영구 저장
        var save = SaveSystem.Load();
        save.currency += earnedCurrency;
        save.totalRuns++;
        if (currentAreaIndex > save.bestAreaReached)
            save.bestAreaReached = currentAreaIndex;
        SaveSystem.Save(save);

        // GameManager를 통해 Hub로 복귀
        if (GameManager.Instance != null)
            GameManager.Instance.EndRun(victory, earnedCurrency);
    }
}
