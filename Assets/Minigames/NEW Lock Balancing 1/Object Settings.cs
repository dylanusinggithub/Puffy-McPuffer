using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectSettings : MonoBehaviour
{
    [SerializeField, Range(0, 3)]
    float waitSeconds;

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

        GameObject GO = Instantiate(objects[Random.Range(0, objects.Length)], transform.position + new Vector3(0, offset, 0), Quaternion.identity);

        GO.transform.parent = transform.parent;
        GO.AddComponent<Rigidbody2D>();
        GO.AddComponent<BoxCollider2D>();

        Destroy(transform.gameObject);
    }

}
