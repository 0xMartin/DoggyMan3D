using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelNameScreen : MonoBehaviour
{
    public TextMeshProUGUI LevelName;
    public Image LevelImage;

    public float Duration = 3.0f;
    public float FadeDuration = 0.4f;


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
        StartCoroutine(FadeIn());
    }

    public void HideScreen()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        float timer = 0f;

        while (timer < FadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, timer / FadeDuration);

            if (LevelName != null)
                LevelName.alpha = alpha;

            if (LevelImage != null)
                LevelImage.color = new Color(LevelImage.color.r, LevelImage.color.g, LevelImage.color.b, alpha);

            timer += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        float timer = 0f;

        while (timer < FadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / FadeDuration);
            if (LevelName != null)
                LevelName.alpha = alpha;

            if (LevelImage != null)
                LevelImage.color = new Color(LevelImage.color.r, LevelImage.color.g, LevelImage.color.b, alpha);

            timer += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }

}
