using UnityEngine;

/// <summary>
/// 공격 시작 시 whoosh(바람소리) 재생.
/// Sound Layering 1층: whoosh → HitFeedback에서 impact + resonance.
/// Player에 붙이거나, 공격 시작 시 PlayWhoosh() 호출.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class CombatSoundPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D 사운드
    }

    /// <summary>
    /// 공격 선동작에 바람소리 재생 (빗나가도 재생됨).
    /// </summary>
    public void PlayWhoosh(AttackData attackData)
    {
        if (attackData == null || attackData.sfxWhoosh == null) return;
        audioSource.PlayOneShot(attackData.sfxWhoosh, attackData.sfxWhooshVolume);
    }
}
