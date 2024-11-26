using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Collections;

public class NEWLockBalancing : MonoBehaviour
{

    #region Boat Movement
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
    #endregion

    #region Gameplay
    [Header("Gameplay")]
    GameObject puffy;

    [SerializeField]
    Text Loading;

    [SerializeField, Range(5f, 30f)]
    float createCompletion = 10;
    float createCount;

    Text createText;

    float puffyScaleX;

    [SerializeField, Range(0f, 10f)]
    float collisionX = 5;

    [SerializeField]
    GameObject GameOver;
    
    [SerializeField]
    GameObject Win;
    #endregion

    #region Objects

    [Header("Objects Settings")]
    [SerializeField]
    GameObject[] Collectable;

    [SerializeField]
    GameObject[] Obstacle;
    List<GameObject> CollectableCount = new List<GameObject>(), ObstacleCount = new List<GameObject>();


    [SerializeField]
    GameObject CollectablePopup, ObstaclePopup;

    [SerializeField, Range(0, 3f)]
    float popupDelay;


    [SerializeField, Range(0, 10f)]
    float CollectableSpawnRate;

    [SerializeField, Range(0, 10f)]
    float ObstacleSpawnRate;
    float CollectableSpawnCooldown, ObstacleSpawnCooldown;

    [SerializeField, Range(0, 8f)]
    float ObjectSpawnRange;


    [SerializeField, Range(0, 100)]
    int CollectableChance;

    [SerializeField, Range(0, 100)]
    int ObstacleChance;


    [SerializeField, Range(0, 180)]
    int ObstacleRotation;

    #endregion

    #region Water Height
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
    float waterPecentage = 0;
    #endregion

    #region Water Movement
    GameObject LeafParticle;

    [Header("Water Movement Settings")]
    [SerializeField, Range(0, 100)]
    int waterMovementStrength = 50;

    [SerializeField, Range(0f, 2f)]
    float waterHeightSeconds = 1;

    [SerializeField]
    GameObject waterMovement, waterCill;
    #endregion

    #region Controls
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
    #endregion

    enum GameState { Start, Play, Fail, Complete };
    GameState state;

    // Start is called before the first frame update
    void Awake()
    {
        LeafParticle = GameObject.Find("LeafParticles");

        puffy = GameObject.Find("Player");
        createText = GameObject.Find("CreateText").GetComponent<Text>();

        createText.text = createCount + " / " + createCompletion;
        createText.enabled = false;

        state = GameState.Start;

        waterObject.transform.localScale = new Vector3(waterObject.transform.localScale.x, waterOffset, 1);
        waterHeight = -waterMinHeight;

        waterCill.transform.position = new Vector3(waterCill.transform.position.x, waterMaxHeight, 0);
        waterCill.transform.GetChild(0).transform.position = new Vector3(0, waterMaxHeight, 0);
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
                    steerBoat();
                    checkCollision();
                    displayWaterMovement();
                    flipPuffy();
                    StartCoroutine(spawnObject());
                    cleanObjects();
                }
                break;

            case GameState.Fail:
                {
                    createText.text = "Failure";
                    GameOver.gameObject.SetActive(true);
                }
                break;

