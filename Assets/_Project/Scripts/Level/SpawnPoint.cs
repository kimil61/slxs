using UnityEngine;

/// <summary>
/// 적 스폰 위치 마커. Arena Prefab 안에 빈 오브젝트로 배치.
/// Gizmo로 에디터에서 시각적 확인 가능.
/// </summary>
public class SpawnPoint : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float gizmoRadius = 0.5f;
    [SerializeField] private Color gizmoColor = Color.red;

    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, gizmoRadius);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * gizmoRadius * 2f);
    }
}
