using System.Collections;
using System.Reflection;
using UnityEngine;

public class Entity : MonoBehaviour
{
    #region Components

    // The player's animator component
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public CapsuleCollider2D cd { get; private set; }
    public EntityFX fx { get; private set; }
    public SpriteRenderer sr { get; private set; }
    public CharacterStats cs { get; private set; }

    #endregion

    [Header("Collision Information")]
    [SerializeField] public Transform attackCheck;
    [SerializeField] public float attackCheckRadius;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] public float groundCheckCancelTime = 0.2f;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;

    [Header("Knockback Information")]
    [SerializeField] protected Vector2 knockbackPower;
    [SerializeField] protected float knockBackDuration;
    protected bool isKnocked;

    
    // Entity model direction control variables
    public int facingDir { get; private set; } = 1;
    protected bool facingRight = true;

    public System.Action onFlipped;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        anim = GetComponentInChildren<Animator>(); // Since the animator is a child object.
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CapsuleCollider2D>();
        fx = GetComponent<EntityFX>();
        sr = GetComponent<SpriteRenderer>();
        cs = GetComponent<CharacterStats>();
    }

    protected virtual void Update()
    {
    }

    // Meant to be overriden by the player and the enemy differently.
    public virtual void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {

    }

    public virtual void ReturnToDefaultSpeed()
    {
        anim.speed = 1.0f;
    }

    public virtual void DamageImpact() => StartCoroutine("HitKnockback");

    protected virtual IEnumerator HitKnockback()
    {
        isKnocked = true;
        rb.linearVelocity = new Vector2(knockbackPower.x * -facingDir, knockbackPower.y);
        
        yield return new WaitForSeconds(knockBackDuration);

        rb.linearVelocity = new Vector2(0, 0);
        isKnocked = false;
    }

    #region Velocity

    // Setting the velocity to 0,0
    public void ZeroVelocity()
    {
        if (isKnocked)
        {
            return;
        }
        
        rb.linearVelocity = new Vector2(0, 0);
    }

    // A more readable way to set the velocity of a player.
    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        if (isKnocked)
        {
            return;
        }

        rb.linearVelocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }

    #endregion

    #region Collision

    // A method to check if the player is within the given distance from a ground object.
    public bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

    // A method to check if the player is within the given distance from a ground object.
    public bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);

    // A method to display a line representing the ray for collision checks.
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }

    #endregion

    #region Flip

    // A method applying a 180 deg flip to the character model.
    public virtual void Flip()
    {
        facingDir = facingDir * -1; // -1 in order to represent flipping on the x axis on the opposite direction.
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);  // 180 Degrees in order to actively flip the player to the opposite x-axis direction.

        if (onFlipped != null)
        {
            onFlipped();
        }
    }

    // A method to control when to flip the character model.
    public virtual void FlipController(float _x)
    {
        // If the player is facing a direction opposite to his movement, flip it's model.
        if ((_x > 0 && !facingRight) || (_x < 0 && facingRight))
        {
            Flip();
        }
    }

    #endregion

    public virtual void Die()
    {

    }
}