using System.Collections;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    int levelIndex;

    [SerializeField]
    GameObject animationObject;
    GameObject Puffy;

    float puffyGamePos;
    float puffyStart = -17;
    float moveSpeed = 0.07f;

    [SerializeField, Range(0.1f, 2)]
    float CinimaticSeconds;

    GameObject CinamaticBars, Timer;

    enum startAnimationState { DriveOnScreen, JumpBack, GameStart }
    [SerializeField]startAnimationState Animation;

    private void Start()
    {
        Physics2D.gravity = new Vector2(-9.81f, 0); // Left (behind player)

        // animation
        if (PlayerPrefs.GetString("showTutorial", "False") == "False")
        {
            this.enabled = false;
            GameObject.Find("Cinematic Bars").SetActive(false);
            GameObject.Find("Game Manager").GetComponent<ScoreScript>().score = 0;
            return;
        }

        Puffy = GameObject.Find("Player");
        Puffy.GetComponent<PlayerScript>().enabled = false;

        puffyGamePos = Puffy.transform.position.x;
        Instantiate(animationObject, new Vector2(puffyGamePos + 1.5f, 0), Quaternion.Euler(0, 0, -90));

        Puffy.transform.position = new Vector2(puffyStart, 0);

        Timer = GameObject.Find("Timer");
        Timer.SetActive(false);

        GameObject.Find("Water Swiggles").GetComponent<Animator>().enabled = false;

        GameObject Cargo = GameObject.Find("Cargo Objects").transform.GetChild(0).gameObject; 
        Cargo.transform.position = new Vector2(puffyStart - 2, 0); // moves cargo behind puffy

        GameObject.Find("Game Manager").GetComponent<ScoreScript>().enabled = false;

        CinamaticBars = GameObject.Find("Cinematic Bars");
    }

    private void FixedUpdate()
    {
        switch (Animation)
        {
            case startAnimationState.DriveOnScreen:
                {
                    if (Puffy.transform.position.x > puffyGamePos + 1f)
                    {
                        GameObject.Find("Game Manager").GetComponent<ScoreScript>().score--;
                        Animation = startAnimationState.JumpBack;
                    }
                    else Puffy.transform.Translate(new Vector2(0, moveSpeed));

                }
                break;
            case startAnimationState.JumpBack:
                {
                    if (Puffy.transform.position.x < puffyGamePos) Animation = startAnimationState.GameStart;
                    else Puffy.transform.Translate(new Vector2(0, -moveSpeed * 1.5f));
                }
                break;
            case startAnimationState.GameStart:
                {
                    Puffy.transform.position = new Vector2(puffyGamePos, 0);
                    GameObject.Find("Game Manager").GetComponent<ScoreScript>().enabled = true;
                    Puffy.GetComponent<PlayerScript>().enabled = true;

                    Timer.SetActive(true);
                    GameObject.Find("Water Swiggles").GetComponent<Animator>().enabled = true;

                    StartCoroutine(FadeOutBars());

                    this.enabled = false;
                }
                break;
        }
    }

    IEnumerator FadeOutBars()
    {
        float smootheness = 100;
        for (int i = 0; i < smootheness; i++)
        {
            yield return new WaitForSeconds(CinimaticSeconds / smootheness);
            float scale = Mathf.Lerp(0, 2 / CinamaticBars.transform.GetChild(0).GetComponent<RectTransform>().rect.height, i / smootheness);

            CinamaticBars.transform.localScale += new Vector3(0, scale, 0);
        }
    }
}
