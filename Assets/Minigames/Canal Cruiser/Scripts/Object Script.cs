using System.Collections;
using UnityEngine;

public class ObjectScript : MonoBehaviour
{
    [SerializeField]
    bool randomiseScale;
    Vector2 startScale;

    [Header("Bobbing Animation")]

    [SerializeField]
    bool bobbingRotation;
    bool goRight;
    int bobbingRotationCount = 0;

    [SerializeField]
    bool bobbingUpwards;
    bool goUp;
    int bobbingUpwardsCount = 0;

    [Header("Object Setting")]

    [SerializeField, Range(-0.5f, 0.5f)]
    float startSpeed = 0;

    float sinkSeconds = 3;

    public bool isHardmode;
    [SerializeField] Color HardModeColor = Color.white;

    [Header("Audio Setting")]
    [SerializeField]
    AudioClip[] objectAudio;

    [SerializeField, Range(0f, 100f)]
    float volumeSpawn = 1;

    private void OnValidate()
    {
        if (randomiseScale)
        {
            transform.localScale = new Vector3(Random.Range(0.5f, 1.5f), transform.localScale.y, transform.localScale.z);
            randomiseScale = false;
        }

        if (isHardmode) GetComponent<SpriteRenderer>().color = HardModeColor;
        else GetComponent<SpriteRenderer>().color = Color.white;
    }

    private void Start()
    {
        if (bobbingRotation)
        {
            if(Random.Range(0, 1) == 0) goRight = true;

            if (goRight) transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 5.2f);
            else transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, -5.2f);
        }

        bobbingUpwardsCount = Random.Range(0, 100);
    }


    private void FixedUpdate()
    {
        transform.Translate(new Vector2(0, startSpeed));   // don't ask why it's reversed, idk why

        if (bobbingRotation)
        {
            int BobbingRotation = 2;

            if (goRight) bobbingRotationCount -= BobbingRotation;
            else bobbingRotationCount += BobbingRotation;

            if (bobbingRotationCount > 100) goRight = true;
            else if (bobbingRotationCount < -100) goRight = false;

            transform.eulerAngles += new Vector3(0, 0, (float)bobbingRotationCount/500);
        }

        if (bobbingUpwards)
        {
            int BobbingUp = 3;

            if (goUp) bobbingUpwardsCount -= BobbingUp;
            else bobbingUpwardsCount += BobbingUp;

            if (bobbingUpwardsCount > 100) goUp = true;
            else if (bobbingUpwardsCount < -100) goUp = false;

            transform.position += new Vector3(0, (float)bobbingUpwardsCount / 10000, 0);
        }
    }

    IEnumerator Sink()
    {
        float sinkSmootheness = 100;
        for (int i = 0; i < sinkSmootheness; i++)
        {
            float sinkRemining = 1 - (i / sinkSmootheness);
            transform.localScale *= new Vector2(sinkRemining, sinkRemining);
            GetComponent<SpriteRenderer>().color *= new Vector4(sinkRemining, sinkRemining, sinkRemining, sinkRemining); // Fades out & turns to black

            yield return new WaitForSeconds(sinkSeconds / sinkSmootheness);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GetComponent<Collider2D>().enabled = false;
            startScale = transform.localScale;
            StartCoroutine(Sink());

            // Creates audio player object and assgins the clip given to the original colliding
            // object so if it's deleted before it finishes playing it wont be cut off
            GameObject AudioPlayer = new GameObject(this.name + " Audio Player");

            AudioPlayer.AddComponent<AudioSource>();

            AudioPlayer.GetComponent<AudioSource>().clip = objectAudio[Random.Range(0, objectAudio.Length)];
            AudioPlayer.GetComponent<AudioSource>().volume = (volumeSpawn / 100) * PlayerPrefs.GetFloat("Volume", 1);
            AudioPlayer.GetComponent<AudioSource>().Play();

            Destroy(AudioPlayer, AudioPlayer.GetComponent<AudioSource>().clip.length);
        }
    }
}