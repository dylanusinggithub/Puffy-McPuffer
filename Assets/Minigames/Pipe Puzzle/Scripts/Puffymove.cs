using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class Puffymove : MonoBehaviour
{

    public GameObject child;

    public Transform parent;

    // Start is called before the first frame update
    void Start()
    {
        //Ham();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
        
        //Invoked when a button is clicked.
        public void Ham(Transform newParent)
        {
            // Sets "newParent" as the new parent of the child GameObject.
            child.transform.SetParent(newParent);

            // Same as above, except worldPositionStays set to false
            // makes the child keep its local orientation rather than
            // its global orientation.
            child.transform.SetParent(newParent, false);

            // Setting the parent to ‘null’ unparents the GameObject
            // and turns child into a top-level object in the hierarchy
            child.transform.SetParent(null);
        }
    
}
