using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField, Range(0, 10)] int LevelIndex;
    LevelDesigner LD;

    [SerializeField] GameObject LevelPreview;
    [SerializeField] RenderTexture VideoCanvas;
    GameObject ComicPanel, Preview;

    bool openingLevel = false;

    private void Start()
    {
        ComicPanel = GameObject.Find("Comic Panels");
        LD = ComicPanel.GetComponent<LevelDesigner>();

        if (PlayerPrefs.GetInt("Levels Unlocked", 0) < LevelIndex)
        {
            GetComponent<Button>().enabled = false;

            Image[] Children = GetComponentsInChildren<Image>(); // Automatically change the icon to the disabled colour
            foreach (Image colours in Children) colours.color = GetComponent<Button>().colors.disabledColor;
        }
    }

    public void BTN_PlayLevel()
    {
        StartCoroutine(PlayLevel());
        openingLevel = true;
    }

    IEnumerator PlayLevel()
    {
        Animator FadeTransition = GameObject.Find("Fadetransition").GetComponent<Animator>();
        FadeTransition.gameObject.GetComponent<AudioSource>().Play();
        FadeTransition.SetTrigger("End");
        UnravelPreview();
        yield return new WaitForSeconds(1);
        FadeTransition.SetTrigger("Start");

        LD.StartLevel(LevelIndex);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // If Button is disabled or Preview doesn't exist don't show
        if (!GetComponent<Button>().enabled || LevelPreview == null || openingLevel) return; 

        Destroy(Preview);
        Preview = Instantiate(LevelPreview, ComicPanel.transform);

        Preview.GetComponent<RectTransform>().anchoredPosition = new Vector2(-300, -60);
        Preview.GetComponent<RectTransform>().localScale = Vector3.one * 2.5f;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UnravelPreview();
    }

    void UnravelPreview()
    {
        if (!GetComponent<Button>().enabled || Preview == null) return; // If Button is disabled or Preview doesn't exist don't show
        Animator anim = Preview.GetComponentInChildren<Animator>();

        anim.SetFloat("Speed", -1);
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) anim.Play("Appear", 0, 1); // Replays Animation only after it completes

        // Removes previous LevelPreview
        float clipLength = anim.runtimeAnimatorController.animationClips[0].length;
        float PlaybackPercent = Mathf.Clamp01(anim.GetCurrentAnimatorStateInfo(0).normalizedTime); // Only care if it's midway
        float DestoryTime = clipLength * PlaybackPercent;
        Destroy(Preview, DestoryTime);
    }
}
