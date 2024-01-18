using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArenaScript : MonoBehaviour
{
    public GameObject BossArenaEnterGame;

    public Dragon Boss;

    public AudioClip MusicClip;
    public float BgAudioVolume = 0.5f;

    private bool _started = false;

    void Start()
    {
        BossArenaEnterGame.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_started)
        {
            if (other.CompareTag("Player"))
            {
                // aktivace draka + aktivace brany aby hrace nemohl z arenu utect zpet + spusti soundtrack 
                Boss.AI_isActive = true;
                BossArenaEnterGame.SetActive(true);
                PlayMusic();
                _started = true;
            }
        }
    }

    public void PlayMusic()
    {
        if (MusicClip == null)
        {
            return;
        }

        AudioSource bga = MainGameManager.GetBgAudioSource();
        if (bga == null)
        {
            Debug.LogError("BossArena - main music audio source not found!");
            return;
        }
        bga.clip = MusicClip;
        bga.volume = BgAudioVolume;
        bga.loop = true;
        bga.Play();
    }

}
