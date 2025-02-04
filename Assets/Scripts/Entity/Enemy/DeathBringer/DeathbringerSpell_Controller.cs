using UnityEngine;

public class DeathbringerSpell_Controller : MonoBehaviour
{
    [SerializeField]
    private Transform check;

    [SerializeField]
    private Vector2 boxSize;

    [SerializeField]
    private LayerMask whatIsPlayer;

    private CharacterStats myStats;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(check.position, boxSize);
    }

    public void SetupSpell(CharacterStats _stats)
    {
        myStats = _stats;
    }


    private void AnimationTrigger()
    {
        var colliders = Physics2D.OverlapBoxAll(check.position, boxSize, whatIsPlayer);

        foreach (var hit in colliders)
            if (hit.GetComponent<Player>() != null)
            {
                myStats.DoDamage(hit.GetComponent<CharacterStats>());
            }
    }

    private void SFXTrigger()
    {
        AudioManager.instance.PlaySFX(49, 0, null);
    }

    private void SelfDestroy()
    {
        Destroy(gameObject);
    }
}