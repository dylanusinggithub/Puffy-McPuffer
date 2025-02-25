using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    Slider timeSlider;
    RawImage timerWater;

    void Start()
    {
        timeSlider = GameObject.Find("Timer").GetComponent<Slider>();
        timeSlider.value = timeWin;
        timeSlider.maxValue = timeWin;

        timerWater = GameObject.Find("Timer").transform.GetChild(0).GetChild(1).GetComponent<RawImage>();

        scoreText = GameObject.Find("Cargo Counter").GetComponent<TextMeshProUGUI>();
    }

    void FixedUpdate()
    {
        if (timeSlider.value > 0)
        {
            timeSlider.value -= Time.deltaTime;
            timerWater.uvRect = new Rect(timerWater.uvRect.position + new Vector2(0.1f, 0) * Time.deltaTime, timerWater.uvRect.size);
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
            gameOver.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "You have crashed and sunk to the bottom of the canal.";
        }
        else
        {
            gameOver.SetActive(true);
            gameOver.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "You have failed to retrieve the requested amount of cargo.";
        }

        Time.timeScale = 0;
    }

}
