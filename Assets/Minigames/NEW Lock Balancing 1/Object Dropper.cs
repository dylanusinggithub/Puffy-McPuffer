using System.Collections.Generic;
using UnityEngine;

public class ObjectDropper : MonoBehaviour
{
    
    [HideInInspector] public float burstDelay;
    float timerDelay;

    [HideInInspector] public float burstSeparationDelay;
    float timerSeparation;

    [HideInInspector] public int burstMaxLimit;

    [HideInInspector] public int burstMin;

    int burstMax;
    int burstCount;

    bool spawning = false;

    public List<GameObject> Layouts;

    List<GameObject> GO = new List<GameObject>();

    WaterController WC;

    private void Start()
    {
        WC = GetComponent<WaterController>();
        timerDelay = 2; // Start delay
        burstMax = Random.Range(burstMin, burstMaxLimit);
        spawning = true;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timerDelay < 0)
        {
            if (!spawning || timerSeparation < 0)
            {
                if (burstCount == burstMax)
                {
                    burstMax = Random.Range(burstMin, burstMaxLimit);
                    timerDelay = burstDelay;
                    burstCount = 0;
                    spawning = false;
                    return;
                }
                burstCount++;

                GO.Add(Instantiate(Layouts[Random.Range(0, Layouts.Count)], new Vector2(0, 4), Quaternion.identity));

                // Increases the sorting order each time and hides the previous so that the newest layout is on top and visable
                foreach (Transform Dropper in GO[GO.Count - 1].transform) Dropper.GetComponent<SpriteRenderer>().sortingOrder = burstCount;
                if(burstCount > 1) foreach (Transform Dropper in GO[GO.Count - 2].transform) Dropper.GetComponent<SpriteRenderer>().enabled = false;
                
                timerSeparation = burstSeparationDelay;

                spawning = true;
                return;
            }
            else timerSeparation -= Time.deltaTime;
        }
        else timerDelay -= Time.deltaTime;

        // Check to see if objects have hit water
        
        for (int i = 0; i < GO.Count; i++)
        {
            if (GO[i].transform.childCount == 0)
            {
                Destroy(GO[i]);
                GO.Remove(GO[i]);

                break;
            }
            else
            {
                for (int j = 0; j < GO[i].transform.childCount; j++)
                {
                    if (GO[i].transform.GetChild(j).position.y < WC.waterHeight) // In the water
                    {
                        GO[i].transform.GetChild(j).GetComponent<BoxCollider2D>().enabled = false;
                        GO[i].transform.GetChild(j).GetComponent<Rigidbody2D>().gravityScale = 0.2f;
                        GO[i].transform.GetChild(j).GetComponent<Rigidbody2D>().velocity = Vector2.zero;

                        // When obstacles get hit they bounce and rotate so if i rotate them again it looks weird
                        if(GO[i].transform.GetChild(j).GetComponent<Rigidbody2D>().angularVelocity != 0)
                            GO[i].transform.GetChild(j).GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-50, 50);

                        Destroy(GO[i].transform.GetChild(j).gameObject, 4);
                        GO[i].transform.GetChild(j).parent = null;

                    }
                }
            }
        }
    }
}