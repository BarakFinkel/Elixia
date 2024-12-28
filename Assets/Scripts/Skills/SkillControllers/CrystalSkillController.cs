using UnityEngine;

public class CrystalSkillController : MonoBehaviour
{
    [SerializeField] private float growSpeed;
    [SerializeField] private Vector3 growSize = new(3, 3);
    [SerializeField] private LayerMask whatIsEnemy;

    private bool canExplode;

    private bool canGrow;
    private bool canMove;

    private Transform closestTarget;

    private float crystalExistTimer;
    private float moveSpeed;
    private Player player;
    private Animator anim => GetComponent<Animator>();
    private CircleCollider2D cd => GetComponent<CircleCollider2D>();

    private void Update()
    {
        crystalExistTimer -= Time.deltaTime;

        if (crystalExistTimer < 0)
        {
            FinishCrystal();
        }

        if (canGrow)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, growSize, Time.deltaTime * growSpeed);
        }

        if (canMove)
        {
            transform.position =
                Vector3.MoveTowards(transform.position, closestTarget.position, Time.deltaTime * moveSpeed);

            if (Vector3.Distance(transform.position, closestTarget.position) < 1)
            {
                FinishCrystal();
                canMove = false;
            }
        }
    }

    public void SetupCrystal(float _crystalDuration, bool _canExplode, bool _canMove, float _moveSpeed,
        Transform _closestTarget, Player _player)
    {
        crystalExistTimer = _crystalDuration;
        canExplode = _canExplode;
        canMove = _canMove;
        moveSpeed = _moveSpeed;
        closestTarget = _closestTarget;
        player = _player;
    }

    public void FinishCrystal()
    {
        if (canExplode)
        {
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

    private void AnimationExplodeEvent()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);

        foreach (var hit in colliders)
            if (hit.GetComponent<Enemy>() != null)
            {
                player.stats.DoMagicalDamage(hit.GetComponent<CharacterStats>());
            }
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