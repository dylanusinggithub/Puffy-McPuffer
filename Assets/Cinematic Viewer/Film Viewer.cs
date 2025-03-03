using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FilmViewer : MonoBehaviour
{
    [SerializeField] GameObject LevelComic;
    [SerializeField] GameObject Film;

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

        GenerateLevelFilm(0, "Union");
        GenerateLevelFilm(1, "Forth Clyde");
        GenerateLevelFilm(2, "Caledonian");
    }

    void GenerateLevelFilm(int LevelIndex, string Name)
    {
        var LD = LevelDesigner.Instance.Levels[LevelIndex];
        Level = Instantiate(LevelComic, transform).transform.GetChild(1).gameObject;

        // If Level 0 = -120, 1 = 160, 2 = 440
        Level.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, LevelIndex % 3 * 285 - 100);

        Level.transform.parent.GetComponentInChildren<TextMeshProUGUI>().text = Name + " Canal";

        // Create films for each StartScene
        EditFilmToType(LD.StartScreen);

        // Create films for each cutscene
        for (int i = 0; i < LD.Sequence[i].Cutscene.Count; i++) EditFilmToType(LD.Sequence[i].Cutscene);

    }

    void EditFilmToType(List<Object> Element)
    {
        if (Element.Count == 0) return;

        GameObject Comic = Instantiate(Film, Level.transform);

        Comic.GetComponent<FilmController>().ComicViewer = ComicViewer; 
        Comic.GetComponent<FilmController>().Comics = Element; 
        Comic.GetComponent<FilmController>().CreatePreview(); 

    }
}
