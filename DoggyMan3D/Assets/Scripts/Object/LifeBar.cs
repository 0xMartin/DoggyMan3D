using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LifeBar : MonoBehaviour
{

    public GameEntityObject TargetEntity;
    public TextMeshProUGUI EntityName;
    public RectTransform LifeBarRect;
    public float YOffset = 1.5f;
    public float VisibleTime = 5.0f;

    private int _lastLifeValue;
    private float time;

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
    }

    private void OnHitEntity()
    {
        if(TargetEntity != null) {
            if(TargetEntity.Lives == 0) {
                Destroy(gameObject);
                return;
            }
        }
        gameObject.SetActive(true);
    }

    private void Update()
    {
        // pokud target entita neexituje tak se life bar odstrani
        if (TargetEntity == null)
        {
            Destroy(gameObject);
        }

        // sledovani zmen zivota
        if (_lastLifeValue != TargetEntity.Lives && TargetEntity.Lives != TargetEntity.GetMaxLives())
        {
            time = 0.0f;
            _lastLifeValue = TargetEntity.Lives;
            RepaintBar();
        }
        else
        {
            time += Time.deltaTime;
            if (time >= VisibleTime)
            {
                gameObject.SetActive(false);
            }
        }

        // Otáčení textu směrem ke hlavni kamere
        GameObject camera = MainGameManager.GetMainCamera();
        if (camera != null)
        {
            Vector3 lookDirection = camera.transform.position - transform.position;
            lookDirection.y = 0;
            transform.rotation = Quaternion.LookRotation(lookDirection) * Quaternion.Euler(0, 180, 0);
        }

        // sledovani pozice target entity
        if (TargetEntity != null)
        {
            transform.position = TargetEntity.transform.position + Vector3.up * YOffset;
        }
    }

    private void RepaintBar()
    {
        float scaleX = ((float)TargetEntity.Lives) / TargetEntity.GetMaxLives();
        LifeBarRect.localScale = new Vector3(scaleX, LifeBarRect.localScale.y, LifeBarRect.localScale.z);
    }

}
