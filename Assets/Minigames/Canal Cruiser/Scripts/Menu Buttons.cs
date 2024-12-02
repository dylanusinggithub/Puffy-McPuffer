using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{

    public void BTN_Exit()
    {
        Time.timeScale = 1;
        Physics2D.gravity = new Vector2(0, -9.81f);
        SceneManager.LoadScene("Level Select Map");
    }

    public void BTN_Retry()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
