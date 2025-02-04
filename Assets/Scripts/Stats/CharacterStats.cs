using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

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
    arcaneDamage,
    lightningDamage
}

public enum MagicType
{
    Fire,
    Ice,
    Poison,
    Arcane,
    Lightning
}

public class CharacterStats : MonoBehaviour
{
    [Header("Major Stats")]
    public Stat strength; // +1 Strength == +1 attack dmg, +1% crit dmg

    public Stat agility; // +1 Agility == +1 evasion, +1% crit chance
    public Stat intelligence; // +1 Intelligence == +1 magic dmg, +[]% magic resistance

    [SerializeField]
    public int intToMR = 3;

    public Stat vitality; // +1 Vitality == +5 Max Hp

    [SerializeField]
    public int vitToHP = 5;

    [Header("Offensive Stats")]
    public Stat damage;

    public Stat critChance;
    public Stat critPower; // Power of an attack when critical.

    [SerializeField]
    private int baseCritPower = 150;

    [Header("Defensive Stats")]
    public Stat maxHealth;

    public Stat armor;

    [SerializeField]
    private int armorBalanceFactor = 300;

    public Stat magicResistance;

    [SerializeField]
    private int mrBalanceFactor = 300;

    [Range(1.0f, 2.0f)]
    [SerializeField]
    private float stunShockDamageMultiplier = 1.1f;

    public Stat evasion;

    [Header("Magic Stats")]
    [Space]
    [SerializeField]
    public MagicType primaryMagicType = MagicType.Fire;

    [SerializeField]
    public bool usingPrimaryMagicType;

    [Space]
    [Header("Fire")]
    public Stat fireDamage;

    public bool isIgnited; // Does fast damage over time.

    [SerializeField]
    private float burnDamageRatio = 0.2f; // ration of the fire damage applied in every damage tick.

    [SerializeField]
    private float ignitedDuration = 2.0f;

    [SerializeField]
    private float ignitedTickFrequency = 0.25f;

    [Header("Ice")]
    public Stat iceDamage;

    public bool isChilled; // Slows and reduces armor.

    [SerializeField]
    private int chilledArmorReduction = 20;

    [SerializeField]
    private float slowPercentage = 0.4f;

    [SerializeField]
    private float chilledDuration = 2.0f;

    [Header("Poison")]
    public Stat poisonDamage;

    public bool isPoisoned; // Does slow damage over time, reduces hit accuracy.

    [SerializeField]
    private float poisonedDamageRatio = 0.5f; // ration of the poison damage applied in every damage tick.

    [SerializeField]
    private int poisonedAccuracyReduction = 20;

    [SerializeField]
    private float poisonedDuration = 5.0f;

    [SerializeField]
    private float poisonedTickFrequency = 1.0f;

    [Header("Arcane")]
    public Stat arcaneDamage;

    public bool isEnchanted; // Reduces magic resistance.

    [SerializeField]
    private float enchantedMagicResistReduction = 20;

    [SerializeField]
    private float enchantedDuration = 3.0f;

    [Header("Electricity")]
    public Stat lightningDamage;

    public bool isElectrified;

    [SerializeField]
    private float electrifiedDuration = 5.0f;

    [SerializeField]
    private GameObject lightningStrikePrefab;

    public int currentHealth;
    private int burnDamage;
    private float chilledTimer;
    private float electrifiedTimer;
    private float enchantedTimer;
    private EntityFX fx;
    private float ignitedTickTimer;
    private float ignitedTimer;
    private int lightningDmg;

    public Action onHealthChanged;
    private int poisonedDamage;
    private float poisonedTickTimer;
    private float poisonedTimer;

    public bool isStunShocked { get; private set; }
    public bool isInvulnerable { get; private set; }
    public bool isDead { get; private set; }

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

