using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    [SerializeField] List<Slider> PullTabs = new List<Slider>();
    float CompletitionThreshold = 0.9f;

    void Start()
    {
        if (gameObject.name.ToUpper().Contains("INTRO"))
        {
            gameObject.AddComponent<BoxCollider2D>();

            if (GetComponentInChildren<Canvas>() != null)
            {
                GetComponentInChildren<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
                GetComponentInChildren<Canvas>().worldCamera = Camera.main;
                transform.parent.GetComponent<Canvas>().sortingOrder = 1000;
            }
        }

        foreach (Slider Tab in Resources.FindObjectsOfTypeAll(typeof(Slider)))
        {
            if (Tab.transform.IsChildOf(transform))
            {
                PullTabs.Add(Tab);
                Tab.onValueChanged.AddListener(delegate { ValueUpdated(Tab); });
            }
        }
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            if (gameObject.name.ToUpper().Contains("INTRO")) LevelDesigner.Instance.StartLevel();
        }        
    }


    public void ValueUpdated(Slider SliderObject)
    {
        if(this.enabled == false) return;

        // If value - aka how far its been dragged - isn't far enough then don't do anything
        // else check every slider to see if they're complete and if so go to next level/comic
        if (SliderObject.value < SliderObject.maxValue * CompletitionThreshold) return;

        foreach(Slider slider in PullTabs) if(slider.value < slider.maxValue * CompletitionThreshold) return;

        this.enabled = false;
        StartCoroutine(NextLevel());
    }

    IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(1);

        LevelDesigner.Instance.StartLevel();
    }
}
