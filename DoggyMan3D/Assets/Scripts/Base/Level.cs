using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BackgroundMusicManager))]
public class Level : MonoBehaviour
{

    [Tooltip("Nazev levelu. To co uvidi hrac ve hre.")]
    public string Name;

    [Tooltip("Spawnpoint levelu. Misto kde hrac zacina")]
    public GameObject SpawnPoint;

    [Tooltip("Konec levelu, kdyz se hrac priblizi k tomuto mistu tak je level dokoncen. Game manager automaticky tuto udalos zpracuje.")]
    public GameObject EndPoint;

    // callback
    public delegate void LevelFinished();
    public LevelFinished OnLevelFinished;

    private BackgroundMusicManager _randomAudioPlayer;

    private void Awake()
    {
        _randomAudioPlayer = GetComponent<BackgroundMusicManager>();
        if (EndPoint != null)
        {
            ColliderTrigger ct = EndPoint.GetComponent<ColliderTrigger>();
            if (ct != null)
            {
                ct.OnEnter += OnEndPointEnter;
            }
        }
    }

    private void OnEndPointEnter()
    {
        OnLevelFinished?.Invoke();
    }

    public void LevelLoadingDoneEvent()
    {
        _randomAudioPlayer.PlayNextMusic();
    }

}
