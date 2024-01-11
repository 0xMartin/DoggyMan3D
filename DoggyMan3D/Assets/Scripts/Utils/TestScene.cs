using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene : MonoBehaviour
{

    public GameEntityObject player;

    void Start()
    {
        PlayerSave ps = new PlayerSave
        {
            Name = "Test User",
            Level = 0,
            PlayerRef = player
        };
        player.EnableMovingStopped(false);
        player.IsEntityEnabled = true;
        player.Name = ps.Name;
        MainGameManager.SetPlayerSave(ps);
    }

}
