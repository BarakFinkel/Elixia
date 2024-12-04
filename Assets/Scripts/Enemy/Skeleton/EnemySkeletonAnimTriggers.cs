using UnityEngine;

public class EnemySkeletonAnimTriggers : MonoBehaviour
{
    private Enemy_Skeleton enemy => GetComponentInParent<Enemy_Skeleton>();

    private void AnimationTrigger()
    {
        enemy.AnimationFinishTrigger();
    }

    private void AttackTrigger()
    {
        var colliders = Physics2D.OverlapCircleAll(enemy.attackCheck.position, enemy.attackCheckRadius);

        foreach (var hit in colliders)
            if (hit.GetComponent<Player>() != null)
            {
                hit.GetComponent<Player>().Damage();
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