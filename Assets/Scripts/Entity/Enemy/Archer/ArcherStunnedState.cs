using UnityEngine;

public class ArcherStunnedState : EnemyState
{
    private readonly Archer enemy;
    private float stunVelocityApplyDuration = 0.05f; // small delay for updating velocity in case of bugs.

    public ArcherStunnedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,
        Archer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
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

        if (stateTimer == 0)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();

        enemy.fx.Invoke("CancelColorChange", 0.0f);
        stunVelocityApplyDuration = 0.05f; // small delay for updating velocity in case of bugs.
    }
}