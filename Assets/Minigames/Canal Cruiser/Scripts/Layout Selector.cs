using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutSelector : MonoBehaviour
{
    [SerializeField]
    int levelIndex;

    [SerializeField]
    GameObject createPrefab;
    GameObject Puffy;

    enum startAnimationState { DriveOnScreen, JumpBack }
    startAnimationState Animation;

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++) transform.GetChild(i).gameObject.SetActive(false);

        transform.GetChild(levelIndex).gameObject.SetActive(true);
        if (levelIndex != 0) this.enabled = false;

        Vector2 pos = new Vector2(-10, 0);
        Instantiate(createPrefab, pos, Quaternion.identity);

        Puffy = GameObject.Find("Player");
    }

    private void FixedUpdate()
    {
        switch (Animation)
        {
            case startAnimationState.DriveOnScreen:
                {

                }
                break;
            case startAnimationState.JumpBack:
                {

                }
                break;

        }

    }
