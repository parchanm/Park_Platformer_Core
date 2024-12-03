using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public float speedMultiplier = 2f;
    public float jumpMultiplier = 1.5f;
    public float dashSpeedAdd = 4;
    public float duration = 3f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) //detect collision objects with Player tag
        {
            PlayerController player = collision.GetComponent<PlayerController>(); //get player controller component (referenced my previous semester's code)
            if (player != null)
            {
                player.ActivatePowerUp(speedMultiplier, jumpMultiplier, dashSpeedAdd, duration); //send them values for the bonuses!
                Destroy(gameObject);
            }
        }
    }
}
