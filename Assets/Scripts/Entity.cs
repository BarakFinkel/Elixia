using System;
using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Knockback Info")] [SerializeField]
    protected Vector2 knockbackDir;

    [SerializeField] protected float knockbackDuration;
    [Header("Collision Info")] public Transform attackCheck;
    public float attackCheckRadius;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDist;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDist;
    [SerializeField] protected LayerMask whatIsGround;
    protected bool facingRight = true;
    protected bool isKnocked;

    public Action onFlipped;
    public int facingDir { get; private set; } = 1;

    public CharacterStats stats { get; private set; }
    public CapsuleCollider2D cd { get; private set; }

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
        animator = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        fx = GetComponent<EntityFx>();
        stats = GetComponent<CharacterStats>();
        cd = GetComponent<CapsuleCollider2D>();
    }


    protected virtual void Update()
    {
    }

    public virtual void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
    }

    protected virtual void ReturnDefaultSpeed()
    {
        animator.speed = 1;
    }

    public virtual void DamageImpact()
    {
        StartCoroutine("HitKnockback");
    }

    protected virtual IEnumerator HitKnockback()
    {
        isKnocked = true;
        rb.linearVelocity = new Vector2(knockbackDir.x * -facingDir, knockbackDir.y);

        yield return new WaitForSeconds(knockbackDuration);

        isKnocked = false;
    }


    public virtual void Die()
    {
    }

    # region Components

    public Animator animator { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public EntityFx fx { get; private set; }

    public SpriteRenderer sr { get; private set; }

    # endregion Components


    #region Collision

    public virtual bool IsGroundDetected()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDist, whatIsGround);
    }

    public virtual bool IsWallDetected()
    {
        return Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDist, whatIsGround);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position,
            new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDist));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDist, wallCheck.position.y));

        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }

    #endregion Collision

    #region Flip

    public virtual void Flip()
    {
        facingDir *= -1;
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);

        if (onFlipped != null)
        {
            onFlipped();
        }
    }

    public virtual void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)
        {
            Flip();
        }
        else if (_x < 0 && facingRight)
        {
            Flip();
        }
    }

    #endregion Flip

    #region Velocity

    public void SetVelocity(Vector2 velocity)
    {
        if (isKnocked)
        {
            return;
        }

        rb.linearVelocity = velocity;
        FlipController(velocity.x);
    }

    public void SetZeroVelocity()
    {
        if (isKnocked)
        {
            return;
        }

        rb.linearVelocity = Vector2.zero;
    }

    #endregion Velocity
}