using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField, Range(0, 10)] int LevelIndex;
    LevelDesigner LD;

    [SerializeField] GameObject LevelPreview;
    [SerializeField] GameObject LevelButton;
    GameObject ComicPanel, Preview;

    Transform PreviewUI;

    bool openingLevel = false;
    [HideInInspector] public bool MouseMoved = false;
    bool ButtonPressed = false;

    private void Start()
    {
        ComicPanel = GameObject.Find("Comic Panels");
        LD = ComicPanel.GetComponent<LevelDesigner>();

        if (PlayerPrefs.GetInt("Levels Unlocked", 0) < LevelIndex)
        {
            GetComponent<Button>().interactable = false;

            // Disables signs
            transform.parent.GetComponentInChildren<Animator>().enabled = false;
            transform.parent.GetComponentInChildren<Image>().color = GetComponent<Button>().colors.disabledColor;
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

        Transform Grid = PreviewUI.GetChild(2);
        if (Grid.childCount == 0)
        {
            EventTrigger.Entry OnEntry = new EventTrigger.Entry()
            {
                eventID = EventTriggerType.PointerEnter
            };
            OnEntry.callback.AddListener(SwapPreview);

            EventTrigger.Entry OnExit = new EventTrigger.Entry()
            {
                eventID = EventTriggerType.PointerExit
            };
            OnExit.callback.AddListener(SwapPreview);

            for (int i = 0; i < LevelDesigner.Instance.Levels[LevelIndex].Sequence.Length; i++)
            {
                GameObject BTN = Instantiate(LevelButton, Grid);
                BTN.GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString();
                BTN.GetComponent<Button>().onClick.AddListener(delegate { PreviewButtonStart(BTN); });

                BTN.AddComponent<EventTrigger>();

                BTN.GetComponent<EventTrigger>().triggers.Add(OnEntry);
                BTN.GetComponent<EventTrigger>().triggers.Add(OnExit);

            }
        }
    }

    public void SwapPreview(BaseEventData eventData)
    {
        Transform UI = PreviewUI;

        UI.GetChild(1).gameObject.SetActive(!UI.GetChild(1).gameObject.activeInHierarchy); // Description
        UI.GetChild(0).gameObject.SetActive(!UI.GetChild(0).gameObject.activeInHierarchy); // Preview
    }

    public void PreviewButtonStart(GameObject Button)
    {
        // Here be jankons, I would make it assign the index itself but
        // that gets overwritten by the next button so im using the text instead
        LevelDesigner.SinglePlay = true;
        LD.StartMinigame(int.Parse(Button.GetComponentInChildren<TextMeshProUGUI>().text) - 1, LevelIndex);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!ButtonPressed && PlayerPrefs.GetInt("Levels Unlocked", 0) >= LevelIndex)
        {
            GetComponent<Button>().interactable = false;
            DisplayPreview();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ButtonPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ButtonPressed = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!ButtonPressed)
        {
            if(PlayerPrefs.GetInt("Levels Unlocked", 0) >= LevelIndex) GetComponent<Button>().interactable = true;

            MouseMoved = true;
            StartCoroutine(UnravelCheck(1));
        }
    }

    public void BTN_Preview()
    {
        StartCoroutine(BTNPressed());

        if (PreviewUI != null) UnravelPreview();
        else DisplayPreview();
    }

    IEnumerator BTNPressed()
    {
        ButtonPressed = true;
        yield return new WaitForSeconds(1);
        ButtonPressed = false;
    }

    void DisplayPreview()
    {
        MouseMoved = false;

        // If Button is disabled or Preview doesn't exist don't show
        if (!GetComponent<Button>().enabled || LevelPreview == null || openingLevel) return;


        Destroy(Preview);
        Preview = Instantiate(LevelPreview, ComicPanel.transform);
        PreviewUI = Preview.transform.GetChild(0).GetChild(0);
        if (PlayerPrefs.GetInt("Levels Unlocked", 0) > LevelIndex) PreviewUI.GetChild(3).gameObject.SetActive(false);

        Preview.AddComponent<PreviewController>();

        Preview.GetComponent<RectTransform>().anchoredPosition = new Vector2(-300, -100);
        Preview.GetComponent<RectTransform>().localScale = Vector3.one * 2.5f;

        if (PlayerPrefs.GetInt("Levels Unlocked", 0) > LevelIndex) StartCoroutine(DisplayButton());
        else
        {
            PreviewUI.GetComponentInChildren<Button>().onClick.AddListener(delegate { StartLevel(); });
        }
    }

    public IEnumerator UnravelCheck(float Delay)
    {
        yield return new WaitForSeconds(Delay);
        if(MouseMoved) UnravelPreview();
    }

    void UnravelPreview()
    {
        if (!GetComponent<Button>().enabled || Preview == null) return; // If Button is disabled or Preview doesn't exist don't show
        Animator anim = Preview.GetComponentInChildren<Animator>();

        anim.SetFloat("Speed", -1);
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) anim.Play("Appear", 0, 1); // Replays Animation only after it completes

        Destroy(PreviewUI.transform.GetChild(1).gameObject);

        // Removes previous LevelPreview
        float clipLength = anim.runtimeAnimatorController.animationClips[0].length;
        float PlaybackPercent = Mathf.Clamp01(anim.GetCurrentAnimatorStateInfo(0).normalizedTime); // Only care if it's midway
        float DestoryTime = clipLength * PlaybackPercent;
        Destroy(Preview, DestoryTime);
    }
}
