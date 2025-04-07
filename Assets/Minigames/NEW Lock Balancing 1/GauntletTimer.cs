using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GauntletTimer : MonoBehaviour
{
    public TextMeshProUGUI timertext;
    public float remainingtime;
    public GameObject screen, objects;
    public PuffyController controller;
    public WheelSteering steering;
    private void Update()
    {
        if (remainingtime < 0)
        {
            remainingtime = 1;
            screen.SetActive(true);
            objects.SetActive(false);
            controller.enabled = false;
            steering.enabled = false;
        }
        remainingtime -= Time.deltaTime;
        int seconds = Mathf.FloorToInt(remainingtime % 60);
        timertext.text = $"{Mathf.FloorToInt(seconds)}s";
    }
}
