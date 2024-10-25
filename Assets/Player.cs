using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Animator animator; 
    [SerializeField] private float moveSpeed = 10f; //player speed
    [SerializeField] private float jumpForce = 30f; //jump force
    [SerializeField] private float glideSpeed = 0.005f; //glide falling speed
    [SerializeField] private float doubleTapTimeWindow = 5.0f; //time allowed between double press of button (Space) currently 5 seconds
    [SerializeField] private float gravityMultiplier = 1.5f; //gravity applied when falling
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private float fallThreshold = -50f; //distance along y axis that indicates falling into the abyss
    [SerializeField] private float fallAngleThreshold = 45f; // Angle in degrees for resetting the character
    [SerializeField] private Vector3 respawnPoint = new Vector3(0f, 2f, 0f); //reference to respawn point
    private Rigidbody rb;
    private bool IsGrounded = true; //check if grounded
    private bool IsJumping = false; //jumping state
    private bool IsGliding = false; //gliding state
    private float lastJumpPressTime; //time of last keypress (Space)
    private bool isReadyForDoubleJump; //check if ready for double button press (Space)
    private float idleDelay = 1.0f; //time in seconds to wait before playing idle animation
    private float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        //animator.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        // Freeze rotation on the X and Z axes to prevent tipping over
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        //trigger idle animation after delay time
        timer += Time.deltaTime;

        if (timer >= idleDelay) //if delay time has passed
        {
            Debug.Log("Idle Delay time has passed.");
            float idleVal = Random.Range(0.0f, 200.0f); //generate random value between 0 and 200 to play idle animation randomly
            if (idleVal < 25.0f)
            {
                Debug.Log("Idle Animation has successfully triggered.");
                animator.SetTrigger("IdleTrigger");
            }
            timer = 0.0f; //reset timer
        }

        //check if player is below fall threshold
        if (transform.position.y < fallThreshold)
        {
            Respawn();
        }

        //check if player has fallen over
        if (IsFallenOver())
        {
            // Reset rotation to an upright position
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }

        //player movement methods
        HandleMovement();
        HandleJump();
        HandleLanding();
    }
    
    void FixedUpdate()
    {
        //apply gravity multiplier when the player is in the air
        if (!IsGrounded && !IsGliding)
        {
            rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);
        }
    }

    void HandleMovement()
    {
        // Get input from the player
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Calculate movement direction based on the player's local space (using forward and right vectors)
        Vector3 movement = (transform.forward * moveVertical + transform.right * moveHorizontal).normalized; //normalized to prevent faster movement

        // Apply movement to Rigidbody's velocity while maintaining vertical velocity (y-axis)
        Vector3 newVelocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, movement.z * moveSpeed);
        rb.velocity = newVelocity;

        // Update the speed in the Animator to control walk/run animations
        float speed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
        animator.SetFloat("Speed", speed);

        // Rotate the character to face the direction of movement
        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed); // Smooth rotation
        }

        // If the player stops moving and is grounded, set Speed to 0 to trigger idle animation
        if (speed < 0.1f && IsGrounded)
        {
            animator.SetFloat("Speed", 0);
        }
    }

    void HandleJump()
    {
        //player object jump logic
        if (Input.GetButtonDown("Jump"))
        {
            //check for double jump logic
            if (Time.time - lastJumpPressTime <= doubleTapTimeWindow && !IsGrounded && !IsGliding)
            {
                //glide if player double jumps while airborne
                StartGliding();
            }
            else if (IsGrounded)
            {
                //regular jump
                rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
                IsJumping = true;
                IsGrounded = false;
                animator.SetBool("IsJumping", true);
            }
            //record time of jump press
            lastJumpPressTime = Time.time;
        }

        //check if player is falling
        if (rb.velocity.y < 0 && !IsGrounded && !IsGliding)
        {
            animator.SetBool("IsFalling", true);
        }
    }

    void StartGliding()
    {
        IsGliding = true; // Set gliding state
        animator.SetBool("IsGliding", true); // Set gliding animation
    }

    void HandleLanding()
    {
        //if player is grounded and not falling, stop the jumping/falling animations
        if (IsGrounded)
        {
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", false);
            animator.SetBool("IsGliding", false);
        }
    }

    void Respawn()
    {
        // Reset the position of the player to the respawn point
        transform.position = respawnPoint;

        // Reset rotation to an upright position
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

        // Optionally reset the velocity to avoid immediate fall
        rb.velocity = Vector3.zero;

        // Reset any other necessary state, like animations
        animator.SetBool("IsFalling", false);
        animator.SetFloat("Speed", 0); // Adjust based on your animation state
    }

    bool IsFallenOver()
    {
        // Check the character's rotation around the X and Z axes
        Vector3 eulerRotation = transform.rotation.eulerAngles;

        // Consider fallen over if X or Z rotation is beyond the threshold
        return Mathf.Abs(eulerRotation.x) > fallAngleThreshold || Mathf.Abs(eulerRotation.z) > fallAngleThreshold;
    }

    //below will check player collision with the ground platforms
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            IsGrounded = true;
            IsGliding = false;
            IsJumping = false;
            animator.SetBool("IsGrounded", true);
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsGliding", false); //reset all animations
        }
    }


    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            IsGrounded = false;
            animator.SetBool("IsGrounded", false);
        }
    }


}

