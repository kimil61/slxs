using UnityEngine;

/// <summary>
/// 구르기 전용 튜닝 데이터. i-frame 시작/지속 조합이 난이도 핵심.
/// 엘든링 참고: 경장 ~0.43초 무적, 중장 ~0.33초 무적.
/// </summary>
[CreateAssetMenu(fileName = "NewDodgeData", menuName = "색랑하산/Combat/DodgeData")]
public class DodgeData : ScriptableObject
{
    [Header("스태미나")]
    public float staminaCost = 25f;

    [Header("무적 프레임 (핵심)")]
    [Tooltip("구르기 시작 후 무적이 시작되는 시점 (초)")]
    [Range(0f, 0.2f)]
    public float iFrameStart = 0.05f;

    [Tooltip("무적 지속 시간 — 이 값이 게임 난이도를 결정")]
    [Range(0.1f, 0.6f)]
    public float iFrameDuration = 0.3f;

    [Header("타이밍")]
    public float totalDuration = 0.6f;
    [Tooltip("구르기 후 행동 불가 시간")]
    public float recoveryTime = 0.15f;

    [Header("이동")]
    public float distance = 3.5f;
    [Tooltip("구르기 속도 곡선 (빠르게 시작 → 느리게 끝)")]
    public AnimationCurve speedCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    [Header("캔슬")]
    [Tooltip("구르기 후반부에서 캔슬 가능한 타이밍 (normalized)")]
    [Range(0f, 1f)]
    public float cancelWindowStart = 0.7f;
}
