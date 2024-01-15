using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BigLifeBar : MonoBehaviour
{

    public GameEntityObject TargetEntity;
    public RectTransform BarFrame;
    public RectTransform LifeFrame;
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
        BarFrame.localScale = new Vector3(0.0f, BarFrame.localScale.y, BarFrame.localScale.z);
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
            BarFrame.localScale = new Vector3(scale, BarFrame.localScale.y, BarFrame.localScale.z);
        }

        // postupne odstraneni
        if (_isVisible && _destroy)
        {
            float scale = Mathf.Lerp(1.0f, 0.0f, _time / ShowTime);
            _time += Time.deltaTime;
            if (scale <= 0.0f)
            {
                Destroy(gameObject);
            }
            BarFrame.localScale = new Vector3(scale, BarFrame.localScale.y, BarFrame.localScale.z);
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
