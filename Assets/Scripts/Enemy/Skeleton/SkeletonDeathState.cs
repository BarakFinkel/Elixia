using UnityEngine;

public class SkeletonDeathState : EnemyState
{
    private Enemy_Skeleton enemy;
    
    public SkeletonDeathState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        
        enemy.animator.SetBool(enemy.lastAnimBoolName, true);
        enemy.animator.speed = 0f; // stop animation

        enemy.cd.enabled = false;

        stateTimer = .15f;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0f)
        {
            rb.linearVelocity = new Vector2(0f, 10); // fly up like in mario
        }
    }

}
