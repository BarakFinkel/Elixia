using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private float parralaxEffect;
    private GameObject cam;
    private float length;

    private float xPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera");

        length = GetComponent<SpriteRenderer>().bounds.size.x;
        xPos = transform.position.x;
    }

    // Update is called once per frame
    private void Update()
    {
        var distanceMoved = cam.transform.position.x * (1 - parralaxEffect);
        var distanceToMove = cam.transform.position.x * parralaxEffect;

        transform.position = new Vector3(xPos + distanceToMove, transform.position.y);

        if (distanceMoved > xPos + length)
        {
            xPos += length;
        }
        else if (distanceMoved < xPos - length)
        {
            xPos -= length;
        }
    }
}