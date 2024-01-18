using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    public event Action OnMusicFinished;

    public AudioClip[] musicClips;
    public float BgAudioVolume = 0.5f;

    public void PlayNextMusic()
    {
        if (musicClips == null || musicClips.Length == 0)
        {
            Debug.LogError("(BackgroundMusicManager) Audio clip list is empty!");
            return;
        }

        int id = UnityEngine.Random.Range(0, musicClips.Count());
        AudioClip nextMusicClip = musicClips[id];
        AudioSource bga = MainGameManager.GetBgAudioSource();
        bga.clip = nextMusicClip;
        bga.loop = false;
        bga.volume = BgAudioVolume;
        bga.Play();
        Invoke("MusicFinished", nextMusicClip.length);
    }

    private void MusicFinished()
    {
        OnMusicFinished?.Invoke();
        PlayNextMusic();
    }
}
