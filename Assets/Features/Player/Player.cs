using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public bool IsPlayer1;
    public bool GodMode = false;
    public Player OtherPlayer;

    public Rigidbody2D rb;

    [Header("Controls")]
    [SerializeField] private InputAction moveAction;
    [SerializeField] private InputAction jumpAction;
    [SerializeField] private InputAction crouchAction;
    [SerializeField] private InputAction useAbilityAction;

    [Header("Ability System")]
    public Ability currentAbility = new DashTest(); // Is not seen by the inspector ?

    [Header("Abilities")]
    public float dashSpeed = 100f;

    [Header("Horizontal Movement")]
    public float moveSpeed = 7f;
    public float groundAcceleration = 150f;
    public float groundFriction = 30f;
    public float airAcceleration = 150f;
    public float airFriction = 30f;
    public bool isFacingRight = true;
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
    [SerializeField] private float maxFallSpeed = 2f;

    [Header("Visuals")]
    [SerializeField] private GameObject visuals;
    [SerializeField] private GameObject sprite;
    [SerializeField] private ParticleSystem dust;
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
        crouchAction.Enable();
        useAbilityAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        crouchAction.Disable();
        useAbilityAction.Disable();
    }

    private void Update()
    {
        currentMoveInput = moveAction.ReadValue<float>();
        if(currentMoveInput > 0 && !isFacingRight) HorizontalFlip();
        else if (currentMoveInput < 0 && isFacingRight) HorizontalFlip();

        if (GodMode) return;
        
        bool prevGrounded = grounded;
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

        if(currentAbility != null)
        {
            currentAbility.OnUpdate(this);
            if (useAbilityAction.WasPressedThisFrame()) currentAbility.OnUse(this);
        }

        // Jump Buffer and Coyote Time
        if (grounded)
        {
            coyoteCounter = coyoteTimer;
            apexed = false;
        }
        coyoteCounter -= Time.deltaTime;
        jumpBufferCounter -= Time.deltaTime;

        if (!prevGrounded && grounded) Land();
        sprite.transform.localScale = Vector2.Lerp(sprite.transform.localScale, originalScale, 10 * Time.deltaTime);
        visuals.transform.localRotation = Quaternion.Lerp(visuals.transform.localRotation, Quaternion.Euler(0, 0, rb.linearVelocityX * gravityModifier), 10 * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        // Handle Horizontal Movement
        if(isGrounded()) rb.linearVelocityX = ComputeHorizontalVelocity(groundAcceleration, groundFriction);
        else rb.linearVelocityX = ComputeHorizontalVelocity(airAcceleration, airFriction);

        if(GodMode)
        {
            rb.linearVelocityY = (-crouchAction.ReadValue<float>() + jumpAction.ReadValue<float>()) * moveSpeed;
            return;
        }
        // Handle Gravity
        //if (!isGrounded())
        {
            rb.linearVelocityY -= getTotalGravity() * Time.fixedDeltaTime;
            if (rb.linearVelocityY * gravityModifier < -maxFallSpeed) rb.linearVelocityY = -gravityModifier * maxFallSpeed;
        }

        if (jumpBufferCounter > 0 && coyoteCounter > 0)
        {
            Jump();
        }
    }

    float ComputeHorizontalVelocity(float acceleration, float friction)
    {
        float newVelocity = rb.linearVelocityX;
        if (currentMoveInput != 0)
        {
            float targetVelocityX = currentMoveInput * moveSpeed;
            newVelocity = Mathf.MoveTowards(newVelocity, targetVelocityX, acceleration * Time.fixedDeltaTime);
        }

        newVelocity = Mathf.MoveTowards(newVelocity, 0, friction * Time.fixedDeltaTime);
        return newVelocity;
    }

    public bool isGrounded()
    {
        return Physics2D.OverlapBox(groundCheck.position, boxSize, 0, groundLayer);
    }

    public float getTotalGravity()
    {
        float totalGravity = gravity * gravityModifier;
        if (Mathf.Abs(rb.linearVelocityY) < 0.5f && !grounded)
        {
            totalGravity = gravity * gravityApexMultiplier * gravityModifier;
            apexed = true;
        }
        else if (apexed && rb.linearVelocityY * gravityModifier < -0.5f)
        {
            totalGravity = gravity * gravityFallMultiplier * gravityModifier;
        }
        return totalGravity;
    }

    public void AddSpeed(float x, float y)
    {
        rb.linearVelocityX += x;
        rb.linearVelocityY += y;
    }

    void Jump()
    {
        rb.linearVelocityY = jumpForce * gravityModifier;

        jumpBufferCounter = 0;
        coyoteCounter = 0;
        sprite.transform.localScale = originalScale * stretch;
        dust.Play();
    }

    void HorizontalFlip()
    {
        isFacingRight = currentMoveInput >= 0;
        visuals.transform.localScale = new Vector3(isFacingRight ? 1 : -1, visuals.transform.localScale.y, visuals.transform.localScale.z);
        if(grounded) dust.Play();
    }

    void Land()
    {
        rb.linearVelocityY = 0f;
        sprite.transform.localScale = originalScale * squash;
        dust.Play();
    }

    public void SetInvertedGravity(bool inverted)
    {
        gravityModifier = inverted ? -1 : 1;
        groundCheck.localPosition = new Vector3(initialGroundCheckPosition.x, initialGroundCheckPosition.y * gravityModifier, initialGroundCheckPosition.z);
        visuals.transform.localScale = new Vector3(visuals.transform.localScale.x, gravityModifier, visuals.transform.localScale.z);
    }

    void OnDrawGizmos()
    {
        if(groundCheck)Gizmos.DrawCube(groundCheck.position, boxSize);
    }
}
