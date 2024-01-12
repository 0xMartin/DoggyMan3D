using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene : MonoBehaviour
{

    public GameEntityObject player;
    public GameObject camera;

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
        MainGameManager.SetPlayerSave(ps);
        MainGameManager.SetMainCamera(camera);
    }

}
