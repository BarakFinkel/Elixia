using UnityEngine;

public class ArcherBattleState : EnemyState
{
    private readonly Archer enemy;
    private int moveDir;
    private Player player;
    private Transform playerTransform;

    public ArcherBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Archer _enemy)
        : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        playerTransform = PlayerManager.instance.player.transform;
        player = PlayerManager.instance.player;

        if (enemy.initialBattleState)
        {
            AudioManager.instance.PlaySFX(36, 0, enemy.transform);
            enemy.initialBattleState = false;
        }
    }

    public override void Update()
    {
        base.Update();

        if (playerTransform.gameObject.GetComponent<CharacterStats>().isDead)
        {
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        // Determine the direcion in which the player is at, and move towards him.
        moveDir = playerTransform.position.x > enemy.transform.position.x ? 1 : -1;
        if (enemy.facingDir != moveDir)
        {
            enemy.Flip();
        }

        // If the player is detected
        var playerDetection = enemy.IsPlayerDetected();
        if (playerDetection)
        {
            stateTimer = enemy.battleTime; // Set the state timer to the desired battle time.

            // If the player gets too close, jump back
            if (playerDetection.distance < enemy.maxDistanceForJump && CanJump())
            {
                stateMachine.ChangeState(enemy.jumpState);
                return;
            }

            // If the enemy is close enough for attacking.
            if (playerDetection.distance < enemy.attackDistance)
            {
                // If the enemy is close enough to the player, it won't move from it's spot
                enemy.anim.SetBool("Move", false);
                enemy.anim.SetBool("Idle", true);

                // If the attack is off cooldown, attack.
                if (CanAttack())
                {
                    stateMachine.ChangeState(enemy.attackState);
                }
            }
            else // We move according to the player's position.
            {
                // If the enemy is supposed to move, we change the animation back to move.
                enemy.anim.SetBool("Idle", false);
                enemy.anim.SetBool("Move", true);

                enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.linearVelocityY);
            }
        }
        else
        {
            enemy.anim.SetBool("Move", false);
            enemy.anim.SetBool("Idle", true);

            if (stateTimer == 0 || Vector2.Distance(playerTransform.transform.position, enemy.transform.position) >
                enemy.battleDistance)
            {
                stateMachine.ChangeState(enemy.idleState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();

        enemy.anim.SetBool("Idle", false);
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

    private bool CanJump()
    {
        if (enemy.IsGrounBehind() && !enemy.IsWallBehind() && Time.time >= enemy.lastTimeJumped + enemy.jumpCooldown)
        {
            enemy.lastTimeJumped = Time.time;
            return true;
        }

        return false;
    }
}