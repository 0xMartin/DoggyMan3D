using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
    public Button MainExit;

    [Header("Components - New")]
    public Button NewGamePlay;
    public Button NewGameBack;

    [Header("Components - Load")]
    public Button LoadGameBack;

    [Header("Components - About")]
    public Button AboutBack;

    private GameInputs _input;

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
    }

    // LOAD ################################################################

}
