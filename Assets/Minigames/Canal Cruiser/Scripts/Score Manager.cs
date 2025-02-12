using UnityEngine;
using TMPro;

public class ScoreScript : MonoBehaviour
{
    // UI
    TextMeshProUGUI scoreText, timeText;

    [Header("Gameplay")]
    [SerializeField, Range(0f, 20f)]
    public int scoreWin;

    [SerializeField]
    public int score = 1;

    [SerializeField]
    GameObject youWin;

    [SerializeField]
    GameObject gameOver;

    [SerializeField, Range(0f, 50f)]
    public int timeWin;

    [SerializeField]
    float timeTillWin = 0;

    void Start()
    {
        timeTillWin = timeWin;

        timeText = GameObject.Find("Time Limit").GetComponent<TextMeshProUGUI>();

        scoreText = GameObject.Find("Cargo Counter").GetComponent<TextMeshProUGUI>();
    }

    void FixedUpdate()
    {
        if (timeTillWin > 0)
        {
            timeTillWin -= Time.deltaTime;
            timeText.text = "Time Remaining: " + timeTillWin.ToString("F2"); // "F2" means 2 sig figures
        }
        else Die();

        scoreText.text = "Cargo Collected: " + score.ToString() + "/ " + scoreWin.ToString();
    }

    public void Die()
    {
        if (scoreWin <= score) youWin.SetActive(true);
        else if (score == 0)
        {
            gameOver.SetActive(true);
            gameOver.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "You have crashed and sunk to the bottom of the canal.";
        }
        else
        {
            gameOver.SetActive(true);
            gameOver.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "You have failed to retrieve the requested amount of cargo.";
        }

        Time.timeScale = 0;
    }

}
