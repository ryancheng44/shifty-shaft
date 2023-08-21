using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private ShaftRotation shaftRotation;
    [SerializeField] private float swipeThreshold;
    [SerializeField] private float jumpBufferTime;
    [SerializeField] private float jumpForce;

    private Rigidbody rb;
    private TrailRenderer trailRenderer;

    private bool onMac;

    private Vector2 touchBegan;
    private Vector2 touchMoved;
    private bool swipeRegistered;

    private float jumpBufferTimer;
    private Direction directionToRotate;
    private bool isGrounded;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();

        // Change to OSXPlayer before release
        if (Application.platform == RuntimePlatform.OSXEditor)
            onMac = true;
    }

    // Update is called once per frame
    void Update()
    {
        jumpBufferTimer -= Time.deltaTime;

        if (onMac)
        {
            if (Input.GetKeyDown("a"))
            {
                if (!GameManager.instance.isGameInProgress)
                    GameManager.instance.GameInProgress();

                jumpBufferTimer = jumpBufferTime;
                directionToRotate = Direction.Left;
            } else if (Input.GetKeyDown("d"))
            {
                if (!GameManager.instance.isGameInProgress)
                    GameManager.instance.GameInProgress();

                jumpBufferTimer = jumpBufferTime;
                directionToRotate = Direction.Right;
            }
        } else
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    swipeRegistered = false;
                    touchBegan = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved && !swipeRegistered)
                {
                    touchMoved = touch.position;

                    if (Mathf.Abs(touchMoved.x - touchBegan.x) >= swipeThreshold)
                    {
                        if (!GameManager.instance.isGameInProgress)
                            GameManager.instance.GameInProgress();

                        swipeRegistered = true;
                        jumpBufferTimer = jumpBufferTime;

                        if (touchMoved.x - touchBegan.x < 0)
                            directionToRotate = Direction.Left;
                        else
                            directionToRotate = Direction.Right;
                    }
                }
            }
        }

        if (jumpBufferTimer > 0f && isGrounded && !GameManager.instance.isGameOver)
        {
            jumpBufferTimer = 0f;

            Jump();
            StartCoroutine(shaftRotation.Rotate(directionToRotate));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name != "Shaft")
            return;

        isGrounded = true;
        trailRenderer.emitting = true;
    }

    private void Jump()
    {
        rb.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
        isGrounded = false;

        trailRenderer.emitting = false;
        trailRenderer.Clear();
    }
}

public enum Direction
{
    Left,
    Right
}
