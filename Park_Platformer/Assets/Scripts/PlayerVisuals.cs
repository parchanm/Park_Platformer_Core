using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script manages updating the visuals of the character based on the values that are passed to it from the PlayerController.
/// NOTE: You shouldn't make changes to this script when attempting to implement the functionality for the W10 journal.
/// </summary>
public class PlayerVisuals : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer bodyRenderer;
    public PlayerController playerController;

    private int idleHash, walkingHash, jumpingHash, deathHash; //note: still don't get how Hash works..

    // Start is called before the first frame update
    void Start()
    {
        idleHash = Animator.StringToHash("Idle");
        walkingHash = Animator.StringToHash("Walking");
        jumpingHash = Animator.StringToHash("Jumping");
        deathHash = Animator.StringToHash("DigWarrior_die");
    }

    // Update is called once per frame
    void Update()
    {
        VisualsUpdate();
    }

    private void VisualsUpdate()
    {
        if (playerController.previousCharacterState != playerController.currentCharacterState)
        {
            switch (playerController.currentCharacterState)
            {
                case PlayerController.CharacterState.idle:
                    animator.CrossFade("Idle", 0f);
                    break;
                case PlayerController.CharacterState.walk:
                    animator.CrossFade("Walking", 0f);
                    break;
                case PlayerController.CharacterState.jump:
                    animator.CrossFade("Jumping", 0f);
                    break;
                case PlayerController.CharacterState.die:
                    animator.CrossFade("DigWarrior_die", 0f);
                    break;
            }
        }

        switch (playerController.GetFacingDirection())
        {
            case PlayerController.FacingDirection.left:
                bodyRenderer.flipX = true;
                break;
            case PlayerController.FacingDirection.right:
            default:
                bodyRenderer.flipX = false;
                break;
        }
    }
}
