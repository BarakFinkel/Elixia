using UnityEngine;

public class DeathBringer_AttackState : EnemyState
{
    private readonly Enemy_DeathBringer enemy;

    public DeathBringer_AttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,
        Enemy_DeathBringer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.chanceToTeleport += 5;

        AudioManager.instance.PlaySFX(46, 0, enemy.transform);
    }

    public override void Update()
    {
        base.Update();

        enemy.ZeroVelocity();

        if (triggerCalled)
        {
            if (enemy.CanTeleport())
            {
                stateMachine.ChangeState(enemy.teleportState);
            }
            else
            {
                stateMachine.ChangeState(enemy.battleState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();

        enemy.lastTimeAttacked = Time.time;
    }
}