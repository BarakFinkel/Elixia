using UnityEngine;

public class PotionSkill : Skill
{
    [Header("Skill Information")]
    [SerializeField] private GameObject potionPrefab; // The potion prefab we want to spawn
    [SerializeField] private SpriteRenderer potionOnPlayer; // The potion sprite renderer displaying the potion before throwing it.
    [SerializeField] private Vector3 potionSpawnOffset; // Relative to the player, where to spawn the potion
    [SerializeField] private Vector2 throwForce; // The direction and power in which we want to throw the potion
    [SerializeField] private float potionGravity; // The gravity scale that will affect the potion
    [SerializeField] public Sprite brokenPotion; // A sprite to switch the potion sprite when colliding with the ground.
    [SerializeField] public float timeTillDelete; // The time interval between the potion breaking and the it's object's deletion.

    [Header("Aim Dots Information")]
    [SerializeField] private int numberOfDots; // Amount of dots to display 
    [SerializeField] private float spaceBetweenDots;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotsParent;

    private bool updateTimer = false;
    private GameObject[] dots;
    private Vector2 finalDirection;

    protected override void Start()
    {
        base.Start();

        GenerateDots();
    }

    protected override void Update()
    {
        // Change the color we display when about to throw the potion.
        potionOnPlayer.color = PotionEffectManager.instance.CurrentEffect().potionColor;

        // We only update the timer when we wish to - controllable via the updateTimer, and it's usage is explained below.
        if (updateTimer)
        {
            base.Update();
        }

        // If right mouse is released - we throw the potion in the chosen direction.
        // When that happens, we allow the timer to update via the base.Update() method.
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            Vector2 aimDirection = AimDirection().normalized;
            finalDirection = new Vector2(aimDirection.x * throwForce.x, aimDirection.y * throwForce.y);

            updateTimer = true;
        }

        // If we hold the right click, and the cooldown timer is exactly the same as the cooldown variable -
        // We know we haven't started the timer yet and did not throw the potion, so we update and display the aim dots.
        if (Input.GetKey(KeyCode.Mouse1) && cooldownTimer == cooldown)
        {       
            for (int i = 0; i < dots.Length; i++)
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

    public void CreatePotion()
    {   
        BasePotionEffect potionEffect = PotionEffectManager.instance.CurrentEffect(); // To be replaced with a better method of keeping track of player inputs and effect retrieval.

        GameObject newPotion = Instantiate(potionEffect.potionPrefab, player.transform.position + potionSpawnOffset, transform.rotation);
        PotionSkillController newPotionScript = newPotion.GetComponent<PotionSkillController>();

        newPotionScript.SetupPotion(finalDirection, potionGravity, potionEffect);

        DotsActive(false); // We stop updating and displaying the aim dots since we already threw the potion.
    }

    #region Aiming

    // Method to return the direction from the player to the cursor in order to calculate the potion throwing direction and accordingly update the aim dots' positions.
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - playerPosition;

        return direction;
    }

    // Method to activate the dots game object.
    public void DotsActive(bool _isActive)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].SetActive(_isActive);
        }
    }

    // Method to generate the dots from within the start function (done above).
    private void GenerateDots()
    {
        dots = new GameObject[numberOfDots];
        
        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position + potionSpawnOffset, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }

    // A method returning the positions in which the dots are supposed to be placed at
    // Note - 0.5f is not a 'magic number', it is necessary for computing the necessary positions and is not flexible.
    private Vector2 DotsPosition(float t)
    {
        Vector2 aimDirection = AimDirection().normalized;
        Vector2 position = (Vector2)dotsParent.transform.position
        + new Vector2 (aimDirection.x * throwForce.x, aimDirection.y * throwForce.y) * t
        + 0.5f * (Physics2D.gravity * potionGravity) * (t * t);

        return position;
    }

    #endregion
}