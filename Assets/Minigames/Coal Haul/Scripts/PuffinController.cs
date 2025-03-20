 using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class PuffinController : MonoBehaviour
{
    private Vector3 offset;
    private Camera cam;
    private bool isGameOver = false; //isGameOver boolean set to false

    [SerializeField] private GameObject gameOverPanel; //Game Over panel object
    [SerializeField] private GameObject youWinPanel;
    [SerializeField] private CoalHaulLevelManager levelManager;

    void Start()
    {
        cam = Camera.main;
        gameOverPanel.SetActive(false);
        youWinPanel.SetActive(false);
    }


    //On Mouse Down Function
    void OnMouseDown()
    {
        if (isGameOver) return; // Stop input when Game Over
        offset = transform.position - Camera.main.ScreenToWorldPoint
            (new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
    }


    //On Mouse Drag Function
    void OnMouseDrag()
    {
        if (isGameOver) return; // Stop dragging when Game Over
        Vector3 newPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)) + offset;
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
    }


    //If Puffin Collides With Any Walls
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            GameOver();  //Game Over Funciton is called
        }

        //If Puffin Collides With Finish Zone
        if (collision.gameObject.CompareTag("Finish Zone"))
        {
            if (levelManager != null)
            {
                levelManager.SavePuffinPosition(transform.position);
            }
            YouWin(); // You Win 
        }
    }

    //Game Over Function
    void GameOver()
    {
        isGameOver = true; // isGameOver boolean set to true (prevent dragging)
        UnityEngine.Debug.Log("Game Over! Dragging disabled.");

        Rigidbody2D rb = GetComponent<Rigidbody2D>(); //Get Rigidbody2D Component

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true); //Enable Game Over Panel
        }

        Time.timeScale = 0f; // Time scale is set to 0 (Game Pauses)
    }

    void YouWin()
    {
        isGameOver = true;
        UnityEngine.Debug.Log("You Win! Dragging disabled.");

        if (levelManager != null)
        {
            levelManager.WinGame(); //Level Manager handles the correct You Win panel
        }

        Time.timeScale = 0f; //Game pauses
    }

    public void ResetPuffin(Vector3 newPosition)
    {
        isGameOver = false; // Enable dragging again
        transform.position = newPosition; // Move Puffin to new level start position
        Time.timeScale = 1f; // Resume time
    }
}
