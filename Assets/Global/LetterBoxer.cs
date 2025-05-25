using UnityEngine;
using UnityEngine.UI;

// Taken Entirely From https://www.youtube.com/watch?v=PClWqhfQlpU
// Would've done something myself but this is perfect
public class LetterBoxer : MonoBehaviour
{
    Vector2 OldScreenRes = Vector2.zero;

    void Awake()
    {
        EditViewport();
    }

    void Update()
    {
        if (OldScreenRes != new Vector2(Screen.width, Screen.height)) EditViewport();
    }

    void EditViewport()
    {
        float targetaspect = 16.0f / 9.0f;

        float windowaspect = (float)Screen.width / (float)Screen.height;

        float scaleheight = windowaspect / targetaspect;

        Camera camera = GetComponent<Camera>();

        if (scaleheight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            camera.rect = rect;
        }
        else
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = camera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }

        foreach (GameObject OBJs in Resources.FindObjectsOfTypeAll(typeof(GameObject)))
        {
            if (OBJs.GetComponent<Canvas>() != null)
            {
                Canvas Canvas = OBJs.GetComponent<Canvas>();

                Canvas.worldCamera = camera;
                Canvas.renderMode = RenderMode.ScreenSpaceCamera;
            }
        }

        OldScreenRes = new Vector2(Screen.width, Screen.height);
    }
}