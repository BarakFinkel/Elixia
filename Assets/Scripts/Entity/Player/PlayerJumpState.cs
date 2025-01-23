using UnityEngine;

public class PlayerJumpState : PlayerState
{
    private bool isToSlide;

    public PlayerJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player,
        _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // If we are next to the wall when we start jumping, we want to prevent a bug that will make us not move.
        isToSlide = !player.IsWallDetected();

        rb.linearVelocity = new Vector2(rb.linearVelocityX, player.jumpForce);
    }

    public override void Update()
    {
        base.Update();

        if (player.IsWallDetected() && isToSlide)
        {
            stateMachine.ChangeState(player.wallSlideState);
        }

        if (rb.linearVelocityY <= player.jumpToAirThreshold)
        {
            stateMachine.ChangeState(player.airState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}