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
    public void PlayCanal()
    {
        SceneManager.LoadScene("River Raiders");
    }
}
