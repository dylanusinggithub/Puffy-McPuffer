using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class LevelSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField, Range(0, 10)] int LevelIndex;
    LevelDesigner LD;

    [SerializeField] UnityEngine.Object LevelPreview;
    [SerializeField] RenderTexture VideoCanvas;
    GameObject Preview;

    GameObject ComicPanel;

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
        Preview = new GameObject("Start Preview");
        Preview.transform.SetParent(ComicPanel.transform);

        // Finds the LevelPreview's Type by finding the last instance of "." (plus 1 as it actually gives the location of the ".")
        // Basically: UnityEngine.Video.VideoClip > .VideoClip > VideoClip
        String ObjectType = LevelPreview.GetType().ToString();
        ObjectType = ObjectType.Substring(ObjectType.LastIndexOf(".") + 1);

        switch (ObjectType)
        {
            case "Sprite":
            case "Texture2D":
                {
                    // Make image centred and fill screen
                    Preview.AddComponent<RectTransform>();
                    Preview.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
                    Preview.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    Preview.GetComponent<RectTransform>().anchorMax = Vector2.one;
                    Preview.GetComponent<RectTransform>().anchorMin = Vector2.zero;


                    // The images keep flip flopping between the 2 and i have no idea why
                    if (ObjectType == "Sprite")
                    {
                        Preview.AddComponent<Image>();
                        Preview.GetComponent<Image>().sprite = (Sprite)LevelPreview;
                        Preview.GetComponent<Image>().raycastTarget = false;
                    }
                    else
                    {
                        Preview.AddComponent<RawImage>();
                        Preview.GetComponent<RawImage>().texture = (Texture2D)LevelPreview;
                        Preview.GetComponent<RawImage>().raycastTarget = false;
                    }
                }
                break;

            case "VideoClip":
                {
                    // Create Video Player
                    Preview.AddComponent<VideoPlayer>();
                    VideoPlayer VP = Preview.GetComponent<VideoPlayer>();
                    VP.clip = (VideoClip)LevelPreview;
                    VP.isLooping = true;

                    VP.SetDirectAudioVolume(0, PlayerPrefs.GetFloat("Volume", 1));
                    VP.targetTexture = VideoCanvas;

                    // Create the image the video will be playing to
                    Preview.AddComponent<RawImage>();
                    Preview.GetComponent<RawImage>().texture = VideoCanvas;

                    Preview.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
                    Preview.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    Preview.GetComponent<RectTransform>().anchorMax = Vector2.one;
                    Preview.GetComponent<RectTransform>().anchorMin = Vector2.zero;
                }
                break;

            case "GameObject":
                {
                    Destroy(Preview);

                    Preview = Instantiate((GameObject)LevelPreview);
                    Preview.transform.parent = transform;
                }
                break;

            default:
                {
                    Destroy(Preview);
                    Debug.LogError("Invalid Type! " + ObjectType);
                }
                break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Removes previous LevelPreview
        Destroy(Preview);
    }
}
