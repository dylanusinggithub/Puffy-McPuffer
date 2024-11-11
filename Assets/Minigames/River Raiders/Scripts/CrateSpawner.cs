using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateSpawner : MonoBehaviour
{
    // Public variable to hold the crate prefab
    public GameObject cratePrefab;

    // Public variable to set the spawn point
    public Transform spawnPoint;

    // Spawn interval (time in seconds between spawns)
    public float spawnInterval = 2.0f;

    // Private variable to keep track of time
    private float timer;

    void Start()
    {
        // Initialize the timer
        timer = spawnInterval;
    }

    void Update()
    {
        // Countdown timer
        timer -= Time.deltaTime;

        // Check if the timer has reached zero
        if (timer <= 0)
        {
            SpawnCrate();
            // Reset timer
            timer = spawnInterval;
        }
    }

    // Function to spawn the crate at the spawn point
    void SpawnCrate()
    {
        // Instantiate a new crate at the spawn point's position and rotation
        Instantiate(cratePrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
