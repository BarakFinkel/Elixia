using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player,
        _stateMachine, _animBoolName)
    {
    }

    // Update is called once per frame
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(new Vector2(xInput * player.moveSpeed, rb.linearVelocity.y));

        if (xInput == 0 || (xInput == player.facingDir && player.IsWallDetected()))
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}