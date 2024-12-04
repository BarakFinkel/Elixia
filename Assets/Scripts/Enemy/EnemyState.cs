using UnityEngine;

public class EnemyState
{
    private readonly string animBoolName;
    protected Enemy enemyBase;
    protected Rigidbody2D rb;

    protected EnemyStateMachine stateMachine;

    protected float stateTimer;


    protected bool triggerCalled;

    public EnemyState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName)
    {
        enemyBase = _enemyBase;
        stateMachine = _stateMachine;
        animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        triggerCalled = false;
        enemyBase.animator.SetBool(animBoolName, true);
        rb = enemyBase.rb;
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
    }

    public virtual void Exit()
    {
        enemyBase.animator.SetBool(animBoolName, false);
    }

    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }
}