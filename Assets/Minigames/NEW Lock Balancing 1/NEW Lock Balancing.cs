using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NEWLockBalancing : MonoBehaviour
{

    [Header("Gameplay")]
    TextMeshProUGUI Loading, createText;

    [SerializeField]
    GameObject GameOver;
    
    [SerializeField]
    GameObject Win;

    [SerializeField, Range(5f, 30f)]
    public float createCompletion = 10;
    float createCount;

    [SerializeField]
    GameObject Puffy;
    public enum GameState { Start, Play, Fail, Complete };
    GameState state;

    WaterController WB;

    private void Start()
    {
        WB = GetComponent<WaterController>();
        Loading = GameObject.Find("Loading").GetComponent<TextMeshProUGUI>();

        createText = GameObject.Find("CreateText").GetComponent<TextMeshProUGUI>();
        state = GameState.Start;
    }


    void FixedUpdate()
    {
        switch (state)
        {
            case GameState.Start:
                {
                    if (Puffy.transform.position.x < 0)
                    {
                        WB.enabled = true;
                        Loading.enabled = false;
                        createText.enabled = true;
                        Puffy.GetComponent<PuffyController>().enabled = true;
                        state = GameState.Play;
                    }
                    else Puffy.transform.Translate(new Vector2(-0.05f, 0));
                }
                break;
            case GameState.Play:
                {

                }
                break;
        }
    }

    public void CollectObject(GameObject Object)
    {
        if (Object.tag == "Collectable")
        {
            createCount++;
            if (createCount >= createCompletion) state = GameState.Complete;

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