using UnityEngine;

public class PoisonEffectController : MonoBehaviour
{
    private float totalDuration;
    private float emissionDuration;
    private float startScale;
    private float endScale;
    private float scalingDuration;
    private ParticleSystem system;
    private Transform colliderObject;

    private float timer = 0f;
    private bool isScaling = false;

    public void SetupPoisonCloud(float _totalDuration, float _emissionDuration, float _startScale, float _endScale, float _scalingDuration)
    {
        emissionDuration = _emissionDuration;
        totalDuration = _totalDuration;
        startScale = _startScale;
        endScale = _endScale;
        scalingDuration = _scalingDuration;
        system = GetComponent<ParticleSystem>();
        colliderObject = transform.GetChild(0);
        isScaling = true;

        if (colliderObject != null)
        {
            colliderObject.localScale = Vector3.one * startScale;
        }
    }

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

    public void UpdateScaling()
    {
        if (isScaling)
        {
            // Calculate the current scale using Lerp.
            float t = Mathf.Clamp01(timer / scalingDuration);
            float currentScale = Mathf.Lerp(startScale, endScale, t);

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