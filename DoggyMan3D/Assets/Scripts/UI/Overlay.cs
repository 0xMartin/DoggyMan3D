using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Overlay : MonoBehaviour
{
    public Image ImageToFade;
    public float Duration = 2.0f;
    public bool FadeIn = true;

    void Start()
    {
        StartCoroutine(FadeImageRoutine());
    }

    IEnumerator FadeImageRoutine()
    {
        float targetAlpha = FadeIn ? 1.0f : 0.0f;
        float initialAlpha = FadeIn ? 0.0f : 1.0f;
        Color currentColor = ImageToFade.color;
        currentColor.a = initialAlpha;
        ImageToFade.color = currentColor;

        float timer = 0.0f;
        while (timer < Duration)
        {
            float alpha = Mathf.Lerp(initialAlpha, targetAlpha, timer / Duration);
            currentColor.a = alpha;
            ImageToFade.color = currentColor;

            timer += Time.deltaTime;
            yield return null;
        }

        currentColor.a = targetAlpha;
        ImageToFade.color = currentColor;
    }
}
