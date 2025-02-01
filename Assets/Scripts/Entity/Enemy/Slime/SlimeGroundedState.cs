using UnityEngine;

public class SlimeGroundedState : EnemyState
{
    protected Slime enemy;
    protected Transform player;
    
    public SlimeGroundedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,
        Slime _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;
    }

    public override void Update()
    {
        base.Update();

        if ((enemy.IsPlayerDetected() ||
            Vector2.Distance(enemy.transform.position, player.position) < enemy.closeAggroDistance) &&
            !player.gameObject.GetComponent<CharacterStats>().isDead)
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
