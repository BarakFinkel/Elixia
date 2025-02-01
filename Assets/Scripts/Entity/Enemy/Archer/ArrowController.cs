using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] private string targetLayerName = "Player";
    [SerializeField] private int damage = 0;

    [SerializeField] private float xVelocity;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private bool canMove = true;

    [SerializeField] private float minDestructionTime = 4.0f;
    [SerializeField] private float maxDestructionTime = 6.0f;

    private void Update()
    {
        if(canMove)
        {
            rb.linearVelocity = new Vector2(xVelocity, rb.linearVelocityY);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(targetLayerName))
        {
            other.GetComponent<CharacterStats>()?.TakeDamage(damage);
            StickInto(other);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            StickInto(other);
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

        Destroy(gameObject, Random.Range(minDestructionTime, maxDestructionTime));
    }
}
