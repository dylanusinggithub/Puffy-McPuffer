using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    // Specify the name or index of the scene to load for this level
    [SerializeField] private string levelName;

    // On Mouse Down Function
    void OnMouseDown()
    {
        // Check if the levelName is set, then load the scene associated with this object
        if (!string.IsNullOrEmpty(levelName))
        {
            UnityEngine.Debug.Log("Loading level: " + levelName);
            SceneManager.LoadScene(levelName); // Load the specified scene
        }
        else
        {
            UnityEngine.Debug.LogWarning("Level name not specified for " + gameObject.name);
        }
    }
}
