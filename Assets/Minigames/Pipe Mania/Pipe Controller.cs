using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PipeController : MonoBehaviour, IPointerClickHandler
{
    public bool solved = false;

    [Flags] enum Position {Zero = 1, Ninety = 2, OneHundredAndEighty = 4, TwoHundredandSeventy = 8 }
    [SerializeField] Position CorrectRotations;

    void Awake()
    {
        string Sprite = GetComponent<Image>().sprite.name;

        if (Sprite.Contains("Corner"))
        {
            CorrectRotations |= GetCorrectRotations((int)transform.eulerAngles.z);
        }
        else if (Sprite.Contains("I Piece"))
        {
            CorrectRotations |= GetCorrectRotations((int)transform.eulerAngles.z);
            CorrectRotations |= GetCorrectRotations((int)(transform.eulerAngles.z + 180));
        }
        else if (Sprite.Contains("T Piece"))
        {
            CorrectRotations |= GetCorrectRotations((int)transform.eulerAngles.z);
            CorrectRotations |= GetCorrectRotations((int)(transform.eulerAngles.z + 90));
            CorrectRotations |= GetCorrectRotations((int)(transform.eulerAngles.z + 180));
        }
        else if (Sprite.Contains("Cross Piece"))
        {
            CorrectRotations |= GetCorrectRotations((int)transform.eulerAngles.z);
            CorrectRotations |= GetCorrectRotations((int)(transform.eulerAngles.z + 90));
            CorrectRotations |= GetCorrectRotations((int)(transform.eulerAngles.z + 180));
            CorrectRotations |= GetCorrectRotations((int)(transform.eulerAngles.z + 270));
        }
        else Debug.LogError("Invalid Sprite? " + Sprite.ToString());
    }

    void Start()
    {
        transform.Rotate(0, 0, Random.Range(0, 3) * 90);
        if (CorrectRotations.HasFlag(GetCorrectRotations((int)transform.eulerAngles.z))) solved = true;
    }

    bool Rotating = false;
    public void OnPointerClick(PointerEventData eventData)
    {
        StartCoroutine(RotatePipe());
        transform.parent.parent.GetComponent<PipeLayout>().CheckPipes();
    }

    Position GetCorrectRotations(int Direction)
    {
        Direction /= 90;

        switch (Direction)
        {
            case 0:
                return Position.Zero;
            case 1:
                return Position.Ninety;
            case 2:
                return Position.OneHundredAndEighty;
            case 3:
                return Position.TwoHundredandSeventy;
            case 4: // For 360
                return Position.Zero;
            default:
                {
                    Debug.LogError("Invalid Rotation" + Direction);
                    return Position.Zero;
                }
        }
    }

    int RotationAmount = 6;
    IEnumerator RotatePipe()
    {
        int Direction;
        if (Input.GetMouseButtonUp(0)) Direction = 1;
        else Direction = -1;

        for (int i = 0; i < 90 / RotationAmount; i++)
        {
            transform.Rotate(0, 0, RotationAmount * Direction);
            yield return new WaitForSeconds(0.001f); // Fastest possible
        }

        if (CorrectRotations.HasFlag(GetCorrectRotations((int)transform.eulerAngles.z))) solved = true;
        else solved = false;
    }
}
