using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 비동기 씬 전환 유틸리티.
/// GameManager에서 호출. 로딩 화면이 필요하면 onProgress 콜백 사용.
/// </summary>
public static class SceneLoader
{
    public static void Load(string sceneName, Action<float> onProgress = null)
    {
        // 코루틴 실행을 위해 GameManager 인스턴스 활용
        if (GameManager.Instance != null)
            GameManager.Instance.StartCoroutine(LoadAsync(sceneName, onProgress));
        else
            SceneManager.LoadScene(sceneName);
    }

    private static IEnumerator LoadAsync(string sceneName, Action<float> onProgress)
    {
        var operation = SceneManager.LoadSceneAsync(sceneName);
        if (operation == null) yield break;

        while (!operation.isDone)
        {
            onProgress?.Invoke(operation.progress);
            yield return null;
        }

        onProgress?.Invoke(1f);
    }
}
