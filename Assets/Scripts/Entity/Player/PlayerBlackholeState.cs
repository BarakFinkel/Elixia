using UnityEngine;

public class PlayerBlackholeState : PlayerState
{
    private readonly int flyingHeight = 4;
    private readonly float flyTime = .8f;
    private float defaultGravity;
    private bool skillUsed;

    public PlayerBlackholeState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player,
        _stateMachine, _animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void Enter()
    {
        base.Enter();
        skillUsed = false;
        stateTimer = flyTime;
        defaultGravity = rb.gravityScale;
        rb.gravityScale = 0;
        player.cs.EnableInvulnerability();
    }

    public override void Update()
    {
        base.Update();
        if (stateTimer > 0)
        {
            rb.linearVelocity = new Vector2(0, flyingHeight);
        }

        if (stateTimer == 0)
        {
            rb.linearVelocity = new Vector2(0, -.1f); // slow falling
            if (!skillUsed)
            {
                player.skillManager.blackhole.UseSkill();
                skillUsed = true;
            }
        }

        if (player.skillManager.blackhole.SkillCompleted())
        {
            stateMachine.ChangeState(player.airState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.rb.gravityScale = defaultGravity;
        player.fx.MakeTransparent(false);
        player.cs.DisableInvulnerability();
    }
}