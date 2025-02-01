using UnityEngine;

public class LightningStrikeController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;

    private Animator animator;
    private int damage;
    private CharacterStats targetStats;
    private bool triggered;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!targetStats || triggered)
        {
            return;
        }

        transform.position =
            Vector2.MoveTowards(transform.position, targetStats.transform.position, moveSpeed * Time.deltaTime);
        transform.right = transform.position - targetStats.transform.position;

        if (Vector2.Distance(transform.position, targetStats.transform.position) < 0.1f)
        {
            animator.transform.localRotation = Quaternion.identity;
            animator.transform.localPosition = new Vector3(0, .5f, 0);
            transform.localRotation = Quaternion.identity;
            transform.localScale = new Vector3(3, 3);

            Invoke("DamageAndSelfDestroy", .2f);

            triggered = true;

            animator.SetTrigger("Hit");
        }
    }

    public void Setup(int _damage, CharacterStats _targetStats)
    {
        damage = _damage;
        targetStats = _targetStats;
    }

    private void DamageAndSelfDestroy()
    {
        targetStats.ApplyShock(true);
        targetStats.TakeDamage(damage);
        Destroy(gameObject, .4f);
    }
}