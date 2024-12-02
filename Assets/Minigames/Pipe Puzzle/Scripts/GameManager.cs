using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject Layouts;
    public GameObject PipeHolder; //pipes (3)
    public GameObject PipeHolder2; //pipes (4)
    public GameObject PipeHolder3; //pipes (5)
    public GameObject[] Pipes;
    public GameObject[] Pipes2;
    public GameObject[] Pipes3;

    public GameObject pipechange1; //for pipes (3)
    public GameObject pipechange2; //for pipes (4)
    public GameObject pipechange3; //for pipes (5)

    public bool condition = false;
    public GameObject[] BrokenObjects;
    public GameObject[] BrokenObjectsForLayout2;
    public GameObject[] BrokenObjectsForLayout3;

    [SerializeField]
    int totalPipes = 0;

    [SerializeField]
    int correctPipes = 0;

    public bool gamewon = false;

    public GameObject layer;
    public GameObject confetti;

    // Start is called before the first frame update
    void Start()
    {

        int rand1 = Random.Range(0, 3);
        
        if (rand1 == 1)
        {
            totalPipes = 0;
            PipeHolder.SetActive(true);
            PipeHolder2.SetActive(false);
            PipeHolder3.SetActive(false);
            pipechange3.SetActive(false);

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
                    Debug.Log("Worms");

                }
            }
        }
        else if (rand1 == 0)
        {
            totalPipes = 0;
            PipeHolder.SetActive(false);
            PipeHolder2.SetActive(true);
            pipechange2.SetActive(true);
            PipeHolder3.SetActive(false);
            pipechange3.SetActive(false);
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
                    Debug.Log("Worms");
                }
            }
        }
        else if (rand1 == 2)
        {
            totalPipes = 0;
            PipeHolder.SetActive(false);
            PipeHolder2.SetActive(false);
            pipechange2.SetActive(false);
            PipeHolder3.SetActive(true);
            pipechange3.SetActive(true);

            totalPipes = PipeHolder3.transform.childCount;


            pipechange1.SetActive(false);

            Pipes3 = new GameObject[totalPipes];

            for (int i = 0; i < Pipes3.Length; i++)
            {
                Pipes3[i] = PipeHolder3.transform.GetChild(i).gameObject;

                int rand2 = Random.Range(0, 5);

                if (rand2 == 1)
                {
                    //Pipes2[i].GetComponent<Renderer>().material.color = Color.black;
                    //condition = true;
                    BrokenObjectsForLayout3[i].SetActive(true);
                    Debug.Log("Worms");
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
            layer.SetActive(true);
            FindObjectOfType<AudioManager>().Play("Yippee");
            confetti.SetActive(true);
        }
    }

    public void wrongMove()
    {
        correctPipes--;
    }

    

}
