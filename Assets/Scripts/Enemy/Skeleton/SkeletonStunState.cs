using UnityEngine;

public class SkeletonStunState : EnemyState
{
    private Enemy_Skeleton enemy;
    
    public SkeletonStunState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        
        enemy.fx.InvokeRepeating("RedColorBlink",0,0.1f);

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
        enemy.fx.Invoke("CancelRedColorBlink",0);
       
    }
}