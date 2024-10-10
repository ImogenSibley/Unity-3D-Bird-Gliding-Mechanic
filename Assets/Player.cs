using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  
    public float speed = 0.1f;
    public float jumpSpeed = 0.1f;
    private Rigidbody rb;

    //private Animator animator;
    //private bool isJumping;
    //private bool isGrounded;


    // Start is called before the first frame update
    void Start()
    {
        //animator.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        //player movement
        float xDirection = Input.GetAxis("Horizontal");
        float zDirection = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(xDirection, 0.0f, zDirection);

        transform.position += moveDirection * speed;
       
        //player jump when space is pressed
        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(new Vector3(0, jumpSpeed, 0), ForceMode.Impulse);
            //animator.SetBool("IsJumping", true);
            //isJumping = true;
        }


    }
}
