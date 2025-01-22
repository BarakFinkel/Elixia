using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public enum StatType
{
    strength,
    agility,
    intelligence,
    vitality,
    damage,
    critChance,
    critPower,
    health,
    armor,
    evasion,
    magicResistance,
    fireDamage,
    iceDamage,
    poisonDamage,
    arcaneDamage
}

public class CharacterStats : MonoBehaviour
{
    EntityFX fx;

    [Header("Major Stats")]
    public Stat strength; // +1 Strength == +1 attack dmg, +1% crit dmg
    public Stat agility; // +1 Agility == +1 evasion, +1% crit chance
    public Stat intelligence; // +1 Intelligence == +1 magic dmg, +[]% magic resistance
    [SerializeField] public int intToMR = 3;
    public Stat vitality; // +1 Vitality == +5 Max Hp
    [SerializeField] public int vitToHP = 5;

    [Header("Offensive Stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower; // Power of an attack when critical.
    [SerializeField] int baseCritPower = 150;

    [Header("Defensive Stats")]
    public Stat maxHealth;
    public Stat armor;
    [SerializeField] private int armorBalanceFactor = 300;
    public Stat magicResistance;
    [SerializeField] private int mrBalanceFactor = 300;
    public Stat evasion;

    [Header("Magic Stats")]
    [Space]
    [Header("Fire")]
    public Stat fireDamage;
    public bool isIgnited; // Does fast damage over time.
    [SerializeField] float burnDamageRatio = 0.2f; // ration of the fire damage applied in every damage tick.
    [SerializeField] float ignitedDuration = 2.0f;
    [SerializeField] float ignitedTickFrequency = 0.25f;
    private int burnDamage;
    private float ignitedTimer;
    private float ignitedTickTimer;

    [Header("Ice")]
    public Stat iceDamage;
    public bool isChilled; // Slows and reduces armor.
    [SerializeField] private int chilledArmorReduction = 20;
    [SerializeField] private float slowPercentage = 0.4f;
    [SerializeField] float chilledDuration = 2.0f;
    private float chilledTimer;

    [Header("Poison")]
    public Stat poisonDamage;
    public bool isPoisoned; // Does slow damage over time, reduces hit accuracy.
    [SerializeField] float poisonedDamageRatio = 0.5f; // ration of the poison damage applied in every damage tick.
    [SerializeField] private int poisonedAccuracyReduction = 20;
    [SerializeField] float poisonedDuration = 5.0f;
    [SerializeField] float poisonedTickFrequency = 1.0f;
    private int poisonedDamage;
    private float poisonedTimer;
    private float poisonedTickTimer;

    [Header("Arcane")]
    public Stat arcaneDamage;
    public bool isEnchanted; // Reduces magic resistance.
    [SerializeField] float enchantedMagicResistReduction = 20;
    [SerializeField] float enchantedDuration = 3.0f;
    private float enchantedTimer;

    public int currentHealth;

    public System.Action onHealthChanged;
    public bool isDead { get; private set; } = false;

    protected virtual void Start()
    {
        fx = GetComponent<EntityFX>();

        currentHealth = GetMaxHealthValue();
        critPower.SetValue(baseCritPower);
    }

    protected virtual void Update()
    {
        // Ignite Timers:
        ignitedTimer = UpdateTimer(ignitedTimer);
        ignitedTickTimer = UpdateTimer(ignitedTickTimer);
        HandleIgnitedAilment();

        // Chill Timers:
        chilledTimer = UpdateTimer(chilledTimer);
        HandleChilledAilment();

        // Poison Timers:
        poisonedTimer = UpdateTimer(poisonedTimer);
        poisonedTickTimer = UpdateTimer(poisonedTickTimer);
        HandlePoisonedAilment();

        // Enchant Timers:
        enchantedTimer = UpdateTimer(enchantedTimer);
        HandleEnchantedAilment();
    }

    // Calculation Pipeline of the physical damage dealt to the target.
    public virtual void DoDamage(CharacterStats _targetStats)
    {
        // We check if the target evades the attack, is so - we return and do nothing.
        if(TargetCanEvadeAttack(_targetStats))
        {
            return;
        }

        // Calculating damage
        int totalDamage = damage.GetValue() + strength.GetValue();

        // If we can crit the target - we multiply the damage by the required percentage.
        if(CanCritTarget())
        {
            totalDamage = CalculateCritDamage(totalDamage);
        }

        // We lower the output damage according to the target's armor
        totalDamage = TargetPhysicalDamageReduction(_targetStats, totalDamage);

        // We deal the overall damage to the target.
        _targetStats.TakeDamage(totalDamage);
        
        //DoMagicalDamage(_targetStats);
    }

    // Method responsible for taking impactful damage (ailments not included)
    public virtual void TakeDamage(int _damage)
    {
        DecreaseHealthBy(_damage);

        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine("FlashFX");

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    // Method responsible for reducing heatlh without impact
    protected virtual void DecreaseHealthBy(int _damage)
    {
        currentHealth -= _damage;

        if (onHealthChanged != null)
        {
            onHealthChanged();
        }
    }

    // Method responsible for healing
    public virtual void IncreaseHealthBy(int _healAmount)
    {
        currentHealth += _healAmount;

        if (currentHealth > GetMaxHealthValue())
        {
            currentHealth = GetMaxHealthValue();
        }

        if (onHealthChanged != null)
        {
            onHealthChanged();
        }
    }

    public virtual void IncreaseStatBy(Stat _statToModifiy, int _modifier, float _duration)
    {
        StartCoroutine(StartModCoroutine(_statToModifiy, _modifier, _duration));
    }

    private IEnumerator StartModCoroutine(Stat _statToModifiy, int _modifier, float _duration)
    {
        _statToModifiy.AddModifier(_modifier);
        Inventory.instance.UpdateUISlots();

        yield return new WaitForSeconds(_duration);

        _statToModifiy.RemoveModifier(_modifier);
        Inventory.instance.UpdateUISlots();
    }

    protected virtual void Die()
    {
        isDead = true;
    }

    #region Magical Damage and Ailments Handling

    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {
        // Seting the magical types' damage
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _poisonDamage = poisonDamage.GetValue();
        int _arcaneDamage = arcaneDamage.GetValue();

        // Summing up them all.
        int totalMagicalDamage = _fireDamage + _iceDamage + _poisonDamage + _arcaneDamage + intelligence.GetValue();
        
        // Calculating MR damage reduction.
        totalMagicalDamage = TargetMagicalDamageReduction(_targetStats, totalMagicalDamage);
        _targetStats.TakeDamage(totalMagicalDamage);

        // Choosing the element which will apply ailment to the target.
        AttemptApplyingAilments(_targetStats, _fireDamage, _iceDamage, _poisonDamage, _arcaneDamage);
    }

    private void AttemptApplyingAilments(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _poisonDamage, int _arcaneDamage)
    {
        // Choosing the element which will apply ailment to the target.
        string DominantElement = GetMaxDamageElement(_fireDamage, _iceDamage, _poisonDamage, _arcaneDamage);

        // If there was no element returned, it means that all elements have no damage, so we can't apply an ailment and return.
        if (DominantElement == string.Empty)
        {
            return;
        }

        // We check which of the elements we can apply
        bool canApplyIgnite = DominantElement == "Fire";
        bool canApplyChill = DominantElement == "Ice";
        bool canApplyPoison = DominantElement == "Poison";
        bool canApplyEnchant = DominantElement == "Arcane";

        // In the following if statements, we change the overtime damage the target takes accordingly, or keep it the same if unnecessary.
        if (canApplyIgnite)
        {
            _targetStats.SetupBurnDamage(Mathf.RoundToInt(_fireDamage * burnDamageRatio));
        }
        
        if (canApplyPoison)
        {
            _targetStats.SetupPoisonedDamage(Mathf.RoundToInt(_poisonDamage * poisonedDamageRatio));
        }

        // Lastly, we apply the chosen ailment to the target.
        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyPoison, canApplyEnchant);
    }

    private string GetMaxDamageElement(int _fireDamage, int _iceDamage, int _poisonDamage, int _arcaneDamage)
    {
        // Check if there is no magical damage type greater than 0
        if (Mathf.Max(_fireDamage, _iceDamage, _poisonDamage, _arcaneDamage) <= 0)
        {
            return string.Empty;
        }

        // Create a list to hold elements with maximum damage
        List<string> tiedElements = new List<string>();
        float maxDamage = Mathf.Max(_fireDamage, _iceDamage, _poisonDamage, _arcaneDamage);

        // Add elements that are tied in damage
        if (_fireDamage == maxDamage && _fireDamage > 0) tiedElements.Add("Fire");
        if (_iceDamage == maxDamage && _iceDamage > 0) tiedElements.Add("Ice");
        if (_poisonDamage == maxDamage && _poisonDamage > 0) tiedElements.Add("Poison");
        if (_arcaneDamage == maxDamage && _arcaneDamage > 0) tiedElements.Add("Arcane");

        // If there's only one tied element, return it directly
        if (tiedElements.Count == 1)
        {
            return tiedElements[0];
        }

        // Randomly select one of the tied elements
        int randomIndex = Random.Range(0, tiedElements.Count);
        return tiedElements[randomIndex];
    }

    public void ApplyAilments(bool _ignited, bool _chilled, bool _poisoned, bool _enchanted)
    {
        if (isIgnited || isChilled || isPoisoned || isEnchanted)
        {
            return;
        }

        if(_ignited)
        {
            isIgnited = _ignited;
            ignitedTimer = ignitedDuration;

            fx.IgnitedFxFor(ignitedTimer);
        }

        if(_chilled)
        {
            isChilled = _chilled;
            chilledTimer = chilledDuration;

            GetComponent<Entity>().SlowEntityBy(slowPercentage, chilledTimer);
            fx.ChilledFxFor(chilledTimer);
        }

        if(_poisoned)
        {
            isPoisoned = _poisoned;
            poisonedTimer = poisonedDuration;

            fx.PoisonedFxFor(poisonedTimer);
        }

        if(_enchanted)
        {
            isEnchanted = _enchanted;
            enchantedTimer = enchantedDuration;

            fx.EnchantedFxFor(enchantedTimer);
        }
    }

    private void HandleIgnitedAilment()
    {
        // We stop being ignited when the timer reaches 0.
        if (ignitedTimer == 0)
        {
            isIgnited = false;
        }

        // We take a burning tick of damage when the tick timer reaches 0.
        if (ignitedTickTimer == 0 && isIgnited)
        {
            DecreaseHealthBy(burnDamage);

            if (currentHealth <= 0 && !isDead)
            {
                Die();
            }

            ignitedTickTimer = ignitedTickFrequency;
        }
    }

    private void HandleChilledAilment()
    {
        // We stop being ignited when the timer reaches 0.
        if (chilledTimer == 0)
        {
            isChilled = false;
        }
    }

    private void HandlePoisonedAilment()
    {
        // We stop being ignited when the timer reaches 0.
        if (poisonedTimer == 0)
        {
            isPoisoned = false;
        }

        // We take a burning tick of damage when the tick timer reaches 0.
        if (poisonedTickTimer == 0 && isIgnited)
        {
            DecreaseHealthBy(poisonedDamage);

            if (currentHealth <= 0 && !isDead)
            {
                Die();
            }

            poisonedTickTimer = poisonedTickFrequency;
        }        
    }

    private void HandleEnchantedAilment()
    {
        // We stop being ignited when the timer reaches 0.
        if (enchantedTimer == 0)
        {
            isEnchanted = false;
        }
    }

    public void SetupBurnDamage(int _damage) => burnDamage = _damage;
    public void SetupPoisonedDamage(int _damage) => poisonedDamage = _damage;

    #endregion

    #region Stats Calculation

    private bool TargetCanEvadeAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isPoisoned)
        {
            totalEvasion += poisonedAccuracyReduction;
        }
        
        if (Random.Range(0, 100) < totalEvasion)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private int TargetPhysicalDamageReduction(CharacterStats _targetStats, int totalDamage)
    {
        int currArmor = _targetStats.armor.GetValue();
        
        // If the target is chilled, remove a portion of it's armor
        if(_targetStats.isChilled)
        {
            currArmor = Mathf.RoundToInt(currArmor * (1 - (chilledArmorReduction / 100.0f)));
        }

        // Calculates the damage after relational reduction by the armor
        totalDamage = Mathf.RoundToInt(totalDamage * (1 - (currArmor / (currArmor + armorBalanceFactor))));
         
        return Mathf.Clamp(totalDamage, 0, int.MaxValue);
    }

    private int TargetMagicalDamageReduction(CharacterStats _targetStats, int totalMagicalDamage)
    {
        int currMR = _targetStats.magicResistance.GetValue();
        
        // If the target is enchanted, remove a portion of it's magical resistance
        if(_targetStats.isEnchanted)
        {
            currMR = Mathf.RoundToInt(currMR * (1 - (enchantedMagicResistReduction / 100.0f)));
        }

        // Calculates the damage after relational reduction by the magical resistance
        totalMagicalDamage = Mathf.RoundToInt(totalMagicalDamage * (1 - (currMR / (currMR + mrBalanceFactor))));
         
        return Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
    }

    private bool CanCritTarget()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();

        if(Random.Range(0,100) <= totalCriticalChance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Calculates total crit damage
    private int CalculateCritDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * .01f; // Not a magic number, used to convert the crit power to a multiplier from a percentage.
        float critDamage = _damage * totalCritPower;

        return Mathf.RoundToInt(critDamage);
    }

    // Returns total current HP.
    public int GetMaxHealthValue() => maxHealth.GetValue() + vitality.GetValue() * vitToHP;

    #endregion

    // Method to update timers down to 0.
    private float UpdateTimer(float currentTime)
    {
        if(currentTime > 0)
        {
            return Mathf.Max(0, currentTime - Time.deltaTime);
        }

        return currentTime;
    }

    public Stat GetStat(StatType _statType)
    {
        switch (_statType)
        {
            case StatType.strength:
            {
                return strength;
            }
            case StatType.agility:
            {
                return agility;
            }
            case StatType.intelligence:
            {
                return intelligence;
            }
            case StatType.vitality:
            {
                return vitality;
            }
            case StatType.damage:
            {
                return damage;
            }
            case StatType.critChance:
            {
                return critChance;
            }
            case StatType.critPower:
            {
                return critPower;
            }
            case StatType.health:
            {
                return maxHealth;
            }
            case StatType.armor:
            {
                return armor;
            }
            case StatType.evasion:
            {
                return evasion;
            }
            case StatType.magicResistance:
            {
                return magicResistance;
            }
            case StatType.fireDamage:
            {
                return fireDamage;
            }
            case StatType.iceDamage:
            {
                return iceDamage;
            }
            case StatType.poisonDamage:
            {
                return poisonDamage;
            }
            case StatType.arcaneDamage:
            {
                return arcaneDamage;
            }
            default:
            {
                return null;
            }
        }
    }
}