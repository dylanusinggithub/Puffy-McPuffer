using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    int HowToIndex = 0;

    [SerializeField] Animator FadeTransition;

    public void PlayGame()
    {
        if (PlayerPrefs.HasKey("Volume")) PlayerPrefs.SetFloat("Volume", 1);
        SceneManager.LoadScene("Level Select Map");

    }

    public void BTN_Main()
    {
        HowToIndex = 0;
        transform.GetChild(1).GetChild(5).gameObject.SetActive(false);
        foreach (Transform Menu in this.transform) Menu.gameObject.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(true);
    }


    public void BTN_HowTo()
    {

        foreach (Transform Menu in this.transform) Menu.gameObject.SetActive(false);
        StartCoroutine(FadeTime());

        transform.GetChild(1).gameObject.SetActive(true);
        
        transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Next Page";

        transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
        transform.GetChild(1).GetChild(3).gameObject.SetActive(true);
        transform.GetChild(1).GetChild(4).gameObject.SetActive(false);
    }

    public void BTN_HowToNext()
    {
        StartCoroutine(FadeTime());
        HowToIndex++;
        if (HowToIndex < 3 ) 
        {
            if (HowToIndex == 2)
            {

                transform.GetChild(1).GetChild(2).gameObject.SetActive(false);
            }
            transform.GetChild(1).GetChild(HowToIndex + 2).gameObject.SetActive(false);
            transform.GetChild(1).GetChild(HowToIndex + 3).gameObject.SetActive(true);

            transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Exit";
        }
        else
        {
            StartCoroutine(FadeTime());
            BTN_Main();
        }
    }

    public void BTN_Options()
    {
        foreach (Transform Menu in this.transform) Menu.gameObject.SetActive(false);
        StartCoroutine(FadeTime());
        transform.GetChild(2).gameObject.SetActive(true);
    }

    public void BTN_Default()
    {
        foreach (Slider Slider in transform.GetComponentsInChildren<Slider>()) 
            Slider.value = Slider.GetComponent<SliderController>().valueDefault;
    }

    IEnumerator FadeTime()
    {
        FadeTransition.gameObject.SetActive(true);
        //FadeTransition.GetComponent<Image>().color = new Color(0, 0, 0, 1);
        FadeTransition.SetTrigger("End");
        yield return new WaitForSeconds(1);
        FadeTransition.SetTrigger("Start");
    }


}
