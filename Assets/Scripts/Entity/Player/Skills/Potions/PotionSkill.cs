using UnityEngine;
using UnityEngine.UI;

public class PotionSkill : Skill
{
    [Header("Unlock Information")]
    [SerializeField]
    private UI_SkillTreeSlot potionUnlockButton;

    [SerializeField]
    private UI_SkillTreeSlot secondPotionUnlockButton;

    [SerializeField]
    private UI_SkillTreeSlot thirdPotionUnlockButton;

    [SerializeField]
    private UI_SkillTreeSlot fourthPotionUnlockButton;

    [SerializeField]
    public UI_Potions potionsUI;

    [SerializeField]
    private UI_InGame ingameUI;

    [Header("Skill Information")]
    [SerializeField]
    private GameObject potionPrefab; // The potion prefab we want to spawn

    [SerializeField]
    private SpriteRenderer potionOnPlayer; // The potion sprite renderer displaying the potion before throwing it.

    [SerializeField]
    private Vector3 potionSpawnOffset; // Relative to the player, where to spawn the potion

    [SerializeField]
    private Vector2 throwForce; // The direction and power in which we want to throw the potion

    [SerializeField]
    private float potionGravity; // The gravity scale that will affect the potion

    [SerializeField]
    public Sprite brokenPotion; // A sprite to switch the potion sprite when colliding with the ground.

    [SerializeField]
    public float timeTillDelete; // The time interval between the potion breaking and the it's object's deletion.

    [Header("Aim Dots Information")]
    [SerializeField]
    private int numberOfDots; // Amount of dots to display 

    [SerializeField]
    private float spaceBetweenDots;

    [SerializeField]
    private GameObject dotPrefab;

    [SerializeField]
    private Transform dotsParent;

    private GameObject[] dots;
    private Vector2 finalDirection;

    private bool updateTimer;
    public bool potionUnlocked { get; private set; }
    public bool secondUnlocked { get; private set; }
    public bool thirdUnlocked { get; private set; }
    public bool fourthUnlocked { get; private set; }

    protected override void Start()
    {
        base.Start();

        potionUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockPotion);
        secondPotionUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSecondPotion);
        thirdPotionUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockThirdPotion);
        fourthPotionUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockFourthPotion);

        GenerateDots();
    }

    protected override void Update()
    {
        CheckUnlock();

        // Change the color we display if it has changed.
        if (potionOnPlayer.color != PotionEffectManager.instance.CurrentEffect().potionColor)
        {
            potionOnPlayer.color = PotionEffectManager.instance.CurrentEffect().potionColor;
        }

        // We only update the timer when we wish to - controllable via the updateTimer, and it's usage is explained below.
        if (updateTimer)
        {
            base.Update();
        }

        // If right mouse is released - we throw the potion in the chosen direction.
        // When that happens, we allow the timer to update via the base.Update() method.
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            var aimDirection = AimDirection().normalized;
            finalDirection = new Vector2(aimDirection.x * throwForce.x, aimDirection.y * throwForce.y);

            updateTimer = true;
        }

        // If we hold the right click, and the cooldown timer is exactly the same as the cooldown variable -
        // We know we haven't started the timer yet and did not throw the potion, so we update and display the aim dots.
        if (Input.GetKey(KeyCode.Mouse1) && cooldownTimer == cooldown)
        {
            for (var i = 0; i < dots.Length; i++)
            {
                dots[i].transform.position = DotsPosition(i * spaceBetweenDots);
            }
        }

        // Only when the cooldown timer reaches 0, we turn off the updateTimer bool.
        // This helps us make sure that when entering the player's aim state and checking if the potion skill is ready - 
        // We will not immediately run the timer.
        if (cooldownTimer == 0)
        {
            updateTimer = false;
        }
    }

    protected override void CheckUnlock()
    {
        UnlockPotion();
        UnlockSecondPotion();
        UnlockThirdPotion();
        UnlockFourthPotion();
    }

    public void CreatePotion()
    {
        var potionEffect =
            PotionEffectManager.instance
                .CurrentEffect(); // To be replaced with a better method of keeping track of player inputs and effect retrieval.

        var newPotion = Instantiate(potionEffect.potionPrefab, player.transform.position + potionSpawnOffset,
            transform.rotation);
        var newPotionScript = newPotion.GetComponent<PotionSkillController>();

        newPotionScript.SetupPotion(finalDirection, potionGravity, potionEffect);

        cooldownTimer = potionEffect.cooldown; // We set the cooldown to match the chosen element's.
        ingameUI.SetCooldownForPotion(potionEffect.cooldown);

        DotsActive(false); // We stop updating and displaying the aim dots since we already threw the potion.
    }

    private void UnlockPotion()
    {
        if (potionUnlockButton.unlocked && !potionUnlocked)
        {
            potionUnlocked = true;
            potionsUI.UnlockPotionOne();
        }
    }

    private void UnlockSecondPotion()
    {
        if (secondPotionUnlockButton.unlocked && !secondUnlocked)
        {
            secondUnlocked = true;
            potionsUI.UnlockPotionTwo();
        }
    }

    private void UnlockThirdPotion()
    {
        if (thirdPotionUnlockButton.unlocked && !thirdUnlocked)
        {
            thirdUnlocked = true;
            potionsUI.UnlockPotionThree();
        }
    }

    private void UnlockFourthPotion()
    {
        if (fourthPotionUnlockButton.unlocked && !fourthUnlocked)
        {
            fourthUnlocked = true;
            potionsUI.UnlockPotionFour();
        }
    }

    #region Aiming

    // Method to return the direction from the player to the cursor in order to calculate the potion throwing direction and accordingly update the aim dots' positions.
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var direction = mousePosition - playerPosition;

        return direction;
    }

    // Method to activate the dots game object.
    public void DotsActive(bool _isActive)
    {
        for (var i = 0; i < dots.Length; i++)
        {
            dots[i].SetActive(_isActive);
        }
    }

    // Method to generate the dots from within the start function (done above).
    private void GenerateDots()
    {
        dots = new GameObject[numberOfDots];

        for (var i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position + potionSpawnOffset, Quaternion.identity,
                dotsParent);
            dots[i].SetActive(false);
        }
    }

    // A method returning the positions in which the dots are supposed to be placed at
    // Note - 0.5f is not a 'magic number', it is necessary for computing the necessary positions and is not flexible.
    private Vector2 DotsPosition(float t)
    {
        var aimDirection = AimDirection().normalized;
        var position = (Vector2)dotsParent.transform.position
                       + new Vector2(aimDirection.x * throwForce.x, aimDirection.y * throwForce.y) * t
                       + 0.5f * (Physics2D.gravity * potionGravity) * (t * t);

        return position;
    }

    #endregion
}