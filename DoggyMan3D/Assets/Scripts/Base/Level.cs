using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{

    [Tooltip("Nazev levelu. To co uvidi hrac ve hre.")]
    public string Name;

    [Tooltip("Spawnpoint levelu. Misto kde hrac zacina")]
    public GameObject SpawnPoint;

    [Tooltip("Konec levelu, kdyz se hrac priblizi k tomuto mistu tak je level dokoncen. Game manager automaticky tuto udalos zpracuje.")]
    public GameObject EndPoint;

    [Tooltip("Soundtracky ktere budou v tomto levelu nahodne hrat")]
    public AudioClip[] LevelMusic;

    // callback
    public delegate void LevelFinished();
    public LevelFinished OnLevelFinished;

    private void Start()
    {
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

}