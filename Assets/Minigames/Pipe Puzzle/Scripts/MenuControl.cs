using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuControl : MonoBehaviour
{
    public Button restart;
    public Button menu;

    // Start is called before the first frame update
    void Start()
    {
        restart.onClick.AddListener(restartgame);
        menu.onClick.AddListener(openmenu);
    }

    // Update is called once per frame
    void restartgame()
    {
        SceneManager.LoadScene("Pipelike");
    }

    void openmenu()
    {
        SceneManager.LoadScene("Level Select");
    }
}
