using UnityEngine;

public class DeathBringer_SpellCastState : EnemyState
{
    private readonly Enemy_DeathBringer enemy;
    private int amountOfSpells;
    private float spellTimer;

    public DeathBringer_SpellCastState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,
        Enemy_DeathBringer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }


    public override void Enter()
    {
        base.Enter();

        amountOfSpells = enemy.amountOfSpells;
        spellTimer = .5f;

        AudioManager.instance.PlaySFX(43, 0, null);
    }

    public override void Update()
    {
        base.Update();

        spellTimer -= Time.deltaTime;

        if (CanCast())
        {
            enemy.CastSpell();
        }


        if (amountOfSpells <= 0)
        {
            stateMachine.ChangeState(enemy.teleportState);
        }
    }

    private bool CanCast()
    {
        if (amountOfSpells >= 0 && spellTimer < 0)
        {
            amountOfSpells--;
            spellTimer = enemy.spellCooldown;
            return true;
        }

        return false;
    }

    public override void Exit()
    {
        base.Exit();
        enemy.lastTimeCast = Time.time;

        AudioManager.instance.StopSFX(43);
    }
}