using UnityEngine;

public class CloneSkillController : MonoBehaviour
{
    [SerializeField] private float colorLoosingSpeed;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius = .8f;
    private Animator anim;


    private float cloneTimer;

    private Transform closestEnemy;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        cloneTimer -= Time.deltaTime;

        if (cloneTimer <= 0)
        {
            sr.color = new Color(1, 1, 1, sr.color.a - Time.deltaTime * colorLoosingSpeed);

            if (sr.color.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetupClone(Transform _newTransform, float _cloneDuration, bool _canAttack)
    {
        if (_canAttack)
        {
            anim.SetInteger("AttackNumber", Random.Range(1, 3));
        }

        transform.position = _newTransform.position;
        cloneTimer = _cloneDuration;

        FaceClosestTarget();
    }

    private void AnimationTrigger()
    {
        cloneTimer = -.1f;
    }

    private void AttackTrigger()
    {
        var colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);

        foreach (var hit in colliders)
            if (hit.GetComponent<Enemy>() != null)
            {
                hit.GetComponent<Enemy>().Damage();
            }
    }

    private void FaceClosestTarget()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, 25);

        var closestDistance = Mathf.Infinity;

        foreach (var hit in colliders)
            if (hit.GetComponent<Enemy>() != null)
            {
                var distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }

        if (closestEnemy != null)
        {
            if (transform.position.x > closestEnemy.position.x)
            {
                transform.Rotate(0, 180, 0);
            }
        }
    }
}