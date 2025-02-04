using UnityEngine;
using UnityEngine.UI;

public class EvadeSkill : Skill
{
    [Header("Unlock Information")]
    [SerializeField]
    private UI_SkillTreeSlot unlockEvadeButton;

    [SerializeField]
    private UI_SkillTreeSlot unlockCloneButton;

    public bool evadeUnlocked;
    public bool cloneUnlocked;

    [Header("Evade Information")]
    [SerializeField]
    private int evasionAmount;

    [SerializeField]
    private Vector3 cloneOffset;

    protected override void Start()
    {
        base.Start();

        unlockEvadeButton.GetComponent<Button>().onClick.AddListener(UnlockEvade);
        unlockCloneButton.GetComponent<Button>().onClick.AddListener(UnlockClone);
    }

    protected override void CheckUnlock()
    {
        UnlockEvade();
        UnlockClone();
    }

    private void UnlockEvade()
    {
        if (unlockEvadeButton.unlocked && !evadeUnlocked)
        {
            evadeUnlocked = true;
        }
    }

    private void UnlockClone()
    {
        if (unlockCloneButton.unlocked && !cloneUnlocked)
        {
            player.cs.evasion.AddModifier(evasionAmount);
            Inventory.instance.UpdateStatsUI();
            cloneUnlocked = true;
        }
    }

    public void CreateCloneOnEvade()
    {
        if (cloneUnlocked)
        {
            cloneOffset.x *= player.facingDir;
            SkillManager.instance.clone.CreateClone(player.transform, cloneOffset);
        }
    }
}