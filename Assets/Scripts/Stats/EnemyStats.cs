using UnityEngine;

public class EnemyStats : CharacterStats
{
    [Header("Level Details")]
    [SerializeField]
    private int level = 1;

    [SerializeField]
    private int startAmount = 10000;

    [Range(0f, 1f)]
    [SerializeField]
    private float percentageModifier = 0.4f;

    private readonly Stat soulsDropAmount = new();

    private Enemy enemy;
    private ItemDrop myDropSystem;

    protected override void Start()
    {
        soulsDropAmount.SetValue(startAmount);
        ApplyLevelModifiers();

        base.Start();

        enemy = GetComponent<Enemy>();
        myDropSystem = GetComponent<ItemDrop>();
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
        Modify(lightningDamage);

        Modify(soulsDropAmount);
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

        PlayerManager.instance.currency += soulsDropAmount.GetValue();
        myDropSystem.GenerateDrop();
    }
}