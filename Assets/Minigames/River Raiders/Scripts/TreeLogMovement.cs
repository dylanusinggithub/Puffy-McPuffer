using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeLogMovement : MonoBehaviour
{
    public float moveSpeed = 2.0f; // Tree log speed is set to 2

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime); // Move the tree log downward based on moveSpeed
    }
}

