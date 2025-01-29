using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ObjectDropper : MonoBehaviour
{
    [SerializeField, Range(0, 3)] 
    float spawnfequency;
    float timer;
    bool spawning = false;

    [SerializeField]
    GameObject[] Layouts;
    GameObject GO;

    WaterController WC;

    private void Start()
    {
        WC = GetComponent<WaterController>();
        timer = spawnfequency;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timer < 0 && !spawning)
        {
            GO = Instantiate(Layouts[Random.Range(0, Layouts.Length)], new Vector2(0, 4), Quaternion.identity);
            spawning = true;
        }
        else if(spawning)
        {
            if (GO.transform.childCount == 0)
            {
                Destroy(GO);
                timer = spawnfequency;
                spawning = false;
            }
            else
            {
                for (int i = 0; i < GO.transform.childCount; i++)
                {
                    if (GO.transform.GetChild(i).position.y < WC.waterHeight) // In the water
                    {
                        GO.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = false;
                        GO.transform.GetChild(i).GetComponent<Rigidbody2D>().gravityScale = 0.2f;
                        GO.transform.GetChild(i).GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                        GO.transform.GetChild(i).GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-50, 50);

                        Destroy(GO.transform.GetChild(i).gameObject, 4);
                        GO.transform.GetChild(i).parent = null;

                    }
                }

            }
        }
        else timer -= Time.deltaTime;

    }
}
