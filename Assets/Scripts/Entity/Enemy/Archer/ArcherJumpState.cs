using UnityEngine;

public class ArcherJumpState : EnemyState
{
    private readonly Archer enemy;

    public ArcherJumpState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Archer _enemy) :
        base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        rb.linearVelocity = new Vector2(enemy.jumpVelocity.x * -enemy.facingDir, enemy.jumpVelocity.y);
        AudioManager.instance.PlaySFX(37, 0, enemy.transform);
    }

    public override void Update()
    {
        base.Update();

        enemy.anim.SetFloat("yVelocity", rb.linearVelocityY);

        if (rb.linearVelocityY < 0 && enemy.IsGroundDetected())
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}