using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PipeController : MonoBehaviour, IPointerClickHandler
{
    public bool solved = false;
    public int broken = 0;

    public GameObject BrokenPrefab;
    GameObject Prefab;

    public GameObject ClickParticle;

    public AudioClip RegularPipe, BrokePipe;
    float initalVolume;

    [Flags] enum Position {Zero = 1, Ninety = 2, OneHundredAndEighty = 4, TwoHundredAndSeventy = 8 }
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
            CorrectRotations |= GetCorrectRotations((int)(transform.eulerAngles.z + 270));
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
        initalVolume = GetComponent<AudioSource>().volume;

        transform.Rotate(0, 0, Random.Range(0, 3) * 90);

        if (broken == 0) CheckIfCorrect();
        else
        {
            Prefab = Instantiate(BrokenPrefab, transform);
            Prefab.transform.rotation = Quaternion.Euler(0, 0, 0);
            Prefab.transform.localScale *= new Vector2(transform.localScale.x, transform.localScale.y); // Corrects inverted scales

            Prefab.GetComponentInChildren<TextMeshProUGUI>().text = broken.ToString();
        }
    }

    bool Rotating = false;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Time.timeScale == 0) return;

        Destroy(Instantiate(ClickParticle, transform), 2);

        if (broken > 0)
        {
            GetComponent<AudioSource>().clip = BrokePipe;

            broken--;
            Prefab.GetComponentInChildren<TextMeshProUGUI>().text = broken.ToString();
            if (broken == 0)
            {
                Destroy(Prefab);
                CheckIfCorrect();
            }
        }
        else
        {
            GetComponent<AudioSource>().clip = RegularPipe;
            StartCoroutine(RotatePipe());
        }

        GetComponent<AudioSource>().volume = initalVolume * PlayerPrefs.GetFloat("Volume", 1);
        GetComponent<AudioSource>().Play();
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
                return Position.TwoHundredAndSeventy;
            case 4: // For 360
                return Position.Zero;
            default:
                {
                    Debug.LogError("Invalid Rotation" + Direction);
                    return Position.Zero;
                }
        }
    }

    void CheckIfCorrect(int Direction)
    {
        if (CorrectRotations.HasFlag(GetCorrectRotations(Direction)))
        {
            solved = true;
            transform.parent.parent.GetComponent<PipeLayout>().CheckPipes();
        }
        else solved = false;
    }

    void CheckIfCorrect()
    {
        CheckIfCorrect(Mathf.RoundToInt(transform.eulerAngles.z));
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

        CheckIfCorrect();
    }
}
