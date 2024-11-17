using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeLogCollision : MonoBehaviour
{
    private ScoreManager scoreManager;

    void Start()
    {
        // Find the ScoreManager in the scene
        scoreManager = FindObjectOfType<ScoreManager>();

        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager not found in the scene.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Deduct points from the score
            if (scoreManager != null)
            {
                scoreManager.AddScore(-1); // Deduct 1 point when colliding with a tree log
            }

            // Optionally, destroy the tree log after collision (if desired)
            Destroy(gameObject);
        }
    }
}
