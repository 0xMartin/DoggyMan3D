using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    public Button Exit;
    public AudioClip ButtonSound;
    public float ButtonSoundVolume = 0.7f;
    public Transform CameraPosition;

    private void Start()
    {
        if (Exit != null)
        {
            Exit.onClick.AddListener(OnBackButtonClick);
        }
    }

    private void Awake()
    {
        AudioListener.volume = 1.0f;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnCreditsEnd()
    {
        // prejde zpet do menu
        SceneManager.LoadScene(SceneList.MAIN_MENU);
    }

    public void OnBackButtonClick()
    {
        AudioSource.PlayClipAtPoint(ButtonSound, CameraPosition.position, ButtonSoundVolume);
        // prejde zpet do menu
        SceneManager.LoadScene(SceneList.MAIN_MENU);
    }

}
