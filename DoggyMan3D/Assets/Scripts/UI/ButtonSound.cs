using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour
{
    public AudioClip Sound;
    public float ButtonSoundVolume = 0.7f;

    private Button _button;
    private Transform _mainAudioPlayerTransform;

    void Start()
    {
        if (MainGameManager.GetBgAudioSource() != null)
        {
            _mainAudioPlayerTransform = MainGameManager.GetBgAudioSource().transform;
        }
        _button = GetComponent<Button>();
        _button.onClick.AddListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        AudioSource.PlayClipAtPoint(Sound, _mainAudioPlayerTransform.position, ButtonSoundVolume);
    }
}