        // Shock Timers:
        electrifiedTimer = UpdateTimer(electrifiedTimer);
        HandleElectricityAilment();
    }

    // Calculation Pipeline of the physical damage dealt to the target.
    public virtual void DoDamage(CharacterStats _targetStats)
    {
        if (_targetStats.isInvulnerable)
        {
            return;
        }

        _targetStats.GetComponent<Entity>().SetupKnockbackDir(transform);

        // We check if the target evades the attack, is so - we return and do nothing.
        if (TargetCanEvadeAttack(_targetStats))
        {
            return;
        }

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
        if (isStunShocked)
        {
            _damage = Mathf.RoundToInt(_damage * stunShockDamageMultiplier);
        }

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

    public void StunShockFor(float _duration)
    {
        StartCoroutine(StunShockedCoroutine(_duration));
    }

    public IEnumerator StunShockedCoroutine(float _duration)
    {
        isStunShocked = true;

        yield return new WaitForSeconds(_duration);

        isStunShocked = false;
    }

    private IEnumerator StartModCoroutine(Stat _statToModifiy, int _modifier, float _duration)
    {
        _statToModifiy.AddModifier(_modifier);
        Inventory.instance.UpdateUISlots();

        yield return new WaitForSeconds(_duration);

        _statToModifiy.RemoveModifier(_modifier);
        Inventory.instance.UpdateUISlots();
    }

    public void EnableInvulnerabilityFor(float _seconds)
    {
        EnableInvulnerability();
        Invoke("DisableInvulnerability", _seconds);
    }

    public void EnableInvulnerability()
    {
        isInvulnerable = true;
    }

    public void DisableInvulnerability()
    {
        isInvulnerable = false;
    }

    protected virtual void Die()
    {
        isDead = true;
    }

    public virtual void OnEvasion()
    {
    }

    // Method to update timers down to 0.
    private float UpdateTimer(float currentTime)
    {
        if (currentTime > 0)
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

    #region Magical Damage and Ailments Handling

    public virtual void DoMagicalDamage(CharacterStats _targetStats, MagicType _magicType, int _damage)
    {
        // Seting the magical types' damage
        var _fireDamage = _magicType == MagicType.Fire ? fireDamage.GetValue() : 0;
        var _iceDamage = _magicType == MagicType.Ice ? iceDamage.GetValue() : 0;
        var _poisonDamage = _magicType == MagicType.Poison ? poisonDamage.GetValue() : 0;
        var _arcaneDamage = _magicType == MagicType.Arcane ? arcaneDamage.GetValue() : 0;
        var _lightningDamage = _magicType == MagicType.Lightning ? lightningDamage.GetValue() : 0;

        // Summing up them all.
        var totalMagicalDamage = _damage + _fireDamage + _iceDamage + _poisonDamage + _arcaneDamage + _lightningDamage +
                                 intelligence.GetValue();

        // Calculating MR damage reduction.
        totalMagicalDamage = TargetMagicalDamageReduction(_targetStats, totalMagicalDamage);
        _targetStats.TakeDamage(totalMagicalDamage);

        // Choosing the element which will apply ailment to the target.
        AttemptApplyingAilments(_targetStats, _magicType, totalMagicalDamage);
    }

    private void AttemptApplyingAilments(CharacterStats _targetStats, MagicType _magicType, float _totalDamage)
    {
        // We check which of the elements we can apply
        var canApplyIgnite = _magicType == MagicType.Fire;
        var canApplyChill = _magicType == MagicType.Ice;
        var canApplyPoison = _magicType == MagicType.Poison;
        var canApplyEnchant = _magicType == MagicType.Arcane;
        var canApplyElectricity = _magicType == MagicType.Lightning;

        // In the following if statements, we change the overtime damage the target takes accordingly, or keep it the same if unnecessary.
        if (canApplyIgnite)
        {
            _targetStats.SetupBurnDamage(Mathf.RoundToInt(_totalDamage * burnDamageRatio));
        }

        if (canApplyPoison)
        {
            _targetStats.SetupPoisonedDamage(Mathf.RoundToInt(_totalDamage * poisonedDamageRatio));
        }

        if (canApplyElectricity)
        {
            _targetStats.SetupLightningDamage(Mathf.RoundToInt(_totalDamage));
        }

        // Lastly, we apply the chosen ailment to the target.
        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyPoison, canApplyEnchant, canApplyElectricity);
    }

    public void ApplyAilments(bool _ignited, bool _chilled, bool _poisoned, bool _enchanted, bool _electrified)
    {
        if (_ignited)
        {
            DisableAllAilments();

            isIgnited = _ignited;
            ignitedTimer = ignitedDuration;

            fx.IgnitedFxFor(ignitedTimer);
        }

        if (_chilled)
        {
            DisableAllAilments();

            isChilled = _chilled;
            chilledTimer = chilledDuration;

            GetComponent<Entity>().SlowEntityBy(slowPercentage, chilledTimer);
            fx.ChilledFxFor(chilledTimer);
        }

        if (_poisoned)
        {
            DisableAllAilments();

            isPoisoned = _poisoned;
            poisonedTimer = poisonedDuration;

            fx.PoisonedFxFor(poisonedTimer);
        }

        if (_enchanted)
        {
            DisableAllAilments();

            isEnchanted = _enchanted;
            enchantedTimer = enchantedDuration;

            fx.EnchantedFxFor(enchantedTimer);
        }

        if (_electrified)
        {
            DisableAllAilments();

            if (!isElectrified)
            {
                ApplyShock(_electrified);
            }
            else if (GetComponent<Player>() == null)
            {
                HitNearesTargetWithLightningStrike();
            }
        }
    }

    private void DisableAllAilments()
    {
        isIgnited = false;
        isChilled = false;
        isPoisoned = false;
        isEnchanted = false;
        isElectrified = false;
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

    private void HandleElectricityAilment()
    {
        if (electrifiedTimer == 0)
        {
            isElectrified = false;
        }
    }

    public void SetupBurnDamage(int _damage)
    {
        burnDamage = _damage;
    }

    public void SetupPoisonedDamage(int _damage)
    {
        poisonedDamage = _damage;
    }

    public void SetupLightningDamage(int _damage)
    {
        lightningDmg = _damage;
    }

    public void ApplyShock(bool _shock)
    {
        if (isElectrified)
        {
            return;
        }

        electrifiedTimer = electrifiedDuration;
        isElectrified = _shock;
        fx.ElectricityFXFor(electrifiedTimer);
    }

    private void HitNearesTargetWithLightningStrike()
    {
        var detectingRadius = 25;
        var colliders = Physics2D.OverlapCircleAll(transform.position, detectingRadius);

        var closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in colliders)
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)
            {
                var distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
            else if (closestEnemy == null)
            {
                closestEnemy = transform;
            }

        if (closestEnemy != null)
        {
            var newShockStrike = Instantiate(lightningStrikePrefab, transform.position, Quaternion.identity);
            newShockStrike.GetComponent<LightningStrikeController>()
                .Setup(lightningDmg, closestEnemy.GetComponent<CharacterStats>());
        }
    }

    #endregion

    #region Stats Calculation

    protected bool TargetCanEvadeAttack(CharacterStats _targetStats)
    {
        var totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isPoisoned)
        {
            totalEvasion += poisonedAccuracyReduction;
        }

        if (Random.Range(0, 100) < totalEvasion)
        {
            _targetStats.OnEvasion();
            Debug.Log("I evaded!");
            return true;
        }

        return false;
    }

    protected int TargetPhysicalDamageReduction(CharacterStats _targetStats, int totalDamage)
    {
        var currArmor = _targetStats.armor.GetValue();

        // If the target is chilled, remove a portion of it's armor
        if (_targetStats.isChilled)
        {
            currArmor = Mathf.RoundToInt(currArmor * (1 - chilledArmorReduction / 100.0f));
        }

        // Calculates the damage after relational reduction by the armor
        totalDamage = Mathf.RoundToInt(totalDamage * (1 - currArmor / (currArmor + armorBalanceFactor)));

        return Mathf.Clamp(totalDamage, 0, int.MaxValue);
    }

    protected int TargetMagicalDamageReduction(CharacterStats _targetStats, int totalMagicalDamage)
    {
        var currMR = _targetStats.magicResistance.GetValue();

        // If the target is enchanted, remove a portion of it's magical resistance
        if (_targetStats.isEnchanted)
        {
            currMR = Mathf.RoundToInt(currMR * (1 - enchantedMagicResistReduction / 100.0f));
        }

        // Calculates the damage after relational reduction by the magical resistance
        totalMagicalDamage = Mathf.RoundToInt(totalMagicalDamage * (1 - currMR / (currMR + mrBalanceFactor)));

        return Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
    }

    protected bool CanCritTarget()
    {
        var totalCriticalChance = critChance.GetValue() + agility.GetValue();

        if (Random.Range(0, 100) <= totalCriticalChance)
        {
            return true;
        }

        return false;
    }

    // Calculates total crit damage
    protected int CalculateCritDamage(int _damage)
    {
        var totalCritPower =
            (critPower.GetValue() + strength.GetValue()) *
            .01f; // Not a magic number, used to convert the crit power to a multiplier from a percentage.
        var critDamage = _damage * totalCritPower;

        return Mathf.RoundToInt(critDamage);
    }

    // Returns total current HP.
    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue() * vitToHP;
    }

    #endregion
}