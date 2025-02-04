using UnityEngine;

public class Archer : Enemy
{
    [Header("Archer Specifics")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float arrowSpeed;
    [Space]
    [SerializeField] public Vector2 jumpVelocity;
    [SerializeField] public float maxDistanceForJump; // The maximal distance from the player to trigger the enemy's jump
    [SerializeField] public float jumpCooldown;
    [HideInInspector] public float lastTimeJumped;
    [SerializeField] private Transform groundBehindCheck;
    [SerializeField] private Vector2 groundBehinCheckSize;

    public bool initialBattleState = true;
    
    #region States

    public ArcherIdleState idleState { get; private set; }
    public ArcherMoveState moveState { get; private set; }
    public ArcherJumpState jumpState { get; private set; }
    public ArcherBattleState battleState { get; private set; }
    public ArcherAttackState attackState { get; private set; }
    public ArcherStunnedState stunnedState { get; private set; }
    public ArcherDeadState deadState { get; private set; }

    #endregion

    protected override void Awake()
    {
        base.Awake();

        idleState = new ArcherIdleState(this, stateMachine, "Idle", this);
        moveState = new ArcherMoveState(this, stateMachine, "Move", this);
        jumpState = new ArcherJumpState(this, stateMachine, "Jump", this);
        battleState = new ArcherBattleState(this, stateMachine, "Idle", this);
        attackState = new ArcherAttackState(this, stateMachine, "Attack", this);
        stunnedState = new ArcherStunnedState(this, stateMachine, "Stunned", this);
        deadState = new ArcherDeadState(this, stateMachine, "Idle", this);
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initiallize(idleState);
    }

    // We check this way if the enemy could be stunned or not, but actively will only be checking once we are in attack-range with the player.
    public override bool CanBeStunned()
    {
        if (base.CanBeStunned())
        {
            stateMachine.ChangeState(stunnedState);
            return true;
        }

        return false;
    }

    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);
    }

    public override RaycastHit2D IsPlayerDetected()
    {
        return Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, battleDistance, whatIsPlayer);
    }

    public override void AnimationSpecialAttackTrigger()
    {
        GameObject newArrow = Instantiate(arrowPrefab, attackCheck.position, Quaternion.identity);
        newArrow.GetComponent<ArrowController>().SetupArrow(arrowSpeed * facingDir, cs);

        AudioManager.instance.PlaySFX(38, 0, this.transform);
    }

    // Helps check if there is ground around the ground check object - will serve checking if the archer can jump back.
    public bool IsGrounBehind() => Physics2D.BoxCast(groundBehindCheck.position, groundBehinCheckSize, 0, Vector2.zero, 0, whatIsGround);
    public bool IsWallBehind() => Physics2D.Raycast(wallCheck.position, Vector2.right * -facingDir, wallCheckSize.y, whatIsGround);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawWireCube(groundBehindCheck.position, groundBehinCheckSize);
    }
}
