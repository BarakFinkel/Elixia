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

        // If we press L-Click, we move to the attack state.
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            stateMachine.ChangeState(player.primaryAttackState);
        }

        // If we press R-Click, we move to the aim state.
        if(!player.canUseSwordSkill && Input.GetKeyDown(KeyCode.Mouse1) && SkillManager.instance.potion.CanUseSkill())
        {
            stateMachine.ChangeState(player.aimState);
        }

        if(Input.GetKeyDown(KeyCode.H) && Inventory.instance.CanUseSyringe() != null)
        {
            stateMachine.ChangeState(player.healState);
        }

        // If we press F, we move to the counter attack state.
        if (Input.GetKeyDown(KeyCode.F))
        {
            stateMachine.ChangeState(player.counterAttackState);
        }

        if (player.canUseSwordSkill && Input.GetKeyDown(KeyCode.Mouse1) && HasNoSword())
        {
            stateMachine.ChangeState(player.aimSwordState);
        }

        if (player.canUseSwordSkill && Input.GetKeyDown(KeyCode.Mouse2) && HasNoSword())
        {
            player.skillManager.sword.ChangeToNextType();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            stateMachine.ChangeState(player.blackholeState);
        }

        // If we press Space - we change to the Jump state.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateMachine.ChangeState(player.jumpState);
        }

        // If we leave the ground, we enter air state
        if (!player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.airState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    private bool HasNoSword()
    {
        if (!player.sword)
        {
            return true;
        }

        player.sword.GetComponent<SwordSkillController>().ReturnSword();

        return false;
    }
}
