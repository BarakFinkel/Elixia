using UnityEngine;

[CreateAssetMenu(fileName = "Freeze Enemies Effect", menuName = "Data/Item Effect/Freeze Enemies Effect")]
public class FreezeEnemiesEffect : ItemEffect
{
    [SerializeField] private float freezeRadius = 2.0f;
    [SerializeField] private float freezeThreshold = 0.2f;
    [SerializeField] private float duration;

    public override void ExecuteEffect(Transform _transform)
    {
        PlayerStats stats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        
        // Bool that checks if the player is below the threshold for freezing enemies
        bool belowThreshold = stats.currentHealth <= stats.GetMaxHealthValue() * freezeThreshold;

        // If the player is below the freeze threshold and his armor's off cooldown, we can freeze all enemies in the given radius.
        if (belowThreshold && Inventory.instance.CanUseArmor())
        {
            // All enemies within the attack range
            Collider2D[] colliders = Physics2D.OverlapCircleAll(_transform.position, freezeRadius);

            foreach(var hit in colliders)
            {
                hit.GetComponent<Enemy>()?.FreezeTimeFor(duration);
            }
        }
    }
}
