using System;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

    // datove objekty vyuzivane interne
    public enum ItemType
    {
        HEALTH_POTION,
        STAMINA_POTION,
        STRENGTH_POTION,
        KEY
    }

    [Serializable]
    public class ItemData
    {
        public string Name;
        public Sprite Icon;
        public Item.ItemType Type;

        public List<float> ParameterValues;

        public float Time { get; set; }

        public GameObject ItemUseFX;
    }

    // nastaveni itemu

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
            ItemData data = new ItemData
            {
                Name = this.Name,
                Icon = this.Icon,
                Type = this.Type,
                ParameterValues = new List<float>(),
                Time = 0.0f,
                ItemUseFX = this.ItemUseFX
            };
            foreach (float par in this.ParameterValues)
            {
                data.ParameterValues.Add(par);
            }
            if (MainGameManager.GetPlayerSave().AddItem(data))
            {
                Destroy(this.gameObject);
            }
        }
    }

}
