using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Rigidbody player;

    private Vector3 targetPosition;
    private float offset;

    // Start is called before the first frame update
    void Start()
    {
        targetPosition = transform.position;
        offset = transform.position.z - player.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        targetPosition.z = player.position.z + offset;
        transform.position = targetPosition;
    }
}