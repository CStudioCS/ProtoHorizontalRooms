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
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float accelerationWeight = 10f;
    [SerializeField] private float frictionWeight = 8f;
    private Vector2 currentMoveInput;

    [Header("Ground Detection")]
    [SerializeField] private float rayLength = 0.3f;
    [SerializeField] private Vector2 boxSize = new Vector2(0.5f, 0.1f);
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private bool grounded;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float jumpReduction = .5f;
    [SerializeField] private float gravityMultiplier = .5f;
    [SerializeField] private float jumpBufferTimer = .15f;
    [SerializeField] private float coyoteTimer = .15f;
    private float jumpBufferCounter;
    private float coyoteCounter;

    private void Awake()
    {
        //à compléter
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
        currentMoveInput = moveAction.ReadValue<Vector2>();
        if (jumpAction.WasPressedThisFrame())
        {
            jumpBufferCounter = jumpBufferTimer;
        }
        else if (jumpAction.WasReleasedThisFrame() && rb.linearVelocityY > 0)
        {
            rb.linearVelocityY *= jumpReduction;
        }

        grounded = isGrounded();

        if (grounded)
        {
            coyoteCounter = coyoteTimer;
        }
        else if (coyoteCounter > 0)
        {
            coyoteCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        float weight = Mathf.Abs(currentMoveInput.x) > 0 ? accelerationWeight : frictionWeight;
        rb.linearVelocityX = Mathf.Lerp(rb.linearVelocityX, currentMoveInput.x * moveSpeed, weight);

        if (jumpBufferCounter > 0 && coyoteCounter > 0)
        {
            Jump();
        }
    }

    //public void OnMove(InputValue inputValue)
    //{
    //    currentMoveInput = inputValue.Get<Vector2>();
    //}

    //public void OnJump(InputValue inputValue)
    //{
    //    if (inputValue.isPressed)
    //    {
    //        jumpBufferCounter = jumpBufferTimer;
    //        Debug.Log("Jump pressed");
    //    }
    //    else
    //    {
    //        Debug.Log("Jump released");
    //    }
    //}

    public bool isGrounded()
    {
        return Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, rayLength, groundLayer);
    }

    void Jump()
    {
        rb.linearVelocityY = jumpForce;

        jumpBufferCounter = 0;
        coyoteCounter = 0;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position - transform.up * rayLength, boxSize);
    }

}
