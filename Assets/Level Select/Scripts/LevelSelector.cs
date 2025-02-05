using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    [SerializeField, Range(0, 10)] int LevelIndex;
    LevelDesigner LD;

    private void Start()
    {
        LD = GameObject.Find("Comic Panels").GetComponent<LevelDesigner>();

        if (PlayerPrefs.GetString("Level " + LevelIndex + " Unlocked", "False") == "False")
        {
            GetComponent<Button>().enabled = false;

            Image[] Children = GetComponentsInChildren<Image>(); // Automatically change the icon to the disabled colour
            foreach (Image colours in Children) colours.color = GetComponent<Button>().colors.disabledColor;
        }
    }

    public void BTN_PlayLevel()
    {
        LD.StartLevel(LevelIndex);
    }
}
