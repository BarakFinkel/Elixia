using UnityEngine;

public class LostCurrencyController : MonoBehaviour
{
    private SpriteRenderer sr => GetComponent<SpriteRenderer>();
    public int currency;
    public bool isFading;
    public float elapsedTime = 0;
    public float fadeDuration = 1.0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() != null && !isFading)
        {
            AudioManager.instance.PlaySFX(22, 0, null);
            PlayerManager.instance.currency += currency;
            isFading = true;
        }
    }

    private void Update()
    {
        HandleFade();
    }

    private void HandleFade()
    {
        if (isFading && sr != null)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;

            // Lerp the alpha value
            Color currentColor = sr.color;
            float newAlpha = Mathf.Lerp(currentColor.a, 0f, t);
            sr.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);

            // Stop fading when done
            if (t >= 1f)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
