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
        SceneManager.LoadScene(0);
    }

    public void OnBackButtonClick()
    {
        AudioSource.PlayClipAtPoint(ButtonSound, new Vector3(0.0f, 1.0f, -10.0f), ButtonSoundVolume);
        // prejde zpet do menu
        SceneManager.LoadScene(0);
    }

}
