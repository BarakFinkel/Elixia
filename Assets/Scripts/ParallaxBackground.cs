using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private GameObject cam;
    
    [SerializeField] private float parralaxEffect;
    
    private float xPos;
    private float length;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        xPos = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceMoved = cam.transform.position.x * (1-parralaxEffect);
        float distanceToMove = cam.transform.position.x * parralaxEffect;
        
        transform.position = new Vector3(xPos + distanceToMove, transform.position.y);
        
        if(distanceMoved > xPos + length)
            xPos += length;
        else if(distanceMoved < xPos - length)
            xPos -= length;
    }
}
