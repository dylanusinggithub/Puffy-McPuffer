using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateMovement : MonoBehaviour
{
    // Speed at which the crate will move downward
    public float moveSpeed = 2.0f;

    void Update()
    {
        // Move the crate downward based on moveSpeed
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
    }
}
