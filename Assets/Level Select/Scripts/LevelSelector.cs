using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    [SerializeField, Range(0, 10)] int LevelIndex;
    LevelDesigner LD;

    private void Start()
    {
        LD = GameObject.Find("Comic Panels").GetComponent<LevelDesigner>();
    }

    public void BTN_PlayLevel()
    {
        LD.StartLevel(LevelIndex);
    }
}
