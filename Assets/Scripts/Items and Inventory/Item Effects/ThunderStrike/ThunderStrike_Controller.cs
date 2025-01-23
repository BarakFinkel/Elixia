using UnityEngine;

public class ThunderStrike_Controller : MonoBehaviour
{
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            var playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
            var enemyStats = collision.GetComponent<EnemyStats>();

            playerStats.DoMagicalDamage(enemyStats);
        }
    }
}