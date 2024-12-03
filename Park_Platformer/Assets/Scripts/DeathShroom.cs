using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathShroom : MonoBehaviour
{
    public int damage = 10;
    private void OnTriggerEnter2D(Collider2D collision) //copy pasted from power up script
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>(); //reference to playercontroller
            if (player != null)
            {
                player.DeathShroomTouch(damage); //send the damage value!
                Destroy(gameObject);
            }
        }
    }
}
