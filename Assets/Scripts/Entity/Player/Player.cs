using System.Collections;
using UnityEngine;

public class Player : Entity
{
    [SerializeField]
    public float busyWaitDuration = 0.2f;

    [Header("Movement Information")]
    [SerializeField]
    public float moveSpeed = 7.0f;

    [SerializeField]
    public float jumpForce = 12.0f;

    [SerializeField]
    public float airSpeedFactor = 0.8f;

    [SerializeField]
    public float jumpToAirThreshold = -0.2f;

    [SerializeField]
    public float swordReturnImpact = 1.0f;

    [Header("Wall Movement Information")]
    [SerializeField]
    public float wallSlideSpeedFactor = 0.6f;

    [SerializeField]
    public float wallSlideLockJumpTime = 0.5f;

    [SerializeField]
    public float wallJumpDuration = 0.15f;

    [SerializeField]
    public float wallJumpXSpeed = 5.0f;

    [SerializeField]
    private PlayerLedgeDetection ledgeDetector;

    [SerializeField]
    private Vector2 ledgeOffset1;

    [SerializeField]
    private Vector2 ledgeOffset2;

    public Vector2 climbBegunPosition;
    public Vector2 climbOverPosition;
    public bool canGrabLedge = true;
    public bool canClimb;
    public bool jumpAfterLedgeClimb;

    [Header("Dodge Information")]
    [SerializeField]
    public float dodgeSpeed = 3.0f;

    [SerializeField]
    public float dodgeDuration = 2.0f / 3.0f;

    [Header("Attack Information")]
    [SerializeField]
    public float maxComboCount = 2; // The number of the attacks contained in the combo.

    [SerializeField]
    public float comboWindow = 1; // Time to wait before resetting combo after we stopped attacking.

    [SerializeField]
    public Vector2[] attackMovement;

    [SerializeField]
    public float attackMovementTime = 0.1f; // Time of movement after an attack was made.

    [SerializeField]
    public float counterAttackDuration = 0.3f;

    public bool canUseSwordSkill;
    public bool ledgeDetected;

    public bool isUIActive;
    private float defaultDodgeSpeed;
    private float defaultJumpForce;
    private float defaultMovespeed;

    [Header("General Information")] public bool isBusy { get; private set; }

    public float dodgeDir { get; private set; }

    // When awaking, we construct the state machines and all possible states.
    protected override void Awake()
    {
        base.Awake();

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "Jump");
        canClimbState = new PlayerCanClimbState(this, stateMachine, "CanClimb");
        dodgeState = new PlayerDodgeState(this, stateMachine, "Dodge");

        primaryAttackState = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttackState = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");

        aimSwordState = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSwordState = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
        aimState = new PlayerAimState(this, stateMachine, "Aim");

        blackholeState = new PlayerBlackholeState(this, stateMachine, "Blackhole");

        healState = new PlayerHealState(this, stateMachine, "Heal");

