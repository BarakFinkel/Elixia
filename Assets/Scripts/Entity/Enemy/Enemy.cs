using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(EntityFX))]
[RequireComponent(typeof(ItemDrop))]
public class Enemy : Entity
{
    [Header("Player Information")]
    [SerializeField]
    protected LayerMask whatIsPlayer;

    [SerializeField]
    protected float detectPlayerRadius = 5.0f;

    [Header("Movement Information")]
    [SerializeField]
    public float moveSpeed = 2.0f;

    [SerializeField]
    public float idleTime = 1.0f;

    [Header("Attack Information")]
    [SerializeField]
    public float attackDistance = 2.0f;

    [SerializeField]
    public float attackCooldown = 0.6f;

    [SerializeField]
    public float battleTime = 4.0f;

    [SerializeField]
    public float battleDistance = 5.0f;

    [SerializeField]
    public float closeAggroDistance = 2.0f;

    [SerializeField]
    public float attackDistanceFromPlayer = 0.1f;

    [HideInInspector]
    public float lastTimeAttacked;

    [Header("Stunned Information")]
    [SerializeField]
    public float stunnedDuration = 1.0f;

    [SerializeField]
    public Vector2 stunnedDirection = new(10, 3);

    [SerializeField]
    public float blinkDelay;

    [SerializeField]
    protected GameObject counterImage;

    [Header("Death Information")]
    [SerializeField]
    public float knockUpTime = 0.1f;

    [SerializeField]
    public Vector2 knockUpVelocity = new(0, 10);

    [SerializeField]
    public float destructionDelay = 3.0f;

    [Header("Sound Information")]
    [SerializeField]
    public int attackSoundIndex;

    public string lastAnimBoolName;
    protected bool canBeStunned;
    private float defaultMoveSpeed;


    public EnemyStateMachine stateMachine { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new EnemyStateMachine();

        defaultMoveSpeed = moveSpeed;
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position,
            new Vector3(transform.position.x + attackDistance * facingDir, transform.position.y));
    }

    public virtual void AssignLastAnimName(string _animBoolName)
    {
        lastAnimBoolName = _animBoolName;
    }

    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        moveSpeed *= 1.0f - _slowPercentage;
        anim.speed *= 1.0f - _slowPercentage;

        Invoke("ReturnToDefaultSpeed", _slowDuration);
    }

    public override void ReturnToDefaultSpeed()
    {
        base.ReturnToDefaultSpeed();
        moveSpeed = defaultMoveSpeed;
    }

    public virtual void AnimationTrigger()
    {
        stateMachine.currentState.AnimationFinishTrigger();
    }

    public virtual void AnimationSpecialAttackTrigger()
    {
    }

    public virtual RaycastHit2D IsPlayerDetected()
    {
        return Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, detectPlayerRadius, whatIsPlayer);
    }

    #region Freeze

    public void FreezeTime(bool _timeFrozen)
    {
        if (_timeFrozen)
        {
            moveSpeed = 0.0f;
            anim.speed = 0.0f;
        }
        else
        {
            moveSpeed = defaultMoveSpeed;
            anim.speed = 1.0f;
        }
    }

    public virtual void FreezeTimeFor(float _duration)
    {
        StartCoroutine(FreezeTimeCoroutine(_duration));
    }

    protected virtual IEnumerator FreezeTimeCoroutine(float _seconds)
    {
        FreezeTime(true);

        yield return new WaitForSeconds(_seconds);

        FreezeTime(false);
    }

    public void DestroySelfInDelay()
    {
        Invoke("DestroySelf", destructionDelay);
    }

    protected void DestroySelf()
    {
        Destroy(gameObject);
    }

    #endregion

    #region Counter Attack Window

    public virtual void OpenCounterAttackWindow()
    {
        canBeStunned = true;
        counterImage.SetActive(true);
    }

    public virtual void CloseCounterAttackWindow()
    {
        canBeStunned = false;
        counterImage.SetActive(false);
    }

    public virtual bool CanBeStunned()
    {
        if (canBeStunned)
        {
            CloseCounterAttackWindow();
            return true;
        }

        return false;
    }

    #endregion
}