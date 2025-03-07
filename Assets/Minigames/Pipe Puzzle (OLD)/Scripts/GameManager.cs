using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public GameObject Win1;
    public GameObject Win2;
    public GameObject Win3;


    public GameObject pipechange1; //for pipes (3)
    public GameObject pipechange2; //for pipes (4)
    public GameObject pipechange3; //for pipes (5)

    public bool condition = false;
    public GameObject[] BrokenObjects;
    public GameObject[] BrokenObjectsForLayout2;
    public GameObject[] BrokenObjectsForLayout3;

    public GameObject pausebutt;

    //[SerializeField]
    public int totalPipes = 0;

    [SerializeField]
    int correctPipes = 0;

    public bool gamewon = false;

    public GameObject layer;
    public GameObject confetti;

    public ParticleSystem drip;
    
    public int brokecounter;

    int difficulty;
    int levelset;

    bool isone;
    bool istwo;
    bool isthree;
    // Start is called before the first frame update
    void Start()
    {
        isone = false;
        istwo = false;
        isthree = false;
        difficulty = PlayerPrefs.GetInt("difficulty", 0);
        Debug.Log("Difficulty is now " + difficulty);
        if (difficulty >= 0 & difficulty < 4)
        {
            levelset = 1;
        }
        else if (difficulty >= 4 & difficulty < 7)
        {
            levelset = 2;
        }
        else if (difficulty >= 7 & difficulty < 11)
        {
            levelset = 3;
        }

            //int rand1 = Random.Range(0, 3);
        
        if (levelset == 1)
        {
            totalPipes = 0;
            PipeHolder.SetActive(true);
            PipeHolder2.SetActive(false);
            PipeHolder3.SetActive(false);
            pipechange3.SetActive(false);
            isone = true;
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
                    brokecounter++;



                }
                else if (rand2 == 2)
                {
                    Vector3 ham = Pipes[i].transform.position;

                    PlayParti(ham);
                }

            }
        }
        else if (levelset == 3)
        {
            totalPipes = 0;
            PipeHolder.SetActive(false);
            PipeHolder2.SetActive(true);
            pipechange2.SetActive(true);
            PipeHolder3.SetActive(false);
            pipechange3.SetActive(false);
            totalPipes = PipeHolder2.transform.childCount;

            istwo = true;
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
                else if (rand2 == 2)
                {
                    Vector3 ham = Pipes2[i].transform.position;

                    PlayParti(ham);
                }

            }
        }
        else if (levelset == 2)
        {
            totalPipes = 0;
            PipeHolder.SetActive(false);
            PipeHolder2.SetActive(false);
            pipechange2.SetActive(false);
            PipeHolder3.SetActive(true);
            pipechange3.SetActive(true);

            totalPipes = PipeHolder3.transform.childCount;

            isthree = true;
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
                else if (rand2 == 2)
                {
                    Vector3 ham = Pipes3[i].transform.position;

                    PlayParti(ham);
                }

            }
        }
    }

    void Update()
    {
        if (levelset == 1)
        {

            foreach (GameObject peeps in Pipes)
            {
                PipeScript peeper = peeps.GetComponent<PipeScript>();
                if (peeper != null && peeper.isPlaced == false)
                {
                    Debug.Log("The pipe is not placed");
                    //play animation here
/*using UnityEngine;

public class WhileLoopExample : MonoBehaviour
{
public bool conditionMet = false;
private int count = 0;

void Update()
{
if (count < 5)
{
Debug.Log("Waiting for condition...");
if (conditionMet)
{
Debug.Log("Condition met! Step: " + count);
count++;
conditionMet = false; // Reset condition
}
}
}
}
*/
}

} //move this out of update maybe? checks too many times but i guess it needs those checks

}
else if (levelset == 2)
{

}
else if (levelset == 3)
{

}
}


public void PlayParti(Vector3 drippos)
{
ParticleSystem newParticle = Instantiate(drip, drippos, Quaternion.identity);
newParticle.Play();
}

public void StopParti()
{
if (drip != null)
{
drip.Stop();
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
                PipeHolder.SetActive(false);
                PipeHolder2.SetActive(false);
                pipechange2.SetActive(false);
                PipeHolder3.SetActive(false);
                pipechange3.SetActive(false);
                pipechange1.SetActive(false);
                pausebutt.SetActive(false);
                if (isone == true)
                {
                Win1.SetActive(true);
                } else if (istwo == true)
                {
                Win2.SetActive(true);
                } else if (isthree == true)
                {
                Win3.SetActive(true);
                }
            }
    }

    public void wrongMove()
    {
        correctPipes--;
    }



}
