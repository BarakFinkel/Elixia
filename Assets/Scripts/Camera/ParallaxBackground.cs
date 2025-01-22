using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private GameObject cam;
    private float startPosition;
    private float length;

    [SerializeField] private float parallaxEffect = 1.0f;

    // Will get the camera and it's position on the x-axis.
    void Start()
    {
        cam = GameObject.Find("Main Camera");

        length = GetComponent<SpriteRenderer>().bounds.size.x;

        startPosition = transform.position.x;
    }

    // Will update the camera's x-placement according to the desired parallax effect.
    void Update()
    {
        float distanceToMove = cam.transform.position.x * parallaxEffect; // Calculating how much to move the background.
        float distanceMoved = cam.transform.position.x * (1 - parallaxEffect); // Calculating how much distance was crossed till now.

        transform.position = new Vector3(startPosition + distanceToMove, transform.position.y); // Moving the background's placement as required.

        // The following takes care of an endless background effect, just in case if ever relevant.
        // If we moved the whole distance of the background rightwards, we place it just after it was supposed to end.
        if (distanceMoved > startPosition + length)
        {
            startPosition += length;
        }
        // Likewise leftwards.
        else if (distanceMoved < startPosition - length)
        {
            startPosition -= length;
        }
    }
}
