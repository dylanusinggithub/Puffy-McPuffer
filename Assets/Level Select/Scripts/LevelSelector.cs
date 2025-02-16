using System;
using UnityEditor;
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
        LD.StartLevel(LevelIndex);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!GetComponent<Button>().enabled) return; // If Button is disabled don't show

        Destroy(Preview);
        Preview = Instantiate(LevelPreview, ComicPanel.transform);

        Preview.GetComponent<RectTransform>().anchoredPosition = new Vector2(-159, -55);
        Preview.transform.localScale = Vector2.one;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!GetComponent<Button>().enabled) return; // If Button is disabled don't show
        Animator anim = Preview.GetComponentInChildren<Animator>();

        anim.SetFloat("Speed", -1);
        if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) anim.Play("Appear", 0, 1); // Replays Animation only after it completes

        // Removes previous LevelPreview
        float clipLength = anim.runtimeAnimatorController.animationClips[0].length;
        float PlaybackPercent = Mathf.Clamp01(anim.GetCurrentAnimatorStateInfo(0).normalizedTime); // Only care if it's midway
        float DestoryTime = clipLength * PlaybackPercent;
        Destroy(Preview, DestoryTime);
    }
}
