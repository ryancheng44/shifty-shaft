using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private ObstacleSpawner obstacleSpawner;
    [SerializeField] private float startSpeed;
    [SerializeField] private float maxSpeed;

    private Rigidbody rb;
    private TrailRenderer trailRenderer;
    private ParticleSystem deathExplosion;

    private float currentSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();
        deathExplosion = GetComponentInChildren<ParticleSystem>(true);

        currentSpeed = startSpeed;
    }

    private void FixedUpdate()
    {
        if (!GameManager.instance.isGameInProgress || GameManager.instance.isGameOver)
            return;

        if (rb.position.z > 100f)
        {
            float playerPositionZ = rb.position.z;

            rb.MovePosition(new Vector3(0f, rb.position.y));

            for (int i = 0; i < trailRenderer.positionCount; i++)
            {
                Vector3 position = trailRenderer.GetPosition(i);
                position.z -= playerPositionZ;
                trailRenderer.SetPosition(i, position);
            }

            obstacleSpawner.FloatingOrigin(playerPositionZ);
        } else {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, currentSpeed);

            if (currentSpeed < maxSpeed)
                currentSpeed *= 1.0001f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "Shaft")
            return;

        OnDeath();
    }

    private void OnTriggerEnter(Collider other)
    {
        GameManager.instance.IncreaseScore();
        StartCoroutine(obstacleSpawner.Release(other.transform.parent.gameObject));
    }

    private void OnDeath()
    {
        SFXManager.instance.PlaySFX("Death");

        GameManager.instance.GameOver();

        trailRenderer.emitting = false;
        trailRenderer.Clear();

        GetComponent<Renderer>().enabled = false;

        ParticleSystem.MainModule main = deathExplosion.main;
        main.startSpeed = main.startSpeed.constant * currentSpeed / startSpeed;
        deathExplosion.gameObject.SetActive(true);
    }
}
