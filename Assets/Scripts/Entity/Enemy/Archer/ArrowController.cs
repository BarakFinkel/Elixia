using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private string targetLayerName = "Player";

    [SerializeField]
    private float minDestructionTime = 4.0f;

    [SerializeField]
    private float maxDestructionTime = 6.0f;

    private CharacterStats archerStats;
    private bool canMove = true;
    private Transform parentTransform;
    private float xVelocity;

    private void Update()
    {
        if (canMove)
        {
            rb.linearVelocity = new Vector2(xVelocity, rb.linearVelocityY);
        }

        if (parentTransform != null)
        {
            var playerStats = parentTransform.GetComponent<PlayerStats>();
            if (playerStats != null && playerStats.isDead)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(targetLayerName))
        {
            var playerStats = other.GetComponent<CharacterStats>();
            if (playerStats != null && !playerStats.isInvulnerable)
            {
                archerStats.DoDamage(other.GetComponent<CharacterStats>());
                StickInto(other);
            }
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            StickInto(other);
        }
    }

    public void SetupArrow(float _speed, CharacterStats _archerStats)
    {
        xVelocity = _speed;
        archerStats = _archerStats;

        if (xVelocity < 0)
        {
            transform.Rotate(0, 180, 0);
        }
    }

    private void StickInto(Collider2D other)
    {
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponentInChildren<ParticleSystem>().Stop();
        canMove = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.parent = other.transform;
        parentTransform = other.transform;

        Destroy(gameObject, Random.Range(minDestructionTime, maxDestructionTime));
    }
}