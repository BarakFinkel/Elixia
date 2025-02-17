using UnityEngine;

public class ArcaneEffectController : MonoBehaviour
{
    [SerializeField]
    private LayerMask whatIsEnemy;

    [SerializeField]
    private AudioSource movementSound;

    [SerializeField]
    private AudioSource explosionSound;

    private bool canExplode;
    private bool canGrow;
    private bool canMove;
    private Transform closestTarget;
    private int crystalDamage;

    private float crystalExistTimer;
    private float growScale;
    private float growSpeed;
    private float moveSpeed;
    private Animator anim => GetComponent<Animator>();
    private CircleCollider2D cd => GetComponent<CircleCollider2D>();

    private void Update()
    {
        if (crystalExistTimer > 0)
        {
            crystalExistTimer = Mathf.Max(crystalExistTimer - Time.deltaTime, 0);
        }

        if (crystalExistTimer == 0)
        {
            movementSound.Stop();

            if (!AudioManager.instance.IsPlayingSFX(42))
            {
                AudioManager.instance.PlaySFX(42, 0, transform);
            }

            EndCrystalCycle();
        }

        // If the crystal explosion can grow, we increase the scale in a given speed to a given scale.
        if (canGrow)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(growScale, growScale),
                growSpeed * Time.deltaTime);
        }

        // If we allow the crystal to move, it'll move towards the closest enemy target if it exists.
        if (canMove && closestTarget != null)
        {
            transform.position =
                Vector2.MoveTowards(transform.position, closestTarget.position, moveSpeed * Time.deltaTime);

            // If the crystal is close enough to the target - it ends it's cycle (either explodes or dissappears)
            // * 1 is not a magic number, it is exactly right for how close the crystal should be to the target before exploding.
            if (Vector2.Distance(transform.position, closestTarget.position) < 1)
            {
                movementSound.Stop();
                explosionSound.Play();
                EndCrystalCycle();
            }
        }
    }


    public void SetupCrystal(float _crystalDuration, bool _canExplode, bool _canMove, float _moveSpeed,
        float _growSpeed, float _growScale, int _damage)
    {
        crystalExistTimer = _crystalDuration;
        canExplode = _canExplode;
        canMove = _canMove;
        moveSpeed = _moveSpeed;
        growSpeed = _growSpeed;
        growScale = _growScale;
        crystalDamage = _damage;
        closestTarget = FindClosestEnemy(gameObject.transform);

        movementSound.Play();
    }

    private void AnimationExplodeEvent()
    {
        // All enemies within the attack range
        var colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);

        foreach (var hit in colliders)
            if (hit.GetComponent<Enemy>() != null)
            {
                hit.GetComponent<Entity>().SetupKnockbackDir(transform);

                PlayerManager.instance.player.cs.DoMagicalDamage(hit.GetComponent<CharacterStats>(), MagicType.Arcane,
                    crystalDamage);
            }
    }

    public void EndCrystalCycle()
    {
        if (canExplode)
        {
            canMove = false;
            canGrow = true;

            anim.SetTrigger("Explode");
        }
        else
        {
            SelfDestroy();
        }
    }

    public void SelfDestroy()
    {
        Destroy(gameObject);
    }

    public Transform FindClosestEnemy(Transform _checkTransform)
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

    public void ChooseRandomEnemy()
    {
        var radius = SkillManager.instance.blackhole.GetBlackHoleRadius();
        var colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsEnemy);
        if (colliders.Length > 0)
        {
            closestTarget = colliders[Random.Range(0, colliders.Length)].transform;
        }
    }
}