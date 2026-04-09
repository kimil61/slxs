using UnityEngine;

/// <summary>
/// 3인칭 오빗 카메라. 마우스로 회전, 벽 충돌 시 줌인.
/// Main Camera 오브젝트에 붙일 것 (Player 자식 아님!).
/// Inspector에서 Target에 Player를 드래그.
/// </summary>
public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1.5f, 0f);

    [Header("Orbit")]
    [SerializeField] private float distance = 4f;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 8f;
    [SerializeField] private float sensitivity = 2f;        // 카메라 회전속도
    [SerializeField] private float verticalMin = -20f;
    [SerializeField] private float verticalMax = 70f;

    [Header("Collision")]
    [SerializeField] private float collisionRadius = 0.2f;
    [SerializeField] private LayerMask collisionLayers;
    [SerializeField] private float collisionSmooth = 10f;

    private float yaw;
    private float pitch;
    private float currentDistance;
    private PlayerInputHandler inputHandler;

    private void Start()
    {
        if (target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }

        inputHandler = target?.GetComponent<PlayerInputHandler>();

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
        currentDistance = distance;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // 1. 입력으로 각도 갱신
        Vector2 look = inputHandler != null ? inputHandler.LookInput : Vector2.zero;
        yaw += look.x * sensitivity;
        pitch -= look.y * sensitivity;
        pitch = Mathf.Clamp(pitch, verticalMin, verticalMax);

        // 2. 피벗 포인트 (캐릭터 어깨)
        Vector3 pivotPoint = target.position + targetOffset;

        // 3. 회전값으로 카메라 방향 계산
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 direction = rotation * Vector3.back;

        // 4. 벽 충돌 → 거리 조절
        float desiredDistance = distance;
        if (Physics.SphereCast(pivotPoint, collisionRadius, direction, out RaycastHit hit,
            distance, collisionLayers))
        {
            desiredDistance = Mathf.Clamp(hit.distance - 0.1f, minDistance, distance);
        }

        // 거리만 부드럽게 보간 (줌인/줌아웃만 자연스럽게)
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * collisionSmooth);

        // 5. 최종 위치 = 피벗 + 방향 * 거리 (즉시 배치, 보간 없음)
        transform.position = pivotPoint + direction * currentDistance;

        // 6. 항상 피벗을 정확히 바라봄 (Slerp 없음)
        transform.LookAt(pivotPoint);
    }
}