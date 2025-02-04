using UnityEngine;

public enum SlimeType
{
    big,
    medium,
    small
}

public class Slime : Enemy
{
    [Header("Slime Specifics")]
    [SerializeField] private SlimeType slimeType;
    [SerializeField] private int slimesToCreate;
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private Vector2 minCreationVelocity;
    [SerializeField] private Vector2 maxCreationVelocity;
    [SerializeField] private float cancelKnockbackDelay = 1.5f;
    [SerializeField] private AudioSource slimeAudio;
    [SerializeField] private float distanceToPlayAudio = 10.0f;

    public bool initialBattleState;
    private Transform player;
    
    #region States

    public SlimeIdleState idleState { get; private set; }
    public SlimeMoveState moveState { get; private set; }
    public SlimeBattleState battleState { get; private set; }
    public SlimeAttackState attackState { get; private set; }
    public SlimeStunnedState stunnedState { get; private set; }
    public SlimeDeadState deadState { get; private set; }

    #endregion

    protected override void Awake()
    {
        base.Awake();

        SetupDefaultFacingDirection(-1);

        idleState = new SlimeIdleState(this, stateMachine, "Idle", this);
        moveState = new SlimeMoveState(this, stateMachine, "Move", this);
        battleState = new SlimeBattleState(this, stateMachine, "Move", this);
        attackState = new SlimeAttackState(this, stateMachine, "Attack", this);
        stunnedState = new SlimeStunnedState(this, stateMachine, "Stunned", this);
        deadState = new SlimeDeadState(this, stateMachine, "Idle", this);
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initiallize(idleState);
        player = PlayerManager.instance.player.transform;
        slimeAudio.Stop();
    }

    protected override void Update()
    {
        base.Update();

        HandleSound();
    }

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

        if (slimeType != SlimeType.small)
        {
            CreateSlimes(slimesToCreate, slimePrefab);
        }
    }

    private void CreateSlimes(int _amountOfSlimes, GameObject _slimePrefab)
    {
        for (int i = 0; i < _amountOfSlimes; i++)
        {
            GameObject newSlime = Instantiate(_slimePrefab, transform.position, Quaternion.identity);

            newSlime.GetComponent<Slime>().SetupSlime(facingDir);
        }
    }

    public void SetupSlime(int _facingDir)
    {
        if (facingDir != _facingDir)
        {
            Flip();
        }
        
        float xVelocity = Random.Range(minCreationVelocity.x, maxCreationVelocity.x);
        float yVelocity = Random.Range(minCreationVelocity.y, maxCreationVelocity.y);

        isKnocked = true;

        GetComponent<Rigidbody2D>().linearVelocity = new Vector2(xVelocity * -facingDir, yVelocity);

        Invoke("CancelKnockback", cancelKnockbackDelay);
    }

    private void CancelKnockback() => isKnocked = false;

    private void HandleSound()
    {
        if (slimeAudio.isPlaying && Vector2.Distance(this.transform.position, player.transform.position) > distanceToPlayAudio)
        {
            slimeAudio.Stop();
        }

        if (!slimeAudio.isPlaying && Vector2.Distance(this.transform.position, player.transform.position) < distanceToPlayAudio)
        {
            slimeAudio.Play();
        }       
    }
}
