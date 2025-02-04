using UnityEngine;

public class SlimeStunnedState : EnemyState
{
    private readonly Slime enemy;
    private bool stunFoldDone;
    private float stunVelocityApplyDuration = 0.05f; // small delay for updating velocity in case of bugs.

    public SlimeStunnedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,
        Slime _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.stunnedDuration;

        enemy.fx.InvokeRepeating("RedColorBlink", 0, enemy.blinkDelay);

        rb.linearVelocity = new Vector2(-enemy.facingDir * enemy.stunnedDirection.x, enemy.stunnedDirection.y);
    }

    public override void Update()
    {
        base.Update();

        if (stunVelocityApplyDuration > 0)
        {
            rb.linearVelocity = new Vector2(-enemy.facingDir * enemy.stunnedDirection.x, enemy.stunnedDirection.y);
            stunVelocityApplyDuration -= Time.deltaTime;
        }

        if (!stunFoldDone && rb.linearVelocityY < .1f && enemy.IsGroundDetected())
        {
            enemy.fx.Invoke("CancelColorChange", 0.0f);
            enemy.anim.SetTrigger("StunFold");
            enemy.cs.EnableInvulnerability();
            stunFoldDone = true;
        }

        if (stateTimer == 0 && stunFoldDone)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();

        enemy.cs.DisableInvulnerability();
        stunFoldDone = false;
        stunVelocityApplyDuration = 0.05f; // small delay for updating velocity in case of bugs.
    }
}