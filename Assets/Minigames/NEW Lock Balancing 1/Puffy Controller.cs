using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PuffyController : MonoBehaviour
{
    NEWLockBalancing LB;

    [SerializeField, Range(0, 8)]
    float maxDist;

    #region Health
    [Header("Health")]
    [SerializeField, Range(1, 5)]
    int healthMax;
    int health;

    float percentage = 0;
    bool isHit, isRed = false;

    GameObject DurabilityBar;
    GameObject DurabilityBG;
    #endregion

    #region Controls
    [Header("Controls")]
    [SerializeField, Range(0f, 100f)]
    int keyStrength = 20;

    [SerializeField, Range(0f, 100f)]
    int mouseStrength = 10;

    [SerializeField, Range(0f, 3f)]
    float mouseMaxDist = 2;

    [SerializeField]
    bool mouseInverted = false;

    [SerializeField, Range(0f, 100f)]
    int decelerationAmount = 0;

    float Velocity;
    #endregion

    void Start()
    {
        LB = GameObject.Find("GameManager").GetComponent<NEWLockBalancing>();
        DurabilityBar = GameObject.Find("Durability").transform.GetChild(1).gameObject;
        DurabilityBG = GameObject.Find("Durability").transform.GetChild(0).gameObject;

        health = healthMax;
    }

    private void FixedUpdate()
    {
        SteerBoat();
        CheckCollison();

        // Flip Puffy depenidng on where they're facing
        if (transform.position.x < 0) GetComponent<SpriteRenderer>().flipX = false;
        else GetComponent<SpriteRenderer>().flipX = true;

        // Flashes Puffy whenever hurt
        if(isRed) GetComponent<SpriteRenderer>().color = Color.red;
        else GetComponent <SpriteRenderer>().color = Color.white;

    }

    void OnDrawGizmos() // Displays puffy movement area
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Vector2.zero, new Vector2(maxDist, 10));
    }

    void CheckCollison()
    {
        // Damage Puffy for hitting walls
        if (Mathf.Abs(transform.position.x) > maxDist)
        {
            // makes it so it cannot get hurt again until decreasedDurability is finished
            if (!isHit) StartCoroutine(decreaseDurability());

            if(health < 0) LB.state = NEWLockBalancing.GameState.Fail;

            // Stops puffy from going past the movement area
            if (transform.position.x > 0) transform.position = new Vector2(maxDist, transform.position.y);
            else transform.position = new Vector2(-maxDist, transform.position.y);
        }
    }

    void SteerBoat()
    {
        if (Input.GetButton("Horizontal"))
        {
            Velocity += Input.GetAxis("Horizontal") * ((float)keyStrength / 1000);
        }
        else if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            float mouseDist = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
            Mathf.Clamp(mouseDist, -mouseMaxDist, mouseMaxDist);

            if (mouseInverted) mouseDist *= -1;

            Velocity += mouseDist * ((float)mouseStrength / 10000);
        }
        else
        {
            // Decelerates by X amount (divided by 100 to make it more reasoanble)
            if (Mathf.Abs(Velocity) > (float)decelerationAmount / 100)
            {
                if (Velocity > 0) Velocity -= (float)decelerationAmount / 100;
                if (Velocity < 0) Velocity += (float)decelerationAmount / 100;
            }
            else Velocity = 0;
        }
        transform.position += new Vector3(Velocity, 0, 0);
    }

    IEnumerator decreaseDurability()
    {
        GetComponent<AudioSource>().volume = 1;
        GetComponent<AudioSource>().Play();

        int Smoothness = 60;
        float Seconds = 1;

        health--;
        isHit = true;
        for (int i = 0; i < Smoothness; i++)
        {
            yield return new WaitForSeconds(Seconds / Smoothness);
            percentage += Seconds / Smoothness;

            float duraHeight = Mathf.Lerp(200, 0, percentage / healthMax);

            if(i % 15 == 0) isRed = !isRed; //inverts every multiple of X (Make sure it's a factor of Smootheness)

            DurabilityBG.GetComponent<Image>().color = Color.white + (Color.red - Color.white) * (percentage / healthMax);
            DurabilityBar.GetComponent<RectTransform>().sizeDelta = new Vector2(100, duraHeight);
        }
        isHit = false;
    }

    // Hit by obstacle
    void OnCollisionEnter2D(Collision2D collision)
    {
        LB.CollectObject(collision.gameObject);

        // Creates audio player object and assgins the clip given to the original colliding
        // object so if it's deleted before it finishes playing it wont be cut off
        GameObject AudioPlayer = new GameObject(collision.gameObject.name + " Audio Player");

        AudioPlayer.AddComponent<AudioSource>();

        AudioPlayer.GetComponent<AudioSource>().clip = collision.gameObject.GetComponent<AudioSource>().clip;
        AudioPlayer.GetComponent<AudioSource>().volume *= PlayerPrefs.GetFloat("Volume");
        AudioPlayer.GetComponent<AudioSource>().Play();

        Destroy(AudioPlayer, AudioPlayer.GetComponent<AudioSource>().clip.length);

        if (collision.gameObject.tag == "Obstacle")
        {
            collision.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 5);
            collision.gameObject.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-30, 30);
        }
        else Destroy(collision.gameObject);
    }
}
