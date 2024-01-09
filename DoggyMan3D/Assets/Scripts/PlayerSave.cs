using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerSave
{

    /// <summary>
    /// Jmeno hrace
    /// </summary>
    public string Name = "";

    /// <summary>
    /// Level ve kterem se hrac nachazi
    /// </summary>
    public int Level = 0; 

    /// <summary>
    /// Inventar predmetu
    /// </summary>
    public List<Item.ItemData> Inventory = new List<Item.ItemData>();

    /// <summary>
    /// Reference na entitu hrace
    /// </summary>
    public GameEntityObject PlayerRef = null;

    public bool AddItem(Item.ItemData item)
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
