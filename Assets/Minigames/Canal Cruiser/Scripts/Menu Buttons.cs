using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{

    public void BTN_Exit()
    {
        SceneManager.LoadScene("Level Select");
    }

    public void BTN_Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
