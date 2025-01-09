using UnityEngine;

public class EnemyStats : CharacterStats
{
    [Header("Level details")]
    [SerializeField]
    private int level = 1;

    [Range(0f, 1f)]
    [SerializeField]
    private float percentageModifier = .4f;

    private Enemy enemy;
    private ItemDrop myDropSystem;

    protected override void Start()
    {
        ApplyLvlModifiers();

        base.Start();

        enemy = GetComponent<Enemy>();
        myDropSystem = GetComponent<ItemDrop>();
    }

    private void ApplyLvlModifiers()
    {
        Modify(strength);
        Modify(agility);
        Modify(intelligence);
        Modify(vitality);

        Modify(damage);
        Modify(critChance);
        Modify(critPower);

        Modify(maxHp);
        Modify(armor);
        Modify(evasion);
        Modify(magicResist);

        Modify(fireDamage);
        Modify(iceDamage);
        Modify(lightningDamage);
    }

    private void Modify(Stat _stat)
    {
        for (var i = 1; i < level; i++)
        {
            var modifier = _stat.GetValue() * percentageModifier;
            _stat.AddModifier(Mathf.RoundToInt(modifier));
        }
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    protected override void Die()
    {
        base.Die();

        enemy.Die();

        myDropSystem.GenerateDrop();
    }
}