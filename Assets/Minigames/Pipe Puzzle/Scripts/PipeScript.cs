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

    //Locate the game manager via the game manager object
    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Start()
    {
        //Set the length of correct rotations
        PossibleRotation = correctRotation.Length;
        //Picks a random number from 0 to the number of angles that are viable
        int rand = Random.Range(0, rotations.Length);

        

        //Randomize rotation based on z axis
        transform.eulerAngles = new Vector3(0, 0, rotations[rand]);

        

        //
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
        if (gameManager.gamewon == true)
        {
            FilledTile.SetActive(true);
        }

        if (clickcounter == 3)
        {
            Pipes.GetComponent<Renderer>().material.color = Color.white;
        }
    }

    /*private void OnMouseDown()
    {
        transform.Rotate(new Vector3(0, 0, 90));
        transform.eulerAngles = new Vector3(0, 0, Mathf.Round(transform.eulerAngles.z));

        clickcounter++;

        if (PossibleRotation > 1)
        {
            if (transform.eulerAngles.z == correctRotation[0] || transform.eulerAngles.z == correctRotation[1] && isPlaced == false)
            {
                isPlaced = true;
                gameManager.correctMove();
                
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
                 
            }
            else if (isPlaced == true)
            {
                isPlaced = false;
                gameManager.wrongMove();
            }
        }
    }*/

    private void OnMouseOver()
    {
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
