using UnityEngine;

public class Enemy_DeathBringer : Enemy
{
    public bool bossFightBegun;

    [Header("Teleport Details")]
    [SerializeField]
    private BoxCollider2D arena;

    [SerializeField]
    private Vector2 surroundingCheckSize;

    public float chanceToTeleport;
    public float defaultChanceToTeleport = 25;

    [Header("SpellCast Details")]
    [SerializeField]
    private GameObject spellPrefab;

    [SerializeField]
    private float spellCastCooldown;

    public float lastTimeCast;
    public int amountOfSpells;

    [SerializeField]
    public float spellCooldown;

    [SerializeField]
    private Vector2 spellOffset;

    // Sound
    private bool bossMusicPlaying;
    private readonly float voiceCooldown = 10.0f;
    private float voiceTimer;

    protected override void Awake()
    {
        base.Awake();

        SetupDefaultFacingDirection(-1);
        idleState = new DeathBringer_IdleState(this, stateMachine, "Idle", this);

        battleState = new DeathBringer_BattleState(this, stateMachine, "Move", this);
        attackState = new DeathBringer_AttackState(this, stateMachine, "Attack", this);

        deadState = new DeathBringer_DeadState(this, stateMachine, "Idle", this);

        teleportState = new DeathBringer_TeleportState(this, stateMachine, "Teleport", this);
        spellCastState = new DeathBringer_SpellCastState(this, stateMachine, "SpellCast", this);
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initiallize(idleState);
    }

    protected override void Update()
    {
        base.Update();

        HandleBossSFX();
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawLine(transform.position,
            new Vector3(transform.position.x, transform.position.y - GroundBelow().distance));
        Gizmos.DrawWireCube(transform.position, surroundingCheckSize);
    }

    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);
    }

    public void FindPosition()
    {
        var x = Random.Range(arena.bounds.min.x + 3, arena.bounds.max.x - 3);
        var y = Random.Range(arena.bounds.min.y + 3, arena.bounds.max.y - 3);

        var collider = cd as CapsuleCollider2D;

        transform.position = new Vector3(x, y);
        transform.position = new Vector3(transform.position.x,
            transform.position.y - GroundBelow().distance + collider.size.y / 2);

        if (!GroundBelow() || SomethingIsAround())
        {
            FindPosition();
        }
    }

    private RaycastHit2D GroundBelow()
    {
        var hit = Physics2D.Raycast(transform.position, Vector2.down, 100, whatIsGround);

        if (hit.collider == null)
        {
            Debug.LogWarning($"GroundBelow() did not detect anything at {transform.position}");
        }

        return hit;
    }

    private bool SomethingIsAround()
    {
        var hit = Physics2D.BoxCast(transform.position, surroundingCheckSize, 0, Vector2.zero, 0, whatIsGround);

        return hit.collider != null;
    }

    public bool CanTeleport()
    {
        if (Random.Range(0, 100) <= chanceToTeleport)
        {
            chanceToTeleport = defaultChanceToTeleport;
            return true;
        }

        return false;
    }

    public bool CanDoSpellCast()
    {
        if (Time.time >= lastTimeCast + spellCastCooldown)
        {
            return true;
        }

        return false;
    }

    public void CastSpell()
    {
        var player = PlayerManager.instance.player;

        float xOffset = 0;

        if (player.rb.linearVelocity.x != 0)
        {
            xOffset = player.facingDir * spellOffset.x;
        }

        var spellPos = new Vector3(player.transform.position.x + player.facingDir * xOffset,
            player.transform.position.y + spellOffset.y);

        var newSpell = Instantiate(spellPrefab, spellPos, Quaternion.identity);
        newSpell.GetComponent<DeathbringerSpell_Controller>().SetupSpell(cs);
    }

    public void HandleBossSFX()
    {
        if (!bossMusicPlaying && bossFightBegun)
        {
            AudioManager.instance.PlayMusic(3);
            bossMusicPlaying = true;
        }

        if (bossFightBegun && voiceTimer == 0)
        {
            var coinFlip = Random.Range(0, 2);

            if (coinFlip == 0)
            {
                AudioManager.instance.PlaySFX(44, 0, transform);
            }
            else
            {
                AudioManager.instance.PlaySFX(45, 0, transform);
            }

            voiceTimer = voiceCooldown;
        }
    }

    #region States

    public DeathBringer_BattleState battleState { get; private set; }
    public DeathBringer_AttackState attackState { get; private set; }
    public DeathBringer_IdleState idleState { get; private set; }
    public DeathBringer_DeadState deadState { get; private set; }
    public DeathBringer_TeleportState teleportState { get; private set; }
    public DeathBringer_SpellCastState spellCastState { get; private set; }

    #endregion
}