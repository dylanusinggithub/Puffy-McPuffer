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

            Lock.transform.localScale = new Vector3(LockSize, 1, 1);
            Lock.transform.GetChild(0).localScale = new Vector3(1 / LockSize, Lock.transform.GetChild(0).localScale.y, 1); // Left Wall
            Lock.transform.GetChild(1).localScale = new Vector3(1 / LockSize, Lock.transform.GetChild(1).localScale.y, 1); // Right Wall

            Lock.transform.GetChild(1).GetChild(1).transform.localPosition = new Vector2(-7 * LockSize, 3); // Cill Line
            Lock.transform.GetChild(1).GetChild(1).transform.localScale = new Vector2(LockSize + Mathf.Lerp(0, 0.35f, LockSize - 1), 0.5f);

            Lock.transform.GetChild(2).localScale = new Vector3(1 / LockSize, 0.9f, 1); // Back Wall
            Lock.transform.GetChild(2).GetComponent<SpriteRenderer>().size = new Vector2(14 * LockSize, 9);
        }

        LockBackground.transform.GetChild(1).position = new Vector3(LockSize * -14 - 6, 6, 0); // Fake Lock
        LockBackground.transform.GetChild(1).GetChild(1).GetChild(1).localPosition = new Vector2(-7 * LockSize, 3.75f); // Fake Lock's Cill Line 

        LockBackground.transform.GetChild(2).position = new Vector3(LockSize * -21 - 7, -5, 0); // Fake Water
        LockBackground.transform.GetChild(2).localScale = new Vector3(Mathf.Lerp(1.1f, 1.8f, LockSize - 1), 7, 0); // Fake Water
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