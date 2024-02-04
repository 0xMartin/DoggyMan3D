using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroCinematic : MonoBehaviour
{

    private void Awake() {
        AudioListener.volume = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnCinematicEnd()
    {
        SceneManager.LoadScene(1);
    }

}