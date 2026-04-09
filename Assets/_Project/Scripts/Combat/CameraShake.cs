using System.Collections;
using UnityEngine;

/// <summary>
/// 카메라 셰이크. ThirdPersonCamera와 함께 Main Camera에 붙일 것.
/// HitFeedback에서 CameraShake.Shake() 호출.
/// </summary>
public class CameraShake : MonoBehaviour
{
    private static CameraShake instance;

    private Vector3 originalLocalPos;
    private Coroutine shakeRoutine;

    private void Awake()
    {
        instance = this;
    }

    public static void Shake(float intensity, float duration)
    {
        if (instance == null) return;

        if (instance.shakeRoutine != null)
            instance.StopCoroutine(instance.shakeRoutine);

        instance.shakeRoutine = instance.StartCoroutine(instance.ShakeRoutine(intensity, duration));
    }

    private IEnumerator ShakeRoutine(float intensity, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;

            // LateUpdate에서 카메라 위치가 설정된 후 오프셋 적용
            transform.localPosition += new Vector3(x, y, 0f);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        shakeRoutine = null;
    }
}
