using UnityEngine;

// Created as a singleton - since we don't want more than 1 to be active.
public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;
    [SerializeField] public float enemyDetectRadius;
    
    // Skills
    public DodgeSkill dash { get; private set; }
    public CloneSkill clone { get; private set; }
    public SwordSkill sword { get; private set; }
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
        dash = GetComponent<DodgeSkill>();
        clone = GetComponent<CloneSkill>();
        sword = GetComponent<SwordSkill>();
        potion = GetComponent<PotionSkill>();
        blackhole = GetComponent<BlackholeSkill>();
    }
}