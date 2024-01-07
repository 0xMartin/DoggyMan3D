using System.Collections;
using System.Collections.Generic;
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

    private Image _healthImage;
    private Image _staminaImage;

    void Start()
    {
        _healthImage = HealthBar.GetComponent<Image>();
        _staminaImage = StaminahBar.GetComponent<Image>();
        PlayerName.text = Player.Name;
    }

    void Update()
    {
        if (Player != null)
        {
            _healthImage.fillAmount = (float) Player.Lives / Player.GetMaxLives();
            _staminaImage.fillAmount = (float) Player.GetStamina() / Player.MaxStamina;
        }
        else
        {
            PlayerName.text = "";
            _healthImage.fillAmount = 0.0f;
            _staminaImage.fillAmount = 0.0f;
        }
    }
}
