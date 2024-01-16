using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BigLifeBar : MonoBehaviour
{

    public GameEntityObject TargetEntity;
    public RectTransform LifeFrame;
    public Image BarFramImage;
    public TextMeshProUGUI EntityName;
    public float ShowTime = 2.5f;

    private bool _isVisible;
    private bool _show;
    private bool _destroy;
    private float _time;

    private void Start()
    {
        if (TargetEntity != null)
        {
            EntityName.text = TargetEntity.Name;
            TargetEntity.OnHit += OnHitEntity;
        }
        else
        {
            Destroy(gameObject);
        }

        gameObject.SetActive(false);
        _isVisible = false;
        _show = false;

        BarFramImage.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        LifeFrame.localScale = new Vector3(0.0f, LifeFrame.localScale.y, LifeFrame.localScale.z);
    }

    private void Update()
    {
        // postupne zobrazeni
        if (_show && !_isVisible)
        {
            float scale = Mathf.Lerp(0.0f, 1.0f, _time / ShowTime);
            _time += Time.deltaTime;
            if (scale >= 1.0f)
            {
                _isVisible = true;
                scale = 1.0f;
            }
            LifeFrame.localScale = new Vector3(scale, LifeFrame.localScale.y, LifeFrame.localScale.z);
            float alpha = scale * 3f;
            alpha = alpha > 1.0f ? 1.0f : alpha;
            BarFramImage.color = new Color(1.0f, 1.0f, 1.0f, alpha);
        }

        // postupne odstraneni
        if (_isVisible && _destroy)
        {
            float alpha = Mathf.Lerp(1.0f, 0.0f, _time / ShowTime);
            _time += Time.deltaTime;
            if (alpha <= 0.0f)
            {
                Destroy(gameObject);
            }
            BarFramImage.color = new Color(1.0f, 1.0f, 1.0f, alpha);
        }
    }

    private void OnHitEntity()
    {
        if (TargetEntity != null)
        {
            RepaintBar();
            if (TargetEntity.Lives == 0)
            {
                _time = 0.0f;
                _destroy = true;
                return;
            }
        }
    }

    private void RepaintBar()
    {
        float scaleX = ((float)TargetEntity.Lives) / TargetEntity.GetMaxLives();
        LifeFrame.localScale = new Vector3(scaleX, LifeFrame.localScale.y, LifeFrame.localScale.z);
    }

    public void ShowBar()
    {
        if (!_isVisible && !_show)
        {
            gameObject.SetActive(true);
            _time = 0.0f;
            _show = true;
        }
    }

}
