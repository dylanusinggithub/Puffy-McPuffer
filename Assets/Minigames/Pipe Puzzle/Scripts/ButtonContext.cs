using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class ButtonContext : MonoBehaviour
{
    public Button start;

    // Start is called before the first frame update
    void Start()
    {
        start.onClick.AddListener(startgame);
    }

    void startgame()
    {
        SceneManager.LoadScene("Pipelike");
    }

    
}
