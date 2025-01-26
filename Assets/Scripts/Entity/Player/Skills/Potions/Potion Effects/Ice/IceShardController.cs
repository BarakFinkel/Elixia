using UnityEngine;

public class IceShardController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector3 initialScale = new Vector3(1,1,1);
    private Vector3 smallestScale;
    private float enemyFreezeDuration;
    private float duration;
    private float durationTimer;
    
    public void SetupIceShard(Vector2 _velocity, float _targetScale, float _enemyFreezeDuration, float _duration)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = _velocity;
        SetupTargetScale(_targetScale);
        enemyFreezeDuration = _enemyFreezeDuration;
        duration = _duration;
        durationTimer = _duration;
    }

    void Update()
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
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            PlayerManager.instance.player.cs.DoMagicalDamage(enemy.GetComponent<CharacterStats>());
            enemy.FreezeTimeFor(enemyFreezeDuration);
            Destroy(gameObject);
        }
    }

    public void ScaleShard()
    {
        float t = 1 - durationTimer / duration;
        transform.localScale = Vector3.Lerp(initialScale, smallestScale, t);
    }

    private void SetupTargetScale(float _targetScale)
    {
        smallestScale = new Vector3(_targetScale, _targetScale, _targetScale); // Target scale
    }
}
