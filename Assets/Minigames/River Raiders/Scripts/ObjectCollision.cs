using UnityEngine;

public class ObjectCollision : MonoBehaviour
{
    ScoreManager scoreManager;

    void Start()
    {
        scoreManager = GameObject.Find("GameManager").GetComponent<ScoreManager>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.gameObject.tag == "Collectable") scoreManager.AddScore(1);
        else scoreManager.AddScore(-1);

        if(collision.gameObject.tag == "Player") Destroy(gameObject);
    }
}
