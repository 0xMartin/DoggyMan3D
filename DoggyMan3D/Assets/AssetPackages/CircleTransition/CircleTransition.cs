using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CircleTransition : MonoBehaviour
{
    public float Duration = 3.0f;
    public bool IsBlackVisible = true;

    public Action<bool> OnTransitionComplete;

    private Canvas _canvas;
    private Image _blackScreen;

    private static readonly int RADIUS = Shader.PropertyToID("_Radius");
    private bool _initDone = false;

    private void Start()
    {
        init();
    }

    private void init()
    {
        if(_initDone) return;
        _initDone = true;

        _canvas = GetComponent<Canvas>();
        _blackScreen = GetComponentInChildren<Image>();

        // velikost kruhu nastavi na maximum = nebude videt
        if (IsBlackVisible)
        {
            var mat = _blackScreen.material;
            mat.SetFloat(RADIUS, 0.0f);
            this.gameObject.SetActive(true);
        }
        else
        {
            var mat = _blackScreen.material;
            mat.SetFloat(RADIUS, 1.0f);
            this.gameObject.SetActive(false);
        }
    }

    public void ShowOverlay()
    {
        if (!IsBlackVisible)
        {
            init();
            IsBlackVisible = true;
            this.gameObject.SetActive(true);
            StartCoroutine(Transition(Duration, 1.0f, 0.0f));
        }
    }

    public void HideOverlay()
    {
        if (IsBlackVisible)
        {
            init();
            IsBlackVisible = false;
            this.gameObject.SetActive(true);
            StartCoroutine(Transition(Duration, 0.0f, 1.0f));
        }
    }

    private IEnumerator Transition(float duration, float beginRadius, float endRadius)
    {
        var mat = _blackScreen.material;
        var time = 0f;
        while (time <= duration)
        {
            time += Time.deltaTime;
            var t = time / duration;
            var radius = Mathf.Lerp(beginRadius, endRadius, t);
            mat.SetFloat(RADIUS, radius);

            yield return null;
        }

        OnTransitionComplete?.Invoke(beginRadius < endRadius);
        if (beginRadius < endRadius)
        {
            gameObject.SetActive(false);
        }
    }

}
