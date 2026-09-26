using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public bool IsPlayer1;
    public Player OtherPlayer;

    [SerializeField] private Rigidbody2D rb;

    [Header("Controls")]
    [SerializeField] private InputAction moveAction;
    [SerializeField] private InputAction jumpAction;
    [SerializeField] private InputAction useAbilityAction;

    [Header("Ability System")]
    public Ability currentAbility; // Is not seen by the inspector ?

    [Header("Horizontal Movement")]
    public float moveSpeed = 5f;
    public float accelerationWeight = 10f;
    public float frictionWeight = 8f;
    private float currentMoveInput;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 boxSize = new Vector2(0.5f, 0.1f);
    [SerializeField] private LayerMask groundLayer;
    private Vector3 initialGroundCheckPosition;
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
    [SerializeField] private GameObject visuals;
    [SerializeField] private GameObject sprite;
    [SerializeField] private Vector2 stretch = new Vector2(.5f, 1f);
    [SerializeField] private Vector2 squash = new Vector2(1.5f, .5f);
    private Vector2 originalScale;

    private void Awake()
    {
        rb.gravityScale = 0;
        originalScale = sprite.transform.localScale;
        initialGroundCheckPosition = groundCheck.localPosition;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        useAbilityAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        useAbilityAction.Disable();
    }

    private void Update()
    {
        bool prevGrounded = grounded;
        currentMoveInput = moveAction.ReadValue<float>();

        grounded = isGrounded();

        // Jump Input
        if (jumpAction.WasPressedThisFrame())
        {
            jumpBufferCounter = jumpBufferTimer;
        }
        else if (jumpAction.WasReleasedThisFrame() && rb.linearVelocityY * gravityModifier > 0)
        {
            rb.linearVelocityY *= jumpReduction;
        }

        if(useAbilityAction.WasPressedThisFrame() && currentAbility != null)
        {
            currentAbility.Use(this);
        }

        // Jump Buffer and Coyote Time
        if (grounded)
        {
            coyoteCounter = coyoteTimer;
            apexed = false;
        }
        coyoteCounter -= Time.deltaTime;
        jumpBufferCounter -= Time.deltaTime;

        if (!prevGrounded && grounded) sprite.transform.localScale = originalScale * squash;
        sprite.transform.localScale = Vector2.Lerp(sprite.transform.localScale, originalScale, 10 * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        float weight = Mathf.Abs(currentMoveInput) > 0 ? accelerationWeight : frictionWeight;
        rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocityX, currentMoveInput * moveSpeed, weight * Time.fixedDeltaTime);

        // Handle Gravity
        if (!isGrounded()) rb.linearVelocityY -= getTotalGravity() * Time.fixedDeltaTime;

        if (jumpBufferCounter > 0 && coyoteCounter > 0)
        {
            Jump();
        }
    }

    public bool isGrounded()
    {
        return Physics2D.OverlapBox(groundCheck.position, boxSize, 0, groundLayer);
    }

    public float getTotalGravity()
    {
        float totalGravity = gravity * gravityModifier;
        if (Mathf.Abs(rb.linearVelocityY) < 0.5f && !grounded && !apexed)
        {
            totalGravity = gravity * gravityApexMultiplier * gravityModifier;
        }
        else if (apexed && rb.linearVelocityY * gravityModifier < -0.5f)
        {
            totalGravity = gravity * gravityFallMultiplier * gravityModifier;
        }
        return totalGravity;
    }

    void Jump()
    {
        rb.linearVelocityY = jumpForce * gravityModifier;

        jumpBufferCounter = 0;
        coyoteCounter = 0;
        sprite.transform.localScale = originalScale * stretch;
    }

    public void SetInvertedGravity(bool inverted)
    {
        gravityModifier = inverted ? -1 : 1;
        groundCheck.localPosition = new Vector3(initialGroundCheckPosition.x, initialGroundCheckPosition.y * gravityModifier, initialGroundCheckPosition.z);
        visuals.transform.localScale = new Vector3(1, gravityModifier, 1);
    }

    void OnDrawGizmos()
    {
        if(groundCheck)Gizmos.DrawCube(groundCheck.position, boxSize);
    }
}
