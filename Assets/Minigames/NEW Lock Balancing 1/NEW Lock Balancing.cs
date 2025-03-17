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

    [SerializeField, Range(1, 2)] public float LockSize;

    GameObject Puffy;

    float arrowStrength = 50;
    GameObject arrowMovement;

    ParticleSystemForceField windForce;

    public enum GameState { Cutscene, Play, Fail, Complete };
    public GameState state;

    WaterController WB;

    float cutsceneSpeed = -0.03f;

    private void Awake()
    {
        WB = GetComponent<WaterController>();

        createText = GameObject.Find("CreateText").GetComponent<TextMeshProUGUI>();

        Puffy = GameObject.Find("Player");

        arrowMovement = GameObject.Find("Arrow Origin");
        windForce = transform.GetChild(0).GetChild(0).GetComponent<ParticleSystemForceField>();

        ScaleLock();
    }

    private void OnValidate()
    {
        ScaleLock();
    }

    void ScaleLock()
    {
        GameObject LockBackground = GameObject.Find("Lock Background");

        for (int i = 0; i < 2; i++)
        {
            GameObject Lock = LockBackground.transform.GetChild(i).gameObject;

            Lock.transform.GetChild(0).localPosition = new Vector3(Mathf.Lerp(-7.25f, -14.5f, LockSize - 1), Mathf.Lerp(-1, -3.5f, i)); // Left Wall
            Lock.transform.GetChild(1).localPosition = new Vector3(Mathf.Lerp(7.25f, 14.5f, LockSize - 1), Mathf.Lerp(-1, -3.5f, i)); // Right Wall

            Lock.transform.GetChild(2).GetChild(0).transform.localScale = new Vector2(LockSize * 12, 0.1f); // Cill Line
            Lock.transform.GetChild(2).GetChild(1).transform.localPosition = new Vector2(LockSize * 6.6f, 0); // Cill Mark

            Lock.transform.GetChild(3).GetComponent<SpriteRenderer>().size = new Vector2(14 * LockSize, 9); // Background

            if (i == 0)
            {
                Lock.transform.GetChild(4).localPosition = new Vector2(LockSize * -7.5f, -5);
                Lock.transform.GetChild(4).localScale = new Vector2(LockSize * 0.75f, 1);
            }
        }

        LockBackground.transform.GetChild(1).localPosition = new Vector2(LockSize * -14.5f - 5, 6);

        LockBackground.transform.GetChild(2).localPosition = new Vector2(Mathf.Lerp(-26, -48, LockSize - 1), -5);
        LockBackground.transform.GetChild(2).localScale = new Vector2(Mathf.Lerp(0.95f, 1.7f, LockSize - 1), 7);


    }

    void FixedUpdate()
    {
        switch (state)
        {
            case GameState.Play:
                {
                    DisplayWaterMovement();

                    if(Puffy.transform.position.y > 0) Camera.main.transform.position = new Vector3(Puffy.transform.position.x, Puffy.transform.position.y, -10);
                    else if (Mathf.Abs(Puffy.transform.position.x) < LockSize * 5 - 3) Camera.main.transform.position = new Vector3(Puffy.transform.position.x, Camera.main.transform.position.y, -10);

                }
                break;
            case GameState.Fail:
                {
                    arrowMovement.SetActive(false);
                    createText.enabled = false;

                    Puffy.GetComponent<PuffyController>().enabled = false;
                    GetComponent<ObjectDropper>().enabled = false;

                    GameOver.SetActive(true);
                    Time.timeScale = 0;
                }
                break;
            case GameState.Complete:
                {
                    if (Puffy.transform.position.x >= LockSize * -17) Puffy.transform.Translate(new Vector2(cutsceneSpeed * 2f, 0));
                    else GameObject.Find("Pause Menu").GetComponent<MenuButtons>().BTN_NextLevel();
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
                Camera.main.transform.localPosition += new Vector3(0, 8);

                GameObject.Find("Durability").SetActive(false);
                Win.SetActive(true);

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