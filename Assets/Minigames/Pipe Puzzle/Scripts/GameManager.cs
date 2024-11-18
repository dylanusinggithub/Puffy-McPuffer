using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject PipeHolder;
    public GameObject[] Pipes;
    public GameObject[] Pipes2;

    [SerializeField]
    int totalPipes = 0;

    [SerializeField]
    int correctPipes = 0;

    public bool gamewon = false;

    // Start is called before the first frame update
    void Start()
    {
        totalPipes = PipeHolder.transform.childCount;

        Pipes = new GameObject[totalPipes];

        

        for (int i = 0; i < Pipes.Length; i++)
        {
            Pipes[i] = PipeHolder.transform.GetChild(i).gameObject;

            int rand2 = Random.Range(0, 5);

            if (rand2 == 1)
            {
                Pipes[i].GetComponent<Renderer>().material.color = Color.black;
                
            }
        }

        for (int i = 0; i < Pipes2.Length; i++)
        {
            Pipes2[i] = PipeHolder.transform.GetChild(i).gameObject;

            int rand2 = Random.Range(0, 5);

            if (rand2 == 1)
            {
                Pipes2[i].GetComponent<Renderer>().material.color = Color.black;

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
}
