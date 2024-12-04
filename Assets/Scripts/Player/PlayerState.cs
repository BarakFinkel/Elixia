using UnityEngine;

public class PlayerState
{
    private readonly string animBoolName;
    protected Player player;

    protected Rigidbody2D rb;
    protected PlayerStateMachine stateMachine;

    protected float stateTimer;
    protected bool triggerCalled;


    protected float xInput;
    protected float yInput;

    public PlayerState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
    {
        player = _player;
        stateMachine = _stateMachine;
        animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        player.animator.SetBool(animBoolName, true);
        rb = player.rb;
        triggerCalled = false;
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;

        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
        player.animator.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    public virtual void Exit()
    {
        player.animator.SetBool(animBoolName, false);
    }

    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }
}