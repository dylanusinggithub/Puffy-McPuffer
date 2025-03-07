using UnityEngine;
using UnityEngine.EventSystems;

public class PreviewController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    LevelSelector LevelButton;

    public void OnPointerEnter(PointerEventData eventData)
    {
        foreach (LevelSelector Button in Resources.FindObjectsOfTypeAll<LevelSelector>())
        {
            if (Button.MouseMoved)
            {
                if(LevelButton == null) LevelButton = Button;

                LevelButton.MouseMoved = false;
            }
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        LevelButton.MouseMoved = true;
        StartCoroutine(LevelButton.UnravelCheck(0.25f));
    }
}
