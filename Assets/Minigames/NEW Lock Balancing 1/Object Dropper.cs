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
    float LockSize;

    List<GameObject> GO = new List<GameObject>();

    WaterController WC;

    private void Start()
    {
        WC = GetComponent<WaterController>();
        LockSize = GetComponent<NEWLockBalancing>().LockSize;
        LockSize *= 7; // Lock is always 7 wide at scale 1

        timerDelay = 2; // Start delay
        burstMax = Random.Range(burstMin, burstMaxLimit);
        spawning = true;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GetComponent<NEWLockBalancing>().state == NEWLockBalancing.GameState.Complete)
        {
            foreach (GameObject Dropper in GO) Destroy(Dropper);
            this.enabled = false;
        }

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

                if (GO.Count > 0) foreach (Transform Dropper in GO[GO.Count - 1].transform)
                {
                    if(Dropper.GetComponent<ObjectSettings>() != null) Dropper.GetComponent<SpriteRenderer>().enabled = false;
                }
                int randomIndex = Random.Range(0, Layouts.Count);
                float spawnOffset = 0;

                if (!Layouts[randomIndex].name.ToUpper().Contains("SEQUENCE"))
                {
                    float furthestPoint = 0;
                    foreach(Transform OBJ in Layouts[randomIndex].GetComponentInChildren<Transform>())     
                    {
                        if (Mathf.Abs(OBJ.position.x) > furthestPoint) furthestPoint = Mathf.Abs(OBJ.position.x);
                    }

                    spawnOffset = Random.Range(furthestPoint - LockSize + 2, LockSize - furthestPoint - 2);
                }

                GO.Add(Instantiate(Layouts[randomIndex], new Vector2(spawnOffset, 4), Quaternion.identity));
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
                        if (Mathf.Abs(GO[i].transform.GetChild(j).GetComponent<Rigidbody2D>().angularVelocity) < 1)
                            GO[i].transform.GetChild(j).GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-50, 50);

                        Destroy(GO[i].transform.GetChild(j).gameObject, 4);
                        GO[i].transform.GetChild(j).parent = null;

                    }
                    else if (GO[i].transform.GetChild(j).position.y > 4f) // when above lock wall reset sorting order
                    {
                        if (GO[i].transform.GetChild(j).GetComponent<SpriteRenderer>() != null)
                        {
                            GO[i].transform.GetChild(j).GetComponent<SpriteRenderer>().sortingOrder = 0;
                            GO[i].transform.GetChild(j).GetComponent<BoxCollider2D>().enabled = true;
                        }
                    }
                }
            }
        }
    }
}