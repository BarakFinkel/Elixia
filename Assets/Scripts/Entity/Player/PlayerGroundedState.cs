using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player,
        _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

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

        // If we press L-Click, we move to the attack state.
        if (Input.GetKeyDown(KeyCode.Mouse0) && !player.isUIActive)
        {
            stateMachine.ChangeState(player.primaryAttackState);
        }

        // If we press R-Click, we move to the aim sword state if we're able to.
        if (Input.GetKeyDown(KeyCode.Mouse1) && !player.isUIActive && player.skillManager.sword.swordUnlocked && player.canUseSwordSkill && HasNoSword() && player.skillManager.sword.CanUseSkill())
        {
            stateMachine.ChangeState(player.aimSwordState);
        }

        // If we press R-Click, we move to the aim potion state if we're able to.
        if (Input.GetKeyDown(KeyCode.Mouse1) && !player.isUIActive && player.skillManager.potion.potionUnlocked && !player.canUseSwordSkill && player.skillManager.potion.CanUseSkill())
        {
            stateMachine.ChangeState(player.aimState);
        }

        // If we press H, and can use a syringe - we move to the heal state.
        if (Input.GetKeyDown(KeyCode.Alpha1) && Inventory.instance.CanUseSyringe() != null)
        {
            stateMachine.ChangeState(player.healState);
        }

        // If we press F, we move to the counter attack state.
        if (Input.GetKeyDown(KeyCode.F) && !player.IsWallDetected() && player.skillManager.counterAttack.counterAttackUnlocked && player.skillManager.counterAttack.CanUseSkill())
        {
            stateMachine.ChangeState(player.counterAttackState);
        }

        // If we press X and the ability's off-cooldown, we enter the blackhole ultimate ability state.
        if (Input.GetKeyDown(KeyCode.X) && player.skillManager.blackhole.blackholeUnlocked && player.skillManager.blackhole.CanUseSkill())
        {
            stateMachine.ChangeState(player.blackholeState);
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