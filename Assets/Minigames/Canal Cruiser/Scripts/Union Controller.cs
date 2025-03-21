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
    [SerializeField] float waterPecentage = 0;

    public float chaseDistance;
    float chaseOffset;

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
        if (Wave.transform.position.x > chaseDistance)
        {
            transform.SetParent(null);
            GetComponent<Animator>().enabled = false;
            this.enabled = false;
            return;
        }
        else if(Wave.transform.position.x > chaseDistance * 0.75f)
        {
            // Adds a offset to the minimum distance the Wave can be from Puffy starting at the last 25% it'll 
            // start to come closer and closer till the last 5% where it'll be right against you
            chaseOffset = Mathf.Lerp(0, 6, (Wave.transform.position.x - (chaseDistance * 0.75f)) / (chaseDistance * 0.20f));
        }

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
        else
        {
            Wave.transform.localPosition = new Vector2(Mathf.Lerp(3, -3 + chaseOffset, waterPecentage / SS.scoreWin), Wave.transform.localPosition.y);
        }
    }


    IEnumerator ChangeMovement(bool MoveBack)
    {
        float waterSeconds = 1;
        int waterSmoothness = 100;

        if (MoveBack)
        {
            for (int i = waterSmoothness; i > 0; i--)
            {
                yield return new WaitForSeconds(waterSeconds / waterSmoothness);
                waterPecentage -= waterSeconds / waterSmoothness;
            }
        }
        else
        {
            for (int i = 0; i < waterSmoothness; i++)
            {
                yield return new WaitForSeconds(waterSeconds / waterSmoothness);
                waterPecentage += waterSeconds / waterSmoothness;
            }
        }

        waterPecentage = Mathf.RoundToInt(waterPecentage);
    }
}
