using UnityEngine;
using UnityEngine.EventSystems;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;


// adapted from https://www.youtube.com/watch?v=kWRyZ3hb1Vc, "Drag and Drop in Unity UI"
public class SteeringController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public float Angle = 0;
    public Vector2 dragPoint, Mouse;

    GameObject MovingThing;

    private AudioSource audioSource; // Steering SFX AudioSource reference

    private void Start()
    {
        MovingThing = GameObject.Find("Moving Thing");
        audioSource = GetComponent<AudioSource>(); // Get AudioSource component
    }

    public void OnDrag(PointerEventData eventData)
    {
        Mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Mouse.Set(Mouse.x, dragPoint.y);

        Angle = Vector2.SignedAngle(Mouse - new Vector2(MovingThing.transform.position.x, 0), dragPoint);
        
        GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, Angle);

        // Play Steer SFX if not already playing
        if (!audioSource.isPlaying)
        {
            audioSource.loop = true; // Keep looping while turning
            audioSource.Play();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Angle = 0;

        //Stop Steer SFX when dragging ends
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dragPoint -= new Vector2(MovingThing.transform.position.x, 0);
    }
}
