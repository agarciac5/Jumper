using UnityEngine;

public class PlayerJump : MonoBehaviour
{

    public int extraJumps = 1;
    private int extraJumpsValue;
    private float jumpBufferTime = 0.15f;
private float jumpBufferCounter;
    private float coyoteTime = 0.15f;
    private float coyoteTimeCounter;


    private Rigidbody2D rb;
    public float jumpForce = 10f;

    // Ground Check variables
    public float fallMultiplier = 4.5f;
    public float lowJumpMultiplier = 2f;
    
    public Transform groundCheck;
    public LayerMask groundLayer;
    private bool isGrounded;
    public float groundCheckRadius = 0.2f;

    void Start()
{
    rb = GetComponent<Rigidbody2D>();
    extraJumpsValue = extraJumps; // Set initial jumps
}


void Update()
{
    isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

    // --- Coyote Time & Double Jump Reset ---
    if (isGrounded)
    {
        coyoteTimeCounter = coyoteTime;
        extraJumpsValue = extraJumps; // Reset double jumps
    }
    else
    {
        coyoteTimeCounter -= Time.deltaTime;
    }

    // --- Jump Buffering Logic ---
    if (Input.GetButtonDown("Jump"))
    {
        jumpBufferCounter = jumpBufferTime;
    }
    else
    {
        jumpBufferCounter -= Time.deltaTime;
    }

    // --- COMBINED Jump Input Check ---
    if (jumpBufferCounter > 0f)
    {
        if (coyoteTimeCounter > 0f) // Priority 1: Ground Jump (uses coyote time)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            coyoteTimeCounter = 0f; // Consume coyote time
            jumpBufferCounter = 0f; // Consume buffer
        }
        else if (extraJumpsValue > 0) // Priority 2: Air Jump
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // You could use a different jumpForce here
            extraJumpsValue--; // Consume an air jump
            jumpBufferCounter = 0f; // Consume buffer
        }
    }
}


  // FixedUpdate remains the same as Step 3
    void FixedUpdate()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }


    // Helper function to visualize the ground check radius in the Scene view
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
