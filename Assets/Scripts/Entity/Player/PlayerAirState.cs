using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        // If we detect a wall to our sides, we change to the Wall Slide state.
        if (player.IsWallDetected())
        {
            stateMachine.ChangeState(player.wallSlideState);
        }

        // If the player touches the ground, we switch to the idle state.
        if (player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }

        // If we have input in the x-Axis, we move accordingly in the air.
        if (xInput != 0)
        {
            player.SetVelocity(player.moveSpeed * player.airSpeedFactor * xInput, rb.linearVelocityY);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
