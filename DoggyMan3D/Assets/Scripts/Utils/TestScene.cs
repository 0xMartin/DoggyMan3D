using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene : MonoBehaviour
{

    public GameEntityObject player;
    public GameObject cameraMain;

    public GameObject spawn;
    
    void Start()
    {
        PlayerSave ps = new PlayerSave
        {
            Name = "Test User",
            Level = 0,
            PlayerRef = player
        };
        player.IsEnabledMoving = true;
        player.IsEntityEnabled = true;
        player.Name = ps.Name;
        if (spawn != null)
        {
            player.transform.position = spawn.transform.position;
            player.transform.rotation = spawn.transform.rotation;
        }
        MainGameManager.SetPlayerSave(ps);
        MainGameManager.SetMainCamera(cameraMain);
    }

}
