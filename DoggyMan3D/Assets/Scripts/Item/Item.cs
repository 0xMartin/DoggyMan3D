using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class Item : MonoBehaviour
{

    public enum ItemType
    {
        POTION,
        KEY
    }

    public string Name;
    public Image Icon;
    public Item.ItemType Type;

    public List<string> ParameterNames;
    public List<float> ParameterValues;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.GetPlayerSave().Inventory.Add(this);
            Destroy(this.gameObject);
        }
    }

}
