using System;
using UnityEngine;

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

    [Header("Bounce Information")]
    [SerializeField]
    private int bounceAmount;

    [SerializeField]
    private float bounceGravity;

    [SerializeField]
    private float bounceSpeed;

    [Header("Pierce Information")]
    [SerializeField]
    private int pierceAmount;

    [SerializeField]
    private float pierceGravity;

    [Header("Spin Information")]
    [SerializeField]
    private float hitCooldown;

    [SerializeField]
    private float maxTravelDist;

    [SerializeField]
    private float spinDuration;

    [SerializeField]
    private float spinGravity;

    [Header("Skill Information")]
    [SerializeField]
    private GameObject swordPrefab;

    [SerializeField]
    private float freezeTimeDuration;

    [SerializeField]
    private float returnSpeed;

    [SerializeField]
    private Vector2 launchForce;

    [SerializeField]
    private float swordGravity;

    [Header("Aim Dots Information")]
    [SerializeField]
    private GameObject aimDotPrefab;

    [SerializeField]
    private int numberOfDots;

    [SerializeField]
    private float spaceBetweenDots;

    [SerializeField]
    private Transform dotsParent;

    private GameObject[] dots;

    private Vector2 finalDir;

    protected override void Start()
    {
        base.Start();

        GenerateDots();

        SetupGravity();
    }

    protected override void Update()
    {
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

    public void ChangeToNextType()
    {
        swordType = (SwordType)(((int)swordType + 1) % Enum.GetValues(typeof(SwordType)).Length);
        SetupGravity();
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