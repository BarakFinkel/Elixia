using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    // We move the player using the corresponding xInput and speed factor.
    public override void Update()
    {
        base.Update();

        if (xInput == 0 || player.IsWallDetected())
        {
            player.stateMachine.ChangeState(player.idleState);
        }

        player.SetVelocity(xInput * player.moveSpeed, rb.linearVelocityY);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
