using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelNameScreen : MonoBehaviour
{
    public TextMeshProUGUI LevelName;
    public Image LevelImage;

    public float Duration = 3.0f;

    public void ShowScreen(string levelName, Sprite levelImage)
    {
        if (levelName != null)
        {
            this.LevelName.text = levelName;
        }
        if (levelImage != null)
        {
            this.LevelImage.sprite = levelImage;
            this.LevelImage.enabled = true; 
        }
        else
        {
            this.LevelImage.enabled = false;    
        }
        this.gameObject.SetActive(true);
    }

    public void HideScreen()
    {
        this.gameObject.SetActive(false);
    }
}
