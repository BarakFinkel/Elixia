using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        if (!player.IsGroundDetected())
            stateMachine.ChangeState(player.airState);
        
        if(Input.GetButtonDown("Jump") && player.IsGroundDetected())
            stateMachine.ChangeState(player.jumpState);
        
        if(Input.GetKeyDown(KeyCode.Mouse0))
            stateMachine.ChangeState(player.pAttackState);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
