using UnityEngine;
using UnityEngine.UI;

public class DodgeSkill : Skill
{
    [Header("Dodge")]
    [SerializeField] private UI_SkillTreeSlot unlockDashButton;
    public bool dodgeUnlocked { get; private set; }

    [Header("Clone on Dodge")]
    [SerializeField] private UI_SkillTreeSlot unlockStartCloneButton;
    [SerializeField] private UI_SkillTreeSlot unlockEndCloneButton;
    public bool cloneOnStartDashUnlocked { get; private set; }
    public bool cloneOnEndDashUnlocked { get; private set; }

    protected override void Start()
    {
        base.Start();

        unlockDashButton.GetComponent<Button>().onClick.AddListener(UnlockDash);
        unlockStartCloneButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnDodgeStart);
        unlockEndCloneButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnDodgeEnd);
    }

    public override void UseSkill()
    {
        base.UseSkill();
    }

    private void UnlockDash()
    {
        if (unlockDashButton.unlocked && !dodgeUnlocked)
        {
            dodgeUnlocked = true;
        }
    }

    private void UnlockCloneOnDodgeStart()
    {
        if (unlockStartCloneButton.unlocked && !cloneOnStartDashUnlocked)
        {
            cloneOnStartDashUnlocked = true;
        }
    }

    private void UnlockCloneOnDodgeEnd()
    {
        if (unlockEndCloneButton.unlocked && !cloneOnEndDashUnlocked)
        {
            cloneOnEndDashUnlocked = true;
        }
    }

    // Responsible for creating a clone when starting to dodge.
    public void CreateCloneOnDodgeStart()
    {
        if (cloneOnStartDashUnlocked)
        {
            SkillManager.instance.clone.CreateClone(player.transform, Vector3.zero);
        }
    }

    // Responsible for creating a clone when finishing to dodge.
    public void CreateCloneOnDodgeEnd()
    {
        if (cloneOnEndDashUnlocked)
        {
            SkillManager.instance.clone.CreateClone(player.transform, Vector3.zero);
        }
    }

}