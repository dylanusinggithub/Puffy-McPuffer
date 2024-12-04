using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("This will be the medium value: Max is 2x and Min is 0 ")]
    public int valueDefault = 100;

    [SerializeField]
    string valueMinText = "Off";

    [SerializeField]
    string valueMaxText = "Maximum";

    void Start()
    {
        GetComponent<Slider>().value = PlayerPrefs.GetFloat(this.name, valueDefault / 100) * 100;
        GetComponent<Slider>().onValueChanged.AddListener(delegate { SliderChange(); }); //Do not ask what this actually does; I DO NOT KNOW

        SliderChange();
    }

    public void SliderChange()
    {
        PlayerPrefs.SetFloat(this.name, GetComponent<Slider>().value / 100); // Volume is 0 - 1

        if (GetComponent<Slider>().value == 0) transform.GetComponentInChildren<TextMeshProUGUI>().text = this.name + ": Off";
        else if (GetComponent<Slider>().value == valueDefault) transform.GetComponentInChildren<TextMeshProUGUI>().text = this.name + ": Default";
        else if (GetComponent<Slider>().value == valueDefault * 2) transform.GetComponentInChildren<TextMeshProUGUI>().text = this.name + ": Owie";
        else transform.GetComponentInChildren<TextMeshProUGUI>().text = this.name + ": " + GetComponent<Slider>().value + "%";
    }
}
