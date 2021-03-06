﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer spr;

    // Public settings
    public float speed;
    public float jumpForce;
    public float deathFloor = -10f;

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
    public float groundCheckRadius;
    public LayerMask whatIsGround;

    bool jumping = false;
    public int numberOfJumps;
    int extraJumps;

    public Transform frontCheck;
    public float frontCheckRadius;
    bool wallSliding = false;
    public float wallSlidingSpeed;

    float wallJumping;
    public float xWallForce;
    public float yWallForce;
    public float wallJumpTime;

    // Colour Handling
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
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround, 0f);
        isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, frontCheckRadius, whatIsGround, 0f);
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
        if (jumping) { Debug.Log(extraJumps); }
        if (fixedJumpInput && isTouchingFront)
        {
            wallJumping = fixedMoveInput;
            jumping = false;
            extraJumps -= 1;
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

        // Reset if out of level
        if (transform.position.y < deathFloor)
        {
            GameObject.Find("Game").GetComponent<Game>().Restart();
        }

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
            spr.color = Game.blue;
            redDepth = 1f;
            blueDepth = -1f;
        } else {
            isRed = true;
            spr.color = Game.red;
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
