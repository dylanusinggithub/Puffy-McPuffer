using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeScript : MonoBehaviour
{
    //Rotations to be randomized
    float[] rotations = { 0, 90, 180, 270};

    //The correct upright rotation
    public float[] correctRotation;

    //A bool to check if the rotation is correct and upright
    [SerializeField]
    bool isPlaced = false;

    //The number of possible correct rotations
    int PossibleRotation = 1;

    //A link the game manager
    GameManager gameManager;

    //Visual representation of complete tile
    public GameObject FilledTile;

    public GameObject Pipes;

    int clickcounter;

    public GameObject broke1;
    public GameObject broke2;
    public GameObject broke0;

    //Locate the game manager via the game manager object
    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        broke0 = this.gameObject.transform.GetChild(0).gameObject;
        broke1 = this.gameObject.transform.GetChild(0).GetChild(0).gameObject;
        broke2 = this.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).gameObject; //I might be able to make gamemanager easier/more efficient if i use this
    }

    private void Start()
    {
        //Set the length of correct rotations
        PossibleRotation = correctRotation.Length;
        //Picks a random number from 0 to the number of angles that are viable
        int rand = Random.Range(0, rotations.Length);

        

        //Randomize rotation based on z axis
        transform.eulerAngles = new Vector3(0, 0, rotations[rand]);

        

        //sciprrrrrrrrrrrrrt
        if (PossibleRotation > 1)
        {
            if (transform.eulerAngles.z == correctRotation[0] || transform.eulerAngles.z == correctRotation[1])
            {
                isPlaced = true;
                gameManager.correctMove();
                 
            }
        }
        else
        {
            if (transform.eulerAngles.z == correctRotation[0])
            {
                isPlaced = true;
                gameManager.correctMove();
                 
            }
        }
        
    }

    private void Update()
    {
        if (clickcounter == 1)
        {           
            broke1.SetActive(true);
        }
        if (clickcounter == 2)
        {
            broke2.SetActive(true);
        }
        if (clickcounter == 3)
        {
            broke0.SetActive(false);
            broke1.SetActive(false);
            broke2.SetActive(false);
        }
    }

    //1 is child, 3 is most parent0

    private void OnMouseDown()
    {
        transform.Rotate(new Vector3(0, 0, 90));
        transform.eulerAngles = new Vector3(0, 0, Mathf.Round(transform.eulerAngles.z));

        clickcounter++;
        FindObjectOfType<AudioManager>().Play("Peeped");
        if (PossibleRotation > 1)
        {
            if (transform.eulerAngles.z == correctRotation[0] || transform.eulerAngles.z == correctRotation[1] && isPlaced == false)
            {
                isPlaced = true;
                gameManager.correctMove();
                FindObjectOfType<AudioManager>().Play("Piped");
                
            }
            else if (isPlaced == true)
            {
                isPlaced = false;
                gameManager.wrongMove();
            }
        }
        else
        {
            if (transform.eulerAngles.z == correctRotation[0] && isPlaced == false)
            {
                isPlaced = true;
                gameManager.correctMove();
                FindObjectOfType<AudioManager>().Play("Piped");

            }
            else if (isPlaced == true)
            {
                isPlaced = false;
                gameManager.wrongMove();
            }
        }
    }

    private void OnMouseOver()
    {
        //FindObjectOfType<AudioManager>().Play("Peeped");
        if (Input.GetMouseButtonDown(1))
        {
            transform.Rotate(new Vector3(0, 0, -90));
            transform.eulerAngles = new Vector3(0, 0, Mathf.Round(transform.eulerAngles.z));

            clickcounter++;

            if (PossibleRotation > 1)
            {
                if (transform.eulerAngles.z == correctRotation[0] || transform.eulerAngles.z == correctRotation[1] && isPlaced == false)
                {
                    isPlaced = true;
                    gameManager.correctMove();
                    FindObjectOfType<AudioManager>().Play("Piped");

                }
                else if (isPlaced == true)
                {
                    isPlaced = false;
                    gameManager.wrongMove();
                }
            }
            else
            {
                if (transform.eulerAngles.z == correctRotation[0] && isPlaced == false)
                {
                    isPlaced = true;
                    gameManager.correctMove();
                    FindObjectOfType<AudioManager>().Play("Piped");

                }
                else if (isPlaced == true)
                {
                    isPlaced = false;
                    gameManager.wrongMove();
                }
            }
        }
    }
}
