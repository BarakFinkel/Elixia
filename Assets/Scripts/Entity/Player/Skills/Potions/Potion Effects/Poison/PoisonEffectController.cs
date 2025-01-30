using UnityEngine;

public class PoisonEffectController : MonoBehaviour
{
    private Transform colliderObject;
    private float emissionDuration;
    private float endScale;
    private bool isScaling;
    private float scalingDuration;
    private float startScale;
    private ParticleSystem system;

    private float timer;
    private float totalDuration;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= totalDuration)
        {
            Destroy(gameObject);
        }

        if (system != null && timer >= emissionDuration)
        {
            system.Stop();
        }

        UpdateScaling();
    }

    public void SetupPoisonCloud(float _totalDuration, float _emissionDuration, float _startScale, float _endScale,
        float _scalingDuration)
    {
        emissionDuration = _emissionDuration;
        totalDuration = _totalDuration;
        startScale = _startScale;
        endScale = _endScale;
        scalingDuration = _scalingDuration;
        system = GetComponent<ParticleSystem>();
        colliderObject = transform.GetChild(0);
        isScaling = true;
        AudioManager.instance.PlaySFX(3, 0, PlayerManager.instance.player.transform);

        if (colliderObject != null)
        {
            colliderObject.localScale = Vector3.one * startScale;
        }
    }

    public void UpdateScaling()
    {
        if (isScaling)
        {
            // Calculate the current scale using Lerp.
            var t = Mathf.Clamp01(timer / scalingDuration);
            var currentScale = Mathf.Lerp(startScale, endScale, t);

            // Apply the scale to the object.
            colliderObject.localScale = Vector3.one * currentScale;

            // Stop scaling once the duration is reached.
            if (t >= 1f)
            {
                isScaling = false;
            }
        }
    }
}