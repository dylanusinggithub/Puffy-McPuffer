using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOffScreen : MonoBehaviour
{
    void Update()
    {
        // Check if crate has moved below the screen
        if (transform.position.y < -6.5f) // adjust based on your screen height
        {
            Destroy(gameObject);
        }
    }
}
