using System.Collections;
using TMPro;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    [SerializeField]
    GameObject createObject;

    [SerializeField]
    GameObject createWarning;
    GameObject Create;

    [SerializeField]
    GameObject obstacleObject;

    [SerializeField]
    GameObject obstacleWarning;
    GameObject Obstacle;

    float timeWait;

    GameObject Puffy, createText;
    float startSpeed = -0.04f;
    Vector2 spawnPos = new Vector2(0, 2);

    [SerializeField, Range(0.1f, 2)]
    float CinimaticSeconds;

    GameObject CinamaticBars;

    public enum Animation { MoveToCentre, SpawnCreate, WaitCreate, SpawnObstacle, WaitObstacle}
    public Animation state;

    private void Start()
    {
        Puffy = GameObject.Find("Player");
        Puffy.GetComponent<PuffyController>().enabled = false;

        createText = GameObject.Find("CreateText");

        CinamaticBars = GameObject.Find("Cinematic Bars");

        state = Animation.MoveToCentre;

        if (PlayerPrefs.GetString("showTutorial", "False") == "False")
        {
            CinamaticBars.SetActive(false);
            this.enabled = false;

            StartCoroutine(WaitTillPlay());
            GetComponent<WaterController>().enabled = true;
            GetComponent<ObjectDropper>().enabled = true;

            GetComponent<NEWLockBalancing>().state = NEWLockBalancing.GameState.Play;
        }
        else createText.GetComponent<TextMeshProUGUI>().text = "Intro Tutorial";
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (state)
        {
            case Animation.MoveToCentre:
                {
                    if (Puffy.transform.position.x < 0)
                    {
                        Puffy.transform.position = new Vector2(0, Puffy.transform.position.y);
                        Create = Instantiate(createWarning, spawnPos, Quaternion.identity);

                        timeWait = 1.5f;

                        state = Animation.SpawnCreate;
                    }
                    Puffy.transform.Translate(new Vector2(startSpeed, 0));
                }
                break;
            case Animation.SpawnCreate:
                {
                    if (timeWait < 0)
                    {
                        Destroy(Create);
                        Create = Instantiate(createObject, spawnPos, Quaternion.identity);
                        Create.AddComponent<Rigidbody2D>();
                        Create.AddComponent<BoxCollider2D>().isTrigger = true;

                        timeWait = Create.GetComponent<ObjectSettings>().waitSeconds * 0.9f;

                        state = Animation.WaitCreate;
                    }
                    else timeWait -= Time.deltaTime;
                }
                break;
            case Animation.WaitCreate:
                {
                    if (Puffy.GetComponent<Collider2D>().OverlapPoint(Create.transform.position)) // Hits puffy
                    {
                        // Creates audio player object and assgins the clip given to the original colliding
                        // object so if it's deleted before it finishes playing it wont be cut off
                        GameObject AudioPlayer = new GameObject(Create.name + " Audio Player");

                        AudioPlayer.AddComponent<AudioSource>();

                        AudioPlayer.GetComponent<AudioSource>().clip = Create.GetComponent<AudioSource>().clip;
                        AudioPlayer.GetComponent<AudioSource>().volume *= PlayerPrefs.GetFloat("Volume", 0.5f);
                        AudioPlayer.GetComponent<AudioSource>().Play();

                        Destroy(AudioPlayer, AudioPlayer.GetComponent<AudioSource>().clip.length);

                        state = Animation.SpawnObstacle;

                        timeWait = Obstacle.GetComponent<ObjectSettings>().waitSeconds * 0.9f;

                        GetComponent<NEWLockBalancing>().CollectObject(Create);

                    }
                }
                break;
            case Animation.SpawnObstacle:
                {
                    if (timeWait < 0)
                    {
                        Destroy(Obstacle);
                        Obstacle = Instantiate(obstacleObject, spawnPos, Quaternion.identity);
                        Obstacle.AddComponent<Rigidbody2D>();
                        Obstacle.AddComponent<BoxCollider2D>().isTrigger = true;

                        state = Animation.WaitObstacle;
                    }
                    else timeWait -= Time.deltaTime;
                }
                break;
            case Animation.WaitObstacle:

                {
                    if (Puffy.GetComponent<Collider2D>().OverlapPoint(Obstacle.transform.position)) // Hits puffy
                    {

                        // I KNOW THIS IS HORRIBLE I JUST WANT THIS DONE
                        // Creates audio player object and assgins the clip given to the original colliding
                        // object so if it's deleted before it finishes playing it wont be cut off
                        GameObject AudioPlayer = new GameObject(Obstacle.name + " Audio Player");

                        AudioPlayer.AddComponent<AudioSource>();

                        AudioPlayer.GetComponent<AudioSource>().clip = Obstacle.GetComponent<AudioSource>().clip;
                        AudioPlayer.GetComponent<AudioSource>().volume *= PlayerPrefs.GetFloat("Volume", 0.5f);
                        AudioPlayer.GetComponent<AudioSource>().Play();

                        Destroy(AudioPlayer, AudioPlayer.GetComponent<AudioSource>().clip.length);

                        GetComponent<NEWLockBalancing>().CollectObject(Obstacle);

                        Puffy.GetComponent<PuffyController>().enabled = true;
                        GetComponent<WaterController>().enabled = true;
                        GetComponent<ObjectDropper>().enabled = true;

                        GetComponent<NEWLockBalancing>().state = NEWLockBalancing.GameState.Play;

                        StartCoroutine(FadeOutBars());

                        this.enabled = false;
                    }
                }
                break;
        }
    }

    IEnumerator WaitTillPlay()
    {
        yield return new WaitForSeconds(1);
        Puffy.GetComponent<PuffyController>().enabled = true;
    }

    IEnumerator FadeOutBars()
    {
        float smootheness = 100;
        for (int i = 0; i < smootheness; i++)
        {
            yield return new WaitForSeconds(CinimaticSeconds / smootheness);
            float scale = Mathf.Lerp(0, 2 / CinamaticBars.transform.GetChild(0).GetComponent<RectTransform>().rect.height, i / smootheness);

            CinamaticBars.transform.localScale += new Vector3(0, scale, 0);
        }
    }
}
