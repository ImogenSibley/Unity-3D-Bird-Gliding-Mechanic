using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator animator; 
    public float moveSpeed = 10f; //player walking speed
    public float sprintModifier = 20f; //value added to walking speed to make character sprint
    public float jumpForce = 30f; //force applied during a jump
    public float diveForce = 50f; //force applied during a dive

    [SerializeField] private float rotationSpeed = 3f; //speed camera rotates around y axis
    [SerializeField] private float doubleTapTimeWindow = 5.0f; //time allowed between double press of button (Space) currently 5 seconds
    [SerializeField] private float gravityMultiplier = 1.5f; //gravity applied when falling
    [SerializeField] private float fallThreshold = -50f; //distance along y axis that indicates falling into the abyss
    [SerializeField] private float fallAngleThreshold = 45f; //angle in degrees for resetting the character
    [SerializeField] private float slideSpeed = 2f; //player slide speed for hitting walls
    [SerializeField] private float wallRayLength = 0.5f; //length of raycast variable to check for walls
    [SerializeField] private float mouseSensitivity = 2.0f; //sensitivity of mouse movement
    [SerializeField] private Vector3 respawnPoint = new Vector3(0f, 2f, 0f); //reference to respawn point

    private Rigidbody rb;
    private bool IsGrounded = true; //check grounded state (true initially)
    private bool IsJumping = false;
    private bool IsGliding = false; //check gliding state
    private float lastJumpPressTime; //variable to store time of last keypress (Space)
    private bool isReadyForDoubleJump; //check if ready for double button press (Space)
    private float idleDelay = 1.0f; //time in seconds to wait before playing idle animation
    private float timer = 0.0f; //variable to store time elapsed
    private float groundCheckCooldown = 1.0f; //cooldown time after landing
    private float lastGroundedTime; //variable to track last time player was grounded

    void Start()
    {
        rb = GetComponent<Rigidbody>(); //get rigidbody
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; //freeze rotation on the x and z axes to prevent tipping over
        animator = GetComponent<Animator>(); //get animator
    }
    
    void Update()
    {
        HandleIdle(); //player idle method
        HandleRespawn();  //player respawn method
        HandleTilt(); //player reset upright method
        HandleMovement(); //player movement method
        HandleJump(); //player jump method
        HandleLanding(); //player landing method
        HandleDive(); //player diving method
    }

    void FixedUpdate()
    {
        if (!IsGrounded && !IsGliding) //if in the air but not gliding
        {
            rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration); //apply gravity multiplier to bring player down
        }
    }

    private void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal"); //get input from the player WASD or arrow keys
        float moveVertical = Input.GetAxis("Vertical"); 
        Vector3 movement = (transform.forward * moveVertical + transform.right * moveHorizontal).normalized; //calculate movement direction based on the player's local space, movement is normalized to prevent faster movement
        bool isMovingForward = moveVertical > 0; //check player is moving forward
        RaycastHit hit; //cast ray forward to check for walls
        bool isAgainstWall = Physics.Raycast(transform.position, transform.forward, out hit, wallRayLength); //use raycasting to determine if player is against wall

        if (isAgainstWall && isMovingForward) //if player is against wall and moving forward
        {
            Vector3 wallNormal = hit.normal; //get normal of the wall
            Vector3 slideDirection = Vector3.Cross(wallNormal, Vector3.up).normalized; //calculate slide direction parallel to wall
            rb.velocity = new Vector3(slideDirection.x * moveSpeed, rb.velocity.y - slideSpeed, slideDirection.z * moveSpeed); //maintain downward velocity to ensure sliding down
        }
        else
        {
            Vector3 newVelocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, movement.z * moveSpeed); //apply movement to rigidbody's velocity while maintaining vertical velocity (y axis)
            rb.velocity = newVelocity;
        }
       
        float speed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;  //update the speed in the Animator to control walk/run animations
        animator.SetFloat("Speed", speed);

        RotateCharacter(movement);

        if (speed < 0.1f && IsGrounded) 
        {
            animator.SetFloat("Speed", 0); //if the player stops moving and is grounded, set Speed to 0. (after some time has passed, idle animation will trigger in idle handler)
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)) //when player is pressing shift, handle sprinting
        {
            moveSpeed += sprintModifier; //increment move speed by sprint speed and reassign move speed
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed -= sprintModifier; //subtract move speed by sprint speed and reassign move speed
        }
    }

    private void HandleJump()
    {
        
        if (Input.GetButtonDown("Jump")) //check if jump key is pressed
        {
            if (Time.time - lastJumpPressTime <= doubleTapTimeWindow && !IsGrounded && !IsGliding) //check if jump is pressed twice for double jump logic
            {
                StartGliding(); //glide if player double jumps while airborne
            }
            else if (IsGrounded) 
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z); //regular jump logic
                IsJumping = true;
                IsGrounded = false;
                animator.SetBool("IsJumping", true);
            }
            lastJumpPressTime = Time.time; //record time of jump press
        }
    }

    private void HandleLanding()
    {
        if (IsGrounded) //if player is grounded and not falling, stop the jumping/falling animations
        {
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsGliding", false);
            animator.SetBool("IsFalling", false);
            lastGroundedTime = Time.time; //update last grounded time to prevent any animations retriggering
        }
        else if (Time.time > lastGroundedTime + groundCheckCooldown) //if time greater than last grounded time + cooldown
        {
            if (rb.velocity.y < 0 && !IsGliding) //check player falling
            {
                animator.SetBool("IsFalling", true);
            }
        }
    }

    private void HandleDive()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !IsGrounded) //check for dive input 'Q' key and that player is not on the ground
        {
            rb.velocity = new Vector3(rb.velocity.x, -diveForce, rb.velocity.z); //apply dive force to y, downward
            animator.SetTrigger("DiveTrigger"); //trigger dive animation (if there is one)
        }
    }

    private void HandleIdle()
    {
        timer += Time.deltaTime; //trigger idle animation after delay time
        if (timer >= idleDelay) //if delay time has passed
        {
            //Debug.Log("Idle Delay time has passed.");
            float idleVal = Random.Range(0.0f, 200.0f); //generate random value between 0 and 200 to play idle animation randomly
            if (idleVal < 25.0f)
            {
                //Debug.Log("Idle Animation has successfully triggered.");
                animator.SetTrigger("IdleTrigger");
            }
            timer = 0.0f; //reset timer
        }
    }

    private void HandleTilt()
    {
        if (IsFallenOver()) //check if player has fallen over
        {
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0); //reset rotation to an upright position
        }
    }

    private void HandleRespawn()
    {
        if (transform.position.y < fallThreshold) //check if player is below fall threshold (falling into the abyss)
        {
            Respawn(); //reset player to starting position
        }
    }

    private void Respawn()
    {
        transform.position = respawnPoint; //reset the position of the player to the respawn point
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);  //reset rotation to an upright position
        rb.velocity = Vector3.zero; //reset the velocity to avoid immediate fall
        animator.SetBool("IsFalling", false); //reset animations
        animator.SetFloat("Speed", 0);
    }

    private bool IsFallenOver()
    {
        Vector3 eulerRotation = transform.rotation.eulerAngles; //check the character's rotation around the x and z axes
        return Mathf.Abs(eulerRotation.x) > fallAngleThreshold || Mathf.Abs(eulerRotation.z) > fallAngleThreshold; //consider fallen over if x or z rotation is beyond the threshold
    }

    private void StartGliding()
    {
        IsGliding = true; //set gliding state
        animator.SetBool("IsGliding", true); //set gliding animation
    }

    private void RotateCharacter(Vector3 movement)
    {
        float mouseX = Input.GetAxis("Mouse X"); //capture mouse movement
        if (Mathf.Abs(mouseX) > 0.01f) //only apply mouse rotation if there is input
        {
            transform.Rotate(0, mouseX * mouseSensitivity, 0); //rotate around y axis based on mouse movement
        }
        else if (movement != Vector3.zero) //if movement is not 0 also rotate to face that direction
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void OnCollisionEnter(Collision collision) //below will check player collision with the ground platforms
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            IsGrounded = true;
            IsJumping = false;
            IsGliding = false;
            animator.SetBool("IsGrounded", true);
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsGliding", false);
            animator.SetBool("IsFalling", false);//reset all animations
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

