using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject Layouts;
    public GameObject PipeHolder; //pipes (3)
    public GameObject PipeHolder2; //pipes (4)
    public GameObject[] Pipes;
    public GameObject[] Pipes2;

    public GameObject pipechange1; //for pipes (3)
    public GameObject pipechange2; //for pipes (4)

    public bool condition = false;
    public GameObject[] BrokenObjects;
    public GameObject[] BrokenObjectsForLayout2;

    [SerializeField]
    int totalPipes = 0;

    [SerializeField]
    int correctPipes = 0;

    public bool gamewon = false;

    // Start is called before the first frame update
    void Start()
    {

        int rand1 = Random.Range(0, 2);
        
        if (rand1 == 1)
        {
            totalPipes = 0;
            PipeHolder.SetActive(true);
            PipeHolder2.SetActive(false);

            totalPipes = PipeHolder.transform.childCount;

            Pipes = new GameObject[totalPipes];
            
            pipechange1.SetActive(true);
            pipechange2.SetActive(false);

            for (int i = 0; i < Pipes.Length; i++)
            {
                Pipes[i] = PipeHolder.transform.GetChild(i).gameObject;

                int rand2 = Random.Range(0, 5);

                if (rand2 == 1)
                {
                    //Pipes[i].GetComponent<Renderer>().material.color = Color.black;
                    //condition = true;
                    BrokenObjects[i].SetActive(true);

                }
            }
        }
        else if (rand1 == 0)
        {
            totalPipes = 0;
            PipeHolder.SetActive(false);
            PipeHolder2.SetActive(true);
            pipechange2.SetActive(true);
            totalPipes = PipeHolder2.transform.childCount;

            
            pipechange1.SetActive(false);

            Pipes2 = new GameObject[totalPipes];

            for (int i = 0; i < Pipes2.Length; i++)
            {
                Pipes2[i] = PipeHolder2.transform.GetChild(i).gameObject;

                int rand2 = Random.Range(0, 5);

                if (rand2 == 1)
                {
                    //Pipes2[i].GetComponent<Renderer>().material.color = Color.black;
                    //condition = true;
                    BrokenObjectsForLayout2[i].SetActive(true);
                }
            }
        }
    }

    public void correctMove()
    {
        correctPipes++;

        if (correctPipes == totalPipes)
        {
            Debug.Log("You win!");
            gamewon = true;
        }
    }

    public void wrongMove()
    {
        correctPipes--;
    }

    //pipegrey_23,35(2),23(2),35(1),36,35,23(1),
}
