using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField, Range(0, 100)]
    float movementStrength = 100;

    [SerializeField, Range(0, 2)]
    float movementDeceleration = 1;

    [SerializeField, Range(0, 100)]
    float rotationStrength = 50;

    [SerializeField, Range(0, 8)]
    float movementArea = 6;

    float velocity = 0;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetButton("Horizontal")) velocity = Input.GetAxis("Horizontal") * (float)movementStrength / 100;
        else
        {
            // Decelerates by X amount (divided by 100 to make it more reasoanble)
            if (Mathf.Abs(velocity) > (float)movementDeceleration / 100)
            {
                if (velocity > 0) velocity -= (float)movementDeceleration / 100;
                if (velocity < 0) velocity += (float)movementDeceleration / 100;
            }
            else velocity = 0;
        }

        // Stops Puffy from going past the edges
        if (Mathf.Abs(transform.position.x + velocity) > movementArea)
        {
            if (transform.position.x > 0) transform.position = new Vector2(movementArea - 0.01f, transform.position.y);
            else transform.position = new Vector2(-movementArea + 0.01f, transform.position.y);

            velocity = 0;
        }

        transform.transform.position = new Vector2(velocity + transform.transform.position.x, transform.transform.position.y);
        transform.rotation = Quaternion.Euler(0, 0, -velocity * (rotationStrength * 5));
    }
}
