using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    float moveInput;

    Rigidbody2D rb;
    SpriteRenderer spr;

    bool facingRight = true;

    bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;

    bool jumping = false;
    public int numberOfJumps;
    int extraJumps;

    bool isTouchingFront;
    public Transform frontCheck;
    bool wallSliding = false;
    public float wallSlidingSpeed;

    bool wallJumping = false;
    public float xWallForce;
    public float yWallForce;
    public float wallJumpTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();

        spr.color = new Color(0.784313f, 0.34902f, 0.235294f);
    }

    private void Update()
    {
        if (!jumping)
        {
            jumping = Input.GetKeyDown(KeyCode.Space) && (extraJumps > 0 || isGrounded);
        }

        if (!wallJumping)
        {
            wallJumping = Input.GetKeyDown(KeyCode.Space) && wallSliding;
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, checkRadius, whatIsGround);

        moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        if ((!facingRight && moveInput > 0) || (facingRight && moveInput < 0))
        {
            Flip();
        }

        if (isGrounded)
        {
            extraJumps = numberOfJumps;
        }

        if (jumping)
        {
            Debug.Log(extraJumps);
            rb.velocity = Vector2.up * jumpForce;
            extraJumps -= 1;
            jumping = false;
        }

        if (isTouchingFront && !isGrounded && moveInput != 0)
        {
            wallSliding = true;
        }
        else
        {
            wallSliding = false;
        }

        if (wallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }

        if (wallJumping)
        {
            Invoke("SetWallJumpingToFalse", wallJumpTime);
        }

        if (wallJumping)
        {
            rb.velocity = new Vector2(xWallForce * -moveInput, yWallForce);
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;

        wallJumping = false; // Fix for "block" when bouncing off wall
    }

    void SetWallJumpingToFalse()
    {
        wallJumping = false;
    }

}
