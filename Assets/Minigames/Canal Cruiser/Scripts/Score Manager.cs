using UnityEngine;
using TMPro;

public class ScoreScript : MonoBehaviour
{
    // UI
    TextMeshProUGUI scoreText, timeText;

    [Header("Gameplay")]
    [SerializeField, Range(0f, 20f)]
    int scoreWin;

    [SerializeField]
    public int score = 1;

    [SerializeField]
    GameObject youWin;

    [SerializeField]
    GameObject gameOver;

    [SerializeField, Range(0f, 50f)]
    int timeWin;

    [SerializeField]
    float timeTillWin = 0;

    void Start()
    {
        timeTillWin = timeWin;
        Physics2D.gravity = new Vector2(-9.81f, 0); // Left (behind player)

        timeText = GameObject.Find("Time Limit").GetComponent<TextMeshProUGUI>();

        scoreText = GameObject.Find("Cargo Counter").GetComponent<TextMeshProUGUI>();
        scoreText.text = "Cargo Collected: " + score.ToString() + "/ " + scoreWin.ToString();
    }

    void FixedUpdate()
    {
        if (timeTillWin > 0)
        {
            timeTillWin -= Time.deltaTime;
            timeText.text = "Time Remaining: " + timeTillWin.ToString("F2"); // "F2" means 2 sig figures
        }
        else Die(false);

        scoreText.text = "Cargo Collected: " + score.ToString() + "/ " + scoreWin.ToString();
    }

    public void Die(bool lostCargo)
    {
        if (scoreWin <= score) youWin.SetActive(true);
        else gameOver.SetActive(true);

        // Grabs the FailInfo's text and changes it acorrdingly
        if (lostCargo) gameOver.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text =
                "You have lost the last remaining cargo crate, and sunk to the bottom of the canal.";

        Time.timeScale = 0;
    }
}
