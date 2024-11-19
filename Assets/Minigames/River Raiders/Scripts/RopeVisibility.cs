using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeVisibility : MonoBehaviour
{
    // Reference to the bone objects and collected cargo crate
    public GameObject bone1;
    public GameObject bone2;
    public GameObject bone3;
    public GameObject bone4;
    public GameObject bone5;
    public GameObject collectedCargo;

    // Start is called before the first frame update
    void Start()
    {
        // Make all parts of the rope and crate invisible at the start
        SetRopeVisibility(false);
    }

    // Method to set visibility of the rope and crate
    public void SetRopeVisibility(bool isVisible)
    {
        // Enable or disable renderers based on visibility
        bone1.GetComponent<Renderer>().enabled = isVisible;
        bone2.GetComponent<Renderer>().enabled = isVisible;
        bone3.GetComponent<Renderer>().enabled = isVisible;
        bone4.GetComponent<Renderer>().enabled = isVisible;
        bone5.GetComponent<Renderer>().enabled = isVisible;
        collectedCargo.GetComponent<Renderer>().enabled = isVisible;
    }

    // This function will be called when the player collects the first cargo crate
    public void OnFirstCargoCollected()
    {
        // Make the rope and crate visible after the first cargo crate is collected
        SetRopeVisibility(true);
    }
}
