using UnityEngine;

public class PlayerCounterAttackState : PlayerState
{
    private bool canCreateClone;

    public PlayerCounterAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(
        _player, _stateMachine, _animBoolName)
    {
    }

    // When we enter,  We start the timer.
    public override void Enter()
    {
        base.Enter();

        canCreateClone = true;
        stateTimer = player.counterAttackDuration;
    }

    public override void Update()
    {
        base.Update();

        player.ZeroVelocity(); // Make sure the player doesn't move within the counter attack.

        // All game objects within range within the attack range
        var colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        // Any of "Enemy" type will be stunned, and the counter attack animation will play.
        foreach (var hit in colliders)
        {
            var enemy = hit.GetComponent<Enemy>();

            if (enemy != null)
            {
                if (hit.GetComponent<Enemy>().CanBeStunned())
                {
                    stateTimer = 10.0f; // Static value - just to make sure the player doesn't exit the state too soon.
                    player.anim.SetBool("SuccessfulCounterAttack", true);
                    AudioManager.instance.PlaySFX(28, 0, null);

                    player.skillManager.counterAttack
                        .UseSkill(); // Will have effect when the player unlocks the health restoration feature on this skill.

                    if (canCreateClone)
                    {
                        canCreateClone = false;
                        player.skillManager.counterAttack.MakeCloneOnCounterAttack(hit.transform);
                    }

                    player.cs.DoDamage(enemy.cs);
                }
            }
        }

        // If we're out of time for countering, or we've successfully counter-attacked an enemy - we change to the idle state.
        if (stateTimer == 0 || triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    // When we exit, we stop the attack animation and prevent movement for the wanted busyDuration.
    public override void Exit()
    {
        base.Exit();

        player.anim.SetBool("SuccessfulCounterAttack", false);
        player.StartCoroutine("BusyFor", player.busyWaitDuration);
    }
}