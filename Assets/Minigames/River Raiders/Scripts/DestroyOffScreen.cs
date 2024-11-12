using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOffScreen : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        
        if (transform.position.y < -6.5f) // If obstacle has reached -6.5 on Y axis of the screen (at the bottom offscreen)
        {
            Destroy(gameObject); // Obstacle is destroyed
        }
    }
}
