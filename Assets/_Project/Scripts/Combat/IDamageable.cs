using UnityEngine;

/// <summary>
/// 데미지를 받을 수 있는 모든 오브젝트가 구현하는 인터페이스.
/// 플레이어, 적, 파괴 가능한 오브젝트 등에 사용.
/// </summary>
public interface IDamageable
{
    float CurrentHP { get; }
    float MaxHP { get; }
    bool IsDead { get; }
    Transform Transform { get; }

    /// <param name="damage">실제 데미지량</param>
    /// <param name="attackData">공격 정보 (넉백, 히트스톱 등 피드백용)</param>
    /// <param name="hitPoint">피격 지점 (이펙트 스폰 위치)</param>
    /// <param name="attacker">공격자 Transform (넉백 방향 계산용)</param>
    void TakeDamage(float damage, AttackData attackData, Vector3 hitPoint, Transform attacker);
}
