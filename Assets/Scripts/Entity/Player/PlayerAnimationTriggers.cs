using UnityEngine;

// Component used on the player's animator object in order to stop animations when finished if necessary.
public class PlayerAnimationTriggers : MonoBehaviour
{
    private bool playSoundEffect = true;
    private Player player => GetComponentInParent<Player>();

    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }

    private void AttackTrigger()
    {
        AudioManager.instance.PlaySFX(13, 0.0f, null); // attack sfx

        // All enemies within the attack range
        var colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
            if (hit.GetComponent<Enemy>() != null)
            {
                var target = hit.GetComponent<EnemyStats>();
                if (target != null)
                {
                    player.cs.DoDamage(target);

                    if (playSoundEffect)
                    {
                        playSoundEffect = false;
                    }
                }

                var weaponData = Inventory.instance.GetEquipmentOfType(EquipmentType.Weapon);
                if (weaponData != null)
                {
                    weaponData.Effect(target.transform);
                }
            }

        playSoundEffect = true;
    }

    private void StepSound1()
    {
        AudioManager.instance.PlaySFX(18, 0, null);
    }

    private void StepSound2()
    {
        AudioManager.instance.PlaySFX(19, 0, null);
    }

    private void PlayHealSound()
    {
        AudioManager.instance.PlaySFX(30, 0, null);
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