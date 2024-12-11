using System.Collections;
using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] protected LayerMask whatIsPlayer;

    [Header("Stunned Info")] public float stunDuration;

    public Vector2 stunDirection;
    [SerializeField] protected GameObject counterImage;

    [Header("Move info")] public float moveSpeed;

    public float idleTime;
    public float battleTime;

    [Header("Attack Info")] public float attackDist;

    public float attackCooldown;
    [HideInInspector] public float lastTimeAttacked;
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
            new Vector3(transform.position.x + attackDist * facingDir, transform.position.y));
    }

    public virtual void FreezeTime(bool _timeFrozen)
    {
        if (_timeFrozen)
        {
            moveSpeed = 0;
            animator.speed = 0;
        }
        else
        {
            animator.speed = 1;
            moveSpeed = defaultMoveSpeed;
        }
    }

    protected virtual IEnumerator FreezeTimeFor(float _seconds)
    {
        FreezeTime(true);
        yield return new WaitForSeconds(_seconds);
        FreezeTime(false);
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

    public virtual void AnimationFinishTrigger()
    {
        stateMachine.currentState.AnimationFinishTrigger();
    }

    public virtual RaycastHit2D IsPlayerDetected()
    {
        int raycastSize = 50;
        return Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, raycastSize, whatIsPlayer);
    }

    #region counterAttack

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

    #endregion
}