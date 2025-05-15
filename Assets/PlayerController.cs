using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    
    
    [Header("Ground Movement")] public float moveSpeed = 100f;
    public float groundDrag = 5f;

    [Header("Air Movement")] public float jumpForce = 30f;
    public float jumpCooldown = 0.25f;
    public float airMultiplier = 1.5f;
    [Space] public float groundPoundForce = -240f;
    public float groundPoundCooldown = 1f;

    [Space]
    //public float gravityMultiplier = 2f;
    [Space]
    public float fallGravityMultiplier = 5f;

    public float lowJumpGravityMultiplier = 3f;
    public float baseGravity = -19.62f;

    [Header("Keybinds")] public KeyCode jumpKey = KeyCode.Space;
    public KeyCode groundPoundKey = KeyCode.LeftShift;


    private bool readyToJump = true;
    private bool readyToGroundPound = true;
    private bool canGroundPound = true;

    [Header("Ground Check")] public float playerHeight;
    public LayerMask whatIsGround;
    private bool grounded;

    [Header("Camera")]
    private Transform cameraTransform;
    
    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    [SerializeField] Rigidbody rb;

    [SerializeField] private UnityEvent onGroundPound;

    private bool pressedGroundPoundKey = false;
    

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (!cameraTransform)
        {
            cameraTransform = Camera.main.transform;
        }
    }


    private void OnCollisionEnter(Collision other)
    {
        if (pressedGroundPoundKey)
        {
            onGroundPound.Invoke();
            StartCoroutine(ResetGroundPoundTimer());
        }
    }

    private IEnumerator ResetGroundPoundTimer()
    {
        yield return new WaitForSeconds(0.2f);
        
        pressedGroundPoundKey = false;
    }

    private void Update()
    {
        //Debug.Log("Velocity: " + rb.velocity.magnitude);
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.5f, whatIsGround);

        Debug.Log("Player is grounded: " + grounded);
        MyInput();
        SpeedControl();

        // handle drag
        if (grounded)
        {
            rb.linearDamping = groundDrag;
            
            if (verticalInput < 0.1f && horizontalInput < 0.1f)
            {
                rb.linearDamping = 10;
            }
        }
        else
        {
            rb.linearDamping = 0;
        }
        

        SetPlayerRotation();
    }

    private void SetPlayerRotation()
    {
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
        
    }

    private void FixedUpdate()
    {
        MovePlayer();
        AddGravity();
        
    }

    private void AddGravity()
    {
        // Custom gravity control
        if (rb.linearVelocity.y < 0) // Falling
        {
            rb.AddForce(baseGravity * fallGravityMultiplier * Vector3.up,
                ForceMode.Acceleration);
        }
        /*else if (rb.linearVelocity.y > 0) // Letting go of jump
        {
            rb.AddForce(baseGravity * lowJumpGravityMultiplier * Vector3.up,
                ForceMode.Acceleration);
        }*/
        else // Rising normally
        {
            rb.AddForce(Vector3.up * baseGravity, ForceMode.Acceleration);
        }
    }


    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");


        // when to jump
        if (Input.GetKeyDown(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyUp(groundPoundKey))
        {
            canGroundPound = true;
        }

        // when to ground pound
        if (Input.GetKeyDown(groundPoundKey) && canGroundPound && readyToGroundPound && !grounded)
        {
            pressedGroundPoundKey = true;
            canGroundPound = false;
            GroundPound();
            Invoke(nameof(ResetGroundPound), groundPoundCooldown);
        }
    }


    private void MovePlayer()
    {
        // calculate movement direction
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

// Flatten the vectors (remove vertical tilt)
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        moveDirection = camForward * verticalInput + camRight * horizontalInput;

        
        // on ground
        if (grounded)
            rb.AddForce(moveSpeed * 10f * moveDirection.normalized, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(airMultiplier * moveSpeed * 10f * moveDirection.normalized,
                ForceMode.Force);
    }


    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);


        if (pressedGroundPoundKey && Input.GetKey(groundPoundKey))
        {
            rb.AddForce(jumpForce * 1.25f * transform.up, ForceMode.Impulse);
            
        }
        else
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            
        }
    }

    private void GroundPound()
    {
        // reset y velocity
        //rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.linearVelocity = Vector3.zero; 
            
        rb.AddForce(transform.up * groundPoundForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void ResetGroundPound()
    {
        readyToGroundPound = true;
        canGroundPound = true;
    }
    
}