using UnityEngine;

public class PuffyController : MonoBehaviour
{

    [SerializeField, Range(0, 3f)]
    float obstaclesStunTime;
    float obstaclesStunCooldown;

    [SerializeField, Range(0, 8f)]
    float obstaclesBounceHeight;

    NEWLockBalancing GM;

    void Start()
    {
        GM = GameObject.Find("GameManager").GetComponent<NEWLockBalancing>();
    }

    // Hit by obstacle
    void OnCollisionEnter2D(Collision2D collision)
    {
        obstaclesStunCooldown = obstaclesStunTime;
        GM.puffyStunned = true;

        collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, obstaclesBounceHeight);
        collision.gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }

    void Update()
    {
        if(obstaclesStunCooldown > 0) obstaclesStunCooldown -= Time.deltaTime;
        else GM.puffyStunned = false;
    }
}
