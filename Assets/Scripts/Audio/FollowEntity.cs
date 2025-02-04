using UnityEngine;

public class FollowEntity : MonoBehaviour
{
    [SerializeField]
    public Transform entityTransform;

    private void LateUpdate()
    {
        transform.position = entityTransform.position;
        transform.rotation = Quaternion.identity; // Keep fixed orientation
    }
}