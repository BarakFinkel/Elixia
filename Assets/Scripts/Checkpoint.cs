using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();
    public string id;
    public bool activationStatus;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() != null)
        {
            ActivateCheckpoint();
        }
    }

    [ContextMenu("Generate Checkpoint ID")]
    private void GenerateID()
    {
        id = System.Guid.NewGuid().ToString();
    }

    public void ActivateCheckpoint()
    {
        activationStatus = true;
        anim.SetBool("active", true);
    }
}
