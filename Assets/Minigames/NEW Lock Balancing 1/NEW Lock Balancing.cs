using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class NEWLockBalancing : MonoBehaviour
{
    // Boat Movement
    GameObject puffy;
    GameObject WaterSimple;

    [Header("Boat Movement")]
    [SerializeField, Range(0f, 5f)]
    float perlinStepSizeX = 1;

    [SerializeField, Range(0f, 5f)]
    float perlinStepSizeY = 1;

    [SerializeField, Range(0f, 5f)]
    float perlinStepSizeR = 1;

    [SerializeField, Range(0f, 100f)]
    float strengthX = 6;

    [SerializeField, Range(0f, 100f)]
    float strengthY = 1;

    [SerializeField, Range(0f, 100f)]
    float strengthR = 1;

    Vector3 boatTransform;
    Vector3 waterTransform;

    int perlinX = 0;
    int perlinY = 50;

    int perlinR = 0;

    // Gameplay
    [Header("Gameplay")]
    [SerializeField, Range(5f, 30f)]
    float timeStart = 10;
    float timeRemaining;

    Text Timer;

    float puffyScaleX;

    [SerializeField, Range(0f, 10f)]
    float collisionX = 5;

    [SerializeField]
    GameObject GameOver;

    // Obstacles 
    [Header("Obstacles Settings")]

    [SerializeField]
    GameObject[] obstacles;
    List<GameObject> obstaclesCount = new List<GameObject>();

    [SerializeField, Range(0, 3f)]
    float obstaclesSpawnRate;
    float obstaclesSpawnCooldown;

    [SerializeField, Range(0, 8f)]
    float obstaclesSpawnRange;

    [SerializeField, Range(0, 100)]
    int obstaclesChance;

    [SerializeField, Range(0f, 1f)]
    float obstacleScaleVariance;

    public bool puffyStunned;

    // Water Setting
    [Header("Water Settings")]
    [SerializeField, Range(1f, 4f)]
    float waterMaxHeight = 5;

    [SerializeField, Range(1f, 4f)]
    float waterMinHeight = 3.5f;

    [SerializeField]
    GameObject waterObject;

    [SerializeField, Range(0f, 1f)]
    float waterOffset = 1;

    float waterHeight;

    // Arrow
    [Header("Arrow Settings")]

    [SerializeField, Range(0f, 100f)]
    int arrowStrength = 50;

    [SerializeField]
    GameObject arrowMovement;

    // Controls
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

    float playerMovement;

    enum GameState { Start, Play, Fail, Complete };
    GameState state;

    // Start is called before the first frame update
    void Awake()
    {
        puffy = GameObject.Find("Player");
        Timer = GameObject.Find("Timer").GetComponent<Text>();
        WaterSimple = GameObject.Find("WaterSimple");

        Timer.enabled = false;

        timeRemaining = timeStart;
        state = GameState.Start;

        waterHeight = -waterMinHeight;
    }

    void FixedUpdate()
    {

        switch (state)
        {
            case GameState.Start:
                {
                    animationMoveLeft();
                }
                break;

            case GameState.Play:
                {
                    moveBoat();
                    updateTimer();
                    steerBoat();
                    increaseHeight();
                    checkCollision();
                    displayArrow();
                    flipPuffy();
                    spawnObstacles();
                }
                break;

            case GameState.Fail:
                {
                    GameOver.gameObject.SetActive(true);
                }
                break;

            case GameState.Complete:
                {
                    animationMoveLeft();
                }
                break;
        }
    }
    public void BTN_Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BTN_Exit()
    {
        SceneManager.LoadScene("Level Select Map");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Vector2.zero, new Vector2(collisionX, 10));
    }

    void spawnObstacles()
    {
        if(obstaclesSpawnCooldown < 0) 
        { 
            if (Random.Range(1, obstaclesChance) < obstaclesChance)
            {
                Vector3 pos = new Vector3(Random.Range(-obstaclesSpawnRange, obstaclesSpawnRange), 5, 0);
                GameObject GOObstatical = Instantiate(obstacles[Random.Range(0, obstacles.Length - 1)], pos, quaternion.identity);

                obstaclesSpawnCooldown = obstaclesSpawnRate;
                obstaclesCount.Add(GOObstatical);

                float scale = Random.Range(-obstacleScaleVariance, obstacleScaleVariance);
                GOObstatical.transform.localScale = new Vector3(scale, scale, 1);

                for(int i = 0; i < obstaclesCount.Count; i++)
                {
                    if (obstaclesCount[i].transform.position.y < -4f)
                    {
                        Destroy(obstaclesCount[i]);
                        obstaclesCount.Remove(obstaclesCount[i]);
                    }
                }
            }
        }
        else obstaclesSpawnCooldown -= Time.deltaTime;

    }

    void flipPuffy()
    {
        if (puffy.transform.position.x < 0) puffy.GetComponent<SpriteRenderer>().flipX = false;
        else puffy.GetComponent<SpriteRenderer>().flipX = true;
    }

    void displayArrow()
    {
        Transform arrowBody = arrowMovement.transform.GetChild(0);
        Transform arrowHead = arrowMovement.transform.GetChild(1);

        float arrowlength = boatTransform.x + playerMovement;

        arrowBody.transform.localScale = new Vector3(arrowlength * (float)arrowStrength / 100, 0.15f, 1);
        arrowBody.transform.position = new Vector2((arrowlength) / 2 * ((float)arrowStrength / 100), 3.9f);

        arrowHead.transform.position = new Vector2(arrowlength * ((float)arrowStrength / 100), 3.9f);
        arrowHead.transform.localScale = new Vector3(0.4f, arrowlength * (float)arrowStrength / 400, 1);


        Color arrowColour = Color.yellow + (Color.red - Color.yellow) * Mathf.Abs((arrowlength * (float)arrowStrength / 100) / 4);
        arrowBody.GetComponent<SpriteRenderer>().color = arrowColour;
        arrowHead.GetComponent<SpriteRenderer>().color = arrowColour;
    }

    void checkCollision()
    {
        if (Mathf.Abs(puffy.transform.position.x) > collisionX)
        {
            state = GameState.Fail;
            Timer.text = "Failure";
        }
    }

    void animationMoveLeft()
    {
        if (state == GameState.Start)
            if (puffy.transform.position.x < 0)
            {
                state = GameState.Play;
                Timer.enabled = true;
            }

        if (state == GameState.Complete)
        {
            // So puffy always faces left at end
            puffy.GetComponent<SpriteRenderer>().flipX = false;
            if (puffy.transform.position.x < -11) Time.timeScale = 0;
        }



        if (perlinY == 100 / perlinStepSizeY) perlinY = 0;
        else perlinY++;

        if (perlinR == 100 / perlinStepSizeR) perlinR = 0;
        else perlinR++;

        float boatTransformY = Mathf.PerlinNoise1D(((float)perlinY / 100) * perlinStepSizeY) - 0.5f;
        float boatRotation = Mathf.PerlinNoise1D(((float)perlinR / 100) * perlinStepSizeR) - 0.5f;

        boatTransformY *= strengthY;
        boatTransformY += waterHeight;

        boatRotation *= strengthR;

        Vector3 boatTransform = new Vector3(puffy.transform.position.x - 2 * Time.deltaTime, boatTransformY, 1);

        puffy.transform.position = boatTransform;
        puffy.transform.eulerAngles = new Vector3(0, 0, boatRotation);
    }

    void increaseHeight()
    {
        float timeElapsed = (timeStart - timeRemaining) / timeStart;
        waterHeight = Mathf.Lerp(-waterMinHeight, waterMaxHeight, timeElapsed);

        float waterObjectScale = Mathf.Lerp(waterOffset, waterMaxHeight + waterOffset + waterMinHeight, timeElapsed);
        waterObject.transform.localScale = new Vector3(waterObject.transform.localScale.x, waterObjectScale, 1);
    }

    void steerBoat()
    {
        if (Input.GetButton("Horizontal") && !puffyStunned)
        {

            playerMovement += Input.GetAxis("Horizontal") * ((float)keyStrength / 100);
        }
        else if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && !puffyStunned)
        {
            float mouseDist = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
            Mathf.Clamp(mouseDist, -mouseMaxDist, mouseMaxDist);

            if (mouseInverted) mouseDist *= -1;

            playerMovement += mouseDist * ((float)mouseStrength / 100);
        }
        else
        {
            // Decelerates by X amount (divided by 100 to make it more reasoanble)
            if (Mathf.Abs(playerMovement) > (float)decelerationAmount / 100)
            {
                if (playerMovement > 0) playerMovement -= (float)decelerationAmount / 100;
                if (playerMovement < 0) playerMovement += (float)decelerationAmount / 100;
            }
            else playerMovement = 0;
        }
        puffy.transform.Translate(new Vector2(playerMovement, 0));
    }

    void updateTimer()
    {
        if (timeRemaining > 0)
        {
            // uses Time.deltaTime so we can change the frame rate if neccessary
            timeRemaining -= Time.deltaTime;
            Timer.text = timeRemaining.ToString("F2");
        }
        else
        {
            state = GameState.Complete;
            Timer.text = "Complete!";
        }
    }

    void moveBoat()
    {
        if (perlinX == 100 / perlinStepSizeX) perlinX = 0;
        else perlinX++;

        if (perlinY == 100 / perlinStepSizeY) perlinY = 0;
        else perlinY++;

        if (perlinR == 100 / perlinStepSizeR) perlinR = 0;
        else perlinR++;

        float boatTransformX = Mathf.PerlinNoise1D(((float)perlinX / 100) * perlinStepSizeX) - 0.5f;
        float boatTransformY = Mathf.PerlinNoise1D(((float)perlinY / 100) * perlinStepSizeY) - 0.5f;

        float boatRotation = Mathf.PerlinNoise1D(((float)perlinR / 100) * perlinStepSizeR) - 0.5f;

        boatTransformX *= strengthX;

        boatTransformY *= strengthY;
        boatTransformY += waterHeight;
        boatRotation *= strengthR;

        boatTransform = new Vector3(boatTransformX, boatTransformY, 0);

        puffy.transform.position = boatTransform;
        puffy.transform.eulerAngles = new Vector3(0, 0, puffy.transform.position.x * boatRotation);
    }
}