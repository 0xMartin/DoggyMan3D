using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    [Header("Player")]
    public GameEntityObject PlayerRef;

    [Header("Main Camera")]
    public GameObject MainCamera;

    [Header("Levels")]
    [Tooltip("Seznam vsech levelu. Zadava se nazev sceny vzdy. Index 0 - prvni level, 1 - druhy level a tak dale..")]
    public string[] LevelNames;
    [Tooltip("Cas jak dloho se bude trvat nez se level plne spusti (postupne odtmaveni obrazovky). Pocitano po nacteni levelu.")]
    public float TimeToShowLevel;
    [Tooltip("Cas jak dloho se bude trvat nez se level plne ukonci (postupne ztmaveni obrazovky).")]
    public float TimeToHideLevel;

    private static PlayerSave _playerSave;
    private static Level _currentLevel;
    private static GameObject _camera;

    public static PlayerSave GetPlayerSave()
    {
        return _playerSave;
    }

    public static Level GetCurrentLevel()
    {
        return _currentLevel;
    }

    public static GameObject GetMainCamera()
    {
        return _camera;
    }

    private void Start()
    {
        _playerSave = null;
        _currentLevel = null;
        _camera = MainCamera;

        _playerSave = new PlayerSave();
        _playerSave.Name = "Player Name";
        _playerSave.Level = 1;
        _playerSave.PlayerRef = PlayerRef;

        PlayerRef.Name = _playerSave.Name;
    }

    private void Update()
    {

    }

    public void LoadPlayerSave()
    {

    }

    public void SavePlayerSave()
    {

    }

    public void LoadLevel(int levelId)
    {

    }

    public void RemoveCurrentLevel()
    {

    }

    public void SpawnPlayer()
    {

    }

    public void GoToNextLevel()
    {

    }

    public void QuitLevel()
    {

    }

}
