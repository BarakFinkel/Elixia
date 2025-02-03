using UnityEngine;

public class BombController : MonoBehaviour
{
    private CharacterStats goblinStats;
    private CircleCollider2D cd;
    private float growScale;
    private float growSpeed;
    private float damageMultiplier;
    private bool bombTriggered = false;

    public void SetupBomb(float _growScale, float _growSpeed, float _damageMultiplier, CharacterStats _goblinStats)
    {
        cd = GetComponent<CircleCollider2D>();
        
        growScale = _growScale;
        growSpeed = _growSpeed;
        damageMultiplier = _damageMultiplier;
        goblinStats = _goblinStats;

        int addedDamage = Mathf.RoundToInt(goblinStats.damage.GetValue() * (damageMultiplier - 1));
        goblinStats.damage.AddModifier(addedDamage);
    }

    private void Update()
    {
        // If the bomb was triggered, it hits targets once and then makes them invulnerable for 1.5f seconds (done to give recovery time, time period won't change).
        if (bombTriggered)
        {
            // Increase size
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(growScale, growScale), growSpeed * Time.deltaTime);

            // Hit enemies caught within the blast radius
            Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, cd.radius);
            foreach (var hit in colliders)
            {
                if (hit.GetComponent<Player>() != null)
                {
                    goblinStats.DoDamage(hit.GetComponent<PlayerStats>());
                    hit.GetComponent<PlayerStats>().EnableInvulnerabilityFor(1.0f);
                }

                if (hit.GetComponent<Enemy>() != null)
                {
                    goblinStats.DoDamage(hit.GetComponent<EnemyStats>());
                    hit.GetComponent<EnemyStats>().EnableInvulnerabilityFor(1.0f);
                }
            }
        }
    }

    public void TriggerBomb()
    {
        bombTriggered = true;
    }

    public void TriggerDestruction()
    {
        Destroy(gameObject);
    }
}