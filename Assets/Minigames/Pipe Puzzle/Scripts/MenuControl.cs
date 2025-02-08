using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuControl : MonoBehaviour
{
    public Button restart;
    public Button menu;
    [SerializeField] Animator Fadetransition;

    // Start is called before the first frame update
    void Start()
    {
        restart.onClick.AddListener(restartgame);
        menu.onClick.AddListener(openmenu);
    }

    // Update is called once per frame
    void restartgame()
    {
        StartCoroutine(FadeRetry());
        SceneManager.LoadScene("Pipelike");
        Time.timeScale = 1f;
    }

    void openmenu()
    {
        StartCoroutine(Fade());
        SceneManager.LoadScene("Level Select");
        Time.timeScale = 1f;
    }

    IEnumerator Fade()
    {
        Fadetransition.SetTrigger("End");
        yield return new WaitForSeconds(1);
        Fadetransition.SetTrigger("Start");
    }

    IEnumerator FadeRetry()
    {
        Fadetransition.SetTrigger("End");
        yield return new WaitForSeconds(0.5f);
        Fadetransition.SetTrigger("Start");
    }
}
