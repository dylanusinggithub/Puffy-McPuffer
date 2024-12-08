using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    int HowToIndex = 0;

    public void PlayGame()
    {
        if (PlayerPrefs.HasKey("Volume")) PlayerPrefs.SetFloat("Volume", 1);
        SceneManager.LoadScene("Level Select Map");

    }

    public void BTN_Main()
    {
        foreach (Transform Menu in this.transform) Menu.gameObject.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(true);
    }


    public void BTN_HowTo()
    {
        foreach (Transform Menu in this.transform) Menu.gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);

        transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Next Page";

        transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
        transform.GetChild(1).GetChild(3).gameObject.SetActive(true);
        transform.GetChild(1).GetChild(4).gameObject.SetActive(false);
    }

    public void BTN_HowToNext()
    {
        HowToIndex++;
        if (HowToIndex == 1)
        {
            transform.GetChild(1).GetChild(2).gameObject.SetActive(false);
            transform.GetChild(1).GetChild(3).gameObject.SetActive(false);
            transform.GetChild(1).GetChild(4).gameObject.SetActive(true);

            transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Exit";
        }
        else BTN_Main();
    }

    public void BTN_Options()
    {
        foreach (Transform Menu in this.transform) Menu.gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(true);
    }

    public void BTN_Default()
    {
        foreach (Slider Slider in transform.GetComponentsInChildren<Slider>()) 
            Slider.value = Slider.GetComponent<SliderController>().valueDefault;
    }
}
