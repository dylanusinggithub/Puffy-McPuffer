using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] Animator Fadetransition;
    [SerializeField] GameObject OptionsPanel, PausePanel;
    [SerializeField] GameObject PauseBTN;

    public void BTN_Exit()
    {
        Time.timeScale = 1;
        Physics2D.gravity = new Vector2(0, -9.81f);
        SceneManager.LoadScene("Level Select Map");
    }

    public void BTN_Retry()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BTN_NextLevel()
    {
        LevelDesigner.AdvanceToNextLevel = true;
        BTN_Exit();
    }

    public void BTN_PauseMenu()
    {
        PausePanel.SetActive(!PausePanel.activeInHierarchy); // Disables if enabled, and enables if disabled

        if (PausePanel.activeInHierarchy) Time.timeScale = 0f;
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
}
