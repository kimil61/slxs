using UnityEngine;

/// <summary>
/// 공격별 개별 튜닝 데이터. 플레이어와 적 모두 이 SO를 공유.
/// Assets/_Project/Data/ScriptableObjects/AttackData/ 에 에셋 생성.
/// </summary>
[CreateAssetMenu(fileName = "NewAttackData", menuName = "색랑하산/Combat/AttackData")]
public class AttackData : ScriptableObject
{
    [Header("기본 정보")]
    public string attackName;
    public AnimationClip animationClip;

    [Header("데미지")]
    [Range(0.1f, 5f)]
    public float damageMultiplier = 1.0f;

    [Header("스태미나")]
    public float staminaCost;

    [Header("히트스톱")]
    [Range(0f, 0.2f)]
    public float hitStopDuration = 0.03f;
    [Range(0f, 0.1f)]
    public float hitStopTimeScale = 0f;

    [Header("카메라 셰이크")]
    [Range(0f, 2f)]
    public float cameraShakeIntensity = 0.2f;
    [Range(0f, 0.5f)]
    public float cameraShakeDuration = 0.1f;

    [Header("넉백")]
    public float knockbackForce = 3.0f;

    [Header("콤보")]
    public AttackData[] canComboInto;
    [Range(0f, 1f)]
    public float comboWindowStart = 0.4f;
    [Range(0f, 1f)]
    public float comboWindowEnd = 0.8f;

    [Header("히트박스 타이밍 (normalized)")]
    [Range(0f, 1f)]
    public float hitboxActivateTime = 0.2f;
    [Range(0f, 1f)]
    public float hitboxDeactivateTime = 0.5f;

    [Header("Sound Layering — 3레이어 SFX")]
    public AudioClip sfxWhoosh;
    public AudioClip sfxImpact;
    public AudioClip sfxResonance;
    [Range(0f, 1f)] public float sfxWhooshVolume = 0.6f;
    [Range(0f, 1f)] public float sfxImpactVolume = 1.0f;
    [Range(0f, 1f)] public float sfxResonanceVolume = 0.4f;
    [Range(0f, 0.2f)] public float sfxImpactDelay = 0f;
    [Range(0f, 0.2f)] public float sfxResonanceDelay = 0.05f;
}
