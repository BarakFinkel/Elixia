using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CloneSkill : Skill
{
    [Header("General Information")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [SerializeField] private float cloneSpawnOffset = 2.0f;
    [SerializeField] private float cloneSpawnDelay = 0.4f;
    private float attackMultiplier = 0.0f;
    
    [Header("Clone Attack Information")]
    [SerializeField] private UI_SkillTreeSlot cloneAttackUnlockButton;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float cloneAttackDamageRatio;
    public bool cloneAttackUnlocked { get; private set; }

    [Header("Enhanced Clone Attack Information")]
    [SerializeField] private UI_SkillTreeSlot enhancedCloneAttackUnlockButton;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float enhancedCloneAttackDamageRatio;
    public bool enhancedCloneAttackUnlocked { get; private set; }

    [Header("Duplicate Clone Information")]
    [SerializeField] private UI_SkillTreeSlot duplicateCloneUnlockButton;
    [SerializeField] private float duplicationChance = 30.0f;
    public bool duplicateCloneUnlocked { get; private set; }

    [Header("Crystal Alternative Information")]
    [SerializeField] private UI_SkillTreeSlot crystalAlternativeUnlockButton;
    public bool crystalAlternativeUnlocked { get; private set; }

    protected override void Start()
    {
        base.Start();
        
        cloneAttackUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockAttack);
        enhancedCloneAttackUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockEnhancedAttack);
        duplicateCloneUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDuplicateClone);
        crystalAlternativeUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalAlternative);
    }

    // We create either a crystal or a clone, depends on the skill tree choices.
    public void CreateClone(Transform _clonePosition, Vector3 _offset)
    {
        // If we chose to create crystals instead of clones via the skill tree, we instantiate a crystal instead
        if (crystalAlternativeUnlocked)
        {
            PotionEffectManager.instance.arcane.CreateCrystal(PlayerManager.instance.player.gameObject, Vector3.zero);
            return;
        }

        var newClone = Instantiate(clonePrefab);

        newClone.GetComponent<CloneSkillController>().SetupClone(_clonePosition, cloneDuration, cloneAttackUnlocked,
            duplicateCloneUnlocked, duplicationChance, _offset, player, FindClosestEnemy(newClone.transform), attackMultiplier);
    }

    public void CreateCloneWithDelay(Transform _enemyTransform)
    {
        StartCoroutine(CloneDelayCoroutine(_enemyTransform,
            new Vector3(cloneSpawnOffset * player.facingDir, 0)));
    }

    public IEnumerator CloneDelayCoroutine(Transform _transform, Vector3 _offset)
    {
        yield return new WaitForSeconds(cloneSpawnDelay);

        CreateClone(_transform, _offset);
    }

    #region Unlock

    private void UnlockAttack()
    {
        if (cloneAttackUnlockButton.unlocked && !cloneAttackUnlocked)
        {
            cloneAttackUnlocked = true;
            attackMultiplier = cloneAttackDamageRatio;
        }
    }

    private void UnlockEnhancedAttack()
    {
        if(enhancedCloneAttackUnlockButton.unlocked && !enhancedCloneAttackUnlocked)
        {
            enhancedCloneAttackUnlocked = true;
            attackMultiplier = enhancedCloneAttackDamageRatio;
        }
    }

    private void UnlockDuplicateClone()
    {
        if (duplicateCloneUnlockButton.unlocked && !duplicateCloneUnlocked)
        {
            duplicateCloneUnlocked = true;
        }
    }

    private void UnlockCrystalAlternative()
    {
        if (crystalAlternativeUnlockButton.unlocked && !crystalAlternativeUnlocked)
        {
            crystalAlternativeUnlocked = true;
        }
    }

    #endregion
}