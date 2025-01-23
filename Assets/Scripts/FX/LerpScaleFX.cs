using UnityEngine;

public class LerpScaleFX : MonoBehaviour
{
    public Transform colliderObject; // The object to scale.
    public float startScale = 0.5f; // Starting scale.
    public float endScale = 1f; // Target scale.
    public float duration = 3f; // Duration of the lerp in seconds.

    private float elapsedTime; // To calculate the scale needed
    private bool isScaling;

    private void Start()
    {
        StartScaling(startScale, endScale, duration);
    }

    private void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() != null)
        {
            Debug.Log("Touched poison!");
        }
    }

    public void UpdateScaling()
    {
        if (isScaling)
        {
            // Increment elapsed time.
            elapsedTime += Time.deltaTime;

            // Calculate the current scale using Lerp.
            var t = Mathf.Clamp01(elapsedTime / duration);
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

    // Start scaling the object.
    public void StartScaling(float from, float to, float time)
    {
        startScale = from;
        endScale = to;
        duration = time;
        elapsedTime = 0f;
        isScaling = true;

        if (colliderObject != null)
        {
            colliderObject.localScale = Vector3.one * startScale;
        }
    }
}