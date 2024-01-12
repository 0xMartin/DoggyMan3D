using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameEntityObject))]
public class AICharacterBase : MonoBehaviour
{
    [Header("Entity move controll")]
    public bool Moving = false;
    public float Direction = 0.0f;
    public bool Grounded = true;

    [Header("AI settings")]
    public float MaxWaitTime = 7.0f;
    public float PlayerDetectionRadius = 8.0f;
    public float MaxDistanceFromSpawn = 6.0f;
    public float InTargetPointTolerance = 0.5f;

    // Game Entity class
    protected GameEntityObject _gameEntity;

    // AI
    protected Vector3 _AI_spawnPoint;
    protected Vector3 _AI_targetPoint;
    protected bool _AI_inTargetPoint;
    protected Transform _AI_player;
    protected float _AI_nextMoveTime = 0f;
    protected bool _AI_initDone = false;

    /********************************************************************************************************/
    public void Init_AI()
    {
        _gameEntity = GetComponent<GameEntityObject>();
        _AI_spawnPoint = transform.position;
        _AI_inTargetPoint = true;
        _AI_initDone = true;
    }

    public void Update_AI()
    {
        // ziskani reference hrace ve scene
        GetPlayerReference();

        // update AI
        UpdateAI();
    }
    /********************************************************************************************************/

    private void GetPlayerReference()
    {
        if (_AI_player == null)
        {
            PlayerSave ps = MainGameManager.GetPlayerSave();
            if (ps != null)
            {
                GameEntityObject pl = ps.PlayerRef;
                if (pl != null)
                {
                    _AI_player = pl.transform;
                }
            }
        }
    }


    /***************************************************************************************************************************************/
    // SIMPLE AI
    // * pokud hrace neni pobliz tak se entita nahodne pohybuje v urcitych casovych intervalech v definovane kruhove oblasti od miste jejiho spawnu
    // * pokud se hrac priblizi na definovanou vzdalenost tak jej entita zacne pronasledovat
    /***************************************************************************************************************************************/

    private void UpdateAI()
    {
        if (!Grounded || !_gameEntity.IsMovingEnabled() || !_gameEntity.IsEntityEnabled) return;

        // vypocet vzdalenosti od hrace (pokud je na scene)
        float playerDist = 999999;
        if (_AI_player != null)
        {
            playerDist = Mathf.Sqrt(Mathf.Pow(transform.position.x - _AI_player.transform.position.x, 2) + Mathf.Pow(transform.position.z - _AI_player.transform.position.z, 2));
        }

        // rohodovani pozice kam se entita bude pohybovat
        if (playerDist < PlayerDetectionRadius)
        {
            // nasleduje hrace
            SetTargetPoint(_AI_player.transform.position);
        }
        else
        {
            // nahodne rozhodnuti smeru pohybu v kruhove oblasti od spawnu entity
            if (Time.time > _AI_nextMoveTime)
            {
                if (_AI_inTargetPoint)
                {
                    SetTargetPoint(GenerateRandomPointInCircle());
                }
                _AI_nextMoveTime = Time.time + MaxWaitTime;
            }
        }

        // nasledovani nastavene target pozice
        if (!_AI_inTargetPoint)
        {
            // vypocet smeru & zahajeni pohybu
            Direction = CalculateAngleToTarget();
            Moving = true;

            // kdyz uz je entita target bodu dostatecne blizko tak zrusi target point (je ignorovana slozka Y)
            float distance = Mathf.Sqrt(Mathf.Pow(transform.position.x - _AI_targetPoint.x, 2) + Mathf.Pow(transform.position.z - _AI_targetPoint.z, 2));
            if (distance <= InTargetPointTolerance)
            {
                _AI_nextMoveTime = Time.time + MaxWaitTime;
                _AI_inTargetPoint = true;
            }
        }
        else
        {
            Moving = false;
        }
    }

    private Vector3 GenerateRandomPointInCircle()
    {
        // nahodne vygenerovani nahodneho uhlu a radiusu "vzdalenost od stredu"
        float randomAngle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
        float randomRadius = UnityEngine.Random.Range(0f, MaxDistanceFromSpawn);

        // vypocet pozice nahodneho bodu v kruhove oblasti
        float x = _AI_spawnPoint.x + randomRadius * Mathf.Cos(randomAngle);
        float z = _AI_spawnPoint.z + randomRadius * Mathf.Sin(randomAngle);
        return new Vector3(x, _AI_spawnPoint.y, z);
    }

    private float CalculateAngleToTarget()
    {
        Vector3 direction = _AI_targetPoint - transform.position;
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        return angle;
    }

    private void SetTargetPoint(Vector3 target)
    {
        _AI_targetPoint = target;
        _AI_inTargetPoint = false;
    }

    /*****************************************************************************************************************************************************/
    // vykreslovani potrebnych nastavovatelnych parametru v editoru
    /*****************************************************************************************************************************************************/

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        DrawCircle(_AI_initDone ? _AI_spawnPoint : transform.position, MaxDistanceFromSpawn, 22);
        if (!_AI_inTargetPoint)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(_AI_targetPoint, new Vector3(0.3f, 0.3f, 0.3f));
        }
    }

    private void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angleIncrement = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0f, 0f);
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleIncrement;
            float x = center.x + Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float z = center.z + Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            Vector3 currentPoint = new Vector3(x, center.y, z);
            Gizmos.DrawLine(prevPoint, currentPoint);
            prevPoint = currentPoint;
        }
    }

}
