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

    [HideInInspector]
    public bool Spawning = false;

    public bool Gauntlet = false;
    public GameObject gauntletthing;

    public List<GameObject> Layouts;
    public List<GameObject> evilLayouts;
    float LockSize;

    List<GameObject> GO = new List<GameObject>();

    WaterController WC;

    [HideInInspector]
    public Algorithms SelectionAlgorithm;
    int randomIndex = 0, previousIndex = -1;

    private void Start()
    {
        WC = GetComponent<WaterController>();
        LockSize = GetComponent<NEWLockBalancing>().LockSize;
        LockSize *= 7; // Lock is always 7 wide at scale 1

        timerDelay = 2; // Start delay
        burstMax = Random.Range(burstMin, burstMaxLimit);
        if (Gauntlet)
        {
            gauntletthing.SetActive(true);
        }

        if (SelectionAlgorithm == Algorithms.InOrder) randomIndex = -1;
        else if (SelectionAlgorithm == Algorithms.RandomWithoutRepeat) randomIndex = Random.Range(0, Layouts.Count);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GetComponent<NEWLockBalancing>().state == NEWLockBalancing.GameState.Complete)
        {
            foreach (GameObject Dropper in GO) Destroy(Dropper);
            this.enabled = false;
        }

        if (timerDelay < 0 && !Spawning)
        {
            if (timerSeparation < 0)
            {
                if (burstCount == burstMax)
                {
                    burstMax = Random.Range(burstMin, burstMaxLimit);
                    timerDelay = burstDelay;
                    burstCount = 0;
                    return;
                }
                burstCount++;

                if (GO.Count > 0) foreach (Transform Dropper in GO[GO.Count - 1].transform)
                {
                    if(Dropper.GetComponent<ObjectSettings>() != null) Dropper.GetComponent<SpriteRenderer>().enabled = false;
                }

                if (Gauntlet == false)
                {
                    switch(SelectionAlgorithm)
                    {
                        case Algorithms.Default:
                            {
                                randomIndex = Random.Range(0, Layouts.Count);
                            }
                            break;

                        case Algorithms.InOrder:
                            {
                                if (randomIndex < Layouts.Count - 1) randomIndex++;
                                else randomIndex = 0;
                            }
                            break;

                        case Algorithms.RandomWithoutRepeat:
                            {
                                if (Layouts.Count > 1)
                                {
                                    while (randomIndex == previousIndex) randomIndex = Random.Range(0, Layouts.Count);
                                    previousIndex = randomIndex;
                                }
                                else randomIndex = 0;
                            }
                            break;
                    }

                    if (!Layouts[randomIndex].name.ToUpper().Contains("SEQUENCE"))
                    {
                        float spawnOffset = 0;
                        float furthestPoint = 0;
                        foreach (Transform OBJ in Layouts[randomIndex].GetComponentInChildren<Transform>())
                        {
                            if (Mathf.Abs(OBJ.position.x) > furthestPoint) furthestPoint = Mathf.Abs(OBJ.position.x);
                        }

                        spawnOffset = Random.Range(furthestPoint - LockSize + 2, LockSize - furthestPoint - 2);

                        GO.Add(Instantiate(Layouts[randomIndex], new Vector2(spawnOffset, 4), Quaternion.identity));
                    }
                    else
                    {
                        GameObject Sequence = Instantiate(Layouts[randomIndex], new Vector3(0, 4), Quaternion.identity);
                        foreach (Transform Layout in Sequence.GetComponentInChildren<Transform>())
                        {
                            GO.Add(Layout.gameObject);
                        }
                    }
                }
                else
                {
                    
                    int randomIndex = Random.Range(0, evilLayouts.Count);


                    if (!evilLayouts[randomIndex].name.ToUpper().Contains("SEQUENCE"))
                    {
                        float spawnOffset = 0;
                        float furthestPoint = 0;
                        foreach (Transform OBJ in evilLayouts[randomIndex].GetComponentInChildren<Transform>())
                        {
                            if (Mathf.Abs(OBJ.position.x) > furthestPoint) furthestPoint = Mathf.Abs(OBJ.position.x);
                        }

                        spawnOffset = Random.Range(furthestPoint - LockSize + 2, LockSize - furthestPoint - 2);

                        GO.Add(Instantiate(evilLayouts[randomIndex], new Vector2(spawnOffset, 4), Quaternion.identity));
                    }
                    else
                    {
                        GameObject Sequence = Instantiate(evilLayouts[randomIndex], new Vector3(0, 4), Quaternion.identity);
                        foreach (Transform evilLayout in Sequence.GetComponentInChildren<Transform>())
                        {
                            GO.Add(evilLayout.gameObject);
                        }
                    }
                }
                


                timerSeparation = burstSeparationDelay;
                Spawning = true;
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
                        GO[i].transform.GetChild(j).GetComponent<Collider2D>().enabled = false;
                        GO[i].transform.GetChild(j).GetComponent<Rigidbody2D>().gravityScale = 0.2f;
                        GO[i].transform.GetChild(j).GetComponent<Rigidbody2D>().velocity = Vector2.zero;

                        Destroy(GO[i].transform.GetChild(j).gameObject, 4);
                        GO[i].transform.GetChild(j).parent = null;

                    }
                    else if (GO[i].transform.GetChild(j).position.y > 6f) // when above lock wall reset sorting order
                    {
                        if (GO[i].transform.GetChild(j).GetComponent<SpriteRenderer>() != null)
                        {
                            GO[i].transform.GetChild(j).GetComponent<SpriteRenderer>().sortingOrder = 0;
                            GO[i].transform.GetChild(j).GetComponent<Collider2D>().enabled = true;
                        }
                    }
                }
            }
        }
    }
}