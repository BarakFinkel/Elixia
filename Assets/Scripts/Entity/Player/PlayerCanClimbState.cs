using UnityEngine;

public class PlayerCanClimbState : PlayerState
{
    private PlayerLedgeDetection pld;

    public PlayerCanClimbState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player,
        _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        rb.simulated = false;
        player.transform.position = player.climbBegunPosition;
        pld = player.GetComponentInChildren<PlayerLedgeDetection>();
    }

    // We climb only if the player pressed the space bar.
    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.anim.SetBool("CanClimb", false);
        }

        if (triggerCalled)
        {
            player.transform.position = player.climbOverPosition;
            player.stateMachine.ChangeState(player.jumpState);
        }
    }

    public override void Exit()
    {
        base.Exit();

        player.rb.simulated = true;
        player.jumpAfterLedgeClimb = true;
        player.AllowLedgeGrabWithDelay();
    }
}