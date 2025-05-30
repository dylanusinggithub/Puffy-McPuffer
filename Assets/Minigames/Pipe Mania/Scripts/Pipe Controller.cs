using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class PipeController : MonoBehaviour
{
    public bool solved = false;
    public int broken = 0;

    public bool isFixed;
    public bool gaunletMode;

    Sprite BrokenSprite, RepairedSprite;

    public AudioClip RegularPipe, BrokePipe;
    float initalVolume;

    public GameObject ClickParticle, BrokenParticle, SteamyParticle;
    GameObject BrokenObject;

    [Flags] enum Position {Zero = 1, Ninety = 2, OneHundredAndEighty = 4, TwoHundredAndSeventy = 8 }
    [SerializeField] Position CorrectRotations;

    void Awake()
    {
        GetComponent<Animator>().enabled = false;

        BrokenSprite = GetComponent<PipeEditor>().Broken;
        RepairedSprite = GetComponent<PipeEditor>().Repaired;

        isFixed = GetComponent<PipeEditor>().isFixed;

        string Sprite = GetComponent<SpriteRenderer>().sprite.name;

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
        }
        else if (Sprite.Contains("Cross Piece"))
        {
            CorrectRotations |= GetCorrectRotations((int)transform.eulerAngles.z);
            CorrectRotations |= GetCorrectRotations((int)(transform.eulerAngles.z + 90));
            CorrectRotations |= GetCorrectRotations((int)(transform.eulerAngles.z + 180));
            CorrectRotations |= GetCorrectRotations((int)(transform.eulerAngles.z + 270));
        }
        else Debug.LogError("Invalid Sprite? " + Sprite.ToString());

        transform.parent.parent.GetComponent<PipeLayout>().OrderedLeaks();
    }

    void Start()
    {
        initalVolume = GetComponent<AudioSource>().volume;

        if (!isFixed && broken == 0 && !gaunletMode) transform.Rotate(0, 0, Random.Range(0, 3) * 90);

        if (broken == 0) CheckIfCorrect();
        else
        {
            GetComponent<SpriteRenderer>().sprite = BrokenSprite;
            int randomInt = Random.Range(1, 3);
            if (randomInt == 1)
            {
                BrokenObject = Instantiate(BrokenParticle, transform);
            }
            else
            {
                BrokenObject = Instantiate(SteamyParticle, transform);
            }
        }
    }

    public void OnMouseOver()
    {
        // OnMouseDown doesn't support right clicks for some reason
        if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) && enabled && Time.timeScale != 0)
        {
            Destroy(Instantiate(ClickParticle, transform), 2);

            GetComponent<Animator>().enabled = false;

            if (broken > 0)
            {
                GetComponent<AudioSource>().clip = BrokePipe;

                broken--;
                if (broken == 0)
                {
                    GetComponent<PipeController>().enabled = false;
                    GetComponent<SpriteRenderer>().sprite = RepairedSprite;
                    BrokenObject.GetComponent<ParticleSystem>().Stop();

                    // Checks if solved & stops audio when Repaired
                    CheckIfCorrect();
                    transform.parent.parent.GetComponent<PipeLayout>().CheckPipes();
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
    }

    Position GetCorrectRotations(float Direction)
    {
        Direction = Direction / 90;
        Direction = Mathf.RoundToInt(Direction % 4);

        switch (Direction)
        {
            case 4: // 360
            case 0:
                return Position.Zero;
            case 1:
                return Position.Ninety;
            case 2:
                return Position.OneHundredAndEighty;
            case 3:
                return Position.TwoHundredAndSeventy;
            default:
                {
                    // Impossible but here to stop it from shouting at me
                    Debug.LogError("Invalid Rotation " + Direction);
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
            transform.parent.parent.GetComponent<PipeLayout>().OrderedLeaks();
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
            transform.Rotate(new Vector3(0, 0, (RotationAmount * Direction) / 2));
            transform.eulerAngles = new Vector3(0, 0, Mathf.Round(transform.eulerAngles.z));
            transform.localScale *= 0.8f;
            yield return new WaitForSeconds(0.0001f);
            transform.Rotate(new Vector3(0, 0, (RotationAmount * Direction) / 2));
            transform.eulerAngles = new Vector3(0, 0, Mathf.Round(transform.eulerAngles.z));
            transform.localScale *= 1.25f;
        }

        CheckIfCorrect();
    }
}
