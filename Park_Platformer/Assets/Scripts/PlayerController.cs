using System.Collections;
using System.Collections.Generic;
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

    private Vector2 currentVelocity = Vector2.zero;
    public Rigidbody2D rbHere;

    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        acceleration = maxSpeed / timeToReachMaxSpeed;
        deceleration = maxSpeed / timeToDecelerate;
    }

    // Update is called once per frame
    void Update()
    {
        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.

        //Vector2 playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        //MovementUpdate(playerInput);
        //Debug.DrawRay(transform.position, Vector2.down, Color.red);
    }

    private void FixedUpdate()
    {
        Vector2 playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0); //moved to fixed update since it's physics based
        MovementUpdate(playerInput);

        IsWalking();
        IsGrounded();
    }

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

        rbHere.velocity = new Vector2(currentSpeed, 0);
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

        if (Physics2D.Raycast(transform.position, Vector2.down, 0.7f, LayerMask.GetMask("Ground"))) //ayman's help
        {
            return true;
        }
        return false;


        //return Physics2D.Raycast(transform.position, Vector2.down, 0.55f, LayerMask.GetMask("Ground"));
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
}
