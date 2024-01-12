using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightIntensityFadeOut : MonoBehaviour
{
    public Light TargetLight;
    public float TimeBeforeChange = 2f;
    public float FadeOutTime = 2f;

    private float _timer = 0f;
    private bool _fadingOut = false;
    private float _startIntensity = 0.0f;
    
    private void Start()
    {
        _startIntensity = TargetLight.intensity;
    }

    private void Update()
    {
        if (!_fadingOut)
        {
            if (_timer < TimeBeforeChange)
            {
                _timer += Time.deltaTime;
            }
            else
            {
                _fadingOut = true;
                _timer = 0f;
            }
        }
        else
        {
            if (_timer < FadeOutTime)
            {
                float t = _timer / FadeOutTime;
                TargetLight.intensity = Mathf.Lerp(_startIntensity, 0f, t);
                _timer += Time.deltaTime;
            }
            else
            {
                _fadingOut = false;
                _timer = 0f;
            }
        }
    }
}
