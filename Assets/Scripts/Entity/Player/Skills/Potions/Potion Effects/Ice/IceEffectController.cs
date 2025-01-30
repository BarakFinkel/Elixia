using UnityEngine;

public class IceEffectController : MonoBehaviour
{
    private CircleCollider2D cd;
    private float enemyFreezeTime;

    public void SetupIceBlast(float _enemyFreezeTime)
    {
        enemyFreezeTime = _enemyFreezeTime;
        cd = GetComponent<CircleCollider2D>();

        AudioManager.instance.PlaySFX(27, 0, null);
    }

    public void IceBlastDamage()
    {
        // All enemies within the attack range
        var colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);
        foreach (var hit in colliders)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.FreezeTimeFor(enemyFreezeTime);
                PlayerManager.instance.player.cs.DoMagicalDamage(hit.GetComponent<CharacterStats>());
            }
        }
    }

    public void SpawnIceShards()
    {
        PotionEffectManager.instance.ice.CreateIceShards(gameObject.transform);
    }

    public void DestroyBlast()
    {
        Destroy(gameObject);
    }
}
