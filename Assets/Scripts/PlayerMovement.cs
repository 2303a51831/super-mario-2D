using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private Camera mainCamera;
    private Rigidbody2D rb;
    private Collider2D capsuleCollider;

    private Vector2 velocity;
    private float inputAxis;

    [Header("Movement Tuning")]
    public float moveSpeed = 8f;
    public float maxJumpHeight = 6.8f; // Increased for powerful, clearing jumps
    public float maxJumpTime = 0.85f;  // Snappy responsive jump duration
    public float jumpForce => (2f * maxJumpHeight) / (maxJumpTime / 2f);
    public float gravity => (-2f * maxJumpHeight) / Mathf.Pow(maxJumpTime / 2f, 2f);

    public bool grounded { get; private set; }
    public bool jumping { get; private set; }
    public bool running => Mathf.Abs(velocity.x) > 0.25f || Mathf.Abs(inputAxis) > 0.25f;
    public bool sliding => (inputAxis > 0f && velocity.x < 0f) || (inputAxis < 0f && velocity.x > 0f);
    public bool falling => velocity.y < 0f && !grounded;

    // Jump buffer & coyote time for responsive controls
    private float jumpBufferTimer = 0f;
    private float coyoteTimer = 0f;

    private void Awake()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        rb.isKinematic = false;
        capsuleCollider.enabled = true;
        velocity = Vector2.zero;
        jumping = false;
        jumpBufferTimer = 0f;
        coyoteTimer = 0f;
    }

    private void OnDisable()
    {
        rb.isKinematic = true;
        capsuleCollider.enabled = false;
        velocity = Vector2.zero;
        inputAxis = 0f;
        jumping = false;
    }

    private void Update()
    {
        if (mainCamera == null) {
            mainCamera = Camera.main;
        }

        HorizontalMovement();

        grounded = rb.Raycast(Vector2.down);
        if (grounded) {
            coyoteTimer = 0.15f;
            if (velocity.y <= 0f) {
                jumping = false;
            }
        } else {
            coyoteTimer -= Time.deltaTime;
        }

        // Jump input: Spacebar, W, or UpArrow
        bool jumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
        bool jumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);

        if (jumpDown) {
            jumpBufferTimer = 0.2f; // Buffer jump for 0.2 seconds
        } else {
            jumpBufferTimer -= Time.deltaTime;
        }

        // Execute jump if buffered and within coyote time
        if (jumpBufferTimer > 0f && coyoteTimer > 0f && !jumping)
        {
            velocity.y = jumpForce;
            jumping = true;
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }

        ApplyGravity(jumpHeld);
    }

    private void FixedUpdate()
    {
        // Move mario based on his velocity
        Vector2 position = rb.position;
        position += velocity * Time.fixedDeltaTime;

        // Clamp within the screen bounds
        if (mainCamera != null)
        {
            Vector2 leftEdge = mainCamera.ScreenToWorldPoint(Vector2.zero);
            Vector2 rightEdge = mainCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
            position.x = Mathf.Clamp(position.x, leftEdge.x + 0.5f, rightEdge.x - 0.5f);
        }

        rb.MovePosition(position);
    }

    private void HorizontalMovement()
    {
        // Accelerate / decelerate
        inputAxis = Input.GetAxis("Horizontal");
        velocity.x = Mathf.MoveTowards(velocity.x, inputAxis * moveSpeed, moveSpeed * Time.deltaTime);

        // Only stop horizontal velocity if grounded and running straight into a wall
        // In the air, allow Mario to glide smoothly over pipes and blocks without zeroing forward speed
        if (grounded && Mathf.Abs(velocity.x) > 0.1f && rb.Raycast(Vector2.right * Mathf.Sign(velocity.x))) {
            velocity.x = 0f;
        }

        // Flip sprite to face direction
        if (velocity.x > 0.05f) {
            transform.eulerAngles = Vector3.zero;
        } else if (velocity.x < -0.05f) {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    private void ApplyGravity(bool jumpHeld)
    {
        // Variable jump height: fall faster when descending or when jump button is released early
        bool isFalling = velocity.y < 0f || !jumpHeld;
        float multiplier = isFalling ? 2.2f : 1f;

        // Apply gravity and clamp terminal velocity
        velocity.y += gravity * multiplier * Time.deltaTime;
        velocity.y = Mathf.Max(velocity.y, gravity / 2f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // Bounce off enemy head
            if (transform.DotTest(collision.transform, Vector2.down))
            {
                velocity.y = jumpForce * 0.75f;
                jumping = true;
            }
        }
        else if (collision.gameObject.layer != LayerMask.NameToLayer("PowerUp"))
        {
            // Stop vertical movement if mario bonks his head from below
            if (transform.DotTest(collision.transform, Vector2.up)) {
                velocity.y = 0f;
            }
        }
    }
}
