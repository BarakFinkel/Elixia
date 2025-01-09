using UnityEngine;

public class Skill : MonoBehaviour
{
    [SerializeField]
    protected float cooldown;

    protected float cooldownTimer;

    protected Player player;

    protected virtual void Start()
    {
        player = PlayerManager.instance.player;
    }

    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }

    public virtual bool CanUseSkill()
    {
        if (cooldownTimer <= 0)
        {
            UseSkill();
            cooldownTimer = cooldown;
            return true;
        }

        return false;
    }

    public virtual void UseSkill()
    {
        // do some skill specific things
    }

    protected virtual Transform FindClosestEnemy(Transform _checkTransform)
    {
        var detectingRadius = 25;
        var colliders = Physics2D.OverlapCircleAll(_checkTransform.position, detectingRadius);

        var closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

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