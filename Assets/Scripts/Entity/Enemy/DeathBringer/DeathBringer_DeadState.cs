public class DeathBringer_DeadState : EnemyState
{
    private readonly Enemy_DeathBringer enemy;

    public DeathBringer_DeadState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,
        Enemy_DeathBringer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.anim.SetBool(enemy.lastAnimBoolName, true);
        enemy.anim.speed = 0;

        enemy.cd.enabled = false;
        stateTimer = enemy.knockUpTime;

        AudioManager.instance.PlaySFX(52, 0, null);
        AudioManager.instance.DisableMusic();

        enemy.DestroySelfInDelay();
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
        {
            rb.linearVelocity = enemy.knockUpVelocity;
        }
    }
}