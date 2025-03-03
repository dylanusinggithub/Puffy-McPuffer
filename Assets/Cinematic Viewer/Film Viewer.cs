using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FilmViewer : MonoBehaviour
{
    [SerializeField] GameObject LevelComic;
    [SerializeField] GameObject Film;
    [SerializeField] AnimationClip AppearAnimation;

    [SerializeField] RenderTexture FullScreenVideo, PreviewVideo;

    GameObject Level, ComicViewer;


    int height = 920;

    public void BTN_Exit()
    {
        SceneManager.LoadScene("Level Select Map");
    }

    // Start is called before the first frame update
    void Start()
    {
        ComicViewer = GameObject.Find("Comic Viewer");
        ComicViewer.SetActive(false);

        int LevelsComplete = PlayerPrefs.GetInt("Levels Unlocked", -1);
        
        if(LevelsComplete > 0) GenerateLevelFilm(0, "Union");
        if(LevelsComplete > 1) GenerateLevelFilm(1, "Forth Clyde");
        if(LevelsComplete > 2) GenerateLevelFilm(2, "Caledonian");
    }

    void GenerateLevelFilm(int LevelIndex, string Name)
    {
        var LD = LevelDesigner.Instance.Levels[LevelIndex];
        Level = Instantiate(LevelComic, transform).transform.GetChild(1).gameObject;

        // If Level 0 = -120, 1 = 160, 2 = 440
        Level.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, LevelIndex % 3 * 285 - 100);

        Level.transform.parent.GetComponentInChildren<TextMeshProUGUI>().text = Name + " Canal";

        // Create films for each StartScene
        EditFilmToType(LD.StartScreen, "Start Comics");

        // Create films for each cutscene
        for (int i = 0; i < LD.Sequence[i].Cutscene.Count; i++) EditFilmToType(LD.Sequence[i].Cutscene, "Level Cutscene " + (i + 1));

    }

    void EditFilmToType(List<Object> Element, string Title)
    {
        if (Element.Count == 0) return;

        GameObject Comic = Instantiate(Film, Level.transform);

        Comic.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Title;

        Comic.GetComponent<FilmController>().ComicViewer = ComicViewer; 
        Comic.GetComponent<FilmController>().Comics = Element;

        Comic.GetComponent<FilmController>().FullscreenVideo = FullScreenVideo;
        Comic.GetComponent<FilmController>().PreviewVideo = PreviewVideo;
        Comic.GetComponent<FilmController>().CreatePreview();

        Comic.GetComponent<FilmController>().AppearAnimation = AppearAnimation;

    }
}
