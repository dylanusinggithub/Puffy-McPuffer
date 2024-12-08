using System.Collections;
using UnityEngine;

public class ObjectScript : MonoBehaviour
{
    [SerializeField]
    bool randomiseScale;

    [SerializeField, Range(-0.5f, 0.5f)]
    float startSpeed = 0;

    float sinkSeconds = 3;
    Vector2 startScale;

    [SerializeField]
    AudioClip[] objectAudio;

    private void OnValidate()
    {
        if (randomiseScale)
        {
            transform.localScale = new Vector3(Random.Range(0.5f, 1.5f), transform.localScale.y, transform.localScale.z);
            randomiseScale = false;
        }
    }


    private void FixedUpdate()
    {
        transform.Translate(new Vector2(0, startSpeed));   // don't ask why it's reversed, idk why
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
            GetComponent<BoxCollider2D>().enabled = false;
            startScale = transform.localScale;
            StartCoroutine(Sink());

            // Creates audio player object and assgins the clip given to the original colliding
            // object so if it's deleted before it finishes playing it wont be cut off
            GameObject AudioPlayer = new GameObject(this.name + " Audio Player");

            AudioPlayer.AddComponent<AudioSource>();

            AudioPlayer.GetComponent<AudioSource>().clip = objectAudio[Random.Range(0, objectAudio.Length)];
            AudioPlayer.GetComponent<AudioSource>().volume *= PlayerPrefs.GetFloat("Volume");
            AudioPlayer.GetComponent<AudioSource>().Play();

            Destroy(AudioPlayer, AudioPlayer.GetComponent<AudioSource>().clip.length);
        }
    }
}