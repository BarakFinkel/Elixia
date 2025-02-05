using UnityEngine;

public class PlayerStats : CharacterStats
{
    private Player player;

    protected override void Start()
    {
        base.Start();

        player = GetComponent<Player>();
    }

    // Calculation Pipeline of the physical damage dealt to the target.
    public override void DoDamage(CharacterStats _targetStats)
    {
        if (_targetStats.isInvulnerable)
        {
            return;
        }

        _targetStats.GetComponent<Entity>().SetupKnockbackDir(transform);

        // We check if the target evades the attack, is so - we return and do nothing.
        if (TargetCanEvadeAttack(_targetStats))
        {
            AudioManager.instance.PlaySFX(57, 0, null);
            return;
        }

        AudioManager.instance.PlaySFX(40, 0, null);

        // Calculating damage
        var totalDamage = damage.GetValue() + strength.GetValue();

        // If we can crit the target - we multiply the damage by the required percentage.
        if (CanCritTarget())
        {
            totalDamage = CalculateCritDamage(totalDamage);
        }

        // We lower the output damage according to the target's armor
        totalDamage = TargetPhysicalDamageReduction(_targetStats, totalDamage);

        // We deal the overall damage to the target.
        _targetStats.TakeDamage(totalDamage);

        // If we want the character to passively do magic damage on phsyical attacks, we allow it via the usingPrimaryMagicType bool
        if (usingPrimaryMagicType)
        {
            DoMagicalDamage(_targetStats, primaryMagicType, 0);
        }
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    protected override void Die()
    {
        base.Die();
        player.Die();

        GameManager.instance.lostCurrencyAmount =
            PlayerManager.instance.currency; // Mark the currency as lost, moving it to the game manager.
        PlayerManager.instance.currency = 0; // Reset the current currency to 0.

        GetComponent<PlayerItemDrop>()?.GenerateDrop();
    }

    protected override void DecreaseHealthBy(int _damage)
    {
        base.DecreaseHealthBy(_damage);

        // If the player receives massive damage, bigger than 30 percent of his max HP, we knock him back.
        if (_damage > GetMaxHealthValue() * 0.3f)
        {
            player.SetupKnockbackPower(new Vector2(8, 6));
        }

        // Apply on-take-hit effects when health is decreased.
        var currentArmor = Inventory.instance.GetEquipmentOfType(EquipmentType.Armor);
        if (currentArmor != null)
        {
            currentArmor.Effect(player.transform);
        }
    }

    public override void OnEvasion()
    {
        player.skillManager.evade.CreateCloneOnEvade();
    }

    public void CloneDoDamage(CharacterStats _targetStats, float _attackMultiplier)
    {
        if (_targetStats.isInvulnerable)
        {
            return;
        }

        // We check if the target evades the attack, is so - we return and do nothing.
        if (TargetCanEvadeAttack(_targetStats))
        {
            return;
        }

        // Calculating damage
        var totalDamage = damage.GetValue() + strength.GetValue();

        if (_attackMultiplier > 0.0f)
        {
            totalDamage = Mathf.RoundToInt(totalDamage * _attackMultiplier);
        }

        // If we can crit the target - we multiply the damage by the required percentage.
        if (CanCritTarget())
        {
            totalDamage = CalculateCritDamage(totalDamage);
        }

        // We lower the output damage according to the target's armor
        totalDamage = TargetPhysicalDamageReduction(_targetStats, totalDamage);

        // We deal the overall damage to the target.
        _targetStats.TakeDamage(totalDamage);
    }
}