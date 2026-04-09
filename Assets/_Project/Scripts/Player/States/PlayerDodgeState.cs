using UnityEngine;

public class PlayerDodgeState : IPlayerState
{
    private float timer;
    private Vector3 dodgeDirection;

    public void Enter(PlayerStateMachine player)
    {
        timer = 0f;
        var data = player.dodgeData;

        // 스태미나 소모
        player.Stamina.Consume(data.staminaCost);

        // 구르기 방향 = 이동 입력 or 캐릭터 전방
        dodgeDirection = player.GetCameraRelativeMoveDir();
        if (dodgeDirection.sqrMagnitude < 0.01f)
            dodgeDirection = player.transform.forward;

        player.RotateTowards(dodgeDirection);
        player.Animator.SetTrigger("Dodge");
    }

    public void Update(PlayerStateMachine player)
    {
        var data = player.dodgeData;
        timer += Time.deltaTime;

        // 무적 프레임 관리
        float iFrameEnd = data.iFrameStart + data.iFrameDuration;
        player.IsInvincible = timer >= data.iFrameStart && timer < iFrameEnd;

        // 속도곡선에 따른 이동
        float normalizedTime = timer / data.totalDuration;
        float curveSpeed = data.speedCurve.Evaluate(normalizedTime);
        float moveSpeed = (data.distance / data.totalDuration) * curveSpeed;
        player.Move(dodgeDirection, moveSpeed);

        // 캔슬 윈도우에서 공격 입력 허용
        if (normalizedTime >= data.cancelWindowStart)
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                player.TransitionTo(player.LightAttackState);
                return;
            }
        }

        // 구르기 종료
        if (timer >= data.totalDuration + data.recoveryTime)
        {
            player.TransitionTo(player.IdleState);
        }
    }

    public void Exit(PlayerStateMachine player)
    {
        player.IsInvincible = false;
    }
}
