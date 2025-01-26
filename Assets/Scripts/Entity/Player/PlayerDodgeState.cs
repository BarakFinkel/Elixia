public class PlayerDodgeState : PlayerState
{
    public PlayerDodgeState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player,
        _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.skillManager.dodge.CreateCloneOnDodgeStart();
        stateTimer = player.dodgeDuration;

        player.cs.EnableInvulnerability();
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer == 0)
        {
            if (xInput != 0)
            {
                stateMachine.ChangeState(player.moveState);
            }
            else
            {
                stateMachine.ChangeState(player.idleState);
            }

            return;
        }

        if (!player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.airState);
        }

        player.SetVelocity(player.dodgeSpeed * player.dodgeDir, rb.linearVelocityY);
    }

    public override void Exit()
    {
        base.Exit();

        player.skillManager.dodge.CreateCloneOnDodgeEnd();

        // Stopping the player's movement in the dodge direction when finished.
        player.SetVelocity(0, rb.linearVelocityY);

        player.cs.DisableInvulnerability();
    }
}