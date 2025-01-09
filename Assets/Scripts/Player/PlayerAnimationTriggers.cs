using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player player => GetComponentInParent<Player>();

    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }

    private void AttackTrigger()
    {
        var colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
            if (hit.GetComponent<Enemy>() != null)
            {
                var target = hit.GetComponent<EnemyStats>();

                if (target != null)
                {
                    player.stats.DoDamage(target);
                }

                var weaponData = Inventory.instance.GetEquipment(EquipmentType.Weapon);
                if (weaponData != null)
                {
                    weaponData.Effect(target.transform);
                }
            }
    }

    private void ThrowSword()
    {
        SkillManager.instance.sword.CreateSword();
    }
}