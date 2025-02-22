using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeLayout : MonoBehaviour
{
    [SerializeField] GameObject[] Layouts;

    void Awake()
    {
        Instantiate(Layouts[PlayerPrefs.GetInt("difficulty", 0)], transform);

        // Ignores first 2 (Start & End pipes)
        for (int i = 2; i < transform.GetChild(0).childCount; i++) 
        {
            transform.GetChild(0).GetChild(i).gameObject.AddComponent<PipeController>();
            transform.GetChild(0).GetChild(i).gameObject.AddComponent<BoxCollider2D>();
        }
    }

    public void CheckPipes()
    {
        // Ignores first 2 (Start & End pipes)
        for (int i = 2; i < transform.GetChild(0).childCount; i++)
        {
            if (!transform.GetChild(0).GetChild(i).GetComponent<PipeController>().solved) return;
        }

        print("Yipee");
    }
}
