using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterStats : MonoBehaviour
{
    [Header("Major Stats")] public Stat strength; // 1 point dmg increase by 1 and crit.power by 1%

    public Stat agility; // 1 point evaison increase by 1 and crit.chance by 1%
    public Stat intelligence; // 1 point magic dmg increase by 1 and magic resist by 3
    public Stat vitality; // 1 point increase hlth by 3 or 5 points

    [Header("Offensive Stats")] public Stat damage;

    public Stat critChance;
    public Stat critPower; // default 150%

    [Header("Defensive Stats")] public Stat maxHp;

    public Stat armor;
    public Stat evasion;
    public Stat magicResist;

    [Header("Magic Stats")] public Stat fireDamage;

    public Stat iceDamage;
    public Stat lightningDamage;

    public bool isIgnited; // does dmg over time
    public bool isChilled; // armor decreased
    public bool isShocked; // reduce accuracy

    public float ailmentsDuration = 4;
    [SerializeField] private GameObject shockStrikePrefab;

    public int currHp;
    private float chilledTimer;
    private EntityFx fx;
    private int igniteDmg;

    private readonly float igniteDmgCooldown = .3f;
    private float igniteDmgTimer;
    private float ignitedTimer;

    protected bool isDead;

    public Action onHealthChanged;
    private int shockDmg;
    private float shockedTimer;

    protected virtual void Start()
    {
        currHp = GetMaxHealthValue();
        critPower.SetDefaultValue(150);

        fx = GetComponent<EntityFx>();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        igniteDmgTimer -= Time.deltaTime;

        if (ignitedTimer <= 0)
        {
            isIgnited = false;
        }

        if (chilledTimer <= 0)
        {
            isChilled = false;
        }

        if (shockedTimer <= 0)
        {
            isShocked = false;
        }


        if (isIgnited)
        {
            ApplyIgniteDmg();
        }
    }


    public virtual void DoDamage(CharacterStats _targetStats)
    {
        if (TargetCanAvoidAttack(_targetStats))
        {
            return;
        }

        var totalDamage = strength.GetValue() + damage.GetValue();

        if (CanCrit())
        {
            totalDamage = CalculateCritDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);
        //DoMagicalDamage(_targetStats);
    }

    protected virtual void Die()
    {
        isDead = true;
    }

    public virtual void TakeDamage(int _damage)
    {
        DecreaseHealthBy(_damage);

        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine("FlashFx");

        if (currHp <= 0 && !isDead)
        {
            Die();
        }
    }

    protected virtual void DecreaseHealthBy(int _damage)
    {
        currHp -= _damage;
        if (onHealthChanged != null)
        {
            onHealthChanged();
        }
    }

    #region Magic damage and ailments

    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {
        var _fireDmg = fireDamage.GetValue();
        var _iceDmg = iceDamage.GetValue();
        var _lightningDmg = lightningDamage.GetValue();
        var totalMagicalDamage = _fireDmg + _iceDmg + _lightningDmg + intelligence.GetValue();

        totalMagicalDamage = CheckTargetResist(_targetStats, totalMagicalDamage);

        _targetStats.TakeDamage(totalMagicalDamage);

        if (Mathf.Max(_fireDmg, _iceDmg, _lightningDmg) <= 0)
        {
            return;
        }

        AttemptToApplyAilments(_targetStats, _fireDmg, _iceDmg, _lightningDmg);
    }

    private void AttemptToApplyAilments(CharacterStats _targetStats, int _fireDmg, int _iceDmg, int _lightningDmg)
    {
        // higher dmg effect will be applied
        var canApplyIgnite = _fireDmg > _iceDmg && _fireDmg > _lightningDmg;
        var canApplyChill = _iceDmg > _fireDmg && _iceDmg > _lightningDmg;
        var canApplyShock = _lightningDmg > _fireDmg && _lightningDmg > _iceDmg;

        // randomly choose one if not choosen early
        if (!canApplyChill && !canApplyIgnite && !canApplyShock)
        {
            var rnd = Random.value; // returns value between 1 and 0
            if (rnd < .33f && _fireDmg > 0)
            {
                canApplyIgnite = true;
                Debug.Log("Ignite");
            }
            else if (rnd < .66f && _iceDmg > 0)
            {
                canApplyChill = true;
                Debug.Log("Chill");
            }
            else if (_lightningDmg > 0)
            {
                canApplyShock = true;
                Debug.Log("Shock");
            }
        }

        if (canApplyIgnite)
        {
            _targetStats.SetupIgniteDmg(Mathf.RoundToInt(_fireDmg * .2f));
        }

        if (canApplyShock)
        {
            _targetStats.SetupShockDmg(Mathf.RoundToInt(_lightningDmg * .1f));
        }

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    public void SetupIgniteDmg(int _damage)
    {
        igniteDmg = _damage;
    }

    public void SetupShockDmg(int _damage)
    {
        shockDmg = _damage;
    }

    private int CheckTargetResist(CharacterStats _targetStats, int totalMagicalDamage)
    {
        totalMagicalDamage -= _targetStats.magicResist.GetValue() + _targetStats.intelligence.GetValue() * 3;
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
        return totalMagicalDamage;
    }

    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        var canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        var canApplyChill = !isIgnited && !isChilled && !isShocked;
        var canApplyShock = !isIgnited && !isChilled;

        if (_ignite && canApplyIgnite)
        {
            isIgnited = _ignite;
            ignitedTimer = ailmentsDuration;
            fx.IgniteFXFor(ailmentsDuration);
        }

        if (_chill && canApplyChill)
        {
            isChilled = _chill;
            chilledTimer = ailmentsDuration;
            var slowPercentage = .2f;
            GetComponent<Entity>().SlowEntityBy(slowPercentage, ailmentsDuration);
            fx.ChillFXFor(ailmentsDuration);
        }

        if (_shock && canApplyShock)
        {
            if (!isShocked)
            {
                ApplyShock(_shock);
            }
            else if (GetComponent<Player>() == null)
            {
                HitNearesTargetWithShockStrike();
            }
        }
    }

    public void ApplyShock(bool _shock)
    {
        if (isShocked)
        {
            return;
        }

        isShocked = _shock;
        shockedTimer = ailmentsDuration;
        fx.ShockFXFor(ailmentsDuration);
    }

    private void HitNearesTargetWithShockStrike()
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
            var newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
            newShockStrike.GetComponent<ShockStrikeController>()
                .Setup(shockDmg, closestEnemy.GetComponent<CharacterStats>());
        }
    }

    private void ApplyIgniteDmg()
    {
        if (igniteDmgTimer <= 0)
        {
            DecreaseHealthBy(igniteDmg);

            if (currHp <= 0 && !isDead)
            {
                Die();
            }

            igniteDmgTimer = igniteDmgCooldown;
        }
    }

    #endregion

    #region Stat calculations

    private int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        if (_targetStats.isChilled)
        {
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        }
        else
        {
            totalDamage -= _targetStats.armor.GetValue();
        }

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue); // not negative dmg
        return totalDamage;
    }

    private bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        var totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isShocked)
        {
            totalEvasion += 20;
        }

        if (Random.Range(0, 100) < totalEvasion)
        {
            Debug.Log("Attack avoided");
            return true;
        }

        return false;
    }

    private bool CanCrit()
    {
        var totalCritChance = critChance.GetValue() + agility.GetValue();

        if (Random.Range(0, 100) < totalCritChance)
        {
            return true;
        }

        return false;
    }

    private int CalculateCritDamage(int _damage)
    {
        var totalCritPower = (critPower.GetValue() + strength.GetValue()) * .01f;

        var critDamage = _damage * totalCritPower;

        return Mathf.RoundToInt(critDamage);
    }

    public int GetMaxHealthValue()
    {
        return maxHp.GetValue() + vitality.GetValue() * 5;
    }

    #endregion
}