using UnityEngine;

public class EnemyStats : CharacterStats
{
    [Header("Level Details")]
    [SerializeField]
    private int level = 1;

    [Range(0f, 1f)]
    [SerializeField]
    private float percentageModifier = 0.4f;

    private Enemy enemy;
    private ItemDrop myDropSystem => GetComponent<ItemDrop>();

    protected override void Start()
    {
        ApplyLevelModifiers();

        base.Start();

        enemy = GetComponent<Enemy>();
    }

    private void ApplyLevelModifiers()
    {
        Modify(strength);
        Modify(agility);
        Modify(intelligence);
        Modify(vitality);

        Modify(damage);
        Modify(critChance);
        Modify(critPower);

        Modify(maxHealth);
        Modify(armor);
        Modify(evasion);
        Modify(magicResistance);

        Modify(fireDamage);
        Modify(iceDamage);
        Modify(poisonDamage);
        Modify(arcaneDamage);
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