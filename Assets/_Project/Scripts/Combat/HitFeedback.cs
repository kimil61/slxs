using System.Collections;
using UnityEngine;

/// <summary>
/// 타격 피드백 통합 처리: 히트스톱, 카메라셰이크, 이펙트, Sound Layering.
/// WeaponHitbox에서 피격 시 HitFeedback.Execute() 호출.
/// 씬에 HitFeedbackRunner를 하나 배치해둘 것.
/// </summary>
public static class HitFeedback
{
    public static void Execute(AttackData attackData, Vector3 hitPoint)
    {
        if (attackData == null) return;

        var runner = HitFeedbackRunner.Instance;
        if (runner == null) return;

        // 히트스톱
        if (attackData.hitStopDuration > 0f)
            runner.StartCoroutine(HitStopRoutine(attackData.hitStopDuration, attackData.hitStopTimeScale));

        // 카메라 셰이크
        if (attackData.cameraShakeIntensity > 0f)
            CameraShake.Shake(attackData.cameraShakeIntensity, attackData.cameraShakeDuration);

        // 히트 이펙트
        SpawnHitEffect(hitPoint);

        // Sound Layering (3레이어)
        PlayHitSounds(attackData, hitPoint);
    }

    private static IEnumerator HitStopRoutine(float duration, float timeScale)
    {
        Time.timeScale = timeScale;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }

    private static void SpawnHitEffect(Vector3 position)
    {
        if (HitFeedbackRunner.Instance.hitEffectPrefab == null) return;

        if (PoolManager.Instance != null)
        {
            var effect = PoolManager.Instance.Spawn(
                HitFeedbackRunner.Instance.hitEffectPrefab,
                position,
                Quaternion.identity
            );
            HitFeedbackRunner.Instance.StartCoroutine(ReturnToPool(effect, 1f));
        }
        else
        {
            var effect = Object.Instantiate(
                HitFeedbackRunner.Instance.hitEffectPrefab,
                position,
                Quaternion.identity
            );
            Object.Destroy(effect, 1f);
        }
    }

    private static IEnumerator ReturnToPool(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (PoolManager.Instance != null)
            PoolManager.Instance.Despawn(HitFeedbackRunner.Instance.hitEffectPrefab, obj);
        else
            obj.SetActive(false);
    }

    /// <summary>
    /// Sound Layering: whoosh는 공격 시작 시 별도 호출, 여기서는 impact + resonance.
    /// </summary>
    private static void PlayHitSounds(AttackData data, Vector3 position)
    {
        if (data.sfxImpact != null)
        {
            AudioSource.PlayClipAtPoint(data.sfxImpact, position, data.sfxImpactVolume);
        }

        if (data.sfxResonance != null && data.sfxResonanceDelay > 0f)
        {
            HitFeedbackRunner.Instance.StartCoroutine(
                PlayDelayedSound(data.sfxResonance, position, data.sfxResonanceVolume, data.sfxResonanceDelay)
            );
        }
        else if (data.sfxResonance != null)
        {
            AudioSource.PlayClipAtPoint(data.sfxResonance, position, data.sfxResonanceVolume);
        }
    }

    private static IEnumerator PlayDelayedSound(AudioClip clip, Vector3 pos, float volume, float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioSource.PlayClipAtPoint(clip, pos, volume);
    }
}

/// <summary>
/// HitFeedback의 코루틴 실행용 MonoBehaviour. 씬에 하나 배치.
/// </summary>
public class HitFeedbackRunner : MonoBehaviour
{
    public static HitFeedbackRunner Instance { get; private set; }

    [Header("Effects")]
    public GameObject hitEffectPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
