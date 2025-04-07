using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GauntletSettings : MonoBehaviour
{
    public GameObject one, two, three, four, five, six, seven, eight, nine, ten, eleven, twelve, thirteen;
    
    public WaterController WB;

    void Start()
    {
        WB = GetComponent<WaterController>();
        one.SetActive(false);
        two.SetActive(false);
        three.SetActive(false);
        four.SetActive(false);
        five.SetActive(false);
        six.SetActive(false);
        seven.SetActive(false);
        eight.SetActive(false);
        nine.SetActive(false);
        ten.SetActive(true);
        eleven.SetActive(false);
        twelve.SetActive(false);
        thirteen.SetActive(true);
    }

   
    void Update()
    {
        
    }
}
