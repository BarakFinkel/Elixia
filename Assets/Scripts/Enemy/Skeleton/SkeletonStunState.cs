using UnityEngine;

public class SkeletonStunState : EnemyState
{
    private readonly float blinkRepeat = 0.1f;
    private readonly Enemy_Skeleton enemy;

    public SkeletonStunState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,
        Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.fx.InvokeRepeating("RedColorBlink", 0, blinkRepeat);

        stateTimer = enemy.stunDuration;
        rb.linearVelocity = new Vector2(-enemy.facingDir * enemy.stunDirection.x, enemy.stunDirection.y);
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer <= 0)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        enemy.fx.Invoke("CancelColorChange", 0);
    }
}