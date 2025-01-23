using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    public PlayerWallSlideState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player,
        _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = player.wallSlideLockJumpTime;
    }

    public override void Update()
    {
        base.Update();

        // If we press space and the timer reached it's limit - we  move to the Wall Jump state.
        if (Input.GetKeyDown(KeyCode.Space) && stateTimer == 0)
        {
            stateMachine.ChangeState(player.wallJumpState);
            return;
        }

        // If the player is moving to the opposite direction from the wall (which he currently faces), we change to Idle State.
        if (xInput != 0 && player.facingDir != xInput)
        {
            stateMachine.ChangeState(player.idleState);
        }

        // The speed in which the player slides the wall at deppends on wether he presses the down key or not (not pressing it slows the slide).
        rb.linearVelocity = new Vector2(0, rb.linearVelocityY * (yInput < 0 ? 1 : player.wallSlideSpeedFactor));

        // If the player touches the ground, or for some reason the wall isn't detected anymore - we set the state to be Idle State.
        if (player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }

        if (!player.IsWallDetected())
        {
            stateMachine.ChangeState(player.airState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}