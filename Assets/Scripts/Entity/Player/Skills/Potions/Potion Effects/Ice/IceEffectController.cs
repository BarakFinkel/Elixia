using UnityEngine;

public class IceEffectController : MonoBehaviour
{
    private CircleCollider2D cd;
    private int damage;
    private float enemyFreezeTime;

    public void SetupIceBlast(int _damage, float _enemyFreezeTime)
    {
        damage = _damage;
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
            var enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.SetupKnockbackDir(transform);

                enemy.FreezeTimeFor(enemyFreezeTime);
                PlayerManager.instance.player.cs.DoMagicalDamage(hit.GetComponent<CharacterStats>(), MagicType.Ice,
                    damage);
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