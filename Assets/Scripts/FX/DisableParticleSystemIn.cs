using UnityEngine;

public class DisableParticleSystemIn : MonoBehaviour
{
    [SerializeField]
    private float duration = 5.0f;

    private ParticleSystem system;
    private float timer;

    private void Start()
    {
        system = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= duration)
        {
            system.Stop();
        }
    }
}