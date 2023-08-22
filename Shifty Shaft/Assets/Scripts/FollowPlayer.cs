using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;

    private Vector3 targetPosition;
    private float offset;

    // Start is called before the first frame update
    void Start()
    {
        targetPosition = transform.position;
        offset = transform.position.z - player.position.z;
    }

    private void LateUpdate() {
        targetPosition.z = player.position.z + offset;
        transform.position = targetPosition;
    }
}