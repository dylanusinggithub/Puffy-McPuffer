using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnionController : MonoBehaviour
{
    GameObject Puffy;
    ScoreScript SS;

    float Velocity;
    GameObject Wave;

    int oldScore = 0;
    float waterPecentage = 0;

    void Start()
    {
        SS = GameObject.Find("Game Manager").GetComponent<ScoreScript>();

        Puffy = GameObject.Find("Player");

        Wave = transform.GetChild(0).gameObject;
        Wave.transform.localPosition = new Vector2(3, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float dist = (Puffy.transform.position.y - Wave.transform.localPosition.y) / 20;
        if (Mathf.Abs(dist) * 20 > 0.1f) Velocity += dist; // Stops moving wave if not far away enough to stop oscillating

        if (Mathf.Abs(Wave.transform.localPosition.y + dist) > 3.2f) Velocity -= dist; // Stops wave from leaving canal

        Wave.transform.localPosition = new Vector2(Wave.transform.localPosition.x, Velocity);
        
        if (SS.score != oldScore)
        {
            if (SS.score > oldScore) StartCoroutine(ChangeMovement(false));
            else StartCoroutine(ChangeMovement(true));

            oldScore = SS.score;
        }
    }


    IEnumerator ChangeMovement(bool MoveBack)
    {
        float waterSeconds = 1;
        int waterSmoothness = 100;

        if (MoveBack)
        {
            for (int i = waterSmoothness; i > 0; i++)
            {
                yield return new WaitForSeconds(waterSeconds / waterSmoothness);
                waterPecentage -= waterSeconds / waterSmoothness;

                Wave.transform.localPosition = new Vector2(Mathf.Lerp(3, -3, waterPecentage / SS.scoreWin), Wave.transform.localPosition.y);
            }
        }
        else
        {
            for (int i = 0; i < waterSmoothness; i++)
            {
                yield return new WaitForSeconds(waterSeconds / waterSmoothness);
                waterPecentage += waterSeconds / waterSmoothness;

                Wave.transform.localPosition = new Vector2(Mathf.Lerp(3, -3, waterPecentage / SS.scoreWin), Wave.transform.localPosition.y);
            }
        }
    }
}
