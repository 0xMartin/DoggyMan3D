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
            Exit.onClick.AddListener(OnCreditsEnd);
        }
    }

    public void OnCreditsEnd()
    {
        AudioSource.PlayClipAtPoint(ButtonSound, new Vector3(0.0f, 1.0f, -10.0f), ButtonSoundVolume);
        // prejde zpet do menu
        SceneManager.LoadScene(0);
    }

}
