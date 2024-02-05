using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slender : MonoBehaviour
{
    public List<Transform> Points = new List<Transform>();
    public GameObject MarkerPrefab;
    public GameObject SlenderObj;
    public GameObject SlenderAttack;
    public float WaitTime = 30.0f;
    public float timeToNextMove;

    private void Start()
    {
        if (MarkerPrefab != null)
        {
            foreach (var point in Points)
            {
                Instantiate(MarkerPrefab, point.position, Quaternion.identity);
            }
        }
        SlenderAttack.SetActive(false);
        MoveToNextPoint();
        SetNextMove();
    }

    private void Update()
    {
        // nahodne presouvani
        timeToNextMove -= Time.deltaTime;
        if (timeToNextMove <= 0)
        {
            MoveToNextPoint();
            SetNextMove();
        }

        // Otáčení textu směrem k hraci
        PlayerSave ps = MainGameManager.GetPlayerSave();
        if (ps != null)
        {
            GameEntityObject obj = ps.PlayerRef;
            if (obj != null)
            {
                Vector3 lookDirection = obj.transform.position - transform.position;
                lookDirection.y = 0;
                transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }
    }

    private Transform randomPointLast = null;

    private void MoveToNextPoint()
    {
        Transform randomPoint = null;
        do
        {
            randomPoint = Points[Random.Range(0, Points.Count)];
        } while (randomPointLast == randomPoint);
        randomPointLast = randomPoint;
        SlenderObj.transform.position = randomPoint.position;
    }

    private void SetNextMove()
    {
        timeToNextMove = Random.Range(WaitTime * 0.4f, WaitTime * 1.4f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(KillPlayer());
        }
    }

    private IEnumerator KillPlayer()
    {
        PlayerSave ps = MainGameManager.GetPlayerSave();
        if (ps != null)
        {
            GameEntityObject obj = ps.PlayerRef;
            if (obj != null)
            {
                obj.IsEnabledMoving = false;
                Instantiate(MarkerPrefab, obj.transform.position, Quaternion.identity);
            }
        }
        yield return new WaitForSeconds(1.0f);
        SlenderAttack.SetActive(true);
    }

}
