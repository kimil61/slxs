using System;
using System.IO;
using UnityEngine;

/// <summary>
/// JSON 기반 영구 진행도 저장/로드.
/// 저장 경로: Application.persistentDataPath/save.json
/// </summary>
public static class SaveSystem
{
    private const string FILE_NAME = "save.json";
    private static string FilePath => Path.Combine(Application.persistentDataPath, FILE_NAME);

    public static void Save(SaveData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(FilePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Save failed: {e.Message}");
        }
    }

    public static SaveData Load()
    {
        if (!File.Exists(FilePath))
            return new SaveData();

        try
        {
            string json = File.ReadAllText(FilePath);
            return JsonUtility.FromJson<SaveData>(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Load failed: {e.Message}");
            return new SaveData();
        }
    }

    public static bool HasSave()
    {
        return File.Exists(FilePath);
    }

    public static void Delete()
    {
        if (File.Exists(FilePath))
            File.Delete(FilePath);
    }
}

/// <summary>
/// 영구 저장 데이터. 런 간 유지되는 메타 진행도.
/// </summary>
[Serializable]
public class SaveData
{
    public int currency;                  // 武功秘笈 (영구 통화)
    public int totalRuns;                 // 총 런 횟수
    public int bestAreaReached;           // 최고 도달 구역
    public bool[] unlockedUpgrades = new bool[20]; // 영구 업그레이드 해금 상태
}
