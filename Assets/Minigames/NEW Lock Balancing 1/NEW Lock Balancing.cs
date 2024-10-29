using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class NEWLockBalancing : MonoBehaviour
{
    // Boat Movement
    GameObject player;

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

    int perlinX = 0;
    int perlinY = 50;

    int perlinR = 0;

    // Gameplay
    [Header("Gameplay")]
    [SerializeField, Range(5f, 30f)]
    float startTime = 10;
    float timeRemaining;

    Text Timer;
    bool gameFailed;

    [SerializeField, Range(1f, 10f)]
    float waterMaxHeight = 5;
    float waterHeight;


    [SerializeField, Range(0f, 100f)]
    int mouseStrength = 0;

    [SerializeField]
    bool mouseInvetered;

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.Find("Player");
        Timer = GameObject.Find("Timer").GetComponent<Text>();

        timeRemaining = startTime;
    }

    void FixedUpdate()
    {
        if (!gameFailed)
        {
            moveBoat();
            updateTimer();
            steerBoat();
            increaseHeight();
        }

    }
    void OnTriggerExit2D(Collider2D other)
    {
        Time.timeScale = 0;
        Timer.text = "Failure";
    }

    void GameOver()
    {
        gameFailed = true;
        Timer.text = "Failure";
    }

    void increaseHeight()
    {
        waterHeight = Mathf.Lerp(-4f, waterMaxHeight, (startTime - timeRemaining) / startTime);
    }

    void steerBoat()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos *= ((float)mouseStrength / 100);
        
        if(mouseInvetered) mousePos = -mousePos;

        player.transform.Translate(mousePos);
    }

    void updateTimer()
    {
        if (timeRemaining > 0)
        {
            // uses Time.deltaTime so we can change the frame rate if neccessary
            timeRemaining -= Time.deltaTime;
            Timer.text = timeRemaining.ToString("F2");
        }
        else Timer.text = "Finished!";
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

        Vector3 boatTransform = new Vector3(boatTransformX, boatTransformY, 0);

        player.transform.position = boatTransform;
        player.transform.eulerAngles = new Vector3(0, 0, player.transform.position.x * boatRotation);
    }
}