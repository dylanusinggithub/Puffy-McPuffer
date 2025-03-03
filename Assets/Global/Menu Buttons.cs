using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    [SerializeField] GameObject OptionsPanel, PausePanel;
    [SerializeField] GameObject PauseBTN;

    Animator FadeTransition;
    AudioSource FadeAS, BTNAS;
    float FadeVol,BTNVol;

    void Start()
    {
        BTNAS = GetComponent<AudioSource>();
        BTNVol = BTNAS.volume;

        BTNAS.volume = BTNVol * PlayerPrefs.GetFloat("Volume", 1);


        FadeTransition = GameObject.Find("Fadetransition").GetComponent<Animator>();
        FadeAS = FadeTransition.gameObject.GetComponent<AudioSource>();
        FadeVol = FadeAS.volume;

        FadeAS.volume = FadeVol * PlayerPrefs.GetFloat("Volume", 1);
    }

    public void BTN_Exit()
    {
        Physics2D.gravity = new Vector2(0, -9.81f); // Canal crusier changes it
        StartCoroutine(FadeLoadScene("Level Select Map"));
    }

    public void BTN_Retry()
    {
        StartCoroutine(FadeLoadScene(SceneManager.GetActiveScene().name));
    }

    public void BTN_NextLevel()
    {
        LevelDesigner.AdvanceToNextLevel = true;
        BTN_Exit();
    }

    public void BTN_PauseMenu()
    {
        BTNAS.volume = BTNVol * PlayerPrefs.GetFloat("Volume", 1);
        BTNAS.Play();

        PausePanel.SetActive(!PausePanel.activeInHierarchy); // Disables if enabled, and enables if disabled

        if (PausePanel.activeInHierarchy) Time.timeScale = 0f;
        else Time.timeScale = 1f;
    }

    public void BTN_OptionsMenu()
    {
        BTNAS.volume = BTNVol * PlayerPrefs.GetFloat("Volume", 1);
        BTNAS.Play();

        bool isEnabled = OptionsPanel.activeInHierarchy;

        // Disables all children only if it is currently closed, renables em when opened
        foreach (Transform child in PausePanel.GetComponentInChildren<Transform>()) child.gameObject.SetActive(isEnabled);

        OptionsPanel.SetActive(!isEnabled);
        PauseBTN.SetActive(isEnabled);
    }

    IEnumerator Fade()
    {
        FadeAS.volume = FadeVol * PlayerPrefs.GetFloat("Volume", 1);
        FadeAS.Play();

        FadeTransition.SetFloat("Speed", 1);
        yield return new WaitForSeconds(1);
        FadeTransition.SetTrigger("End");
    }

    IEnumerator FadeLoadScene(string Scene)
    {
        BTNAS.volume = BTNVol * PlayerPrefs.GetFloat("Volume", 1);
        BTNAS.Play();

        FadeTransition.SetFloat("Speed", 1);
        FadeTransition.SetTrigger("End");
        yield return new WaitForSecondsRealtime(1);

        Time.timeScale = 1;
        SceneManager.LoadScene(Scene);
    }
}
