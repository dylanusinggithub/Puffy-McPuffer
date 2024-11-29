using UnityEngine;

public class RopeController : MonoBehaviour
{
    ScoreScript SM;
    Vector3 initalOffset;

    private void Start()
    {
        SM = GameObject.Find("Game Manager").GetComponent<ScoreScript>();
        initalOffset = transform.position;
    }

    void FixedUpdate()
    {
        this.transform.GetChild(0).transform.position = transform.position + initalOffset;


        // Stops it from changing nonexistent ropes
        if (SM.score < transform.childCount)
        {
            // Enables all ropes from 1 to whatever Score is
            for (int i = 1; i < SM.score || i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }

            // Disables the rest of the ropes
            for (int i = SM.score + 1; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
