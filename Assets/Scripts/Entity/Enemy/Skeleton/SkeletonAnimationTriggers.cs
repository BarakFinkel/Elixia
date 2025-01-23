using UnityEngine;

// Component used on the player's animator object in order to stop animations when finished if necessary.
public class SkeletonAnimationTriggers : MonoBehaviour
{
    private Skeleton enemy => GetComponentInParent<Skeleton>();

    private void AnimationTrigger()
    {
        enemy.AnimationTrigger();
    }

    private void AttackTrigger()
    {
        // All enemies within the attack range
        var colliders = Physics2D.OverlapCircleAll(enemy.attackCheck.position, enemy.attackCheckRadius);

        foreach (var hit in colliders)
            if (hit.GetComponent<Player>() != null)
            {
                var target = hit.GetComponent<PlayerStats>();
                enemy.cs.DoDamage(target);
            }
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