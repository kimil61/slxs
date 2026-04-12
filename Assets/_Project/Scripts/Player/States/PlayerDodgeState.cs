using UnityEngine;

public class PlayerDodgeState : IPlayerState
{
    private float timer;
    private Vector3 dodgeDirection;

    public void Enter(PlayerStateMachine player)
    {
        timer = 0f;
        var data = player.dodgeData;

        player.Stamina.Consume(data.staminaCost);

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

        float iFrameEnd = data.iFrameStart + data.iFrameDuration;
        player.IsInvincible = timer >= data.iFrameStart && timer < iFrameEnd;

        float normalizedTime = timer / data.totalDuration;
        float curveSpeed = data.speedCurve.Evaluate(normalizedTime);
        float moveSpeed = (data.distance / data.totalDuration) * curveSpeed;
        player.Move(dodgeDirection, moveSpeed);

        if (normalizedTime >= data.cancelWindowStart && player.Input.AttackPressed)
        {
            player.TransitionTo(player.LightAttackState);
            return;
        }

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
