using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField, Range(0, 100)]
    float movementStrength = 100;

    [SerializeField, Range(0, 100)]
    float mouseStrength = 100;

    [SerializeField]
    bool mouseInverted;

    [SerializeField, Range(0, 2)]
    float movementDeceleration = 1;

    [SerializeField, Range(0, 5)]
    float velocityMax = 1;

    [SerializeField, Range(0, 100)]
    float rotationStrength = 50;

    [SerializeField, Range(0, 8)]
    float movementArea = 6;

    public float velocity = 0;

    public GameObject gameOverPanel; // Game Over panel when player loses all cargo crates

    public GameObject gameOverPanel2; // Game Over panel when player runs out of time

    private ScoreManager scoreManager; // Reference to Score Manager script

    
    void Start()
    {
        gameOverPanel.SetActive(false); // Game OVer panel for losing all cargo crates is set to false
        gameOverPanel2.SetActive(false); // Game Over panel for running out of time is set to false

        // Find the ScoreManager in the scene
        scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager == null)
        {
            UnityEngine.Debug.LogError("ScoreManager not found in the scene.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // Use the score from the ScoreManager
            if (scoreManager != null && scoreManager.GetScore() <=0)
            {
                gameOverPanel.SetActive(true);
                gameOverPanel2.SetActive(false);
                Time.timeScale = 0;
                Destroy(gameObject);
            }
        }
    }

    float test = 0;

    // Update is called once per frame
    void FixedUpdate()
    {

        if (Mathf.Abs(velocity) < velocityMax)
        {
            if(Input.GetButton("Vertical")) velocity += -Input.GetAxis("Vertical") * ((float)movementStrength / 100);
            else if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                float mouseDist = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
                mouseDist -= transform.position.x;

                if (mouseInverted) mouseDist *= -1;

                velocity += mouseDist * ((float)mouseStrength / 1000);
            }
        }

        if (Mathf.Abs(velocity) > (float)movementDeceleration / 100)
        {
            if (velocity > 0) velocity -= (float)movementDeceleration / 100;
            if (velocity < 0) velocity += (float)movementDeceleration / 100;
        }
        else velocity = 0;

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
