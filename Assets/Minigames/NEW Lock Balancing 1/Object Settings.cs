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

    ObjectDropper OD;
    // Start is called before the first frame update

    void Start()
    {
        OD = GameObject.Find("GameManager").GetComponent<ObjectDropper>();

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
        GO.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-10, 10);

        // Hides behind lock wall, which later gets undone 
        GO.GetComponent<BoxCollider2D>().enabled = false;
        Physics2D.IgnoreLayerCollision(0, 0); // Disables Default Colliding with defualt
        GO.GetComponent<SpriteRenderer>().sortingOrder = -5;

        GO.GetComponent<AudioSource>().volume *= PlayerPrefs.GetFloat("Volume", 1);

        // activtates the next layout in sequence if there is one
        if (transform.parent.parent != null)
        {
            if (transform.parent.parent.childCount > 1)
            {
                int SpawnerCount = 0;
                foreach (Transform Spawners in transform.parent.GetComponentInChildren<Transform>())
                {
                    if (Spawners.GetComponent<ObjectSettings>()) SpawnerCount++;
                }

                // Only does anything when its the last object to drop
                if (transform.parent.childCount == SpawnerCount * 2 + SpawnerCount - 1 || SpawnerCount < 2 || (SpawnerCount > 2 && (transform.parent.childCount == SpawnerCount * 2)))
                {
                    // Enable the next layout in sequence
                    transform.parent.parent.GetChild(transform.parent.GetSiblingIndex() + 1).gameObject.SetActive(true);
                    transform.parent.SetParent(null);
                }
            }
            else
            {
                OD.Spawning = false;
                Destroy(transform.parent.parent.gameObject, 1);
                transform.parent.SetParent(null);
            }
        }
        else
        {
            OD.Spawning = false;
            transform.parent.SetParent(null);
        }

        Destroy(transform.gameObject);
    }

}
