using UnityEngine;
using UnityEngine.UI;

public class CounterAttackSkill : Skill
{
    [Header("Counter Attack")]
    [SerializeField]
    private UI_SkillTreeSlot counterAttackUnlockButton;

    [Header("Resource Restoration")]
    [SerializeField]
    private UI_SkillTreeSlot restoreUnlockButton;

    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float restoreHealthPercentage;

    [Header("Counter Attack Clone")]
    [SerializeField]
    private UI_SkillTreeSlot cloneUnlockButton;

    public bool counterAttackUnlocked { get; private set; }
    public bool restoreUnlocked { get; private set; }
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
            var restoreAmount = Mathf.RoundToInt(player.cs.GetMaxHealthValue() * restoreHealthPercentage);
            player.cs.IncreaseHealthBy(restoreAmount);
        }
    }

    protected override void CheckUnlock()
    {
        UnlockCounterAttack();
        UnlockRestoration();
        UnlockClone();
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