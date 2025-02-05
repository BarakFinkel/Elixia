using UnityEngine;

public class PopupTrigger : MonoBehaviour
{
    public GameObject popupWindow; // Assign in Inspector

    void Start()
    {
        popupWindow.SetActive(false); // Hide the popup initially
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            popupWindow.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            popupWindow.SetActive(false);
        }
    }
}