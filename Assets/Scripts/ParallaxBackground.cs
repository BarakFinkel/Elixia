using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField]
    private float parralaxEffect;

    private GameObject cam;
    private float length;

    private float xPos;
    private float yPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera");

        length = GetComponent<SpriteRenderer>().bounds.size.x;
        xPos = transform.position.x;
        yPos = transform.position.y; // Store initial y-position
    }

    // Update is called once per frame
    private void Update()
    {
        // Calculate movement on both axes
        var distanceMovedX = cam.transform.position.x * (1 - parralaxEffect);
        var distanceToMoveX = cam.transform.position.x * parralaxEffect;

        var distanceMovedY = cam.transform.position.y * (1 - parralaxEffect);
        var distanceToMoveY = cam.transform.position.y * parralaxEffect;

        // Update position
        transform.position = new Vector3(xPos + distanceToMoveX, yPos + distanceToMoveY, transform.position.z);

        // Handle infinite scrolling (x-axis only, assuming y-axis is static for scrolling backgrounds)
        if (distanceMovedX > xPos + length)
        {
            xPos += length;
        }
        else if (distanceMovedX < xPos - length)
        {
            xPos -= length;
        }
    }
}