using UnityEngine;

/// <summary>
/// 구역 간 이동 트리거. 출구 길에 배치.
/// 플레이어가 통과하면 다음 Arena 활성화.
/// </summary>
[RequireComponent(typeof(Collider))]
public class AreaTransitionTrigger : MonoBehaviour
{
    [Header("연결")]
    [SerializeField] private ArenaManager nextArena;
    [SerializeField] private Transform teleportTarget;   // 다음 구역 입구

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        // 플레이어 이동
        if (teleportTarget != null)
        {
            var controller = other.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
                other.transform.position = teleportTarget.position;
                other.transform.rotation = teleportTarget.rotation;
                controller.enabled = true;
            }
        }

        // 다음 Arena 활성화
        if (nextArena != null)
            nextArena.Activate();
    }
}
