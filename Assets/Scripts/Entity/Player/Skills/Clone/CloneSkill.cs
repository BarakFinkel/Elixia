using System.Collections;
using UnityEngine;

public class CloneSkill : Skill
{
    [Header("General Information")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [SerializeField] private bool canAttack;
    
    [Header("Counter Attack Clone Information")]
    [SerializeField] private bool canCreateCloneOnCounterAttack;
    [SerializeField] private float counterAttackCloneOffset = 2.0f;
    [SerializeField] private float counterAttackCloneDelay = 0.4f;

    [Header("Dodge Clone Information")]
    [SerializeField] private bool createCloneOnDodgeStart;
    [SerializeField] private bool createCloneOnDodgeOver;
    
    [Header("Duplicate Clone Information")]
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float duplicationChance = 30.0f;
    
    [Header("Crystal Alternative Information")]
    [SerializeField] public bool createCrystalInsteadOfClone;

    public void CreateClone(Transform _clonePosition, Vector3 _offset)
    {
        if(createCrystalInsteadOfClone)
        {
            PotionEffectManager.instance.arcane.CreateCrystal(PlayerManager.instance.player.gameObject, Vector3.zero);
            return;
        }
        
        GameObject newClone = Instantiate(clonePrefab);

        newClone.GetComponent<CloneSkillController>().SetupClone(_clonePosition, cloneDuration, canAttack, canDuplicateClone, duplicationChance,_offset, player, FindClosestEnemy(newClone.transform));
    }

    // Responsible for creating a clone when starting to dodge.
    public void CreateCloneOnDodgeStart()
    {
        if (createCloneOnDodgeStart)
        {
            CreateClone(player.transform, Vector3.zero);
        }
    }

    // Responsible for creating a clone when finishing to dodge.
    public void CreateCloneOnDodgeOver()
    {
        if (createCloneOnDodgeOver)
        {
            CreateClone(player.transform, Vector3.zero);
        }        
    }

    public void CreateCloneOnCounterAttack(Transform _enemyTransform)
    {
        if (canCreateCloneOnCounterAttack)
        {
            StartCoroutine(CreateCloneWithDelay(_enemyTransform, new Vector3(counterAttackCloneOffset * player.facingDir, 0)));
        }  
    }

    public IEnumerator CreateCloneWithDelay(Transform _transform, Vector3 _offset)
    {
        yield return new WaitForSeconds(counterAttackCloneDelay);

        CreateClone(_transform, _offset);
    }
}
