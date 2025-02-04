using UnityEngine;

public class IceShardController : MonoBehaviour
{
    private int damage;
    private float duration;
    private float durationTimer;
    private float enemyFreezeDuration;
    private readonly Vector3 initialScale = new(1, 1, 1);
    private Rigidbody2D rb;
    private Vector3 smallestScale;

    private void Update()
    {
        durationTimer -= Time.deltaTime;

        if (durationTimer <= 0)
        {
            Destroy(gameObject);
            return;
        }

        ScaleShard();
    }

    // Handles collisions of the ice shards
    private void OnTriggerEnter2D(Collider2D other)
    {
        // If the shard touched any ground object, it shatters.
        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
            return;
        }

        // If the shard touches an enemy, deal magic damage to it, freeze it, and destroy the shard.
        var enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.SetupKnockbackDir(transform);
            
            PlayerManager.instance.player.cs.DoMagicalDamage(enemy.GetComponent<CharacterStats>(), MagicType.Ice,
                damage);
            enemy.FreezeTimeFor(enemyFreezeDuration);
            Destroy(gameObject);
        }
    }

    public void SetupIceShard(int _damage, Vector2 _velocity, float _targetScale, float _enemyFreezeDuration,
        float _duration)
    {
        damage = _damage;
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = _velocity;
        SetupTargetScale(_targetScale);
        enemyFreezeDuration = _enemyFreezeDuration;
        duration = _duration;
        durationTimer = _duration;
    }

    public void ScaleShard()
    {
        var t = 1 - durationTimer / duration;
        transform.localScale = Vector3.Lerp(initialScale, smallestScale, t);
    }

    private void SetupTargetScale(float _targetScale)
    {
        smallestScale = new Vector3(_targetScale, _targetScale, _targetScale); // Target scale
    }
}