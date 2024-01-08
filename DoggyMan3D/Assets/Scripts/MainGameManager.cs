using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{

    public GameEntityObject PlayerRef;

    private static PlayerSave _playerSave;

    public static PlayerSave GetPlayerSave() {
        return _playerSave;
    }

    void Start()
    {
        _playerSave = new PlayerSave();
        _playerSave.Name = "Player Name";
        _playerSave.Level = 1;
        _playerSave.PlayerRef = PlayerRef;
        
        PlayerRef.Name = _playerSave.Name;

        SaveSystem.SavePlayer(_playerSave);
    }

    void Update()
    {
        
    }
}
