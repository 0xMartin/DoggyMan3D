using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCrossfade : MonoBehaviour
{
    public AudioSource[] audioSources;
    public AnimationCurve volumeCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public float fadeTime = 2.0f;

    private int currentAudioIndex = 0;
    private float currentFadeTime = 0.0f;
    private bool isCrossfading = false;

    void Start()
    {
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.volume = 0.0f;
        }
    }

    public void StartCrossfade(int nextAudioIndex)
    {
        if (isCrossfading)
            return;

        isCrossfading = true;
        currentFadeTime = 0.0f;

        currentAudioIndex = nextAudioIndex;
        audioSources[currentAudioIndex].Play();
    }

    private void Update()
    {
        if (isCrossfading)
        {
            currentFadeTime += Time.deltaTime;

            // case prechodu u konce
            if (currentFadeTime >= fadeTime)
            {
                // zastavi fade + uplne vypne vsechny ostatni audio klipy
                isCrossfading = false;
                foreach (AudioSource audioSource in audioSources)
                {
                    if (audioSource != audioSources[currentAudioIndex])
                        audioSource.Stop();
                }
                return;
            }

            // postupne zesilovani nasledujiciho "vybreneho" audio klipu a zaroven zeslavovani vsech ostatnich
            float t = currentFadeTime / fadeTime;
            float volumeMultiplier = volumeCurve.Evaluate(t);
            foreach (AudioSource audioSource in audioSources)
            {
                if (audioSource == audioSources[currentAudioIndex])
                    audioSource.volume = volumeMultiplier;
                else
                    audioSource.volume = 1.0f - volumeMultiplier;
            }
        }
    }
}
