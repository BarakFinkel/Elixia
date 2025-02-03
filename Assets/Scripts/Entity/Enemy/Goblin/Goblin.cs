using UnityEngine;
using UnityEngine.UI;

public class Goblin : Enemy
{
    [Header("Goblin Specifics")]
    [SerializeField] public GameObject bombPrefab;
    [SerializeField] public Transform bombSpawn;
    [SerializeField] public Slider healthSlider;
    [SerializeField] public GameObject rageMark;
    [SerializeField] public float bombDamageMultiplier = 10.0f;
    [SerializeField] public float bombTriggerHealthPercentage = 0.30f;
    [SerializeField] public float bombTriggerDistance = 0.5f;
    [SerializeField] public float bombMaxScale = 2.0f;
    [SerializeField] public float bombGrowthSpeed = 5.0f;
    [SerializeField] public float delayBeforeMoveFast = 1.0f;
    [SerializeField] public float fastMoveSpeed = 10.0f;

    public bool initialBattleState = true;
    private bool enteredMoveFastState = false;
    
    #region States

    public GoblinIdleState idleState { get; private set; }
    public GoblinMoveState moveState { get; private set; }
    public GoblinBattleState battleState { get; private set; }
    public GoblinAttackState attackState { get; private set; }
    public GoblinStunnedState stunnedState { get; private set; }
    public GoblinMoveFastState moveFastState { get; private set; }
    // public GoblinBombState bombState { get; private set; }
    public GoblinDeadState deadState { get; private set; }

    #endregion

    // When awaking, we construct the state machines and all possible states.
    protected override void Awake()
    {
        base.Awake();

        idleState = new GoblinIdleState(this, stateMachine, "Idle", this);
        moveState = new GoblinMoveState(this, stateMachine, "Move", this);
        battleState = new GoblinBattleState(this, stateMachine, "Move", this);
        attackState = new GoblinAttackState(this, stateMachine, "Attack", this);
        stunnedState = new GoblinStunnedState(this, stateMachine, "Stunned", this);
        moveFastState = new GoblinMoveFastState(this, stateMachine, "Idle", this);
        //bombState = new GoblinBombState(this, stateMachine, "Bomb", this);
        deadState = new GoblinDeadState(this, stateMachine, "Idle", this);
    }

    // When starting, we initiallize the state machine and get child components.
    protected override void Start()
    {
        base.Start();
        stateMachine.Initiallize(idleState);
    }

    protected override void Update()
    {
        base.Update();

        if (!enteredMoveFastState && healthSlider.value < Mathf.RoundToInt(bombTriggerHealthPercentage * healthSlider.maxValue))
        {
            rageMark.SetActive(true);
            stateMachine.ChangeState(moveFastState);
            enteredMoveFastState = true;
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

        rageMark.SetActive(false);
        stateMachine.ChangeState(deadState);
    }

    public override void AnimationSpecialAttackTrigger()
    {
        GameObject newBomb = Instantiate(bombPrefab, bombSpawn.position, Quaternion.identity);
        newBomb.GetComponent<BombController>().SetupBomb(bombMaxScale, bombGrowthSpeed, bombDamageMultiplier, cs);
    } 
}
