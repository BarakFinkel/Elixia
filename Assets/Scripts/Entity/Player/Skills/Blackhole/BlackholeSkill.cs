using UnityEngine;
using UnityEngine.UI;

public class BlackholeSkill : Skill
{
    [SerializeField] private UI_SkillTreeSlot BlackholeUnlockButton;
    public bool blackholeUnlocked { get; private set; }
    [SerializeField] private float cloneCooldown;
    [SerializeField] private int amountOfAttacks;
    [SerializeField] private float blackholeDuration;

    [Space]
    [SerializeField] private GameObject blackholePrefab;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;
    [SerializeField] private float yOffset;

    private BlackholeSkillController currentBlackhole;
    

    protected override void Start()
    {
        base.Start();

        BlackholeUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBlackhole);
    }

    protected override void Update()
    {
        base.Update();
    }

    public override bool CanUseSkill()
    {
        if (cooldownTimer == 0)
        {
            cooldownTimer = cooldown;
            return true;
        }

        Debug.Log("Skill is on cooldown");
        return false;
    }

    public override void UseSkill()
    {
        base.UseSkill();

        var newBlackhole = Instantiate(blackholePrefab, player.transform.position, Quaternion.identity);
        currentBlackhole = newBlackhole.GetComponent<BlackholeSkillController>();

        currentBlackhole.SetupBlackHole(maxSize, growSpeed, shrinkSpeed, amountOfAttacks, cloneCooldown,
            blackholeDuration);
    }

    public bool SkillCompleted()
    {
        if (!currentBlackhole)
        {
            return false;
        }

        if (currentBlackhole.playerCanExitState)
        {
            currentBlackhole = null;
            return true;
        }

        return false;
    }

    public float GetBlackHoleRadius()
    {
        return maxSize / 2;
    }

    protected override void CheckUnlock()
    {
        UnlockBlackhole();
    }

    private void UnlockBlackhole()
    {
        if (BlackholeUnlockButton.unlocked && !blackholeUnlocked)
        {
            blackholeUnlocked = true;
        }
    }
}