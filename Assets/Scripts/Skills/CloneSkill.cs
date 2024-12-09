using UnityEngine;

public class CloneSkill : Skill
{
    [Header("Clone info")] [SerializeField]
    private GameObject clonePrefab;

    [SerializeField] private float cloneDuration;
    [SerializeField] private bool canAttack;

    public void CreateClone(Transform _clonePosition, Vector3 _offset)
    {
        var newClone = Instantiate(clonePrefab);

        newClone.GetComponent<CloneSkillController>().SetupClone(_clonePosition, cloneDuration, canAttack, _offset);
    }
}