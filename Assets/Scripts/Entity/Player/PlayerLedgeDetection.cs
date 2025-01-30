using UnityEngine;

public class PlayerLedgeDetection : MonoBehaviour
{
    private Player player => GetComponentInParent<Player>();
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float radius;

    private bool canDetect;

    private void Update()
    {
        if (canDetect)
        {
            player.ledgeDetected = Physics2D.OverlapCircle(transform.position, radius, whatIsGround);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            canDetect = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            canDetect = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
