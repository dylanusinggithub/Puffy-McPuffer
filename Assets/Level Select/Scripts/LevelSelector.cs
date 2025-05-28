using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{

    [SerializeField, Range(0, 10)] int LevelIndex;
    LevelDesigner LD;

    [SerializeField] GameObject LevelPreview;
    [SerializeField] GameObject LevelButton;
    GameObject ComicPanel, Preview;

    Transform PreviewUI;

    bool openingLevel = false, PreviewShown = false;

    private void Start()
    {
        ComicPanel = GameObject.Find("Comic Panels");
        LD = ComicPanel.GetComponent<LevelDesigner>();
        
        EnableOrDisableLevel();
    }
   
    public void EnableOrDisableLevel()
    {
        if (PlayerPrefs.GetInt("Levels Unlocked", 0) < LevelIndex && !LevelDesigner.FreePlay)
        {
            GetComponent<Button>().interactable = false;
            GetComponent<Image>().color = GetComponent<Button>().colors.disabledColor;

            GetComponent<Button>().enabled = false;

            // Disables signs
            GetComponentInChildren<Animator>().enabled = false;
            transform.GetChild(0).GetComponent<Image>().color = GetComponent<Button>().colors.disabledColor;
            // Why GetComponetInChildren doesn't work is beyond me
        }
        else
        {
            GetComponent<Button>().interactable = true;
            GetComponent<Image>().color = GetComponent<Button>().colors.normalColor;

            GetComponent<Button>().enabled = true;

            // Enables signs
            GetComponentInChildren<Animator>().enabled = true;
            transform.GetChild(0).GetComponent<Image>().color = GetComponent<Button>().colors.normalColor;
            // Why GetComponetInChildren doesn't work is beyond me
        }
    }

    void StartLevel()
    {
        StartCoroutine(PlayLevel());
        openingLevel = true;
    }

    IEnumerator PlayLevel()
    {
        Animator FadeTransition = GameObject.Find("Fadetransition").GetComponent<Animator>();
        FadeTransition.gameObject.GetComponent<AudioSource>().Play();
        FadeTransition.SetTrigger("End");
        UnravelPreview();
        yield return new WaitForSeconds(1);
        FadeTransition.SetTrigger("Start");

        LD.StartLevel(LevelIndex);
    }

    IEnumerator DisplayButton()
    {
        yield return new WaitForSeconds(0.5f);

        PreviewShown = true;

        Transform Grid = PreviewUI.GetChild(1);
        if (Grid.childCount == 0)
        {
            for (int i = 0; i < LevelDesigner.Instance.Levels[LevelIndex].Sequence.Length; i++)
            {
                GameObject BTN = Instantiate(LevelButton, Grid);
                BTN.GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString();
                BTN.GetComponent<Button>().onClick.AddListener(delegate { PreviewButtonStart(BTN); });
            }
        }
    }
    public void PreviewButtonStart(GameObject Button)
    {
        // Here be jankons, I would make it assign the index itself but
        // that gets overwritten by the next button so im using the text instead
        LevelDesigner.SinglePlay = true;
        LD.StartMinigame(int.Parse(Button.GetComponentInChildren<TextMeshProUGUI>().text) - 1, LevelIndex);
    }

    IEnumerator DisplayRegularButton()
    {
        yield return new WaitForSeconds(0.5f);

        PreviewShown = true;

        PreviewUI.GetChild(2).gameObject.SetActive(true);
        PreviewUI.GetComponentInChildren<Button>().onClick.AddListener(delegate { StartLevel(); });
    }


    // Closes Preview When you click
    void Update()
    {
        if (PreviewShown)
        {
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                UnravelPreview();
            }
        }
    }

    public void DisplayPreview()
    {
        if (PreviewShown)
        {
            UnravelPreview();
            return;
        }

        foreach(Transform child in ComicPanel.GetComponentInChildren<Transform>()) Destroy(child.gameObject);

        // If Button is disabled or Preview doesn't exist don't show
        if (!GetComponent<Button>().enabled || LevelPreview == null || openingLevel) return;


        Preview = Instantiate(LevelPreview, ComicPanel.transform);
        PreviewUI = Preview.transform.GetChild(0).GetChild(0);

        Preview.GetComponent<RectTransform>().anchoredPosition = new Vector2(-300, -100);
        Preview.GetComponent<RectTransform>().localScale = Vector3.one * 2.5f;

        if (PlayerPrefs.GetInt("Levels Unlocked", 0) > LevelIndex || LevelDesigner.FreePlay) StartCoroutine(DisplayButton());
        else StartCoroutine(DisplayRegularButton());
    }

    void UnravelPreview()
    {
        PreviewShown = false;

        if (!GetComponent<Button>().enabled || Preview == null) return; // If Button is disabled or Preview doesn't exist don't show
        Animator anim = Preview.GetComponentInChildren<Animator>();

        anim.SetFloat("Speed", -1);
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) anim.Play("Appear", 0, 1); // Replays Animation only after it completes

        PreviewUI.GetChild(1).gameObject.SetActive(false);

        // Removes previous LevelPreview
        float clipLength = anim.runtimeAnimatorController.animationClips[0].length;
        float PlaybackPercent = Mathf.Clamp01(anim.GetCurrentAnimatorStateInfo(0).normalizedTime); // Only care if it's midway
        float DestoryTime = clipLength * PlaybackPercent;
        Destroy(Preview, DestoryTime);
    }
}
