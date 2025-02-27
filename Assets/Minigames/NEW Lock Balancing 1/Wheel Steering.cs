using UnityEngine;
using UnityEngine.EventSystems;


// adapted from https://www.youtube.com/watch?v=kWRyZ3hb1Vc, "Drag and Drop in Unity UI"
public class WheelSteering : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public float Angle = 0;
    Vector2 dragPoint;

    private AudioSource audioSource; //Wooden Wheel Steer AudioSource reference

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Get AudioSource component
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 Mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Mouse.Set(Mouse.x, dragPoint.y);

        Angle = Vector2.SignedAngle(Mouse, dragPoint);

        GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, Angle);

        //Play sound if not already playing
        if (!audioSource.isPlaying)
        {
            audioSource.loop = true; // Sound should loop while turning
            audioSource.Play();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Angle = 0;

        //Stop sound when dragging ends
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        dragPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
