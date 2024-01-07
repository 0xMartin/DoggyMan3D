using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simuluje efekt levitace. Vyuziva funkce sinus.
/// </summary>
public class LevitationScript : MonoBehaviour
{

    public float amplitude = 1.0f; /**amplituda efektu levitace */
    public float frequency = 1.0f; /** Frekvence efektu */

    private Vector3 startPosition; /** povodni pozice objeku. pozice ke ktere se hodnota funkce sinus pripocitavat */
    private float timeRandomOffset; /** nahodny casovy offset aby vsechny objekty s timto skriptem nebyli ve fazi. pak efekt vypada podivne. */

    void Start()
    {
        startPosition = transform.position;
        timeRandomOffset = (float)(Random.Range(0, 100) / 5.0);
    }

    void Update()
    {
        float y = amplitude * Mathf.Sin(frequency * Time.time + timeRandomOffset);
        transform.position = startPosition + new Vector3(0, y, 0);
    }

}
