using System;
using UnityEngine;
using UnityEngine.UI;

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
    float timeStart = 10;
    float timeRemaining;

    Text Timer;

    [SerializeField, Range(0f, 10f)]
    float collisionX = 5;

    [SerializeField, Range(1f, 4f)]
    float waterMaxHeight = 5;
    float waterHeight = -3.8f;

    // Controls
    [Header("Controls")]
    [SerializeField, Range(0f, 100f)]
    int mouseStrength = 0;

    [SerializeField]
    bool mouseInverted;


    enum GameState { Start, Play, Fail, Complete };
    GameState state;

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.Find("Player");
        Timer = GameObject.Find("Timer").GetComponent<Text>();

        Timer.enabled = false;

        timeRemaining = timeStart;
        state = GameState.Start;
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
                }
                break;

            case GameState.Complete:
                {
                    animationMoveLeft();
                }
                break;
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Vector2.zero, new Vector2(collisionX, 10));
    }

    void checkCollision()
    {
        if (Mathf.Abs(player.transform.position.x) > collisionX)
        {
            state = GameState.Fail;
            Timer.text = "Failure";
        }
    }

    void animationMoveLeft()
    {
        if (state == GameState.Start)
            if (player.transform.position.x < 0)
            {
                state = GameState.Play;
                Timer.enabled = true;
            }

        if (state == GameState.Complete)
            if (player.transform.position.x < -11) Time.timeScale = 0;


        if (perlinY == 100 / perlinStepSizeY) perlinY = 0;
        else perlinY++;

        if (perlinR == 100 / perlinStepSizeR) perlinR = 0;
        else perlinR++;

        float boatTransformY = Mathf.PerlinNoise1D(((float)perlinY / 100) * perlinStepSizeY) - 0.5f;
        float boatRotation = Mathf.PerlinNoise1D(((float)perlinR / 100) * perlinStepSizeR) - 0.5f;

        boatTransformY *= strengthY;
        boatTransformY += waterHeight;

        boatRotation *= strengthR;

        Vector3 boatTransform = new Vector3(player.transform.position.x - 2 * Time.deltaTime, boatTransformY, 1);

        player.transform.position = boatTransform;
        player.transform.eulerAngles = new Vector3(0, 0, boatRotation);
    }

    void increaseHeight()
    {
        float timeElapsed = (timeStart - timeRemaining) / timeStart;
        waterHeight = Mathf.Lerp(-3.8f, waterMaxHeight, timeElapsed);
    }

    void steerBoat()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos *= ((float)mouseStrength / 100);

        if (mouseInverted) mousePos = -mousePos;

        player.transform.Translate(new Vector2(mousePos.x, 0));
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

        Vector3 boatTransform = new Vector3(boatTransformX, boatTransformY, 0);

        player.transform.position = boatTransform;
        player.transform.eulerAngles = new Vector3(0, 0, player.transform.position.x * boatRotation);
    }
}