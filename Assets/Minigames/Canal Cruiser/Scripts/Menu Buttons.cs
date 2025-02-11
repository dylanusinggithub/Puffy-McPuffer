using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    [SerializeField] Animator Fadetransition;

    public void BTN_Exit()
    {
        Time.timeScale = 1;
        StartCoroutine(Fade());
        Physics2D.gravity = new Vector2(0, -9.81f);
        SceneManager.LoadScene("Level Select Map");
    }

    public void BTN_Retry()
    {
        Time.timeScale = 1;
        StartCoroutine(FadeRetry());
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BTN_NextLevel()
    {
        LevelDesigner.AdvanceToNextLevel = true;
        BTN_Exit();
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
