using UnityEngine;

public class PlayerWallJumpState : PlayerState
{
    public PlayerWallJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    // When we enter the Wall Jump state, we jump in the opposite direction of the player's body with the power being set by the player's variables
    public override void Enter()
    {
        base.Enter();

        stateTimer = player.wallJumpDuration;

        player.SetVelocity(player.wallJumpXSpeed * -player.facingDir, player.jumpForce);
    }

    public override void Update()
    {
        base.Update();

        // If the timer ended, move to air state.
        if (stateTimer == 0)
        {
            stateMachine.ChangeState(player.airState);
        }

        // If the player reached the ground - move to idle state.
        if (player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }

        // To enable smooth wall jumping - reaching a wall even before the timer ends will switch the player into the Wall Slide State.
        if (player.IsWallDetected())
        {
            stateMachine.ChangeState(player.wallSlideState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
