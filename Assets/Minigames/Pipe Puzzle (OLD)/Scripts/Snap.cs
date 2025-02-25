using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Snap : MonoBehaviour
{
    public List<Transform> snapPoints;
    public List<Draggable> draggableObjects;
    public float snapRange = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Draggable draggable in draggableObjects)
        {
            draggable.dragEndedCallBack = OnDragEnded;
        }
    }

    private void OnDragEnded(Draggable draggable) 
    {
        float closestDistance = -1;
        Transform closestSnapPoint = null;

        foreach(Transform snapPoint in snapPoints) 
        { 
            float currentDistance = Vector2.Distance(draggable.transform.position, snapPoint.position);
            if(closestSnapPoint == null || currentDistance < closestDistance)
            {
                closestDistance = currentDistance;
                closestSnapPoint = snapPoint;
            }
        }

        if(closestSnapPoint != null )
        {
            draggable.transform.localPosition = closestSnapPoint.localPosition;
        }

    }   
}
