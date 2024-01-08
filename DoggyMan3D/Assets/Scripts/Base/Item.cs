using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Item : MonoBehaviour
{

    public enum ItemType
    {
        HEALTH_POTION,
        STAMINA_POTION,
        STRENGTH_POTION,
        KEY
    }

    public string Name;
    public Sprite Icon;
    public Item.ItemType Type;

    public List<float> ParameterValues;

    public float Time { get; set; }

    public GameObject ItemUseFX;


    private void OnTriggerEnter(Collider other)
    {
        if (MainGameManager.GetPlayerSave() == null)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            if (MainGameManager.GetPlayerSave().AddItem(this))
            {
                Destroy(this.gameObject);
            }
        }
    }

}
