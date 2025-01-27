using System;
using UnityEngine;
using UnityEngine.UI;

public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}

public class SwordSkill : Skill
{
    public SwordType swordType = SwordType.Regular;

    [SerializeField] private UI_SkillTreeSlot bounceUnlockButton;
    [SerializeField] public UI_InGame uiIngame;
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceGravity;
    [SerializeField] private float bounceSpeed;

    [Header("Pierce Information")]
    [SerializeField] private UI_SkillTreeSlot pierceUnlockButton;
    [SerializeField] private int pierceAmount;
    [SerializeField] private float pierceGravity;

    [Header("Spin Information")]
    [SerializeField] private UI_SkillTreeSlot spinUnlockButton;
    [SerializeField] private float hitCooldown;
    [SerializeField] private float maxTravelDist;
    [SerializeField] private float spinDuration;
    [SerializeField] private float spinGravity;

    [Header("Skill Information")]
    [SerializeField] private UI_SkillTreeSlot swordUnlockButton;
    public bool swordUnlocked { get; private set; }
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private float freezeTimeDuration;
    [SerializeField] private float returnSpeed;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private float swordGravity;

    [Header("Passive Skills")]
    [SerializeField] private UI_SkillTreeSlot stunEnemyUnlockButton;
    [SerializeField] private UI_SkillTreeSlot stunShockUnlockButton;
    public bool stunUnlocked { get; private set; }
    public bool shockUnlocked { get; private set; }

    [Header("Aim Dots Information")]
    [SerializeField] private GameObject aimDotPrefab;
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBetweenDots;
    [SerializeField] private Transform dotsParent;

    private GameObject[] dots;
    private Vector2 finalDir;

    protected override void Start()
    {
        base.Start();

        GenerateDots();
        SetupGravity();

        bounceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBounce);
        pierceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockPierce);
        spinUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSpin);
        swordUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSword);
        stunEnemyUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockStun);
        stunShockUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockShock);
    }

    protected override void Update()
    {
        base.Update();
        
        if (player.canUseSwordSkill)
        {
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                finalDir = new Vector2(AimDirection().normalized.x, AimDirection().normalized.y) * launchForce;
            }

            if (Input.GetKey(KeyCode.Mouse1))
            {
                for (var i = 0; i < dots.Length; i++)
                {
                    dots[i].transform.position = DotsPosition(i * spaceBetweenDots);
                }
            }
        }
    }

    protected override void CheckUnlock()
    {
        UnlockSword();
        UnlockStun();
        UnlockShock();
        UnlockBounce();
        UnlockPierce();
        UnlockSpin();
    }

    private void SetupGravity()
    {
        if (swordType == SwordType.Bounce)
        {
            swordGravity = bounceGravity;
        }
        else if (swordType == SwordType.Pierce)
        {
            swordGravity = pierceGravity;
        }
        else if (swordType == SwordType.Spin)
        {
            swordGravity = spinGravity;
        }
    }

    public void CreateSword()
    {
        var newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        var newSwordScript = newSword.GetComponent<SwordSkillController>();

        if (swordType == SwordType.Bounce)
        {
            newSwordScript.SetupBounce(true, bounceAmount, bounceSpeed);
        }
        else if (swordType == SwordType.Pierce)
        {
            newSwordScript.SetupPierce(pierceAmount);
        }
        else if (swordType == SwordType.Spin)
        {
            newSwordScript.SetupSpin(true, maxTravelDist, spinDuration, hitCooldown);
        }

        newSwordScript.SetupSword(finalDir, swordGravity, player, freezeTimeDuration, returnSpeed);

        player.AssignNewSword(newSword);

        DotsActive(false);
    }

    #region Unlock

    private void UnlockSword()
    {
        if (swordUnlockButton.unlocked && !swordUnlocked)
        {
            swordType = SwordType.Regular;
            swordUnlocked = true;
        }
    }

    private void UnlockStun()
    {
        if (stunEnemyUnlockButton.unlocked && !stunUnlocked)
        {
            stunUnlocked = true;
        }
    }

    private void UnlockShock()
    {
        if (stunShockUnlockButton.unlocked && !shockUnlocked)
        {
            shockUnlocked = true;
        }
    }

    private void UnlockBounce()
    {
        if (bounceUnlockButton.unlocked && swordType != SwordType.Bounce)
        {
            swordType = SwordType.Bounce;
        }
    }

    private void UnlockPierce()
    {
        if (pierceUnlockButton.unlocked && swordType != SwordType.Pierce)
        {
            swordType = SwordType.Pierce;
        }
    }

    private void UnlockSpin()
    {
        if (spinUnlockButton.unlocked && swordType != SwordType.Spin)
        {
            swordType = SwordType.Spin;
        }
    }

    #endregion

    public void ChangeToNextType()
    {
        swordType = (SwordType)(((int)swordType + 1) % Enum.GetValues(typeof(SwordType)).Length);
        SetupGravity();
    }

    public void SetCooldown()
    {
        cooldownTimer = cooldown;
    }

    public override bool CanUseSkill()
    {
        if (cooldownTimer == 0)
        {
            return true;
        }
        Debug.Log("Skill on Cooldown");
        return false;
    }

    #region Aiming

    public Vector2 AimDirection()
    {
        Vector2 playerPos = player.transform.position;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var direction = mousePos - playerPos;

        return direction;
    }

    public void DotsActive(bool _isActive)
    {
        for (var i = 0; i < dots.Length; i++)
        {
            dots[i].SetActive(_isActive);
        }
    }

    private void GenerateDots()
    {
        dots = new GameObject[numberOfDots];
        for (var i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(aimDotPrefab, player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }

    private Vector2 DotsPosition(float t)
    {
        var position = (Vector2)player.transform.position + new Vector2(
                AimDirection().normalized.x,
                AimDirection().normalized.y)
            * launchForce
            * t + .5f * (Physics2D.gravity * swordGravity) * (t * t);

        return position;
    }

    #endregion
}