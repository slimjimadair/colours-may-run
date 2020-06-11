using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer spr;

    // Public settings
    public float speed;
    public float jumpForce;

    // Controls
    float moveInput;
    float fixedMoveInput;
    bool jumpInput;
    bool fixedJumpInput;

    // Context
    bool facingRight = true;
    bool isGrounded;
    bool isTouchingFront;

    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;

    bool jumping = false;
    public int numberOfJumps;
    int extraJumps;

    public Transform frontCheck;
    bool wallSliding = false;
    public float wallSlidingSpeed;

    float wallJumping;
    public float xWallForce;
    public float yWallForce;
    public float wallJumpTime;

    // Colour Handling
    Color32 red = new Color32(200, 89, 60, 255);
    Color32 blue = new Color32(92, 129, 131, 255);
    bool isRed = true;
    GameObject[] redPlatforms;
    float redDepth = 1f;
    GameObject[] bluePlatforms;
    float blueDepth = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();

        ChangeColour();
    }

    private void Update()
    {
        // Get Player Inputs
        moveInput = Input.GetAxisRaw("Horizontal");
        jumpInput = Input.GetKeyDown(KeyCode.Space) || jumpInput;
    }

    private void FixedUpdate()
    {
        // Get Fixed Inputs
        fixedMoveInput = moveInput;
        if (jumpInput)
        {
            fixedJumpInput = true;
            jumpInput = false;
        }

        // Get Context
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround, 0f);
        isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, checkRadius, whatIsGround, 0f);
        if ((!facingRight && moveInput > 0) || (facingRight && moveInput < 0))
        {
            Flip();
        }
        if (isGrounded)
        {
            extraJumps = numberOfJumps;
        }

        // Assess Actions
        jumping = fixedJumpInput && extraJumps > 0;
        if (fixedJumpInput && isTouchingFront)
        {
            wallJumping = fixedMoveInput;
            jumping = false;
            Invoke("SetWallJumpingToFalse", wallJumpTime);
            ChangeColour();
        }
        wallSliding = isTouchingFront && !isGrounded && moveInput != 0;
        fixedJumpInput = false;

        // Set Velocity
        float hVel = fixedMoveInput * speed;
        if (wallJumping == fixedMoveInput && wallJumping != 0)
        {
            hVel = xWallForce * -fixedMoveInput;
        } else {
            wallJumping = 0;
        }

        float vVel = rb.velocity.y;
        if (wallSliding)
        {
            vVel = Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue);
        }
        if (jumping)
        {
            vVel = jumpForce;
            ChangeColour();
            extraJumps -= 1;
            jumping = false;
        }
        if (wallJumping != 0f)
        {
            vVel = yWallForce;
        }

        rb.velocity = new Vector2(hVel, vVel);

    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;

        wallJumping = 0; // Fix for "block" when bouncing off wall
    }

    void SetWallJumpingToFalse()
    {
        wallJumping = 0f;
    }

    void ChangeColour()
    {
        if (isRed)
        {
            isRed = false;
            spr.color = blue;
            redDepth = 1f;
            blueDepth = -1f;
        } else {
            isRed = true;
            spr.color = red;
            redDepth = -1f;
            blueDepth = 1f;
        }

        redPlatforms = GameObject.FindGameObjectsWithTag("Red");
        foreach (GameObject platform in redPlatforms)
        {
            platform.GetComponent<BoxCollider2D>().isTrigger = isRed; // Set to Trigger (no collision) if same colour
            Vector3 Position = platform.transform.position;
            Position.z = redDepth;
            platform.transform.position = Position;
        }

        bluePlatforms = GameObject.FindGameObjectsWithTag("Blue");
        foreach (GameObject platform in bluePlatforms)
        {
            platform.GetComponent<BoxCollider2D>().isTrigger = !isRed; // Set to Trigger (no collision) if same colour
            Vector3 Position = platform.transform.position;
            Position.z = blueDepth;
            platform.transform.position = Position;
        }

    }

}
