using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 4;
    FacingDirection lastDirection;
    public float timeToReachMaxSpeed = 2.5f;
    private float acceleration = 2;
    public float timeToDecelerate = 2.5f;
    private float deceleration = 2;
    private float currentSpeed = 0;
    //private float currentJump = 0;

    public float jumpHeight = 50;

    private Vector2 currentVelocity = Vector2.zero;

    public Rigidbody2D rbHere;

    //week10 ->
    //public float gravity = 10;
    public float jumpPower = 10;
    public bool jumpingRn = false;
    public bool groundereRn = false;

    //Josh's help
    public float apexHeight = 7;
    public float apexTime = 0.7f;
    private float gravity; //start putting continuously changing variables in private (used for calculation)
    private float jumpVelocity;
    private float initialJumpSpeed;
    public float terminalVelocity = 5;

    //coyote time 
    public float coyoteTimeValue = 1f;
    private float timerCounter = 0;


    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        acceleration = maxSpeed / timeToReachMaxSpeed;
        deceleration = maxSpeed / timeToDecelerate;

        //josh again && slate
        gravity = -2 * apexHeight / Mathf.Pow(apexTime, 2);
        jumpVelocity = 2 * apexHeight / apexTime;
        initialJumpSpeed = jumpVelocity; // don't know why this is needed
    }

    // Update is called once per frame
    void Update()
    {
        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.

        //Vector2 playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        //MovementUpdate(playerInput);
        Debug.DrawLine(transform.position, Vector2.down, Color.red);

        Gravity();
        Debug.Log(timerCounter);
        //IsGrounded();
        //if (Input.GetKey(KeyCode.Space) && IsGrounded() && !jumpingRn) //doesn't work with getkeydown??? why???
        //{
        //    jumpVelocity = initialJumpSpeed;
        //    jumpingRn = true;
        //    Debug.Log("jumped");
        //}

        //coyote time
        if (IsGrounded() || timerCounter > 0f) //grounded or timer bigger than 0 allows player to jump
        {
            if (Input.GetKey(KeyCode.Space) && !jumpingRn) //doesn't work with getkeydown??? why???
            {
                jumpVelocity = initialJumpSpeed;
                jumpingRn = true;
                Debug.Log("jumped");
            }
        }

        Vector2 playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0); //moved to fixed update since it's physics based
        MovementUpdate(playerInput);

        IsWalking();
        //IsGrounded();
    }

    //private void FixedUpdate()
    //{
    //    //gravity
    //    //rbHere.AddForce(transform.up * -gravity);

    //    //groundereRn = IsGrounded();
    //    //rbHere.velocity += new Vector2(0, gravity * Time.deltaTime);//@@
    //    //if (Input.GetKeyDown(KeyCode.Space) && groundereRn)
    //    //{
    //    //    rbHere.velocity = new Vector2(rbHere.velocity.x, jumpPower * Time.deltaTime);
    //    //    jumpingRn = true;
    //    //    Debug.Log("you jumped");
    //    //}
    //    //if (jumpingRn && rbHere.velocity.y <= 0)
    //    //{
    //    //    jumpingRn = false;
    //    //}

    //    //Gravity();

    //    Vector2 playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0); //moved to fixed update since it's physics based
    //    MovementUpdate(playerInput);

    //    //if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
    //    //{
    //    //    jumpVelocity = startedJumpVelocity;
    //    //    Debug.Log("jumped");
    //    //}

    //    IsWalking();
    //    IsGrounded();
    //}

    //acceleration
    //velocity

    private void MovementUpdate(Vector2 playerInput)
    {
        if (playerInput.x != 0)
        {
            currentSpeed += playerInput.x * acceleration * Time.deltaTime; //detect input and accelerate (-1, 1)
        }
        
        if (playerInput.x == 0)
        {
            if (currentSpeed > 0)
            {
                currentSpeed -= deceleration * Time.deltaTime;
            }
            if (currentSpeed < 0)
            {
                currentSpeed += deceleration * Time.deltaTime;
            }
        }

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    rbHere.AddForce(transform.up * jumpHeight);
        //    Debug.Log("you jumped");
        //}

        rbHere.velocity = new Vector2(currentSpeed, jumpVelocity); //@@@@@@@@@@@gravity and jump here
    }

    public bool IsWalking()
    {
        Vector2 playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0); 
        if (playerInput.x != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool IsGrounded()
    {
        //trials and errors
        //Physics2D.BoxCast(transform.position, new Vector2(0, 0.01f), 0, Vector2.down); this won't work..
        //RaycastHit2D don't know how to use this
        //return Physics2D.Raycast(transform.position, Vector2.down, 0.1f);
        //return true;

        /* //blocked for coyote time
        if (Physics2D.Raycast(transform.position, Vector2.down, 0.7f, LayerMask.GetMask("Ground"))) //ayman's help
        {
            timerCounter = coyoteTimer;//grounded to reset timer
            return true;
        }
        else
        {
            if (timerCounter > 0)
            {
                timerCounter -= Time.deltaTime; //start decreasing the counter when they're not grounded
                return true; // coyote time (stay in the air for a sec)
            }
        }
        return false; //coyote time expired and return false
        */

        //return Physics2D.Raycast(transform.position, Vector2.down, 0.55f, LayerMask.GetMask("Ground"));


        //updated gounded for coyote time. Josh's help pt.2 
        bool groundedRn = Physics2D.Raycast(transform.position, Vector2.down, 0.7f, LayerMask.GetMask("Ground")); //ayman's help
        //raycast is true/false check

        if (groundedRn) //currently on the ground
        {
            timerCounter = coyoteTimeValue; //reset the timer
            return true;
        }
        else //not on the ground
        {
            if (timerCounter > 0) //timer system. while not grounded, decreae the timer
            {
                timerCounter -= Time.deltaTime;
            }

            return false;
        }
    }

    public FacingDirection GetFacingDirection()
    {
        //Vector2 playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0); //can't use vector values
        float playerInput = Input.GetAxisRaw("Horizontal");

        if (playerInput > 0)
        {
            lastDirection = FacingDirection.right;
        }
        if (playerInput < 0)
        {
            lastDirection = FacingDirection.left;
        }
        return lastDirection; //the code gives error if I remove this line
    }

    private void Gravity() //josh's help
    {
        //if (!IsGrounded()) //when player's not grounded apply gravity
        //{
        //    rbHere.velocity += Vector2.up * gravity * Time.deltaTime;
        //}

        //jumpvelocity = rigidbody vector y
        jumpVelocity += gravity * Time.deltaTime; //add gravity to jumpvelocity
        if (!IsGrounded()) //apply gravity when player's not grounded
        {
            jumpVelocity += gravity * Time.deltaTime;

            if(jumpVelocity < -terminalVelocity) //max fall speed for task 2
            {
                jumpVelocity = -terminalVelocity;// if jumpVelocity is bigger (going downwards) than terminal velocity, clamp it with TV!
                //terminal velocity = max speed cap (just fancy word...)
            }
        }
        else //when grounded, set velocity to 0 (gravity doesn't apply)
        {
            jumpVelocity = 0;
            jumpingRn = false;
        }
    }
}
