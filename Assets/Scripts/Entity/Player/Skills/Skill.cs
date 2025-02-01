using UnityEngine;

public class Skill : MonoBehaviour
{
    [SerializeField] public float cooldown;
    protected float cooldownTimer;

    protected Player player;

    protected virtual void Start()
    {
        player = PlayerManager.instance.player;
    }

    protected virtual void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer = Mathf.Max(cooldownTimer - Time.deltaTime, 0);
        }

        CheckUnlock();
    }

    protected virtual void CheckUnlock()
    {
        
    }

    public virtual bool CanUseSkill()
    {
        if (cooldownTimer == 0)
        {
            UseSkill();
            cooldownTimer = cooldown;

            return true;
        }

        Debug.Log("Skill is on cooldown");
        return false;
    }

    public virtual void UseSkill()
    {
    }

    public virtual Transform FindClosestEnemy(Transform _checkTransform)
    {
        // All enemies within the attack range
        var colliders = Physics2D.OverlapCircleAll(_checkTransform.position, SkillManager.instance.enemyDetectRadius);

        var closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        // We go over all enemies in the attack range, we find the one closest and store it in our closestEnemy varible.
        foreach (var hit in colliders)
            if (hit.GetComponent<Enemy>() != null)
            {
                var distanceToEnemy = Vector2.Distance(_checkTransform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }

        return closestEnemy;
    }
}