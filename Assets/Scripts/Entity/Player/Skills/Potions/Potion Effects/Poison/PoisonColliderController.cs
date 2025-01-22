using System.Collections.Generic;
using UnityEngine;

public class PoisonColliderController : MonoBehaviour
{
    protected float damageCooldown = 2f;

    protected CircleCollider2D cd;
    protected Dictionary<Enemy, float> damageCooldowns = new Dictionary<Enemy, float>();

    private void Start()
    {
        cd = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        // All enemies within the attack range
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);

        foreach(var hit in colliders)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (CanDamage(enemy))
                {
                    PlayerManager.instance.player.cs.DoMagicalDamage(hit.GetComponent<CharacterStats>());
                    damageCooldowns[enemy] = Time.time + damageCooldown;
                }
            }
        }
    }

    private bool CanDamage(Enemy enemy)
    {
        // Add the enemy to the dictionary if not already present
        if (!damageCooldowns.ContainsKey(enemy))
        {
            damageCooldowns[enemy] = 0f; // Initialize with a time in the past to allow immediate damage
        }

        // Check if the enemy's cooldown has expired
        return Time.time >= damageCooldowns[enemy];
    }
}