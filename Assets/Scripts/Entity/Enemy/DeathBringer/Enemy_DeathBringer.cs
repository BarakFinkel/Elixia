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

    
    #region States

    public DeathBringer_BattleState battleState { get;private set; }
    public DeathBringer_AttackState attackState { get;private set; }
    public DeathBringer_IdleState idleState { get;private set; }
    public DeathBringer_DeadState deadState { get;private set; }
    public DeathBringer_TeleportState teleportState { get;private set; }
    public DeathBringer_SpellCastState spellCastState { get;private set; }

    #endregion

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
    
    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);
    }

    public void FindPosition()
    {
        float x = Random.Range(arena.bounds.min.x + 3, arena.bounds.max.x - 3);
        float y = Random.Range(arena.bounds.min.y + 3, arena.bounds.max.y - 3);
        
        CapsuleCollider2D collider = cd as CapsuleCollider2D;
        
        transform.position = new Vector3(x, y);
        transform.position = new Vector3(transform.position.x, transform.position.y - GroundBelow().distance + (collider.size.y/2));

        if (!GroundBelow() || SomethingIsAround())
        {
            FindPosition();
        }
    }
    
    private RaycastHit2D GroundBelow()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 100, whatIsGround);
    
        if (hit.collider == null)
        {
            Debug.LogWarning($"GroundBelow() did not detect anything at {transform.position}");
        }
    
        return hit;
    }
    private bool SomethingIsAround()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, surroundingCheckSize, 0, Vector2.zero, 0, whatIsGround);
    
        return hit.collider != null;
    }
        
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - GroundBelow().distance));
        Gizmos.DrawWireCube(transform.position, surroundingCheckSize);
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
        Player player = PlayerManager.instance.player;

        float xOffset = 0;
        
        if (player.rb.linearVelocity.x != 0)
        {
            xOffset = player.facingDir * spellOffset.x;
        }
        
        Vector3 spellPos = new Vector3(player.transform.position.x + player.facingDir * xOffset, player.transform.position.y + spellOffset.y);
        
        GameObject newSpell = Instantiate(spellPrefab, spellPos, Quaternion.identity);
        newSpell.GetComponent<DeathbringerSpell_Controller>().SetupSpell(cs);
    }
}