            case GameState.Complete:
                {
                    animationMoveLeft();
                    Win.gameObject.SetActive(true);
                }
                break;
        }
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Vector2.zero, new Vector2(collisionX, 10));
    }

    private IEnumerator spawnObject()
    {
        bool isCollect = false, isObstacle = false;

        if (CollectableSpawnCooldown < 0)
        {
            isCollect = true;
            CollectableSpawnCooldown = CollectableSpawnRate;
            if (Random.Range(1, 100) > CollectableChance) yield break;
        }
        else CollectableSpawnCooldown -= Time.deltaTime;

        if (ObstacleSpawnCooldown < 0)
        {
            isObstacle = true;
            ObstacleSpawnCooldown = ObstacleSpawnRate;
            if (Random.Range(1, 100) > ObstacleChance) yield break;
        }
        else ObstacleSpawnCooldown -= Time.deltaTime;

        if (isCollect || isObstacle)
        {
            Vector3 pos = new Vector3(Random.Range(-ObjectSpawnRange, ObjectSpawnRange), 4, 0);

            GameObject popup;
            if (isCollect) popup = Instantiate(CollectablePopup, pos, quaternion.identity);
            else popup = Instantiate(ObstaclePopup, pos, quaternion.identity);

            yield return new WaitForSeconds(popupDelay);
            Destroy(popup);

            GameObject GOObject;
            pos += new Vector3(0, 4, 0);

            if (isCollect)
            {
                GOObject = Instantiate(Collectable[Random.Range(0, Collectable.Length)], pos, quaternion.identity);
                CollectableCount.Add(GOObject);
            }
            else
            {
                GOObject = Instantiate(Obstacle[Random.Range(0, Obstacle.Length)], pos, quaternion.identity);
                GOObject.transform.Rotate(new Vector3(0, 0, Random.Range(-ObstacleRotation, ObstacleRotation)));
                ObstacleCount.Add(GOObject);
            }
        }
    }

    void cleanObjects()
    {
        for (int i = 0; i < CollectableCount.Count; i++)
        {
            if (CollectableCount[i].transform.position.y < -4f)
            {
                Destroy(CollectableCount[i]);
                CollectableCount.Remove(CollectableCount[i]);
            }
        }

        for (int i = 0; i < ObstacleCount.Count; i++)
        {
            if (ObstacleCount[i].transform.position.y < -4f)
            {
                Destroy(ObstacleCount[i]);
                ObstacleCount.Remove(ObstacleCount[i]);
            }
        }
    }

    void flipPuffy()
    {
        if (puffy.transform.position.x < 0) puffy.GetComponent<SpriteRenderer>().flipX = false;
        else puffy.GetComponent<SpriteRenderer>().flipX = true;
    }

    void displayWaterMovement()
    {
        float rotation = Mathf.Lerp(-1, 1, (boatTransform.x + collisionX) / (collisionX * 2));
        waterMovement.transform.localScale = new Vector3(rotation * ((float)waterMovementStrength/100), waterMovement.transform.localScale.y, waterMovement.transform.localScale.z);
    }

    void checkCollision()
    {
        if (Mathf.Abs(puffy.transform.position.x) > collisionX) state = GameState.Fail;
    }

    void animationMoveLeft()
    {
        if (state == GameState.Start)
            if (puffy.transform.position.x < 0)
            {
                state = GameState.Play;
                createText.enabled = true;
                Loading.enabled = false;
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

    // get called from Puffy Controller
    public IEnumerator changeHeight(bool increase)
    {
        if (increase)
        {
            createCount++;
            if (createCount >= createCompletion) state = GameState.Complete;
        }
        else if (createCount > 0) createCount--;
        else yield break;

        createText.text = createCount + " / " + createCompletion;

        int waterSmoothness = 100;
        if (increase)
        {
            for (int i = 0; i < waterSmoothness; i++)
            {
                yield return new WaitForSeconds(waterHeightSeconds / waterSmoothness);
                waterPecentage += waterHeightSeconds / waterSmoothness;

                waterHeight = Mathf.Lerp(-waterMinHeight, waterMaxHeight, waterPecentage / createCompletion);
                float waterObjectScale = Mathf.Lerp(0, waterMaxHeight + waterMinHeight, waterPecentage / createCompletion) + waterOffset;

                waterObject.transform.localScale = new Vector3(waterObject.transform.localScale.x, waterObjectScale, 1);
            }
        }
        else
        {
            for (int i = waterSmoothness; i > 0; i--)
            {
                yield return new WaitForSeconds(waterHeightSeconds / waterSmoothness);
                waterPecentage -= waterHeightSeconds / waterSmoothness;

                waterHeight = Mathf.Lerp(-waterMinHeight, waterMaxHeight, waterPecentage / createCompletion);
                float waterObjectScale = Mathf.Lerp(0, waterMaxHeight + waterMinHeight, waterPecentage / createCompletion) + waterOffset;

                waterObject.transform.localScale = new Vector3(waterObject.transform.localScale.x, waterObjectScale, 1);
            }
        }


    }

    void steerBoat()
    {
        if (Input.GetButton("Horizontal"))
        {

            playerMovement += Input.GetAxis("Horizontal") * ((float)keyStrength / 100);
        }
        else if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
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
        LeafParticle.transform.GetChild(0).GetComponent<ParticleSystemForceField>().directionX = boatTransformX;

        boatTransformY *= strengthY;
        boatTransformY += waterHeight;
        boatRotation *= strengthR;

        boatTransform = new Vector3(boatTransformX, boatTransformY, 0);

        puffy.transform.position = boatTransform;
        puffy.transform.eulerAngles = new Vector3(0, 0, puffy.transform.position.x * boatRotation);
    }
}