using System.Collections;
using UnityEngine;

public class CloneSkill : Skill
{
    [Header("Clone info")] [SerializeField]
    private GameObject clonePrefab;

    [SerializeField] private float cloneDuration;
    [SerializeField] private bool canAttack;

    [SerializeField] private bool createClonOnDashStart;
    [SerializeField] private bool createClonOnDashOver;
    [SerializeField] private bool createCloneOnCounter;
    [SerializeField] private float cloneOffset = 2;
    [SerializeField] private float delay = .4f;
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceToDup = 99;

    [Header("Crystal instead of clone")] [SerializeField]
    public bool crystalInstead;

    public void CreateClone(Transform _clonePosition, Vector3 _offset)
    {
        if (crystalInstead)
        {
            SkillManager.instance.crystal.CreateCrystal();
            return;
        }

        var newClone = Instantiate(clonePrefab);

        newClone.GetComponent<CloneSkillController>().SetupClone(_clonePosition, cloneDuration, canAttack,
            _offset, FindClosestEnemy(newClone.transform), canDuplicateClone, chanceToDup, player);
    }

    public void CreateCloneOnDashStart()
    {
        if (createClonOnDashStart)
        {
            CreateClone(player.transform, Vector3.zero);
        }
    }

    public void CreateCloneOnDashOver()
    {
        if (createClonOnDashOver)
        {
            CreateClone(player.transform, Vector3.zero);
        }
    }

    public void CreateCloneOnCounter(Transform _enemyTransform)
    {
        if (createCloneOnCounter)
        {
            StartCoroutine(CreateCloneWithDelay(_enemyTransform));
        }
    }

    private IEnumerator CreateCloneWithDelay(Transform _enemyTransform)
    {
        yield return new WaitForSeconds(delay);
        CreateClone(_enemyTransform, new Vector3(cloneOffset * player.facingDir, 0));
    }
}