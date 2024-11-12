using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeLogCollision : MonoBehaviour
{
    //On Trigger method
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object (the player) has the "Player" tag
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject); // Tree log game object is destroyed
        }
    }
}
