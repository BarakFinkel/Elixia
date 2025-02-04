using UnityEngine;

// Component used on the player's animator object in order to stop animations when finished if necessary.
public class EnemyAnimationTriggers : MonoBehaviour
{
    private Enemy enemy => GetComponentInParent<Enemy>();

    private void AnimationTrigger()
    {
        enemy.AnimationTrigger();
    }

    private void AttackTrigger()
    {
        AudioManager.instance.PlaySFX(enemy.attackSoundIndex, 0, null);

        // All enemies within the attack range
        var colliders = Physics2D.OverlapCircleAll(enemy.attackCheck.position, enemy.attackCheckRadius);

        foreach (var hit in colliders)
            if (hit.GetComponent<Player>() != null)
            {
                AudioManager.instance.PlaySFX(41, 0, null);
                var target = hit.GetComponent<PlayerStats>();
                enemy.cs.DoDamage(target);
            }
    }

    private void SpecialAttackTrigger()
    {
        enemy.AnimationSpecialAttackTrigger();
    }

    private void OpenCounterWindow()
    {
        enemy.OpenCounterAttackWindow();
    }

    private void CloseCounterWindow()
    {
        enemy.CloseCounterAttackWindow();
    }
}