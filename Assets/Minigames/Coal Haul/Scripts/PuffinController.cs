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

    void Start()
    {
        cam = Camera.main;
        gameOverPanel.SetActive(false);
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
            GameOver();  //Game Over FUnciton is called
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
}