        deadState = new PlayerDeadState(this, stateMachine, "Die");
    }

    // When starting, we initiallize the state machine and get child components.
    protected override void Start()
    {
        base.Start();

        skillManager = SkillManager.instance;
        stateMachine.Initialize(idleState);
        ledgeDetector = gameObject.GetComponentInChildren<PlayerLedgeDetection>();

        defaultMovespeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaultDodgeSpeed = dodgeSpeed;
    }

    // We constantly update using the state machine's current state (Non-MonoBehaviour) update method.
    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();
        CheckForDodgeInput();
        CheckForLedge();
        CheckForWeaponSkillChange();
    }

    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    public void CatchTheSword()
    {
        if (canUseSwordSkill)
        {
            stateMachine.ChangeState(catchSwordState);
            Destroy(sword);
        }
    }

    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        moveSpeed *= 1 - _slowPercentage;
        jumpForce *= 1 - _slowPercentage;
        dodgeSpeed *= 1 - _slowPercentage;
        anim.speed *= 1 - _slowPercentage;

        Invoke("ReturnToDefaultSpeed", _slowDuration);
    }

    public override void ReturnToDefaultSpeed()
    {
        base.ReturnToDefaultSpeed();

        moveSpeed = defaultMovespeed;
        jumpForce = defaultJumpForce;
        dodgeSpeed = defaultDodgeSpeed;
    }

    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;

        yield return new WaitForSeconds(_seconds);

        isBusy = false;
    }

    private void CheckForLedge()
    {
        if (ledgeDetected && canGrabLedge)
        {
            canGrabLedge = false;

            Vector2 ledgePosition = GetComponentInChildren<PlayerLedgeDetection>().transform.position;

            climbBegunPosition = ledgePosition + new Vector2(ledgeOffset1.x * facingDir, ledgeOffset1.y);
            climbOverPosition = ledgePosition + new Vector2(ledgeOffset2.x * facingDir, ledgeOffset2.y);

            canClimb = true;
        }

        if (canClimb)
        {
            stateMachine.ChangeState(canClimbState);
            canClimb = false;
        }
    }

    public void AllowLedgeGrabWithDelay()
    {
        Invoke("AllowLedgeGrab", 0.5f);
    }

    private void AllowLedgeGrab()
    {
        canGrabLedge = true;
    }

    public IEnumerator CancelGroundCheck(float _seconds)
    {
        groundCheck.gameObject.SetActive(false);

        yield return new WaitForSeconds(_seconds);

        groundCheck.gameObject.SetActive(true);
    }

    // Used to trigger the animation end boolean varible in the PlayerState component.
    public void AnimationTrigger()
    {
        stateMachine.currentState.AnimationFinishTrigger();
    }


    // To improve performance of dodge, enabling to dodge when doing other activities.
    // This state transition is being made in the player script because we want to enable dodging when performing other activities.
    private void CheckForDodgeInput()
    {
        if (!IsWallDetected() && skillManager.dodge.dodgeUnlocked)
        {
            // If we press Shift and also on the ground.
            if (Input.GetKeyDown(KeyCode.LeftShift) && IsGroundDetected() && SkillManager.instance.dodge.CanUseSkill())
            {
                dodgeDir = Input.GetAxisRaw("Horizontal");

                // If there is no x-Axis input, we just set the dodge direction to the facing direction.
                if (dodgeDir == 0)
                {
                    dodgeDir = facingDir;
                }

                stateMachine.ChangeState(dodgeState);
            }
        }
    }

    // temporary
    private void CheckForWeaponSkillChange()
    {
        // If we're not currently trying to use the weapon skill and try scrolling, change the weapon skill.
        if (!Input.GetKeyDown(KeyCode.Mouse1) && Input.GetAxis("Mouse ScrollWheel") != 0.0f)
        {
            canUseSwordSkill = !canUseSwordSkill;
        }
    }

    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);
    }

    #region Components

    public SkillManager skillManager { get; private set; }
    public GameObject sword { get; private set; }

    [SerializeField]
    public SpriteRenderer potionOnPlayer;

    #endregion

    #region States

    // The state machine
    public PlayerStateMachine stateMachine { get; private set; }

    // Movement States
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerCanClimbState canClimbState { get; private set; }
    public PlayerDodgeState dodgeState { get; private set; }

    // Attack States
    public PlayerPrimaryAttackState primaryAttackState { get; private set; }
    public PlayerCounterAttackState counterAttackState { get; private set; }
    public PlayerAimSwordState aimSwordState { get; private set; }
    public PlayerCatchSwordState catchSwordState { get; private set; }
    public PlayerAimState aimState { get; private set; }

    // Black Hole State
    public PlayerBlackholeState blackholeState { get; private set; }

    // Heal State
    public PlayerHealState healState { get; private set; }

    // Death State
    public PlayerDeadState deadState { get; private set; }

    #endregion
}