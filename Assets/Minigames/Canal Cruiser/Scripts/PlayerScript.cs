using System.Collections;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
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

    GameObject Wheel;
    SteeringController SC;

    [SerializeField]
    Color flashColor;
    Color oldColor;

    [SerializeField, Range(0, 2)]
    float flashSeconds;

    [SerializeField, Range(0, 50)]
    float flashAmount;

    [SerializeField] private ParticleSystem obstacleParticle;
    private ParticleSystem obstacleParticleInstance;

    [SerializeField]
    Animator HitCruiser;

    private void Awake()
    {
        SM = GameObject.Find("Game Manager").GetComponent<ScoreScript>();
        Wheel = GameObject.Find("Wheel");
        SC = Wheel.GetComponent<SteeringController>();

        oldColor = GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            if (Input.GetButton("Horizontal"))
            {
                velocity += -Input.GetAxis("Horizontal") * (movementStrength / 10000);
            }
            else velocity += Input.GetAxis("Vertical") * (movementStrength / 10000);

            Wheel.transform.eulerAngles = new Vector3(0, 0, velocity * 100);
        }
        else if (SC.Angle != 0) velocity += (SC.Angle / 15000);
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
        int points;
        if (collision.tag == "Collectable")
        {
            points = 1;
            Destroy(collision.gameObject);
        }
        else
        {
            points = -1;
            StartCoroutine(DamageFlash());
            StartCoroutine(AnimOn());
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

    IEnumerator AnimOn()
    {
        HitCruiser.enabled = true;
        HitCruiser.SetTrigger("Hit"); //plays the hit animation when hit by an object
        yield return new WaitForSeconds(1.01f);
        HitCruiser.enabled = false;
    }

    private void SpawnObstacleParticles()
    {
        obstacleParticleInstance = Instantiate(obstacleParticle, transform.position, Quaternion.identity);
    }
}
