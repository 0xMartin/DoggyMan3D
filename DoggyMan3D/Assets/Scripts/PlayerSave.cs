using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerSave
{

    public string Name = "";
    public int Level = 1;
    public List<Item> Inventory = new List<Item>();
    public GameEntityObject PlayerRef = null;

    public bool AddItem(Item item)
    {
        if (this.Inventory.Count >= 4)
        {
            return false;
        }

        if (PlayerRef != null)
        {
            AudioSource.PlayClipAtPoint(PlayerRef.TakeItemSound, PlayerRef.transform.position, PlayerRef.TakeItemSoundVolume);
        }
        this.Inventory.Add(item);
        return true;
    }

    

}
