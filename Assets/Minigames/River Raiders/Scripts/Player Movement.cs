using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Array to hold the X positions for the three lanes
    private Vector3[] lanes = new Vector3[3];
    private int currentLane = 1; // Start in the middle lane

    public float laneDistance = 6.0f; // Distance between lanes
    public float maxSpeed = 10f; // Max speed for lane switching
    public float accelerationRate = 2f; // Speed increase per second
    public float decelerationRate = 3f; // Speed decrease per second

    private Vector3 targetPosition; // Target position for the player
    private float currentSpeed = 0f; // Current speed
    private bool isMovingToNewLane = false; // Check if player is changing lanes

    // Rotation settings
    public float rotationAngle = 15f; // Angle to rotate when moving left or right
    public float rotationSpeed = 5f; // Speed to rotate towards target angle

    private Quaternion targetRotation; // Target rotation when switching lanes
    private Quaternion defaultRotation; // Upward rotation for reset


    void Start()
    {
        // Initialize lane positions assuming lanes are evenly spaced on the X-axis
        lanes[0] = new Vector3(-laneDistance, transform.position.y, transform.position.z); // Left lane
        lanes[1] = new Vector3(0, transform.position.y, transform.position.z);  // Middle lane
        lanes[2] = new Vector3(laneDistance, transform.position.y, transform.position.z);  // Right lane

        targetPosition = lanes[currentLane]; // Start at middle lane
        defaultRotation = transform.rotation; // Store the default upward rotation
        targetRotation = defaultRotation; // Initialize target rotation to be upward
    }

    void Update()
    {
        HandleInput(); // Handle Input Function is called
        SmoothMovement(); // Smooth Movement Funciton is called
        SmoothRotation(); // Smooth Rotation Funciton is called
    }

    // Handle Input Function
    private void HandleInput()
    {
        if (isMovingToNewLane) return; // Prevent new input while moving to a lane

        // Check for left movement
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentLane > 0)
            {
                currentLane--; // Move one lane left
                targetPosition = lanes[currentLane];
                isMovingToNewLane = true;

                // Set rotation to the left
                targetRotation = Quaternion.Euler(0, 0, rotationAngle);
            }
        }

        // Check for right movement
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentLane < lanes.Length - 1)
            {
                currentLane++; // Move one lane right
                targetPosition = lanes[currentLane];
                isMovingToNewLane = true;

                // Set rotation to the right
                targetRotation = Quaternion.Euler(0, 0, -rotationAngle);
            }
        }
    }

    // Smooth Movement Function
    private void SmoothMovement()
    {
        if (!isMovingToNewLane) return; // Only move if transitioning to a new lane

        // Accelerate to max speed if not yet at max speed
        if (currentSpeed < maxSpeed)
        {
            currentSpeed += accelerationRate * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed); // Clamp speed
        }

        // Move the player smoothly to the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);

        // Decelerate and stop when close to the target
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            transform.position = targetPosition; // Snap to target
            currentSpeed = 0; // Reset speed
            isMovingToNewLane = false; // Allow new lane input

            // Reset rotation back to default (upward) after reaching the lane
            targetRotation = defaultRotation;
        }
    }

    // Smooth Rotation Funciton
    private void SmoothRotation()
    {
        // Smoothly rotate towards the target rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}