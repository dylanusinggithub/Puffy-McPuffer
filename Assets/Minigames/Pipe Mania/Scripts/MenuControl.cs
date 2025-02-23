using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour
{
    [SerializeField] Animator Fadetransition;
    [SerializeField] GameObject OptionsPanel, PausePanel;
    [SerializeField] GameObject PauseBTN;

    // Update is called once per frame
    public void BTN_Restart()
    {
        StartCoroutine(FadeRetry());
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    public void BTN_ReturnToLevelSelect()
    {
        StartCoroutine(Fade());
        SceneManager.LoadScene("Level Select Map");
        Time.timeScale = 1f;
    }

    public void BTN_AdvanceToNextLevel()
    {
        LevelDesigner.AdvanceToNextLevel = true;
        BTN_ReturnToLevelSelect();
    }

    public void BTN_PauseMenu()
    {
        PausePanel.SetActive(!PausePanel.activeInHierarchy); // Disables if enabled, and enables if disabled
        
        if(PausePanel.activeInHierarchy) Time.timeScale = 0f;
        else Time.timeScale = 1f;
    }

    public void BTN_OptionsMenu()
    {
        bool isEnabled = OptionsPanel.activeInHierarchy;

        // Disables all children only if it is currently closed, renables em when opened
        foreach (Transform child in PausePanel.GetComponentInChildren<Transform>()) child.gameObject.SetActive(isEnabled);

        OptionsPanel.SetActive(!isEnabled);
        PauseBTN.SetActive(isEnabled);
    }

    IEnumerator Fade()
    {
        GameObject.Find("Fadetransition").GetComponent<AudioSource>().Play();
        Fadetransition.SetTrigger("End");
        yield return new WaitForSeconds(1);
        Fadetransition.SetTrigger("Start");
    }

    IEnumerator FadeRetry()
    {
        GameObject.Find("Fadetransition").GetComponent<AudioSource>().Play();
        Fadetransition.SetTrigger("End");
        yield return new WaitForSeconds(0.5f);
        Fadetransition.SetTrigger("Start");
    }
}
