using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ArcanePotionEffect : BasePotionEffect
{
    [Header("General Information")]
    [SerializeField] private GameObject arcaneEffectPrefab;
    [SerializeField] private GameObject crystalPrefab;
    [SerializeField] private float crystalDuration;
    [SerializeField] private float spawnHeightOffset;
    public GameObject currentEffect;
    public GameObject currentCrystal { get; private set; }

    [Header("Multiplication information")]
    [SerializeField] private int amountOfCrystals = 3;

    [SerializeField] private float spawnRadius = 2.0f;
    // [SerializeField] private bool canUseMultiStack;
    // [SerializeField] private float multiStackCooldown;
    // [SerializeField] private float useTimeWindow;
    // [SerializeField] private List<GameObject> crystalsLeft = new List<GameObject>();

    [Header("Explosion Information")]
    [SerializeField] private bool canExplode;

    [Header("Movement Information")]
    [SerializeField] private bool canMoveToEnemy;
    [SerializeField] private float minMoveSpeed;
    [SerializeField] private float maxMoveSpeed;

    [Header("Grow Information")]
    [SerializeField] private float growSpeed;
    [SerializeField] private float growScale;

    // [Header("Clone Information")]
    // [SerializeField] private bool cloneInsteadOfCrystal;


    // The potion spawns arcane crystals that will seek the nearest enemy to explode on.
    // For now, we've canceled teleportation to crystal since we are interested only in homing and exploding crystals functionality.
    public override void ActivatePotionEffect(GameObject potion)
    {
        /*
        // If possible, the player will use the given method below instead of the regualar functionality of the arcane skill.
        if (CanUseMultiCrystal(potion))
        {
            return;
        }
        */

        // Create the effect for the crystals.
        Vector3 effectOffset = new Vector3(0, spawnHeightOffset, 0);
        CreatePowerEffect(potion, effectOffset);

        /*
        if(currentCrystal == null)
        {
        */
        // Now, we instantiate the given amount of crystals with the given parameters, passing them to the effect controller.
        for (var i = 0; i < amountOfCrystals; i++)
        {
            CreateCrystal(potion, GetCrystalCoordinate(i, spawnHeightOffset));
        }
        /*
        }
        else // If there already exists a crystal, we would like to teleport the player to it's location.
        {
            // If the crystal can move to the enemy unit, we don't want this part of the code functioning so we return.
            if (canMoveToEnemy)
            {
                return;
            }

            // We switch between the player and the crystal's position upon reactivation of the crystal skill.
            Vector2 playerPos = PlayerManager.instance.player.transform.position;
            PlayerManager.instance.player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;

            // If we can create a clone instead of a crystal, we spawn a clone where the crystal is teleporting to.
            if (cloneInsteadOfCrystal)
            {
                SkillManager.instance.clone.CreateClone(currentCrystal.transform, Vector3.zero);
                Destroy(currentCrystal);
            }
            else // Otherwise, we just explode the crystal upon switch.
            {
                currentCrystal.GetComponent<ArcaneEffectController>()?.EndCrystalCycle();
            }
        }
        */
    }

    public void CreatePowerEffect(GameObject obj, Vector3 offset)
    {
        currentEffect = Instantiate(arcaneEffectPrefab, obj.transform.position + offset, Quaternion.identity);

        AudioManager.instance.PlaySFX(0, 0, PlayerManager.instance.player.transform);
    }

    public void CreateCrystal(GameObject obj, Vector3 offset)
    {
        currentCrystal = Instantiate(crystalPrefab, obj.transform.position + offset, Quaternion.identity);

        var currentCrystalScript = currentCrystal.GetComponent<ArcaneEffectController>();
        currentCrystalScript.SetupCrystal
        (
            crystalDuration,
            canExplode,
            canMoveToEnemy,
            Random.Range(minMoveSpeed, maxMoveSpeed),
            growSpeed,
            growScale
        );
    }

    // Responsible for setting the coordinates around a (0,yOffset,0) point
    public Vector3 GetCrystalCoordinate(int index, float heightOffset)
    {
        // Ensure index is within valid bounds
        if (index < 0 || index >= amountOfCrystals)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index must be between 0 and pointsNumber - 1.");
        }

        // Compute the angle for the vertex in radians
        var angle = 2 * Mathf.PI / amountOfCrystals * index;

        // Adjust the angle to align one base with the x-axis
        var offset = Mathf.PI / 2 - Mathf.PI / amountOfCrystals; // Rotate by half the angle between vertices
        angle += offset;

        // Compute the Cartesian coordinates
        var x = spawnRadius * Mathf.Cos(angle);
        var y = spawnRadius * Mathf.Sin(angle);

        return new Vector2(x, y + heightOffset);
    }

    public void CurrentCrystalChooseRandomTarget()
    {
        currentCrystal.GetComponent<ArcaneEffectController>().ChooseRandomEnemy();
    }

    /*
    private bool CanUseMultiCrystal(GameObject potion)
    {
        if (canUseMultiStack)
        {
            if (crystalsLeft.Count > 0)
            {
                if (crystalsLeft.Count == amountOfCrystals)
                {
                    Invoke("ResetAbility", useTimeWindow);
                }

                // cooldown = 0;

                GameObject crystalToSpawn = crystalsLeft[crystalsLeft.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn, potion.transform.position, Quaternion.identity);

                crystalsLeft.Remove(crystalToSpawn);

                newCrystal.GetComponent<ArcaneEffectController>().SetupCrystal
                (
                    crystalDuration,
                    canExplode,
                    canMoveToEnemy,
                    Random.Range(minMoveSpeed, maxMoveSpeed),
                    growSpeed,
                    growScale,
                    potion.GetComponent<PotionSkillController>()?.closestEnemy.transform // Passing the potion's closest enemy
                );

                if (crystalsLeft.Count <= 0)
                {
                    // cooldown = multiStackCooldown;
                    RefillCrystals();
                }
            }

            return true;
        }

        return false;
    }

    private void RefillCrystals()
    {
        int amountToAdd = amountOfCrystals - crystalsLeft.Count;

        for (int i = 0; i < amountToAdd; i++)
        {
            crystalsLeft.Add(crystalPrefab);
        }
    }

    private void ResetAbility()
    {
        if (cooldownTimer > 0)
        {
            return;
        }

        // cooldownTimer = multiStackCooldown;
        RefillCrystals();
    }
    */
}