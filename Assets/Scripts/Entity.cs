using System;
using UnityEngine;

public class Entity : MonoBehaviour
{
    
    [Header("Collision Info")]
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDist;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDist;
    [SerializeField] protected LayerMask whatIsGround;
    
    public int facingDir { get; private set; } = 1;
    protected bool facingRight = true;
    
    # region Components
    public Animator animator {get; private set;}
    public Rigidbody2D rb {get; private set;}
    
    # endregion Components
    
    protected virtual void Awake()
    {
        
    }

    protected virtual void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

  
    protected virtual void Update()
    {
        
    }
    
    #region Collision
    
    public virtual bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDist, whatIsGround);
    
    public virtual bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDist, whatIsGround);

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDist));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDist, wallCheck.position.y));
    }
    #endregion Collision
    
    #region Flip
    public virtual void Flip()
    {
        facingDir *= -1;
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    public virtual void FlipController(float _x)
    {
        if(_x > 0 && !facingRight )
            Flip();
        else if(_x < 0 && facingRight )
            Flip();
    }
    #endregion Flip
    
    #region Velocity
    public void SetVelocity(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
        FlipController(velocity.x);
    }
    
    public void SetZeroVelocity() => rb.linearVelocity = Vector2.zero;
    #endregion Velocity
}
