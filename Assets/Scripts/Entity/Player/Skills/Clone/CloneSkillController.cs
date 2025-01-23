using UnityEngine;

public class CloneSkillController : MonoBehaviour
{
    [SerializeField]
    private float colorFadeSpeed;

    [SerializeField]
    private Transform attackCheck;

    [SerializeField]
    private float attackCheckRadius = 0.8f;

    private Animator anim;
    private bool canDuplicateClone;

    private float cloneTimer;
    private Transform closestEnemy;
    private float duplicationChance;
    private int facingDir = 1;
    private Player player;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // We decrement the timer, and if done - we start fading the sprite of the clone.
    // If done fading, we delete the game object.
    private void Update()
    {
        if (cloneTimer > 0)
        {
            cloneTimer = Mathf.Max(cloneTimer - Time.deltaTime, 0);
        }

        if (cloneTimer == 0)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a - Time.deltaTime * colorFadeSpeed);

            if (sr.color.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    // This is the method that initiates a clone - meant to be called from various player states.
    public void SetupClone(Transform _newTransform, float _cloneDuration, bool _canAttack, bool _canDuplicate,
        float _duplicationChance, Vector3 offset, Player _player, Transform _closestEnemy)
    {
        if (_canAttack)
        {
            anim.SetInteger("AttackNumber",
                Random.Range(1, 3)); // Only the first 2 attacks of the player are allowed - will stay static.
        }

        player = _player;
        transform.position = _newTransform.position + offset;
        cloneTimer = _cloneDuration;

        closestEnemy = _closestEnemy;
        canDuplicateClone = _canDuplicate;
        duplicationChance = _duplicationChance;
        FaceClosestTarget();
    }

    // This method takes care of getting the clone to face the closest enemy target.
    private void FaceClosestTarget()
    {
        // If there exists a closest enemy.
        if (closestEnemy != null)
        {
            // And he is to the clone's left.
            if (transform.position.x > closestEnemy.position.x)
            {
                facingDir = -1;
                transform.Rotate(0, 180, 0); // We flip the clone's direction (which is defaultively facing right). 
            }
        }
    }

    // Changes the clone timer to 0, will be used as an event by the animator when finishing to attack.
    private void AnimationTrigger()
    {
        cloneTimer = 0.0f;
    }

    // When the player's clone is attempting to attack, only if there are enemies in it's attack radius will the be hit.
    // Will be used as an event by the animator when executing the attack from within the animation.
    private void AttackTrigger()
    {
        // All enemies within the attack range
        var colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);

        foreach (var hit in colliders)
            if (hit.GetComponent<Enemy>() != null)
            {
                player.cs.DoDamage(hit.GetComponent<CharacterStats>());

                if (canDuplicateClone)
                {
                    if (Random.Range(0, 100) < duplicationChance)
                    {
                        SkillManager.instance.clone.CreateClone(hit.transform,
                            new Vector3(1.0f * facingDir, 0.0f, 0.0f));
                    }
                }
            }
    }
}