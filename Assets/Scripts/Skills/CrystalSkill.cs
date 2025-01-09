using System.Collections.Generic;
using UnityEngine;

public class CrystalSkill : Skill
{
    [SerializeField]
    private float crystalDuration;

    [SerializeField]
    private GameObject crystalPrefab;

    [Header("Crystal Mirage")]
    [SerializeField]
    private bool cloneInsteadOfCrystal;

    [Header("Explosive crystal")]
    [SerializeField]
    private bool canExplode;

    [Header("Moving Crystal")]
    [SerializeField]
    private bool canMoveToEnemy;

    [SerializeField]
    private float moveSpeed;

    [Header("Multi stacking crystal")]
    [SerializeField]
    private bool canMultiStack;

    [SerializeField]
    private int amountOfStacks;

    [SerializeField]
    private float multiStackCooldown;

    [SerializeField]
    private float useTimeWindow;

    [SerializeField]
    private List<GameObject> crystaslLeft = new();

    private GameObject currentCrystal;

    private void Awake()
    {
        RefillCrystal();
    }

    public override void UseSkill()
    {
        base.UseSkill();


        if (CanUseMultiCrystal())
        {
            return;
        }

        if (currentCrystal == null)
        {
            CreateCrystal();
        }
        else
        {
            if (canMoveToEnemy)
            {
                return;
            }

            (player.transform.position, currentCrystal.transform.position) =
                (currentCrystal.transform.position, player.transform.position);

            if (cloneInsteadOfCrystal)
            {
                SkillManager.instance.clone.CreateClone(currentCrystal.transform, Vector3.zero);
                Destroy(currentCrystal);
            }
            else
            {
                currentCrystal.GetComponent<CrystalSkillController>()?.FinishCrystal();
            }
        }
    }

    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        var currentCrystalScript = currentCrystal.GetComponent<CrystalSkillController>();

        currentCrystalScript.SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed,
            FindClosestEnemy(currentCrystal.transform), player);
    }

    public void CurrentCrystalChooseRandomTarget()
    {
        currentCrystal.GetComponent<CrystalSkillController>().ChooseRandomEnemy();
    }


    private bool CanUseMultiCrystal()
    {
        if (canMultiStack)
        {
            if (crystaslLeft.Count > 0)
            {
                if (crystaslLeft.Count == amountOfStacks)
                {
                    Invoke("ResetAbility", useTimeWindow);
                }

                cooldown = 0; // no cooldown when has crystals
                var crystalToSpawn = crystaslLeft[crystaslLeft.Count - 1];
                var newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);

                crystaslLeft.Remove(crystalToSpawn);

                newCrystal.GetComponent<CrystalSkillController>()
                    .SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed,
                        FindClosestEnemy(newCrystal.transform), player);

                if (crystaslLeft.Count <= 0)
                {
                    cooldown = multiStackCooldown;
                    RefillCrystal();
                }
            }

            return true;
        }

        return false;
    }

    private void RefillCrystal()
    {
        var amountToAdd = amountOfStacks - crystaslLeft.Count;

        for (var i = 0; i < amountToAdd; i++)
        {
            crystaslLeft.Add(crystalPrefab);
        }
    }

    private void ResetAbility()
    {
        if (cooldownTimer > 0)
        {
            return;
        }

        cooldownTimer = multiStackCooldown;
        RefillCrystal();
    }
}