using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{

    [Header("Player reference")]
    public GameEntityObject Player;

    [Header("Player stats")]
    public GameObject HealthBar;
    public GameObject StaminahBar;
    public Text PlayerName;

    [Header("Active Potions")]
    public Image[] PotionImgs;
    public TextMeshProUGUI[] PotionNames;
    public TextMeshProUGUI[] PotionTimes;

    [Header("Inventory")]
    public Image[] InventoryItemImgs;

    [Header("Level info")]
    public TextMeshProUGUI LevelName;


    private Image _healthImage;
    private Image _staminaImage;

    void Start()
    {
        _healthImage = HealthBar.GetComponent<Image>();
        _staminaImage = StaminahBar.GetComponent<Image>();
        PlayerName.text = Player.Name;

        for (int i = 0; i < PotionImgs.Count(); ++i)
        {
            PotionImgs[i].enabled = false;
            PotionNames[i].enabled = false;
            PotionTimes[i].enabled = false;
        }
        foreach (Image img in InventoryItemImgs)
        {
            img.enabled = false;
        }
    }

    void Update()
    {
        // live, stamina & name
        if (Player != null)
        {
            _healthImage.fillAmount = (float)Player.Lives / Player.GetMaxLives();
            _staminaImage.fillAmount = (float)Player.GetStamina() / Player.MaxStamina;
        }
        else
        {
            PlayerName.text = "";
            _healthImage.fillAmount = 0.0f;
            _staminaImage.fillAmount = 0.0f;
        }

        // active potions
        RefreshActivePotions();

        // inventory
        RefreshInventory();

        // level name
        Level l = MainGameManager.GetCurrentLevel();
        if (LevelName != null && l != null)
        {
            LevelName.text = l.Name;
        }
    }

    private void RefreshInventory()
    {
        PlayerSave ps = MainGameManager.GetPlayerSave();
        if (ps != null)
        {
            int index = 0;
            foreach (Item.ItemData item in ps.Inventory)
            {
                InventoryItemImgs[index].enabled = true;
                InventoryItemImgs[index].sprite = item.Icon;
                index++;
            }
            for (int i = index; i < InventoryItemImgs.Count(); ++i)
            {
                InventoryItemImgs[i].enabled = false;
            }
        }
    }

    private void RefreshActivePotions()
    {
        if (Player == null)
        {
            for (int i = 0; i < PotionImgs.Count(); ++i)
            {
                PotionImgs[i].enabled = false;
                PotionNames[i].enabled = false;
                PotionTimes[i].enabled = false;
            }
            return;
        }

        int index = 0;
        foreach (Item.ItemData potion in Player.GetActivePotions())
        {
            PotionImgs[index].sprite = potion.Icon;
            PotionNames[index].text = potion.Name;
            if (potion.ParameterValues.Count() > 0)
            {
                PotionTimes[index].text = TimeFormatter.FormatTimeMMSS(potion.ParameterValues[0] - potion.Time);
            }
            else
            {
                PotionTimes[index].text = "undefined";
            }
            PotionImgs[index].enabled = true;
            PotionNames[index].enabled = true;
            PotionTimes[index].enabled = true;
            index++;
        }
        for (int i = index; i < PotionImgs.Count(); ++i)
        {
            PotionImgs[i].enabled = false;
            PotionNames[i].enabled = false;
            PotionTimes[i].enabled = false;
        }
    }
}
