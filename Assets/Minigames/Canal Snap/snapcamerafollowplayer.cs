using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snapcamerafollowplayer : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        transform.position = player.transform.position + new Vector3(4, 1, -5);
    }
}
