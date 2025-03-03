using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    int HowToIndex = 0;

    [SerializeField] Animator FadeTransition;
    [SerializeField, Range(0.1f, 2)] float FadeDelay;

    GameObject Main, HowToPlay, Options;

    GameObject Fadetransition;
    AudioSource AS, FadeAS;

    float BTNVol, FadeVol;

    private void Start()
    {
        Main = transform.GetChild(0).gameObject;
        HowToPlay = transform.GetChild(1).gameObject;
        Options = transform.GetChild(2).gameObject;

        Fadetransition = GameObject.Find("Fadetransition");
        Fadetransition.GetComponent<AudioSource>().volume *= PlayerPrefs.GetFloat("Volume", 1);
        
        AS = GameObject.Find("UI").GetComponent<AudioSource>();
        FadeAS = Fadetransition.GetComponent<AudioSource>();

        BTNVol = AS.volume;
        FadeVol = FadeAS.volume;
    }

    void HideUI()
    {
        // Disable everything but the fade
        foreach (Transform UI in transform) UI.gameObject.SetActive(false);
        FadeTransition.gameObject.SetActive(true);
    }


    public void BTN_PlayGame()
    {
        AS.volume = BTNVol * PlayerPrefs.GetFloat("Volume", 1);
        AS.Play();
        StartCoroutine(FadeTime());
        StartCoroutine(PlayGame());
    }

    IEnumerator PlayGame()
    {
        yield return new WaitForSeconds(FadeDelay);

        // Sets volume to 100% on first time playing
        if (!PlayerPrefs.HasKey("Volume")) PlayerPrefs.SetFloat("Volume", 1);
        SceneManager.LoadScene("Level Select Map");
    }

    public void BTN_Main()
    {
        AS.volume = BTNVol * PlayerPrefs.GetFloat("Volume", 1);
        AS.Play();
        StartCoroutine(FadeTime(Main));
    }

    public void BTN_HowTo()
    {
        AS.volume = BTNVol * PlayerPrefs.GetFloat("Volume", 1);
        AS.Play();
        StartCoroutine(FadeTime(HowToPlay));
        StartCoroutine(HowTo());
    }

    IEnumerator HowTo()
    {
        yield return new WaitForSeconds(FadeDelay);

        HowToPlay.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        HowToPlay.transform.GetChild(1).gameObject.SetActive(true); // Back
        HowToPlay.transform.GetChild(2).gameObject.SetActive(true); // Exit

        HowToPlay.transform.GetChild(3).gameObject.SetActive(false); // Centred Exit
        HowToIndex = 0;
    }

    public void BTN_HowToNext()
    {
        AS.volume = BTNVol * PlayerPrefs.GetFloat("Volume", 1);
        AS.Play();
        StartCoroutine(FadeTime(HowToPlay));
        StartCoroutine(HowToNext());
    }

    IEnumerator HowToNext()
    {
        yield return new WaitForSeconds(FadeDelay);

        HowToIndex++;
        HowToPlay.transform.GetChild(0).GetChild(HowToIndex - 1).gameObject.SetActive(false);
        if (HowToIndex < transform.childCount - 1)
        {
            // Remove buttons on last page
            if (HowToIndex == transform.childCount - 2)
            {
                HowToPlay.transform.GetChild(1).gameObject.SetActive(false); // Back
                HowToPlay.transform.GetChild(2).gameObject.SetActive(false); // Exit

                HowToPlay.transform.GetChild(3).gameObject.SetActive(true); // Centred Exit
            }

            
            HowToPlay.transform.GetChild(0).GetChild(HowToIndex).gameObject.SetActive(true);
        }
        else
        {
            HowToIndex = 0;

            // Displays main manually to avoid double fade
            HideUI();
            Main.SetActive(true);
        }
    }

    public void BTN_Options()
    {
        AS.volume = BTNVol * PlayerPrefs.GetFloat("Volume", 1);
        AS.Play();
        StartCoroutine(FadeTime(Options));
    }

    IEnumerator FadeTime(GameObject Menu)
    {
        FadeAS.volume = FadeVol * PlayerPrefs.GetFloat("Volume", 1);
        FadeAS.Play();

        // The fade takes 1s so if i want it to be half that - 0.5 - then i need to double it
        FadeTransition.SetFloat("Speed", 1 / FadeDelay);
        FadeTransition.SetTrigger("End");

        yield return new WaitForSeconds(FadeDelay);

        FadeTransition.SetFloat("Speed", 1 / FadeDelay);
        FadeTransition.SetTrigger("Start");
        FadeTransition.SetFloat("Speed", 1 / FadeDelay);

        HideUI();
        Menu.gameObject.SetActive(true);
    }

    IEnumerator FadeTime()
    {
        FadeAS.volume = FadeVol * PlayerPrefs.GetFloat("Volume", 1);
        FadeAS.Play();

        // The fade takes 1s so if i want it to be half that - 0.5 - then i need to double it
        Fadetransition.GetComponent<AudioSource>().Play();
        FadeTransition.SetFloat("Speed", 1 / FadeDelay);
        FadeTransition.SetTrigger("End");

        yield return new WaitForSeconds(FadeDelay);

        FadeTransition.SetFloat("Speed", 1 / FadeDelay);
        FadeTransition.SetTrigger("Start");
        FadeTransition.SetFloat("Speed", 1 / FadeDelay);

        HideUI();
    }
}
