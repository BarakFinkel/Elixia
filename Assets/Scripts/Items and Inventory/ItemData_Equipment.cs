using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Armor,
    Jewelry,
    Syringe
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Equipment")]
public class ItemData_Equipment : ItemData
{
    public EquipmentType equipmentType;

    [Header("Unique Effect")]
    public float itemCooldown;

    public ItemEffect[] itemEffects;

    // Stats:
    [Header("Major Stats")]
    public int strength;

    public int agility;
    public int intelligence;
    public int vitality;

    [Header("Offensive Stats")]
    public int damage;

    public int critChance;
    public int critPower;

    [Header("Defensive Stats")]
    public int health;

    public int armor;
    public int evasion;
    public int magicResistance;

    [Header("Magic Stats")]
    public int fireDamage;

    public int iceDamage;
    public int poisonDamage;
    public int arcaneDamage;

    [Header("Crafting Requirements")]
    public List<InventoryItem> craftingMaterials;

    private int descriptionLength;

    public void Effect(Transform _enemyPosition)
    {
        foreach (var item in itemEffects) item.ExecuteEffect(_enemyPosition);
    }

    public void AddModifiers()
    {
        var playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.AddModifier(strength);
        playerStats.agility.AddModifier(agility);
        playerStats.intelligence.AddModifier(intelligence);

        playerStats.damage.AddModifier(damage);
        playerStats.critChance.AddModifier(critChance);
        playerStats.critPower.AddModifier(critPower);

        var healthToHeal = (float)playerStats.currentHealth / playerStats.GetMaxHealthValue(); // current health percent
        playerStats.maxHealth.AddModifier(health);
        playerStats.vitality.AddModifier(vitality);
        playerStats.currentHealth = (int)(healthToHeal * playerStats.GetMaxHealthValue());

        playerStats.armor.AddModifier(armor);
        playerStats.evasion.AddModifier(evasion);
        playerStats.magicResistance.AddModifier(magicResistance);

        playerStats.fireDamage.AddModifier(fireDamage);
        playerStats.iceDamage.AddModifier(iceDamage);
        playerStats.poisonDamage.AddModifier(poisonDamage);
        playerStats.arcaneDamage.AddModifier(arcaneDamage);
    }

    public void RemoveModifiers()
    {
        var playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.RemoveModifier(strength);
        playerStats.agility.RemoveModifier(agility);
        playerStats.intelligence.RemoveModifier(intelligence);


        playerStats.damage.RemoveModifier(damage);
        playerStats.critChance.RemoveModifier(critChance);
        playerStats.critPower.RemoveModifier(critPower);

        var healthToHeal = (float)playerStats.currentHealth / playerStats.GetMaxHealthValue(); // current health percent
        playerStats.maxHealth.RemoveModifier(health);
        playerStats.vitality.RemoveModifier(vitality);
        playerStats.currentHealth = (int)(healthToHeal * playerStats.GetMaxHealthValue());

        playerStats.armor.RemoveModifier(armor);
        playerStats.evasion.RemoveModifier(evasion);
        playerStats.magicResistance.RemoveModifier(magicResistance);

        playerStats.fireDamage.RemoveModifier(fireDamage);
        playerStats.iceDamage.RemoveModifier(iceDamage);
        playerStats.poisonDamage.RemoveModifier(poisonDamage);
        playerStats.arcaneDamage.RemoveModifier(arcaneDamage);
    }

    public override string GetDescription(int minDescriptionLength)
    {
        sb.Length = 0;
        descriptionLength = 0;

        AddItemDescription(strength, "Strength");
        AddItemDescription(agility, "Agility");
        AddItemDescription(intelligence, "Intelligence");
        AddItemDescription(vitality, "Vitality");

        AddItemDescription(damage, "Damage");
        AddItemDescription(critChance, "Crit Chance");
        AddItemDescription(critPower, "Crit Power");

        AddItemDescription(health, "Health");
        AddItemDescription(evasion, "Evasion");
        AddItemDescription(armor, "Armor");
        AddItemDescription(magicResistance, "Magic Resist");

        AddItemDescription(fireDamage, "Fire Damage");
        AddItemDescription(iceDamage, "Ice Damage");
        AddItemDescription(poisonDamage, "Poison Damage");
        AddItemDescription(arcaneDamage, "Arcane Damage");

        for (var i = 0; i < itemEffects.Length; i++)
        {
            if (itemEffects[i].itemEffectDescription.Length > 0)
            {
                if (i == 0 && sb.Length > 0)
                {
                    sb.AppendLine();
                }

                sb.AppendLine("* " + itemEffects[i].itemEffectDescription);

                if (i != itemEffects.Length - 1)
                {
                    sb.AppendLine();
                }

                descriptionLength++;
            }
        }

        return sb.ToString();
    }

    private void AddItemDescription(int _value, string _name)
    {
        if (_value != 0)
        {
            if (_value > 0)
            {
                sb.Append("+ " + _value + " " + _name);
            }

            sb.AppendLine();
            descriptionLength++;
        }
    }
}