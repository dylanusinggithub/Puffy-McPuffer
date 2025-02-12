using UnityEngine;
using UnityEngine.EventSystems;


// adapted from https://www.youtube.com/watch?v=kWRyZ3hb1Vc, "Drag and Drop in Unity UI"
public class SteeringController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public float Angle = 0;
    Vector2 dragPoint;

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 Mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Mouse.Set(dragPoint.x, Mouse.y);

        Angle = Vector2.SignedAngle(Mouse, dragPoint);

        GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, Angle);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Angle = 0;
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        dragPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
