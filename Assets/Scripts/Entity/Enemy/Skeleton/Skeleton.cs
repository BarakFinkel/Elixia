using UnityEngine;

public class Skeleton : Enemy
{
    // When awaking, we construct the state machines and all possible states.
    protected override void Awake()
    {
        base.Awake();

        idleState = new SkeletonIdleState(this, stateMachine, "Idle", this);
        moveState = new SkeletonMoveState(this, stateMachine, "Move", this);
        battleState = new SkeletonBattleState(this, stateMachine, "Move", this);
        attackState = new SkeletonAttackState(this, stateMachine, "Attack", this);
        stunnedState = new SkeletonStunnedState(this, stateMachine, "Stunned", this);
        deadState = new SkeletonDeadState(this, stateMachine, "Idle", this);
    }

    // When starting, we initiallize the state machine and get child components.
    protected override void Start()
    {
        base.Start();
        stateMachine.Initiallize(idleState);
    }

    // We constantly update using the state machine's current state (Non-MonoBehaviour) update method.
    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.M))
        {
            stateMachine.ChangeState(stunnedState);
        }
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

    #region States

    public SkeletonIdleState idleState { get; private set; }
    public SkeletonMoveState moveState { get; private set; }
    public SkeletonBattleState battleState { get; private set; }
    public SkeletonAttackState attackState { get; private set; }
    public SkeletonStunnedState stunnedState { get; private set; }
    public SkeletonDeadState deadState { get; private set; }

    #endregion
}