using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class FilmController : MonoBehaviour, IPointerClickHandler
{
    public GameObject ComicViewer;
    public List<Object> Comics;
    int ComicIndex = 0;

    public AnimationClip AppearAnimation;
    Vector2 Size = new Vector2(1500, 844);

    public void CreatePreview()
    {
        // Finds the Comic's Type by finding the last instance of "." (plus 1 as it actually gives the location of the ".")
        // Basically: UnityEngine.Video.VideoClip > .VideoClip > VideoClip
        String ObjectType = Comics[0].GetType().ToString();
        ObjectType = ObjectType.Substring(ObjectType.LastIndexOf(".") + 1);

        switch (ObjectType)
        {
            case "Texture2D":
                {
                    gameObject.AddComponent<RawImage>();
                    GetComponent<RawImage>().texture = (Texture2D)Comics[0];
                }
                break;
            case "Sprite":
                {
                    gameObject.AddComponent<Image>();
                    GetComponent<Image>().sprite = (Sprite)Comics[0];
                }
                break;
            case "VideoClip":
                {
                    Debug.LogError("Video is no longer supported </3");
                }
                break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ComicViewer.SetActive(true);
        DisplayComic();
    }

    public void DisplayComic()
    {
        foreach(Transform child in ComicViewer.GetComponentInChildren<Transform>()) Destroy(child.gameObject);

        if (ComicIndex == Comics.Count)
        {
            ComicViewer.SetActive(false);
            ComicIndex = 0;
            return;
        }

        // Finds the Comic's Type by finding the last instance of "." (plus 1 as it actually gives the location of the ".")
        // Basically: UnityEngine.Video.VideoClip > .VideoClip > VideoClip
        String ObjectType = Comics[ComicIndex].GetType().ToString();
        ObjectType = ObjectType.Substring(ObjectType.LastIndexOf(".") + 1);

        GameObject GOComic = new GameObject("Comic");
        GOComic.transform.SetParent(ComicViewer.transform);

        switch (ObjectType)
        {
            case "Sprite":
            case "Texture2D":
                {

                    GOComic.AddComponent<RectTransform>();
                    GOComic.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    GOComic.GetComponent<RectTransform>().sizeDelta = Size;
                    GOComic.GetComponent<RectTransform>().localScale = Vector2.one;

                    // The images keep flip flopping between the 2 and i have no idea why
                    if (ObjectType == "Sprite")
                    {
                        GOComic.AddComponent<Image>();
                        GOComic.GetComponent<Image>().sprite = (Sprite)Comics[ComicIndex];
                    }
                    else
                    {
                        GOComic.AddComponent<RawImage>();
                        GOComic.GetComponent<RawImage>().texture = (Texture2D)Comics[ComicIndex];
                    }
                }
                break;
            case "VideoClip":
                {
                    Debug.LogError("Video is no longer supported </3");
                }
                break;
        }

        // Makes GOComic advance whenever clicked
        GOComic.AddComponent<Button>();
        GOComic.GetComponent<Button>().onClick.AddListener(DisplayComic);
        GOComic.GetComponent<Button>().transition = Selectable.Transition.None;

        if (ComicIndex == 0)
        {
            GOComic.AddComponent<Animation>();
            GOComic.GetComponent<Animation>().AddClip(AppearAnimation, "Appear");
            GOComic.GetComponent<Animation>().Play("Appear"); // Play Automatically doesn't play automatically :/
        }

        ComicIndex++;

    }
}
