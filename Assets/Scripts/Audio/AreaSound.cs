using UnityEngine;

public class AreaSound : MonoBehaviour
{
    [SerializeField] private int areaSoundIndex;
    private bool isInside = false;

    

    private void OnTriggerStay2D(Collider2D other)
    {
        if(!isInside && other.gameObject.GetComponent<Player>() != null)
        {
            AudioManager.instance.PlaySFX(areaSoundIndex, 0, null);
            isInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>() != null)
        {
            AudioManager.instance.StopSFXWithTime(areaSoundIndex);
            isInside = false;
        }
    }
}