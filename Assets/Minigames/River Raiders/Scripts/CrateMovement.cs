using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateMovement : MonoBehaviour
{
    public float moveSpeed = 2.0f; // Crate speed is set to 2

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime); // Move the crate downward based on moveSpeed
    }
}
