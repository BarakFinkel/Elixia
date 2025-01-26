using UnityEngine;
using UnityEngine.UI;

public class CounterAttackSkill : Skill
{
    [Header("Counter Attack")]
    [SerializeField] private UI_SkillTreeSlot counterAttackUnlockButton;
    public bool counterAttackUnlocked { get; private set; }

    [Header("Resource Restoration")]
    [SerializeField] private UI_SkillTreeSlot restoreUnlockButton;
    
    [Range(0.0f, 1.0f)]
    [SerializeField] private float restoreHealthPercentage;
    public bool restoreUnlocked { get; private set; }

    [Header("Counter Attack Clone")]
    [SerializeField] private UI_SkillTreeSlot cloneUnlockButton;
    public bool cloneUnlocked { get; private set; }

    protected override void Start()
    {
        base.Start();

        counterAttackUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCounterAttack);
        restoreUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockRestoration);
        cloneUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockClone);
    }

    public override void UseSkill()
    {
        base.UseSkill();

        if (restoreUnlocked)
        {
            int restoreAmount = Mathf.RoundToInt(player.cs.GetMaxHealthValue() * restoreHealthPercentage);
            player.cs.IncreaseHealthBy(restoreAmount);
        }
    }

    private void UnlockCounterAttack()
    {
        if (counterAttackUnlockButton.unlocked && !counterAttackUnlocked)
        {
            counterAttackUnlocked = true;
        }
    }

    private void UnlockRestoration()
    {
        if (restoreUnlockButton.unlocked && !restoreUnlocked)
        {
            restoreUnlocked = true;
        }
    }

    private void UnlockClone()
    {
        if (cloneUnlockButton.unlocked && !cloneUnlocked)
        {
            cloneUnlocked = true;
        }
    }

    public void MakeCloneOnCounterAttack(Transform _spawnTransform)
    {
        if (counterAttackUnlocked)
        {
            SkillManager.instance.clone.CreateCloneWithDelay(_spawnTransform);
        }
    }
}
