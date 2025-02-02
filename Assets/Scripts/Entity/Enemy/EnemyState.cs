using UnityEngine;

public class EnemyState
{
    protected readonly string animBoolName;
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
        enemyBase.anim.SetBool(animBoolName, true);
        rb = enemyBase.rb;
    }

    public virtual void Update()
    {
        if (stateTimer > 0)
        {
            stateTimer = Mathf.Max(stateTimer - Time.deltaTime, 0);
        }
    }

    public virtual void Exit()
    {
        enemyBase.anim.SetBool(animBoolName, false);
        enemyBase.AssignLastAnimName(animBoolName);
    }

    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }
}