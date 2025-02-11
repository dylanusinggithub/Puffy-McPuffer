using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    [SerializeField, Range(0, 10)] int LevelIndex;
    LevelDesigner LD;
    [SerializeField] Animator Fadetransition;

    private void Start()
    {
        LD = GameObject.Find("Comic Panels").GetComponent<LevelDesigner>();

        if (PlayerPrefs.GetInt("Levels Unlocked", 0) >= LevelIndex)
        {
            GetComponent<Button>().enabled = false;

            Image[] Children = GetComponentsInChildren<Image>(); // Automatically change the icon to the disabled colour
            foreach (Image colours in Children) colours.color = GetComponent<Button>().colors.disabledColor;
        }
    }

    public void BTN_PlayLevel()
    {
        StartCoroutine(Fade());
        LD.StartLevel(LevelIndex);
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
