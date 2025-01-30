using System.Collections;
using UnityEngine;

public class PotionSkillController : MonoBehaviour
{
    private bool canBreak = true;
    private PolygonCollider2D cd;
    private Player player;
    private BasePotionEffect potionEffect;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<PolygonCollider2D>();
        player = PlayerManager.instance.player;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && canBreak)
        {
            StartCoroutine("breakPotion", SkillManager.instance.potion.timeTillDelete);
        }
    }

    public void SetupPotion(Vector2 _throwDirection, float _gravityScale, BasePotionEffect _potionEffect)
    {
        potionEffect = _potionEffect;
        rb.linearVelocity = new Vector2(_throwDirection.x, _throwDirection.y);
        rb.gravityScale = _gravityScale;
        rb.AddTorque(1 - _throwDirection.normalized.y,
            ForceMode2D.Impulse); // Sets the spin power of the potion based on the normalized y force value.
    }

    private IEnumerator breakPotion(float deleteTime)
    {
        canBreak = false;

        sr.sprite = SkillManager.instance.potion.brokenPotion;
        rb.linearVelocity = new Vector2(0, 0);
        AudioManager.instance.PlaySFX(26, 0, null);

        potionEffect.ActivatePotionEffect(gameObject);

        yield return new WaitForSeconds(deleteTime);

        Destroy(gameObject);
    }
}