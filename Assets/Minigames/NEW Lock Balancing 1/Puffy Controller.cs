using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PuffyController : MonoBehaviour
{
    NEWLockBalancing LB;
    WheelSteering WS;

    [SerializeField, Range(0, 8)]
    float maxDist;

    float HitVol;

    GameObject Wheel;

    #region Health
    [Header("Health")]
    [SerializeField, Range(1, 5)]
    int healthMax;
    int health;

    float percentage = 0;
    bool isHit, isRed = false;

    [SerializeField]
    GameObject HitParticle;
    Animator Player;

    [SerializeField]
    Color StartColour, EndColour;


    GameObject DurabilityBar;
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
        float LockSize = GameObject.Find("GameManager").GetComponent<NEWLockBalancing>().LockSize;
        maxDist *= LockSize + Mathf.Lerp(0, 0.35f, LockSize - 1);

        HitVol = GetComponent<AudioSource>().volume;

        LB = GameObject.Find("GameManager").GetComponent<NEWLockBalancing>();
        Wheel = GameObject.Find("Wheel");
        WS = Wheel.GetComponent<WheelSteering>();

        DurabilityBar = GameObject.Find("Durability").transform.GetChild(0).gameObject;

        health = healthMax;
    }

    private void OnValidate()
    {
        GameObject.Find("Health Bar").GetComponent<Image>().color = StartColour;
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

    void CheckCollison()
    {
        // Damage Puffy for hitting walls
        if (Mathf.Abs(transform.position.x) > maxDist)
        {
            // Stops puffy from going past the movement area
            if (transform.position.x > 0) transform.position = new Vector2(maxDist, transform.position.y);
            else transform.position = new Vector2(-maxDist, transform.position.y);

            // makes it so it cannot get hurt again until decreasedDurability is finished
            if (!isHit)
            {
                StartCoroutine(decreaseDurability());
                if (health < 0) LB.state = NEWLockBalancing.GameState.Fail;
            }
        }
    }

    void SteerBoat()
    {
        if (Input.GetButton("Horizontal"))
        {
            Velocity += Input.GetAxis("Horizontal") * ((float)keyStrength / 1000);
        }
        else if (WS.Angle != 0) Velocity += -WS.Angle/1000;
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

        Wheel.transform.eulerAngles = new Vector3(0, 0, -Velocity * 10); 
    }

    IEnumerator decreaseDurability()
    {
        GetComponent<AudioSource>().volume = HitVol * PlayerPrefs.GetFloat("Volume", 1);
        GetComponent<AudioSource>().Play();

        GetComponent<Animator>().SetTrigger("Hit"); //plays animation when hit :)

        Destroy(Instantiate(HitParticle, transform.localPosition + new Vector3(-1.2f, -0.2f), Quaternion.identity, transform), 1);

        int Smoothness = 60;
        float Seconds = 1;

        health--;
        isHit = true;
        for (int i = 0; i < Smoothness; i++)
        {
            yield return new WaitForSeconds(Seconds / Smoothness);
            percentage += Seconds / Smoothness;

            float duraHeight = Mathf.Lerp(160, 0, percentage / healthMax);

            if(i % 15 == 0) isRed = !isRed; //inverts every multiple of X (Make sure it's a factor of Smootheness)

            DurabilityBar.GetComponent<Image>().color = StartColour + (EndColour - StartColour) * (percentage / healthMax);
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
        AudioPlayer.GetComponent<AudioSource>().volume *= PlayerPrefs.GetFloat("Volume", 1);
        AudioPlayer.GetComponent<AudioSource>().Play();

        Destroy(AudioPlayer, AudioPlayer.GetComponent<AudioSource>().clip.length);

        if (collision.gameObject.tag == "Obstacle")
        {
            GetComponent<Animator>().SetTrigger("Hit"); //plays animation when hit :)
            collision.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 5);
            collision.gameObject.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-30, 30);
        }
        else Destroy(collision.gameObject);
    }
}
