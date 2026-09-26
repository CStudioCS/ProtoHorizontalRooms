using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform pointA;
    public Transform pointB;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    private Vector3 nextTarget;

    [Header("Lift Boost")]
    public Vector2 liftVelocity { get; private set; }
    public float coyoteLiftTimer = 0.5f;
    private float coyoteLiftCounter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextTarget = pointB.position;
    }

    void FixedUpdate()
    {
        Vector3 previousPosition = transform.position;
        MovePlatform();
        ComputeLiftVelocity(previousPosition);
    }

    void MovePlatform()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextTarget, moveSpeed * Time.fixedDeltaTime);

        if (Vector3.Distance(transform.position, nextTarget) < 0.1f)
        {
            nextTarget = nextTarget == pointA.position ? pointB.position : pointA.position;
        }
    }

    void ComputeLiftVelocity(Vector3 previousPosition)
    {
        Vector2 platformVelocity = (transform.position - previousPosition) / Time.fixedDeltaTime;

        if(platformVelocity.sqrMagnitude > .1f)
        {
            liftVelocity = platformVelocity;
            coyoteLiftCounter = coyoteLiftTimer;
        }
        else
        {
            coyoteLiftCounter -= Time.fixedDeltaTime;
            if (coyoteLiftCounter <= 0)
            {
                liftVelocity = Vector2.zero;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
