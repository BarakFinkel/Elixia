using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public string id;
    public bool activationStatus;
    private Animator anim => GetComponent<Animator>();

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
        id = Guid.NewGuid().ToString();
    }

    public void ActivateCheckpoint()
    {
        if (!activationStatus)
        {
            AudioManager.instance.PlaySFX(20, 0, PlayerManager.instance.player.transform);
        }

        activationStatus = true;
        anim.SetBool("active", true);
    }
}