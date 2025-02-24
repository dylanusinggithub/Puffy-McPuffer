using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NEWLockBalancing : MonoBehaviour
{

    [Header("Gameplay")]
    [HideInInspector] public TextMeshProUGUI createText;

    [SerializeField]
    GameObject GameOver, Win;

    [HideInInspector] public int createCompletion = 10;
    int createCount;

    GameObject Puffy;

    float arrowStrength = 50;
    GameObject arrowMovement;

    ParticleSystemForceField windForce;

    public enum GameState { Cutscene, Play, Fail, Complete };
    public GameState state;

    WaterController WB;

    float cutsceneSpeed = -0.03f;

    private void Start()
    {
        WB = GetComponent<WaterController>();

        createText = GameObject.Find("CreateText").GetComponent<TextMeshProUGUI>();

        Puffy = GameObject.Find("Player");

        arrowMovement = GameObject.Find("Arrow Origin");
        windForce = transform.GetChild(0).GetChild(0).GetComponent<ParticleSystemForceField>();
    }


    void FixedUpdate()
    {
        switch (state)
        {
            case GameState.Play:
                {
                    DisplayWaterMovement();
                    if (Puffy.transform.position.y > 0) Camera.main.transform.position = new Vector3(0, Puffy.transform.position.y, -10);
                }
                break;
            case GameState.Fail:
                {
                    arrowMovement.SetActive(false);
                    createText.enabled = false;

                    Puffy.GetComponent<PuffyController>().enabled = false;
                    GetComponent<ObjectDropper>().enabled = false;

                    GameOver.SetActive(true);

                }
                break;
            case GameState.Complete:
                {
                    if (Puffy.transform.position.x >= -18) Puffy.transform.Translate(new Vector2(cutsceneSpeed * 2f, 0));
                    else GameObject.Find("Pause Menu").GetComponent<MenuButtons>().BTN_NextLevel();

                    if (Puffy.transform.position.x < -9) Win.SetActive(true);

                    Camera.main.transform.position = new Vector3(Mathf.Clamp(Puffy.transform.position.x - 2, -18, 0), 6, -10);

                    if (Camera.main.transform.position.x <= -18) Camera.main.transform.parent = null;

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
        arrowHead.transform.localScale = new Vector3(0.4f, arrowlength * (float)arrowStrength / 300, 1);

        Color arrowColour = Color.yellow + (Color.red - Color.yellow) * Mathf.Abs((arrowlength * (float)arrowStrength / 100) / 4);
        arrowBody.GetComponent<Image>().color = arrowColour;
        arrowHead.GetComponent<Image>().color = arrowColour;

        windForce.directionX = WB.boatTransformX;
    }

    public void CollectObject(GameObject Object)
    {
        if (Object.tag == "Collectable")
        {
            createCount++;
            if (createCount >= createCompletion)
            {
                arrowMovement.SetActive(false);
                createText.enabled = false;

                Puffy.GetComponent<PuffyController>().enabled = false;

                Camera.main.transform.parent = Puffy.transform;

                GameObject.Find("Durability").SetActive(false);

                state = GameState.Complete;
                Puffy.GetComponent<SpriteRenderer>().flipX = false;
            }
            Destroy(Object);
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
}