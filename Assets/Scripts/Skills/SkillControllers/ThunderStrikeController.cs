using UnityEngine;

public class ThunderStrikeController : MonoBehaviour
{
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            var playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
            var enemyTarget = collision.GetComponent<EnemyStats>();
            playerStats.DoMagicalDamage(enemyTarget);
        }
    }
}