using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour
{
    // UI
    TextMeshProUGUI scoreText, timeText;

    [Header("Gameplay")]

    [SerializeField, Range(0f, 50f)]
    public int timeWin;

    [SerializeField, Range(0f, 20f)]
    public int scoreWin;

    [SerializeField]
    public int score = 1;
    int oldScore = -1; // So on start it generates the Ui

    [Header("Score UI")]
    Transform ScoreUI;

    [SerializeField]
    GameObject NormalCreate, DisabledCreate;

    [Header("UI")]

    [SerializeField]
    GameObject youWin;

    [SerializeField]
    GameObject gameOver;

    void Start()
    {
        ScoreUI = GameObject.Find("Score Counter").transform.GetChild(0);
    }

    void FixedUpdate()
    {
        // If Score has Changed
        if (score != oldScore)
        {

            foreach (Transform child in ScoreUI.GetComponentInChildren<Transform>()) Destroy(child.gameObject);

            for (int i = 0; i < score && i < scoreWin; i++) Instantiate(NormalCreate, ScoreUI);

            for (int i = score; i < scoreWin; i++) Instantiate(DisabledCreate, ScoreUI);

            // If score is greater than the completition threshold then display bonus text
            // else hide bonus text and play the creates animation
            if (score > scoreWin) 
            {
                ScoreUI.parent.GetChild(1).gameObject.SetActive(true);
                ScoreUI.parent.GetComponentInChildren<TextMeshProUGUI>().text = (score - scoreWin).ToString() + "+";
            }
            else
            {
                ScoreUI.parent.GetChild(1).gameObject.SetActive(false);

                if (0 < oldScore)
                {
                    if (ScoreUI.GetChild(oldScore).name.Contains("Disabled"))
                    {
                        ScoreUI.GetChild(oldScore).GetComponent<Animator>().SetTrigger("Disappear");
                    }
                    else ScoreUI.GetChild(oldScore).GetComponent<Animator>().SetTrigger("Appear");
                }
            }

            oldScore = score;
        }
    }

    public void Die()
    {
        if (scoreWin <= score) youWin.SetActive(true);
        else if (score == 0) 
        {
            gameOver.SetActive(true);
            gameOver.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "You have crashed and sunk to the bottom of the canal.";
        }
        else
        {
            gameOver.SetActive(true);
            gameOver.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "You have failed to retrieve the requested amount of cargo.";
        }

        Time.timeScale = 0;
    }

}
