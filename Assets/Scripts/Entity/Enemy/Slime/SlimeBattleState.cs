using UnityEngine;

public class SlimeBattleState : EnemyState
{
    private Slime enemy;
    private int moveDir;
    private Transform player;

    public SlimeBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Slime _enemy)
        : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;

        if (enemy.initialBattleState)
        {
            AudioManager.instance.PlaySFX(10,0,enemy.transform);
            enemy.initialBattleState = false;
        }
    }

    public override void Update()
    {
        base.Update();

        if (player.gameObject.GetComponent<CharacterStats>().isDead)
        {
            stateMachine.ChangeState(enemy.idleState);
        }

        // If the player is detected
        if (enemy.IsPlayerDetected())
        {
            stateTimer = enemy.battleTime; // Set the state timer to the desired battle time.

            if (enemy.IsPlayerDetected().distance < enemy.attackDistance)
            {
                if (CanAttack())
                {
                    stateMachine.ChangeState(enemy.attackState);
                }
            }
        }
        else
        {
            if (stateTimer == 0 || Vector2.Distance(player.transform.position, enemy.transform.position) >
                enemy.battleDistance)
            {
                stateMachine.ChangeState(enemy.idleState);
            }
        }

        // Determine the direcion in which the player is at, and move towards him.
        if (player.position.x > enemy.transform.position.x)
        {
            moveDir = 1;
        }
        else if (player.position.x < enemy.transform.position.x)
        {
            moveDir = -1;
        }

        // If the enemy is close enough to the player, it won't move.
        if (enemy.IsPlayerDetected() && enemy.IsPlayerDetected().distance < enemy.attackDistance - enemy.attackDistanceFromPlayer)
        {
            return;
        }

        enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.linearVelocityY);
    }

    public override void Exit()
    {
        base.Exit();
    }

    private bool CanAttack()
    {
        if (Time.time >= enemy.lastTimeAttacked + enemy.attackCooldown)
        {
            enemy.lastTimeAttacked = Time.time;
            return true;
        }

        return false;
    }
}
