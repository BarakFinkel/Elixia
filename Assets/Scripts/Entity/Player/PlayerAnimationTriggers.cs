using UnityEngine;

// Component used on the player's animator object in order to stop animations when finished if necessary.
public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player player => GetComponentInParent<Player>();

    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }

    private void AttackTrigger()
    {
        // All enemies within the attack range
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                EnemyStats target = hit.GetComponent<EnemyStats>();
                if (target != null)
                {
                    player.cs.DoDamage(target);
                }

                ItemData_Equipment weaponData = Inventory.instance.GetEquipmentOfType(EquipmentType.Weapon);
                if (weaponData != null)
                {
                    weaponData.Effect(target.transform);
                }
            }
        }
    }

    private void HealPlayer()
    {
        var syringe = Inventory.instance.CanUseSyringe();
        if (syringe != null)
        {
            Inventory.instance.UseSyringe(syringe);
        }
    }

    private void ThrowSword()
    {
        SkillManager.instance.sword.CreateSword();
    }

    private void ThrowPotion()
    {
        SkillManager.instance.potion.CreatePotion();
    }
}