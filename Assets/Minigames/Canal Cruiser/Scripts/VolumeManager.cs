using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    public Slider volumeSlider;

    private float defaultVolume = 1f;


    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            volumeSlider.value = PlayerPrefs.GetFloat("Volume");
        }

        else
        {
            volumeSlider.value = defaultVolume;
        }
    }

    public void ResetToDefault()
    {
        volumeSlider.value = defaultVolume;
        PlayerPrefs.SetFloat("Volume", defaultVolume);
        PlayerPrefs.Save();

        SliderController sliderController = volumeSlider.GetComponent<SliderController>();
        if (sliderController != null)
        {
            volumeSlider.value = sliderController.valueDefault;
        }
    }
}
