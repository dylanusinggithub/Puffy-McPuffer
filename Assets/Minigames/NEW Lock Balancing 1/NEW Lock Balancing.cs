using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NEWLockBalancing : MonoBehaviour
{

    [Header("Gameplay")]
    TextMeshProUGUI Loading, createText;

    [SerializeField]
    GameObject GameOver, Win;

    [SerializeField, Range(5f, 30f)]
    public float createCompletion = 10;
    float createCount;

    GameObject Puffy;

    [SerializeField, Range(0, 100)]
    float arrowStrength;
    GameObject arrowMovement;

    ParticleSystemForceField windForce;

    public enum GameState { Start, Play, Fail, Complete };
    public GameState state;

    WaterController WB;

    float cutsceneSpeed = -0.03f;

    private void Start()
    {
        WB = GetComponent<WaterController>();
        Loading = GameObject.Find("Loading").GetComponent<TextMeshProUGUI>();

        createText = GameObject.Find("CreateText").GetComponent<TextMeshProUGUI>();
        state = GameState.Start;

        Puffy = GameObject.Find("Player");

        arrowMovement = GameObject.Find("Arrow Origin");
        windForce = transform.GetChild(0).GetChild(0).GetComponent<ParticleSystemForceField>();
    }


    void FixedUpdate()
    {
        switch (state)
        {
            case GameState.Start:
                {
                    if (Puffy.transform.position.x < 0) state = GameState.Play;
                    else Puffy.transform.Translate(new Vector2(cutsceneSpeed, 0));
                }
                break;
            case GameState.Play:
                {
                    DisplayWaterMovement();
                    WB.enabled = true;
                    Loading.enabled = false;
                    createText.enabled = true;
                    Puffy.GetComponent<PuffyController>().enabled = true;
                    GetComponent<ObjectDropper>().enabled = true;

                }
                break;
            case GameState.Fail:
                {
                    arrowMovement.SetActive(false);

                    createText.enabled = true;
                    
                    Puffy.GetComponent<PuffyController>().enabled = false;
                    GetComponent<ObjectDropper>().enabled = false;
                    GameOver.SetActive(true);

                    Puffy.transform.Translate(new Vector2(cutsceneSpeed, 0));
                }
                break;
            case GameState.Complete:
                {
                    arrowMovement.SetActive(false);
                    createText.enabled = false;

                    Puffy.GetComponent<PuffyController>().enabled = false;
                    GetComponent<ObjectDropper>().enabled = false;

                    Win.SetActive(true);

                    Puffy.transform.Translate(new Vector2(cutsceneSpeed, 0));
                }
                break;
        }
    }

    void DisplayWaterMovement()
    {
        Transform arrowBody = arrowMovement.transform.GetChild(0);
        Transform arrowHead = arrowMovement.transform.GetChild(1);

        float arrowlength = WB.boatTransformX;

        arrowBody.transform.localScale = new Vector3(arrowlength * (float)arrowStrength / 100, 0.15f, 1);
        arrowBody.transform.position = new Vector2((arrowlength) / 2 * ((float)arrowStrength / 100), arrowBody.transform.position.y);

        arrowHead.transform.localScale = new Vector3(0.4f, arrowlength * (float)arrowStrength / 400, 1);
        arrowHead.transform.position = new Vector2(arrowlength * ((float)arrowStrength / 100), arrowBody.transform.position.y);


        Color arrowColour = Color.yellow + (Color.red - Color.yellow) * Mathf.Abs((arrowlength * (float)arrowStrength / 100) / 4);
        arrowBody.GetComponent<SpriteRenderer>().color = arrowColour;
        arrowHead.GetComponent<SpriteRenderer>().color = arrowColour;

        windForce.directionX = WB.boatTransformX;
    }

    public void CollectObject(GameObject Object)
    {
        if (Object.tag == "Collectable")
        {
            createCount++;
            if (createCount >= createCompletion)
            {
                state = GameState.Complete;
                Puffy.GetComponent<SpriteRenderer>().flipX = false;
            }
            StartCoroutine(GetComponent<WaterController>().changeHeight(true));
        }
        else if (createCount > 0)
        {
            createCount--;
            StartCoroutine(GetComponent<WaterController>().changeHeight(false));
        }
        else return; // Tries to go into negatives

        createText.GetComponent<TextMeshProUGUI>().text = createCount + " / " + createCompletion;
    }

    public void BTN_Retry()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BTN_Exit()
    {
        SceneManager.LoadScene("Level Select Map");
    }

}