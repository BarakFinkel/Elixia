using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    private float slidingSpeed = .7f;
    public PlayerWallSlideState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player,
        _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetButtonDown("Jump"))
        {
            stateMachine.ChangeState(player.wallJumpState);
            return;
        }

        if (xInput != 0 && player.facingDir != xInput)
        {
            player.stateMachine.ChangeState(player.idleState);
        }

        if (player.IsGroundDetected())
        {
            player.stateMachine.ChangeState(player.idleState);
        }

        if (yInput < 0)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y * slidingSpeed);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}