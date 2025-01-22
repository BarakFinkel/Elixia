using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.ZeroVelocity();
    }

    public override void Update()
    {
        base.Update();

        // If we detect a wall, we won't move towards it. Although, we still want to be able to jump on it if sticking to it.
        if (xInput == player.facingDir && player.IsWallDetected())
        {
            return;
        }

        // If we get an x-Axis input, we change to the move state.
        if (xInput != 0 && !player.isBusy)
        {
            player.stateMachine.ChangeState(player.moveState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
