using UnityEngine;

public class PuffyController : MonoBehaviour
{
    NEWLockBalancing GM;

    void Start()
    {
        GM = GameObject.Find("GameManager").GetComponent<NEWLockBalancing>();
    }

    // Hit by obstacle
    void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.transform.position = new Vector2(0, -10);

        if(collision.gameObject.transform.tag == "Collectable") GM.StartCoroutine(GM.changeHeight(true));
        else GM.StartCoroutine(GM.changeHeight(false));
    }
}
