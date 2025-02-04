using System.Collections.Generic;
using UnityEngine;

public class FireEffectController : MonoBehaviour
{
    private readonly Dictionary<Enemy, float> damageCooldowns = new();
    private CapsuleCollider2D cd;
    private int damage;
    private float damageCooldown;
    private bool toDamage;

    private void Update()
    {
        // All enemies within the attack range
        var colliders = Physics2D.OverlapCapsuleAll(transform.position, cd.size, cd.direction, transform.eulerAngles.z);

        foreach (var hit in colliders)
        {
            var enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (CanDamage(enemy))
                {
                    enemy.SetupKnockbackDir(transform);
                    
                    PlayerManager.instance.player.cs.DoMagicalDamage(hit.GetComponent<CharacterStats>(), MagicType.Fire,
                        damage);
                    damageCooldowns[enemy] = Time.time + damageCooldown;
                }
            }
        }
    }

    public void SetupFlame(int _damage, float _damageCooldown)
    {
        damage = _damage;
        damageCooldown = _damageCooldown;
        cd = GetComponent<CapsuleCollider2D>();
        AudioManager.instance.PlaySFX(7, 0, PlayerManager.instance.player.transform);
    }

    private bool CanDamage(Enemy enemy)
    {
        if (!toDamage)
        {
            return false;
        }

        // Add the enemy to the dictionary if not already present
        if (!damageCooldowns.ContainsKey(enemy))
        {
            damageCooldowns[enemy] = 0f; // Initialize with a time in the past to allow immediate damage
        }

        // Check if the enemy's cooldown has expired
        return Time.time >= damageCooldowns[enemy];
    }

    // Animation Triggers:
    public void startDamage()
    {
        toDamage = true;
    }

    public void stopDamage()
    {
        toDamage = false;
    }

    public void destroyFlame()
    {
        Destroy(gameObject);
    }
}