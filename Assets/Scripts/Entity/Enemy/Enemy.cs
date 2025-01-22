using System.Collections;
using UnityEngine;

public class Enemy : Entity
{
    [Header("Player Information")]
    [SerializeField] protected LayerMask whatIsPlayer;
    [SerializeField] protected float detectPlayerRadius = 5.0f;

    [Header("Movement Information")]
    [SerializeField] public float moveSpeed = 2.0f;
    private float defaultMoveSpeed;
    [SerializeField] public float idleTime = 1.0f;

    [Header("Attack Information")]
    [SerializeField] public float attackDistance;
    [SerializeField] public float attackCooldown = 0.6f;
    [SerializeField] public float battleTime = 4.0f;
    [SerializeField] public float battleDistance = 5.0f;
    [SerializeField] public float closeAggroDistance = 2.0f;
    [HideInInspector] public float lastTimeAttacked;

    [Header("Stunned Information")]
    [SerializeField] public float stunnedDuration;
    [SerializeField] public float blinkDelay;
    [SerializeField] public Vector2 stunnedDirection;
    [SerializeField] protected GameObject counterImage;
    protected bool canBeStunned = false;

    [Header("Death Information")]
    [SerializeField] public float knockUpTime = 0.1f;
    [SerializeField] public Vector2 knockUpVelocity = new Vector2(0, 10); 


    public EnemyStateMachine stateMachine { get; private set; }
    public string lastAnimBoolName;

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

    public virtual void AssignLastAnimName(string _animBoolName) => lastAnimBoolName = _animBoolName;

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

    public virtual void FreezeTimeFor(float _duration) => StartCoroutine(FreezeTimeCoroutine(_duration));

    protected virtual IEnumerator FreezeTimeCoroutine(float _seconds)
    {
        FreezeTime(true);

        yield return new WaitForSeconds(_seconds);

        FreezeTime(false);
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
        if(canBeStunned)
        {
            CloseCounterAttackWindow();
            return true;
        }
        
        return false;
    }

    #endregion

    public virtual void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    public virtual RaycastHit2D IsPlayerDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, detectPlayerRadius, whatIsPlayer);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + attackDistance * facingDir, transform.position.y));
    }
}
