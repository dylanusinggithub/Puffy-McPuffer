using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField, Range(0, 10)]
    float startSpeed = 5;
    
    [SerializeField, Range(0, 100)]
    float movementStrength = 100;
    
    [SerializeField, Range(0, 100)]
    float mouseStrength = 100;

    [SerializeField, Range(0, 2)]
    float mouseMax = 2;

    [SerializeField]
    bool mouseInverted;

    [SerializeField, Range(0, 2)]
    float movementDeceleration = 1;

    [SerializeField, Range(0, 100)]
    float rotationStrength = 50;

    [SerializeField, Range(0, 8)]
    float movementArea = 6;

    float velocity = 0;

    ScoreScript SM;

    private void Start()
    {
        SM = GameObject.Find("Game Manager").GetComponent<ScoreScript>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.parent.Translate(new Vector3(startSpeed, 0, 0));

        if(Input.GetButton("Horizontal")) velocity += -Input.GetAxis("Horizontal") * (movementStrength / 10000);
        else if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            float mouseDist = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
            mouseDist -= transform.localPosition.y;

            // Clamp isn't working?? so im setting it manually
            if (Mathf.Abs(mouseDist) > mouseMax)
            {
                if (mouseDist > 0) mouseDist = mouseMax;
                else mouseDist = -mouseMax;
            }

            velocity += mouseDist / (mouseStrength * 10);
        }
        else if (Mathf.Abs(velocity) > movementDeceleration / 1000)
        {
            if (velocity > 0) velocity -= movementDeceleration / 1000;
            if (velocity < 0) velocity += movementDeceleration / 1000;
        }
        else velocity = 0;

        // Stops Puffy from going past the edges
        if (Mathf.Abs(transform.localPosition.y + velocity) > movementArea)
        {
            if (transform.localPosition.y > 0) transform.localPosition = new Vector2(transform.localPosition.x, movementArea - 0.01f);
            else transform.localPosition = new Vector2(transform.localPosition.x, -movementArea + 0.01f);

            velocity = 0;
        }

        transform.transform.localPosition = new Vector3(transform.localPosition.x, velocity + transform.transform.localPosition.y, 0);
        transform.rotation = Quaternion.Euler(0, 0, velocity * (rotationStrength * 5) + -90);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        Destroy(collision.gameObject);

        int points;
        if (collision.tag == "Collectable") points = 1;
        else points = -1;

        if (points + SM.score > -1) SM.score += points;
        else SM.Die(true);
    }
}