using System.Collections;
using System.ComponentModel;
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

    GameObject Wheel, Durability;


    float timeWait;

    [SerializeField]
    GameObject GauntletPopup;

    GameObject Puffy, createText;
    float startSpeed = -0.04f;
    Vector2 spawnPos = new Vector2(0, 2);

    public enum AnimationState { MoveToCentre, SpawnCreate, WaitCreate, SpawnObstacle, WaitObstacle}
    public AnimationState state;

    private void Start()
    {
        Puffy = GameObject.Find("Player");

        createText = GameObject.Find("CreateText");

        state = AnimationState.MoveToCentre;

        if (PlayerPrefs.GetString("showTutorial", "False") == "False")
        {
            GameObject.Find("Cinematic Bars").SetActive(false);

            GetComponent<WaterController>().enabled = true;
            GetComponent<ObjectDropper>().enabled = true;

            GetComponent<NEWLockBalancing>().state = NEWLockBalancing.GameState.Play;
            Destroy(GauntletPopup, GauntletPopup.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length);

            this.enabled = false;
        }
        else
        {
            createText.gameObject.SetActive(false);
            createText.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -170, 0);

            GauntletPopup.SetActive(false);
            GameObject.Find("Pause Menu").transform.GetChild(1).gameObject.SetActive(false);

            Wheel = GameObject.Find("Wheel");
            Wheel.SetActive(false);

            Durability = GameObject.Find("Durability");
            Durability.SetActive(false);

            Puffy.GetComponent<PuffyController>().enabled = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (state)
        {
            case AnimationState.MoveToCentre:
                {
                    if (Puffy.transform.position.x < 0)
                    {
                        Puffy.transform.position = new Vector2(0, Puffy.transform.position.y);
                        Create = Instantiate(createWarning, spawnPos, Quaternion.identity);

                        timeWait = 1.5f;

                        state = AnimationState.SpawnCreate;
                    }
                    Puffy.transform.Translate(new Vector2(startSpeed, 0));
                }
                break;
            case AnimationState.SpawnCreate:
                {
                    if (timeWait < 0)
                    {
                        Destroy(Create);
                        Create = Instantiate(createObject, spawnPos, Quaternion.identity);
                        Create.AddComponent<Rigidbody2D>();
                        Create.AddComponent<BoxCollider2D>().isTrigger = true;

                        state = AnimationState.WaitCreate;
                    }
                    else timeWait -= Time.deltaTime;
                }
                break;
            case AnimationState.WaitCreate:
                {
                    if (Puffy.GetComponent<Collider2D>().OverlapPoint(Create.transform.position + new Vector3(0, -0.4f))) // Hits puffy
                    {
                        // Creates audio player object and assgins the clip given to the original colliding
                        // object so if it's deleted before it finishes playing it wont be cut off
                        GameObject AudioPlayer = new GameObject(Create.name + " Audio Player");

                        Destroy(Create);

                        AudioPlayer.AddComponent<AudioSource>();

                        AudioPlayer.GetComponent<AudioSource>().clip = Create.GetComponent<AudioSource>().clip;
                        AudioPlayer.GetComponent<AudioSource>().volume *= PlayerPrefs.GetFloat("Volume", 0.5f);
                        AudioPlayer.GetComponent<AudioSource>().Play();

                        Destroy(AudioPlayer, AudioPlayer.GetComponent<AudioSource>().clip.length);

                        createText.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -130, 0);

                        state = AnimationState.SpawnObstacle;

                        timeWait = 1.5f;

                        GetComponent<NEWLockBalancing>().CollectObject(Create);

                    }
                }
                break;
            case AnimationState.SpawnObstacle:
                {
                    if (timeWait < 0)
                    {
                        Destroy(Obstacle);
                        Obstacle = Instantiate(obstacleObject, spawnPos, Quaternion.identity);
                        Obstacle.AddComponent<Rigidbody2D>();
                        Obstacle.AddComponent<BoxCollider2D>().isTrigger = true;

                        state = AnimationState.WaitObstacle;

                        GauntletPopup.SetActive(true);
                        Destroy(GauntletPopup, GauntletPopup.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length);
                    }
                    else timeWait -= Time.deltaTime;
                }
                break;
            case AnimationState.WaitObstacle:

                {
                    if (Puffy.GetComponent<Collider2D>().OverlapPoint(Obstacle.transform.position)) // Hits puffy
                    {

                        //// I KNOW THIS IS HORRIBLE I JUST WANT THIS DONE
                        //// Creates audio player object and assgins the clip given to the original colliding
                        //// object so if it's deleted before it finishes playing it wont be cut off
                        //GameObject AudioPlayer = new GameObject(Obstacle.name + " Audio Player");

                        //AudioPlayer.AddComponent<AudioSource>();

                        //AudioPlayer.GetComponent<AudioSource>().clip = Obstacle.GetComponent<AudioSource>().clip;
                        //AudioPlayer.GetComponent<AudioSource>().volume *= PlayerPrefs.GetFloat("Volume", 0.5f);
                        //AudioPlayer.GetComponent<AudioSource>().Play();

                        //Destroy(AudioPlayer, AudioPlayer.GetComponent<AudioSource>().clip.length);
                        Destroy(Obstacle, 2f);

                        GetComponent<NEWLockBalancing>().CollectObject(Obstacle);

                        Puffy.GetComponent<PuffyController>().enabled = true;
                        GetComponent<WaterController>().enabled = true;
                        GetComponent<ObjectDropper>().enabled = true;

                        GetComponent<NEWLockBalancing>().state = NEWLockBalancing.GameState.Play;
                        GameObject.Find("Pause Menu").transform.GetChild(1).gameObject.SetActive(true);

                        createText.gameObject.SetActive(true);

                        Wheel.SetActive(true);
                        Durability.SetActive(true);

                        GameObject.Find("Cinematic Bars").GetComponent<Animation>().Play();
                        this.enabled = false;
                    }
                }
                break;
        }
    }
}
