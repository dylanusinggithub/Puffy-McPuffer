using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateCollision : MonoBehaviour
{
    private ScoreManager scoreManager; // Score Manager game object
    private PlayerMovement player; // Player Movement script reference

    // Start is called before the first frame update
    void Start()
    {
        // Find the ScoreManager in the scene
        scoreManager = FindObjectOfType<ScoreManager>(); // Find Score Manager object

        player = FindObjectOfType<PlayerMovement>();  // Ensure this finds the Player object with the Ropes holder
    }

    // On Trigger Method
    private void OnTriggerEnter2D(Collider2D other)
    {
        // If Crate onject collides with Player
        if (other.CompareTag("Player"))
        {
            // If Score Manager is not null
            if (scoreManager != null)
            {
                scoreManager.AddScore(1); // then 1 point is accumulated to game score
            }

            Destroy(gameObject); // Crate object is destroyed
        }
    }
}
