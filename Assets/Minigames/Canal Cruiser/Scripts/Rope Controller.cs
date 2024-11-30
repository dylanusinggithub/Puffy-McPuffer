using UnityEngine;

public class RopeController : MonoBehaviour
{
    ScoreScript SM;

    private void Start()
    {
        SM = GameObject.Find("Game Manager").GetComponent<ScoreScript>();
    }

    void FixedUpdate()
    {
        // Stops it from changing nonexistent ropes
        if (SM.score < transform.childCount + 1)
        {
            // Enables all ropes from 1 to whatever Score is
            for (int i = 0; i < SM.score - 1 || i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }

            // Disables the rest of the ropes
            for (int i = SM.score; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
