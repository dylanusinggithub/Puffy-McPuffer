using UnityEngine;

public class RopeController : MonoBehaviour
{
    ScoreScript SM;
    GameObject Puffy;

    private void Start()
    {
        SM = GameObject.Find("Game Manager").GetComponent<ScoreScript>();
        Puffy = GameObject.Find("Player");
    }

    void FixedUpdate()
    {
        // Stops it from changing nonexistent ropes
        if (SM.score < transform.childCount + 1)
        {
            // Enables all ropes from 1 to whatever Score is
            for (int i = 0; i < SM.score - 1 || i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<LineRenderer>().enabled = true;
                transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = true;
            }

            // Disables the rest of the ropes
            for (int i = SM.score; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<LineRenderer>().enabled = false;
                transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        transform.position = new Vector3(0, 0, 0);
    }
}
