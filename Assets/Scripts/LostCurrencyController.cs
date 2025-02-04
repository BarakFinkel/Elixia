using UnityEngine;

public class LostCurrencyController : MonoBehaviour
{
    public int currency;
    public bool isFading;
    public float elapsedTime;
    public float fadeDuration = 1.0f;
    private SpriteRenderer sr => GetComponent<SpriteRenderer>();

    private void Update()
    {
        HandleFade();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() != null && !isFading)
        {
            AudioManager.instance.PlaySFX(22, 0, null);
            PlayerManager.instance.currency += currency;
            isFading = true;
        }
    }

    private void HandleFade()
    {
        if (isFading && sr != null)
        {
            elapsedTime += Time.deltaTime;
            var t = elapsedTime / fadeDuration;

            // Lerp the alpha value
            var currentColor = sr.color;
            var newAlpha = Mathf.Lerp(currentColor.a, 0f, t);
            sr.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);

            // Stop fading when done
            if (t >= 1f)
            {
                Destroy(gameObject);
            }
        }
    }
}