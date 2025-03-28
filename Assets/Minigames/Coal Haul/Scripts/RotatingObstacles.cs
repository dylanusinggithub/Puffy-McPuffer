using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class RotatingObstacles : MonoBehaviour
{
    [SerializeField] private float rotationSpeed; // Obstacle rotation speed


    void Update()
    {
        //Rotates object on Z-axis (2D Rotation)
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}
