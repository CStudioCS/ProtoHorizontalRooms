using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public bool IsPlayer1;
    public Player OtherPlayer;
    public Ability CurrentAbility;

    [SerializeField] private Rigidbody2D rb;

    [Header("Controls")]
    [SerializeField] private InputAction moveAction;
    [SerializeField] private InputAction jumpAction;

    [Header("Horizontal Movement")]
    public float moveSpeed = 5f;
    public float accelerationWeight = 10f;
    public float frictionWeight = 8f;
    private float currentMoveInput;

    [Header("Ground Detection")]
    [SerializeField] private float rayLength = 0.3f;
    [SerializeField] private Vector2 boxSize = new Vector2(0.5f, 0.1f);
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private bool grounded;

    [Header("Jumping")]
    public float jumpForce = 15f;
    [SerializeField] private float jumpReduction = .5f;
    [SerializeField] private float jumpBufferTimer = .15f;
    [SerializeField] private float coyoteTimer = .15f;
    private float jumpBufferCounter, coyoteCounter;
    private bool apexed;

    [Header("Falling")]
    public int gravityModifier = 1;
    public float gravity = 40f;
    [SerializeField] private float gravityApexMultiplier = .5f;
    [SerializeField] private float gravityFallMultiplier = 2f;

    [Header("Visuals")]
    [SerializeField] private GameObject sprite;
    [SerializeField] private Vector2 stretch = new Vector2(.5f, 1f);
    [SerializeField] private Vector2 squash = new Vector2(1.5f, .5f);

    private void Awake()
    {
        rb.gravityScale = 0;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
    }

    private void Update()
    {
        bool prevGrounded = grounded;
        currentMoveInput = moveAction.ReadValue<float>();

        grounded = isGrounded();

        if (jumpAction.WasPressedThisFrame())
        {
            jumpBufferCounter = jumpBufferTimer;
        }
        else if (jumpAction.WasReleasedThisFrame() && rb.linearVelocityY * gravityModifier > 0)
        {
            rb.linearVelocityY *= jumpReduction;
        }

        if (grounded)
        {
            coyoteCounter = coyoteTimer;
            apexed = false;
        }
        else if (coyoteCounter > 0)
        {
            coyoteCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Handle Gravity
        if (Mathf.Abs(rb.linearVelocityY) < 0.5f && !grounded && !apexed)
        {
            rb.linearVelocityY -= gravity * gravityApexMultiplier * gravityModifier * Time.deltaTime;
            apexed = true;
        }
        else if (apexed && rb.linearVelocityY * gravityModifier < -0.5f)
        {
            rb.linearVelocityY -= gravity * gravityFallMultiplier * gravityModifier * Time.deltaTime;
        }
        else if (!grounded)
        {
            rb.linearVelocityY -= gravity * gravityModifier * Time.deltaTime;
        }

        if (!prevGrounded && grounded)
        {
            sprite.transform.localScale = squash;
        }
        sprite.transform.localScale = Vector2.Lerp(sprite.transform.localScale, new Vector2(1f, 1f), 10 * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        float weight = Mathf.Abs(currentMoveInput) > 0 ? accelerationWeight : frictionWeight;
        rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocityX, currentMoveInput * moveSpeed, weight * Time.fixedDeltaTime);

        if (jumpBufferCounter > 0 && coyoteCounter > 0)
        {
            Jump();
        }
    }

    public bool isGrounded()
    {
        return Physics2D.BoxCast(transform.position, boxSize, 0, -gravityModifier * transform.up, rayLength, groundLayer);
    }

    void Jump()
    {
        rb.linearVelocityY = jumpForce * gravityModifier;

        jumpBufferCounter = 0;
        coyoteCounter = 0;
        sprite.transform.localScale = stretch;
    }

    public void SetInvertedGravity(bool inverted)
    {
        gravityModifier = inverted ? -1 : 1;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position - gravityModifier * transform.up * rayLength, boxSize);
    }
}
