using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 무기에 붙이는 히트박스. Collider(IsTrigger=true) 필수.
/// 공격 시 Activate → 적 Collider와 겹치면 IDamageable.TakeDamage 호출 → Deactivate.
/// 한 스윙에 같은 대상을 두 번 때리지 않도록 hitTargets로 관리.
/// </summary>
[RequireComponent(typeof(Collider))]
public class WeaponHitbox : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private Transform owner;

    private Collider hitboxCollider;
    private AttackData currentAttack;
    private float currentDamage;
    private readonly HashSet<IDamageable> hitTargets = new();

    private void Awake()
    {
        hitboxCollider = GetComponent<Collider>();
        hitboxCollider.isTrigger = true;
        hitboxCollider.enabled = false;
    }

    /// <summary>
    /// 공격 시작 시 호출. 히트박스 활성화.
    /// </summary>
    public void Activate(AttackData attackData, float baseDamage)
    {
        currentAttack = attackData;
        currentDamage = baseDamage * attackData.damageMultiplier;
        hitTargets.Clear();
        hitboxCollider.enabled = true;
    }

    /// <summary>
    /// 공격 종료 시 호출. 히트박스 비활성화.
    /// </summary>
    public void Deactivate()
    {
        hitboxCollider.enabled = false;
        currentAttack = null;
        hitTargets.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 레이어 체크
        if ((hitLayers.value & (1 << other.gameObject.layer)) == 0) return;

        var damageable = other.GetComponent<IDamageable>();
        if (damageable == null || damageable.IsDead) return;

        // 같은 스윙에서 중복 히트 방지
        if (!hitTargets.Add(damageable)) return;

        Vector3 hitPoint = other.ClosestPoint(transform.position);
        damageable.TakeDamage(currentDamage, currentAttack, hitPoint, owner);

        // 타격 피드백
        HitFeedback.Execute(currentAttack, hitPoint);
    }
}
