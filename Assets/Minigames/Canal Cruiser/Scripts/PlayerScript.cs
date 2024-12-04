using System.Collections;
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

    [SerializeField] private AudioClip cargoCollectedSound;
    private AudioSource audioSource;

    float velocity = 0;

    ScoreScript SM;


    [SerializeField]
    Color flashColor;
    Color oldColor;

    [SerializeField, Range(0, 2)]
    float flashSeconds;

    [SerializeField, Range(0, 50)]
    float flashAmount;

    [SerializeField] private ParticleSystem obstacleParticle;
    private ParticleSystem obstacleParticleInstance;

    private void Start()
    {
        SM = GameObject.Find("Game Manager").GetComponent<ScoreScript>();
        oldColor = GetComponent<SpriteRenderer>().color;

        audioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.parent.Translate(new Vector3(startSpeed, 0, 0));

        if (Input.GetButton("Horizontal")) velocity += -Input.GetAxis("Horizontal") * (movementStrength / 10000);
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

            GetComponent<AudioSource>().volume = 1;
            GetComponent<AudioSource>().Play();

            velocity = 0;
        }

        transform.transform.localPosition = new Vector3(transform.localPosition.x, velocity + transform.transform.localPosition.y, 0);
        transform.rotation = Quaternion.Euler(0, 0, velocity * (rotationStrength * 5) + -90);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!this.enabled) return; // Disabled scripts aren't fully disabled? <3 unity

        collision.gameObject.GetComponent<AudioSource>().volume = 1;
        collision.gameObject.GetComponent<AudioSource>().Play();

        int points;

        if (collision.tag == "Collectable")
        {
            points = 1;

            if (cargoCollectedSound != null && audioSource != null)
            {
            audioSource.PlayOneShot(cargoCollectedSound);
            }

            Destroy(collision.gameObject);
        }
        else
        {
            points = -1;
            StartCoroutine(DamageFlash());
            SpawnObstacleParticles();
        }

        if (points + SM.score > -1) SM.score += points;
        else SM.Die();
    }

    IEnumerator DamageFlash()
    {
        for (int i = 0; i < flashSeconds * flashAmount; i++)
        {
            if (i % 2 == 0) GetComponent<SpriteRenderer>().color = flashColor; // changes colour every other time
            else GetComponent<SpriteRenderer>().color = oldColor;

            yield return new WaitForSeconds(flashSeconds / flashAmount);
        }
        GetComponent<SpriteRenderer>().color = oldColor;
    }

    private void SpawnObstacleParticles()
    {
        obstacleParticleInstance = Instantiate(obstacleParticle, transform.position, Quaternion.identity);
    }
}
