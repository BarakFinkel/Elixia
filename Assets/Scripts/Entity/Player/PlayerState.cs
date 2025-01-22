using System.Dynamic;
using UnityEngine;

public class PlayerState
{
    // Components
    protected PlayerStateMachine stateMachine;
    protected Player player;
    protected Rigidbody2D rb;

    // Variables
    protected float xInput;
    protected float yInput;
    private string animBoolName;
    protected float stateTimer = 0.0f;
    protected bool triggerCalled;

    // PlayerState Constructor
    public PlayerState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    // Enter method (Not MonoBehaviour's!)
    public virtual void Enter()
    {
        player.anim.SetBool(animBoolName, true);
        rb = player.rb;
        triggerCalled = false; // So we only trigger when and if needed.
    }

    // Update method (Not MonoBehaviour's!)
    public virtual void Update()
    {
        if (stateTimer > 0)
        {
            stateTimer = Mathf.Max(stateTimer - Time.deltaTime, 0);
        }

        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
        player.anim.SetFloat("yVelocity", rb.linearVelocityY);
    }

    // Exit method (Not MonoBehaviour's!)
    public virtual void Exit()
    {
        player.anim.SetBool(animBoolName, false);
    }

    // Changes the trigger to be true to allow altering states in a child state.
    // Will be used via the animator component on the player's animator child object.
    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }
}
