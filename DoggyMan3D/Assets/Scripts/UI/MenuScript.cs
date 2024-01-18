using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{

    public AudioClip ButtonSound;
    public float ButtonSoundVolume = 0.7f;

    [Header("Sections")]
    public GameObject MainSection;
    public GameObject NewGameSection;
    public GameObject LoadGameSection;
    public GameObject AboutSection;

    [Header("Components - Main")]
    public Button MainNewGame;
    public Button MainLoadGame;
    public Button MainAbout;
    public Button MainCredits;
    public Button MainExit;

    [Header("Components - New")]
    public Button NewGamePlay;
    public Button NewGameBack;
    public TMP_InputField NewGamePlayerName;

    [Header("Components - Load")]
    public Transform scrollViewContent;
    public Scrollbar scrollbar;
    public GameObject SaveFileButtonPrefab;
    public int verticalSpacing = 5;
    public int horisontalSpacing = 5;
    public int topOffset = 5;
    public Button LoadGameBack;

    [Header("Components - About")]
    public Button AboutBack;

    [Header("Transitions")]
    public CircleTransition CircleTransition;

    private GameInputs _input;

    private List<GameObject> _instantiatedButtons = new List<GameObject>();

    void Start()
    {
        _input = GetComponent<GameInputs>();
        _input.SetCursorState(false);

        MainSection.SetActive(true);
        NewGameSection.SetActive(false);
        LoadGameSection.SetActive(false);
        AboutSection.SetActive(false);

        // main
        MainNewGame.onClick.AddListener(ClickMainNewGame);
        MainLoadGame.onClick.AddListener(ClickMainLoadGame);
        MainCredits.onClick.AddListener(ClickMainCredits);
        MainAbout.onClick.AddListener(ClickMainAbout);
        MainExit.onClick.AddListener(ClickMainExit);

        // new
        NewGamePlay.onClick.AddListener(ClickNewGamePlay);
        NewGameBack.onClick.AddListener(ClickBack);

        // load
        LoadGameBack.onClick.AddListener(ClickBack);

        // about
        AboutBack.onClick.AddListener(ClickBack);

    }


    private void Awake()
    {
        if (_input != null)
        {
            _input.SetCursorState(false);
        }
        
        AudioListener.volume = 0.0f;
        StartCoroutine(AudioFadeInAsync());
    }

    private IEnumerator AudioFadeInAsync()
    {
        for (float v = 0.0f; v <= 1.0f; v += 0.02f)
        {
            yield return new WaitForSeconds(0.03f);
            AudioListener.volume = v;
        }
    }

    private void ClickBack()
    {
        MainSection.SetActive(true);
        NewGameSection.SetActive(false);
        LoadGameSection.SetActive(false);
        AboutSection.SetActive(false);
        AudioSource.PlayClipAtPoint(ButtonSound, new Vector3(0.0f, 1.0f, -10.0f), ButtonSoundVolume);
    }

    // MAIN ################################################################
    private void ClickMainNewGame()
    {
        MainSection.SetActive(false);
        NewGameSection.SetActive(true);
        AudioSource.PlayClipAtPoint(ButtonSound, new Vector3(0.0f, 1.0f, -10.0f), ButtonSoundVolume);
    }

    private void ClickMainLoadGame()
    {
        MainSection.SetActive(false);
        LoadGameSection.SetActive(true);
        AudioSource.PlayClipAtPoint(ButtonSound, new Vector3(0.0f, 1.0f, -10.0f), ButtonSoundVolume);
        reloadSaveList();
    }

    private void ClickMainCredits()
    {
        // prejde na scenu s titulkama
        SceneManager.LoadScene(2);
    }

    private void ClickMainAbout()
    {
        MainSection.SetActive(false);
        AboutSection.SetActive(true);
        AudioSource.PlayClipAtPoint(ButtonSound, new Vector3(0.0f, 1.0f, -10.0f), ButtonSoundVolume);
    }

    private void ClickMainExit()
    {
        Application.Quit();
        AudioSource.PlayClipAtPoint(ButtonSound, new Vector3(0.0f, 1.0f, -10.0f), ButtonSoundVolume);
    }

    // NEW ################################################################

    private void ClickNewGamePlay()
    {
        AudioSource.PlayClipAtPoint(ButtonSound, new Vector3(0.0f, 1.0f, -10.0f), ButtonSoundVolume);
        StartCoroutine(NewGamePlayAsync());
    }

    private IEnumerator NewGamePlayAsync()
    {
        MainGameManager.SetPlayerNameOnGameManagerStart(NewGamePlayerName.text);
        // je nutne cestu k save nastavit na null, protoze pokud by predesla hra byla z nejakeho save tak by se ted nacetla znovu
        MainGameManager.SetPlayerSaveToLoadOnGameManagerStart(null);    
        CircleTransition.ShowOverlay();
        yield return new WaitForSeconds(CircleTransition.Duration);
        SceneManager.LoadScene(1);
    }

    // LOAD ################################################################

    private IEnumerator LoadGamePlayAsync(string savePath)
    {
        MainGameManager.SetPlayerSaveToLoadOnGameManagerStart(savePath);
        CircleTransition.ShowOverlay();
        yield return new WaitForSeconds(CircleTransition.Duration);
        SceneManager.LoadScene(1);
    }

    private void reloadSaveList()
    {
        // Remove previously instantiated buttons
        foreach (GameObject button in _instantiatedButtons)
        {
            Destroy(button);
        }
        _instantiatedButtons.Clear();

        // Load all saves from game persistent directory
        string[] saveList = SaveSystem.ListFilesInDirectory(SaveSystem.SavePath);

        // show in list
        int index = 0;
        RectTransform contentPanelRect = scrollViewContent.GetComponent<RectTransform>();
        foreach (string savePath in saveList)
        {
            if (!savePath.EndsWith(".json")) continue;

            GameObject fileButton = Instantiate(SaveFileButtonPrefab, scrollViewContent);

            TextMeshProUGUI buttonText = fileButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = Path.GetFileNameWithoutExtension(savePath).Replace("_", " ");

            RectTransform rt = fileButton.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(horisontalSpacing, -index * (rt.sizeDelta.y + verticalSpacing));
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, contentPanelRect.rect.width - 4 * horisontalSpacing);
            index++;

            _instantiatedButtons.Add(fileButton);

            Button button = fileButton.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                AudioSource.PlayClipAtPoint(ButtonSound, new Vector3(0.0f, 1.0f, -10.0f), ButtonSoundVolume);
                StartCoroutine(LoadGamePlayAsync(savePath));
            });
        }

        UpdateContentHeight();
        RepositionButtons();
        scrollbar.value = 1.0f;
    }

    private void UpdateContentHeight()
    {
        float totalHeight = topOffset * 2;

        // Procházení všech potomků (tlačítek) v Content panelu
        for (int i = 0; i < scrollViewContent.childCount; i++)
        {
            RectTransform child = scrollViewContent.GetChild(i).GetComponent<RectTransform>();

            // Přičtení výšky tlačítka a mezery
            totalHeight += child.sizeDelta.y + verticalSpacing;
        }

        // Nastavení vypočítané výšky Content panelu
        scrollViewContent.GetComponent<RectTransform>().sizeDelta = new Vector2(
            scrollViewContent.GetComponent<RectTransform>().sizeDelta.x,
            totalHeight);
    }

    private void RepositionButtons()
    {
        float contentHeight = scrollViewContent.GetComponent<RectTransform>().sizeDelta.y;

        // Posunutí tlačítek o polovinu výšky Content panelu nahoru
        for (int i = 0; i < scrollViewContent.childCount; i++)
        {
            RectTransform child = scrollViewContent.GetChild(i).GetComponent<RectTransform>();

            // Posun v ose Y o polovinu výšky obsahu
            child.anchoredPosition += new Vector2(0, contentHeight * 0.5f - child.sizeDelta.y / 2 - topOffset);
        }
    }

}
