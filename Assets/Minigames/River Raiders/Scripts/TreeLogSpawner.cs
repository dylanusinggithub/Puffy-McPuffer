using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeLogSpawner : MonoBehaviour
{
    public GameObject treeLogPrefab; // Tree Log prefab

    public Transform spawnPoint; // Tree Log spawn point

    public float spawnInterval = 7.0f; // Tree Log spawn interval is set to 7 seconds (between spawns)

    // Private variable to keep track of time
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the timer
        timer = spawnInterval;
    }

    // Update is called once per frame
    void Update()
    {
        // Countdown timer
        timer -= Time.deltaTime;

        // Check if the timer has reached zero
        if (timer <= 0)
        {
            SpawnTreeLog(); // Spawn Tree Log function is called

            // Reset timer
            timer = spawnInterval;
        }
    }

    // Spawn Tree Log Function
    void SpawnTreeLog()
    {
        // Instantiate a new crate at the spawn point's position and rotation
        Instantiate(treeLogPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
