using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(GameEntityObject))]
public class AIDragon : MonoBehaviour
{
    [Header("AI Entity controll")]
    public bool AI_isActive = false; // je AI aktivni ?
    public bool AI_isSleeping = false;
    public bool Moving = false;
    public bool Flying = false;
    public float Direction = 0.0f;
    public bool Grounded = true;

    [Header("AI settings")]
    public float PlayerDetectionRadius = 100.0f;
    public float MaxDistanceFromSpawn = 15.0f;
    public float InTargetPointTolerance = 5.9f;
    public float MaxBasicAttackDist = 5.2f;
    public float MaxFlameAttackDist = 12.0f;
    public float PlayerFarDistance = 19.0f;

    // Game Entity class
    protected GameEntityObject _gameEntity;

    // AI
    protected Vector3 _AI_spawnPoint; // spawpoint draka
    protected GameEntityObject _AI_playerRef; // reference na hrace
    protected bool _AI_initDone = false; // true pokud je AI uz inicializovano
    protected Vector3 _AI_targetPoint; // target pozice kam se ma drak presunout
    protected bool _AI_inTargetPoint; // false pokud drak jeste nedosel na danou target pozici
    protected bool _playerIsAlive; // true pokud je hrac zivi
    protected bool _AI_dragon_in_air;  // drak muze utocit jen pokud neni ve vzduchu = false (properta je ovladane externe s child tridy)
    public bool _AI_flameAttack; // chrleni ohne pokud je true
    protected bool _AI_rotateOnly; // true -> drak se bude pouze otacet na miste, bez chuze
    private float _flameAttackCooldownTime = 0.0f; // cool down cas, chrlit bude moct az bude 0
    private float _timeToAISleep = 0.0f; // cas za jak dlouho bude AI uspano na nejakou dobu

    /********************************************************************************************************/
    // public func

    public void Init_AI()
    {
        _gameEntity = GetComponent<GameEntityObject>();
        _AI_spawnPoint = transform.position;
        _AI_dragon_in_air = false;
        _AI_inTargetPoint = true;
        _AI_initDone = true;
        _AI_rotateOnly = false;
        _AI_flameAttack = false;
        _flameAttackCooldownTime = Random.Range(8.0f, 37.0f);
        _timeToAISleep = Random.Range(30.0f, 70.0f);
    }

    public void Update_AI()
    {
        // neaktivni stav
        if (!AI_isActive)
        {
            Moving = false;
            Flying = false;
            return;
        }

        // neaktivni cas
        if (AI_isSleeping)
        {
            _timeToAISleep -= Time.deltaTime;
            if (_timeToAISleep <= 0.0f)
            {
                _timeToAISleep = Random.Range(30.0f, 70.0f);
                AI_isSleeping = false;
            }
            Moving = false;
            Flying = false;
            return;
        }

        // ziskani reference hrace ve scene
        GetPlayerReference();
        if (_AI_playerRef == null) return;

        // update AI
        UpdateAI();

        // attack
        if (!_AI_dragon_in_air)
        {
            float dist = DistanceFromPlayer();

            // basic attack
            if (dist < MaxBasicAttackDist && !AI_isSleeping)
            {
                int id = Random.Range(1, _gameEntity.AttacksDamages.Count() + 1);
                _gameEntity.DoAttack(id);
                _gameEntity.IsEnabledMoving = false;
            }
            else
            {
                _gameEntity.StopAttack();
                _gameEntity.IsEnabledMoving = true;
            }

            // flame attack
            if (dist < MaxFlameAttackDist && !AI_isSleeping)
            {
                if (_flameAttackCooldownTime <= 0.0f)
                {
                    _flameAttackCooldownTime = Random.Range(8.0f, 37.0f);
                    _AI_flameAttack = true;
                }
                else
                {
                    _AI_flameAttack = false;
                }
            }
            else
            {
                _AI_flameAttack = false;
            }
        }

        // casovani flame utoku
        if (_flameAttackCooldownTime > 0.0f)
        {
            _flameAttackCooldownTime -= Time.deltaTime;
            _flameAttackCooldownTime = _flameAttackCooldownTime < 0.0f ? 0.0f : _flameAttackCooldownTime;
        }

        // casovani uspani
        _timeToAISleep -= Time.deltaTime;
        if (_timeToAISleep <= 0.0f)
        {
            _timeToAISleep = Random.Range(6.0f, 14.0f);
            AI_isSleeping = true;
            Flying = false;
        }
    }

    public float DistanceFromPlayer()
    {
        if (_AI_playerRef == null)
        {
            return 99999.0f;
        }
        if (!_AI_playerRef.IsAlive())
        {
            return 99999.0f;
        }
        return Mathf.Sqrt(Mathf.Pow(transform.position.x - _AI_playerRef.transform.position.x, 2) + Mathf.Pow(transform.position.z - _AI_playerRef.transform.position.z, 2));
    }

    /********************************************************************************************************/

    private void GetPlayerReference()
    {
        if (_AI_playerRef == null)
        {
            PlayerSave ps = MainGameManager.GetPlayerSave();
            if (ps != null)
            {
                _AI_playerRef = ps.PlayerRef;
                _playerIsAlive = _AI_playerRef.IsAlive();
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
        // pokud je uz blizko hrace a drak uz se nebude dele pohybovat, tak se jen na miste bude otacet za hracem
        RotateOnPlayerWhenAttacking();

        // pohyb za hracem / na urcite misto
        if (!Grounded || !_gameEntity.IsEnabledMoving || !_gameEntity.IsEntityEnabled) return;

        // vypocet vzdalenosti od hrace (pokud je na scene)
        float playerDist = 999999;
        if (_AI_playerRef != null)
        {
            if (_AI_playerRef.IsAlive())
            {
                playerDist = Mathf.Sqrt(Mathf.Pow(transform.position.x - _AI_playerRef.transform.position.x, 2) + Mathf.Pow(transform.position.z - _AI_playerRef.transform.position.z, 2));
            }
            else
            {
                if (_playerIsAlive && !_AI_playerRef.IsAlive())
                {
                    _AI_inTargetPoint = true;
                }
            }
            _playerIsAlive = _AI_playerRef.IsAlive();
        }

        // rohodovani pozice kam se entita bude pohybovat
        if (playerDist < PlayerDetectionRadius)
        {
            // drak poleti pokud je to prilis daleko
            if (playerDist > PlayerFarDistance)
            {
                Flying = true;
            }
            // nasleduje hrace
            SetTargetPoint(_AI_playerRef.transform.position);
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
                _AI_inTargetPoint = true;

                // pokud drak letel, tak ukonceni letu az se dostane na dane misto
                Flying = false;
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

    private void RotateOnPlayerWhenAttacking()
    {
        if (_AI_playerRef == null) return;
        Vector3 player_pos = _AI_playerRef.transform.position;
        if (Mathf.Sqrt(Mathf.Pow(transform.position.x - player_pos.x, 2) + Mathf.Pow(transform.position.z - player_pos.z, 2)) <= InTargetPointTolerance)
        {
            Vector3 direction = player_pos - transform.position;
            Direction = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            _AI_rotateOnly = true;
        }
        else
        {
            _AI_rotateOnly = false;
        }
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
