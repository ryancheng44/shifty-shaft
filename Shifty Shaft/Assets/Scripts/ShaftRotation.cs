using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShaftRotation : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float angleThreshold;

    private Rigidbody rb;
    private WaitForFixedUpdate waitForFixedUpdate;

    // Start is called before the first frame update
    void Start() => rb = GetComponent<Rigidbody>();

    public IEnumerator Rotate(Direction directionToRotate)
    {
        Quaternion targetRotation = rb.rotation;

        switch (directionToRotate)
        {
            case Direction.Left:
                targetRotation.eulerAngles += new Vector3(0f, 0f, -90f);
                break;
            case Direction.Right:
                targetRotation.eulerAngles += new Vector3(0f, 0f, 90f);
                break;
        }

        while (Quaternion.Angle(rb.rotation, targetRotation) > angleThreshold)
        {
            yield return waitForFixedUpdate;
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRotation, speed * Time.fixedDeltaTime));
        }
    }
}
