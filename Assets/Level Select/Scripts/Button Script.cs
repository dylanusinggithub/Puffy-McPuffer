using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    public void PlayBalance()
    {
        SceneManager.LoadScene("NEW Lock balancing");
    }

    public void PlayPipe()
    {
        SceneManager.LoadScene("Pipebackstory");
    }
}
