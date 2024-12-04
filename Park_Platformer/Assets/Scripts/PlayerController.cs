using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    FacingDirection lastDirection;

    //horizontal movement
    public float maxSpeed = 4;
    public float timeToReachMaxSpeed = 2.5f;
    private float acceleration = 2;
    public float timeToDecelerate = 2.5f;
    private float deceleration = 2;
    private float currentSpeed = 0;
    public float jumpHeight = 50;

    private Vector2 currentVelocity = Vector2.zero;

    public Rigidbody2D rbHere;

    //week10 ->
    //vertical movement
    public float jumpPower = 10;
    public bool jumpingRn = false;
    public bool groundereRn = false;

    //Josh's help with vertical movement math stuff
    public float apexHeight = 7;
    public float apexTime = 0.7f;
    private float gravity; //note: start putting continuously changing variables in private (used for calculation)
    private float jumpVelocity;
    private float initialJumpSpeed;
    public float terminalVelocity = 5;

    //coyote time 
    public float coyoteTimeValue = 1f;
    private float timerCounter = 0;

    //inclass week12
    public int health = 10;

    //final task1 horizontal dash
    public float dashPower = 10f;
    private bool dashingRn = false;
    private float dashDuration = 0.3f;
    private float dashTimerCounter = 1.3f;

    //final task2 variable jump height
    public float maxJumpTimeValue = 0.5f;
    private float jumpTimerCounter = 0f;
    private bool canJump = true;

    //final task3 powerup
    private bool powerUpActivated = false;
    private float powerUpTimerCounter = 0f;
    private float originalMaxSpeed;
    private float originalApexHeight;

    public SpriteRenderer spriteHere;
    private Color originalColor;

    public enum FacingDirection
    {
        left, right
    }

    public enum CharacterState
    {
        idle, walk, jump, die
    }
    public CharacterState currentCharacterState = CharacterState.idle;
    public CharacterState previousCharacterState = CharacterState.die;

    void Start()
    {
        //horizontal movement formula
        acceleration = maxSpeed / timeToReachMaxSpeed;
        deceleration = maxSpeed / timeToDecelerate;

        //josh again && slate (for vertical movement)
        gravity = -2 * apexHeight / Mathf.Pow(apexTime, 2);
        jumpVelocity = 2 * apexHeight / apexTime;
        initialJumpSpeed = jumpVelocity; // don't know why this is needed

        originalColor = spriteHere.color; //save original color
    }

    void Update()
    {
        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.

        IsWalking();

        //week12 in class
        previousCharacterState = currentCharacterState;

        switch (currentCharacterState)
        {
            case CharacterState.die:
                // do nothing
                break;
            case CharacterState.jump:

                if (IsGrounded())
                {
                    //We know we need to make a transition because we're not grounded anymore
                    if (IsWalking())
                    {
                        currentCharacterState = CharacterState.walk;
                    }
                    else
                    {
                        currentCharacterState = CharacterState.idle;
                    }
                }

                break;
            case CharacterState.walk:
                if (!IsWalking())
                {
                    currentCharacterState = CharacterState.idle;
                }
                //Are we jumping?
                if (!IsGrounded())
                {
                    currentCharacterState = CharacterState.jump;
                }
                break;
            case CharacterState.idle:
                //Are we walking?
                if (IsWalking())
                {
                    currentCharacterState = CharacterState.walk;
                }
                //Are we jumping?
                if (!IsGrounded())
                {
                    currentCharacterState = CharacterState.jump;
                }

                break;
        }
        if (IsDead()) //player is dead, play death animation
        {
            currentCharacterState = CharacterState.die;
        }

        if (Input.GetKeyUp(KeyCode.Space) && jumpingRn) //I put this so that it can check it constantly every frame (was afraid fixed update causing issue) cuz getkeyup registers only once
        { //stop the jump when spacebar is released
            jumpingRn = false;
            jumpTimerCounter = maxJumpTimeValue; //set to max jump and stop jumping higher
        }

        if (powerUpActivated) //power up activated
        {
            powerUpTimerCounter -= Time.deltaTime; //start counting the timer

            if (powerUpTimerCounter <= 0) //time's over
            {
                //return all the bonus values to original
                maxSpeed = originalMaxSpeed;
                jumpHeight = originalApexHeight;
                spriteHere.color = originalColor;
                health = 10;
                powerUpActivated = false;
            }
        }
    }
    private void FixedUpdate()
    {
        //gravity
        Gravity();

        //coyote time && variable jump (josh's help)
        if (IsGrounded() || timerCounter > 0f) //grounded or timer bigger than 0 allows player to jump
        {
            if (Input.GetKey(KeyCode.Space) && !jumpingRn && canJump)
            {
                //jumpVelocity = initialJumpSpeed;
                jumpingRn = true;
                jumpTimerCounter = 0f;
                jumpVelocity = initialJumpSpeed;

                Debug.Log("jumping");
            }
        }

        //dashing
        DashRn();

        //horizontal movement
        Vector2 playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        MovementUpdate(playerInput);
        Debug.Log(playerInput);
    }


    private void MovementUpdate(Vector2 playerInput)
    {
        if (playerInput.x != 0)
        {
            currentSpeed += playerInput.x * acceleration * Time.deltaTime; //detect input and accelerate (-1, 1)
        }
        
        if (playerInput.x == 0) //if there's no input, slow the speed (decelerate)
        {
            if (currentSpeed > 0)
            {
                if((currentSpeed - deceleration * Time.deltaTime) < 0) //prof keely and evan's help
                    //if subtracting deceleration goes over the zero, just set the current speed to 0. Else, keep decelerating
                {
                    currentSpeed = 0f;
                }
                else
                {
                    currentSpeed -= deceleration * Time.deltaTime;
                }
                //Debug.Log("decreasing");
            }
            if (currentSpeed < 0)
            {
                if((currentSpeed + deceleration * Time.deltaTime) > 0)
                {
                    currentSpeed = 0f;
                }
                else
                {
                    currentSpeed += deceleration * Time.deltaTime;
                }
                //Debug.Log("increasing");
            }
            //if (Mathf.Abs(currentSpeed) < 0.1f)
            //    currentSpeed = 0f;
        }
        //apply horizontal, vertical physics
        rbHere.velocity = new Vector2(currentSpeed, jumpVelocity);
    }

    public bool IsWalking()
    {
        //detect player horizontal input and return true/false
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
        //updated gounded for coyote time. Josh's help pt.2 
        //bool groundedRn = Physics2D.Raycast(transform.position, Vector2.down, 0.7f, LayerMask.GetMask("Ground")); //ayman's help
        bool groundedRn = Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y - 0.6f), new Vector2(0.7f, 0.2f), 0f, Vector2.down, 0f, LayerMask.GetMask("Ground"));

        //raycast is true/false check (grounded check)

        if (groundedRn) //currently on the ground
        {
            timerCounter = coyoteTimeValue; //reset the timer
            canJump = true;
            return true;
        }
        else //not on the ground
        {
            if (timerCounter > 0) //timer system. while not grounded, decreae the timer
            {
                timerCounter -= Time.deltaTime;
            }
            canJump = false;
            return false;
        }
    }

    public FacingDirection GetFacingDirection()
    {
        //ayman's help (get player horizontal input and change the player's FacingDirection)
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
        jumpVelocity += gravity * Time.deltaTime; //add gravity to jumpvelocity
        if (!IsGrounded()) //apply gravity when player's not grounded
        {
            jumpVelocity += gravity * Time.deltaTime;

            //variable jump (josh's help)
            if (jumpingRn)
            {
                if (jumpTimerCounter < maxJumpTimeValue)
                {
                    jumpVelocity = initialJumpSpeed + (gravity * jumpTimerCounter); //keep increasing the height (josh's help)
                    //note: gravity * jumptimercounter to apply gravity with jumpspeed. without gravity, the vertical movement feels weirdly light
                    jumpTimerCounter += Time.deltaTime; //increase the counter while holding space
                }
            }

            if (Input.GetKey(KeyCode.S)) //add 2x gravity when s key is pressed
            {
                jumpVelocity += gravity * Time.deltaTime * 2f;
            }

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

    public bool IsDead() //in class exercise
    {
        return health <= 0;
    }

    public void OnDeathAnimationComplete() //in class exercise
    {
        gameObject.SetActive(false);
    }

    private void DashRn()
    {
        if (Input.GetKey(KeyCode.LeftShift) && !dashingRn)
        {
            //start dash when the player's not dashing/dashing is finished && shift key is pressed
            dashingRn = true; //switch the bool
            dashTimerCounter = dashDuration; //set the timer
            float direction = Input.GetAxisRaw("Horizontal");

            if (direction != 0)
            {
                currentSpeed = dashPower * direction; //apply dashpower to current speed
                Debug.Log("dashing...");
            }
        }

        if (dashingRn) //timer for dashing duration
        {
            //dash started! start decreasing the timer
            dashTimerCounter -= Time.deltaTime;

            if (dashTimerCounter <= 0) //counter reached 0, set the dashing to false
            {
                dashingRn = false;
            }
        }
    }

    public void ActivatePowerUp(float speedBonus, float jumpBonus, float dashBonus, float duration)
    {
        if (!powerUpActivated) //when power up's not activated, save original values
        {
            originalMaxSpeed = maxSpeed;
            originalApexHeight = apexHeight;
        }

        //apply bonuses
        maxSpeed *= speedBonus;
        apexHeight *= jumpBonus;
        dashPower += dashBonus;
        health = 99;

        powerUpTimerCounter = duration;
        powerUpActivated = true;
        //turn the color to yellow
        spriteHere.color = Color.yellow;
    }

    public void DeathShroomTouch(int damage)
    {
        health -= damage;
    }
}
