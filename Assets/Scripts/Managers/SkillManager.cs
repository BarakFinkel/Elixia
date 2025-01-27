using UnityEngine;

// Created as a singleton - since we don't want more than 1 to be active.
public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    [SerializeField]
    public float enemyDetectRadius;

    // Skills
    public DodgeSkill dodge { get; private set; }
    public EvadeSkill evade { get; private set; }
    public CloneSkill clone { get; private set; }
    public SwordSkill sword { get; private set; }
    public CounterAttackSkill counterAttack { get; private set; }
    public BlackholeSkill blackhole { get; private set; }
    public PotionSkill potion { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        dodge = GetComponent<DodgeSkill>();
        evade = GetComponent<EvadeSkill>();
        clone = GetComponent<CloneSkill>();
        sword = GetComponent<SwordSkill>();
        counterAttack = GetComponent<CounterAttackSkill>();
        potion = GetComponent<PotionSkill>();
        blackhole = GetComponent<BlackholeSkill>();
    }
}