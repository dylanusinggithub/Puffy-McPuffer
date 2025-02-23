using System.Collections;
using UnityEngine;

public class ObjectSettings : MonoBehaviour
{
    [SerializeField, Range(0, 3)]
    public float waitSeconds;

    [SerializeField]
    float offset;

    [SerializeField]
    GameObject[] objects;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DropObject());
    }

    IEnumerator DropObject()
    {
        // Hides it for a split second so when it spawns in it's more visible
        GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(0.25f);
        GetComponent<SpriteRenderer>().enabled = true;

        yield return new WaitForSeconds(waitSeconds - 0.25f);

        GameObject GO = Instantiate(objects[Random.Range(0, objects.Length)], new Vector3(transform.position.x, 2f, 0), Quaternion.identity);

        GO.transform.parent = transform.parent;
        GO.AddComponent<Rigidbody2D>();
        GO.AddComponent<BoxCollider2D>();

        GO.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 10);

        // Hides behind lock wall, which later gets undone 
        GO.GetComponent<BoxCollider2D>().enabled = false;
        Physics2D.IgnoreLayerCollision(0, 0); // Disables Default Colliding with defualt
        GO.GetComponent<SpriteRenderer>().sortingOrder = -5;

        GO.GetComponent<AudioSource>().volume *= PlayerPrefs.GetFloat("Volume", 1);

        Destroy(transform.gameObject);
    }

}
