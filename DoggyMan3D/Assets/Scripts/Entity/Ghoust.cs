using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using System;
using TMPro;

[RequireComponent(typeof(GameEntityObject))]
public class Ghoust : MonoBehaviour
{
    [Header("Moving")]
    public bool Moving = false;
    public float Direction = 0.0f;
    public float MoveSpeed = 2.8f;
    public float SpeedChangeRate = 10.0f;
    public float RotationSmoothTime = 0.12f;
    public float Gravity = -15.0f;

    [Header("Grounding")]
    public bool Grounded = true;
    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.28f;
    public LayerMask GroundLayers;

    [Header("AI")]
    public float MaxWaitTime = 7.0f;
    public float PlayerDetectionRadius = 8.0f;
    public float MaxDistanceFromSpawn = 6.0f;
    public float InTargetPointTolerance = 0.5f;

    // main
    private GameEntityObject _gameEntity;
    private bool _initDone = false;
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    // AI
    private Vector3 _spawnPoint;
    private Vector3 _targetPoint;
    private bool _inTargetPoint;
    private Transform _player;
    private float _nextMoveTime = 0f;


    private void Start()
    {
        _initDone = true;
        _spawnPoint = transform.position;
        Debug.Log("SPAWN POINT = " + _spawnPoint);
        _gameEntity = GetComponent<GameEntityObject>();
        _inTargetPoint = true;
    }

    private void Update()
    {
        // ziskani reference hrace ve scene
        GetPlayerReference();

        // AI
        UpdateAI();

        // pohyb entity
        UpdateGravity();
        GroundedCheck();
        Move();
    }

    private void GetPlayerReference()
    {
        if (_player == null)
        {
            PlayerSave ps = MainGameManager.GetPlayerSave();
            if (ps != null)
            {
                GameEntityObject pl = ps.PlayerRef;
                if (pl != null)
                {
                    _player = pl.transform;
                }
            }
        }
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);
    }

    private void Move()
    {
        // update entity move
        _gameEntity.UpdateMove(Moving, false);

        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = MoveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (!_gameEntity.IsMoving() || !_gameEntity.IsMovingEnabled() || !_gameEntity.IsEntityEnabled) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(_gameEntity.GetCharacterController().velocity.x, 0.0f, _gameEntity.GetCharacterController().velocity.z).magnitude;

        float speedOffset = 0.1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(_speed, targetSpeed,
                Time.deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (Moving && _gameEntity.IsAlive() && _gameEntity.IsMovingEnabled() && _gameEntity.IsEntityEnabled)
        {
            _targetRotation = Direction;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // move the player
        _gameEntity.GetCharacterController().Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                         new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
    }

    private void UpdateGravity()
    {
        if (Grounded)
        {
            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
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
        if (_player != null)
        {
            playerDist = Mathf.Sqrt(Mathf.Pow(transform.position.x - _player.transform.position.x, 2) + Mathf.Pow(transform.position.z - _player.transform.position.z, 2));
        }

        // rohodovani pozice kam se entita bude pohybovat
        if (playerDist < PlayerDetectionRadius)
        {
            // nasleduje hrace
            SetTargetPoint(_player.transform.position);
        }
        else
        {
            // nahodne rozhodnuti smeru pohybu v kruhove oblasti od spawnu entity
            if (Time.time > _nextMoveTime)
            {
                if (_inTargetPoint)
                {
                    SetTargetPoint(GenerateRandomPointInCircle());
                }
                _nextMoveTime = Time.time + MaxWaitTime;
            }
        }

        // nasledovani nastavene target pozice
        if (!_inTargetPoint)
        {
            // vypocet smeru & zahajeni pohybu
            Direction = CalculateAngleToTarget();
            Moving = true;

            // kdyz uz je entita target bodu dostatecne blizko tak zrusi target point (je ignorovana slozka Y)
            float distance = Mathf.Sqrt(Mathf.Pow(transform.position.x - _targetPoint.x, 2) + Mathf.Pow(transform.position.z - _targetPoint.z, 2));
            if (distance <= InTargetPointTolerance)
            {
                _nextMoveTime = Time.time + MaxWaitTime;
                _inTargetPoint = true;
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
        float x = _spawnPoint.x + randomRadius * Mathf.Cos(randomAngle);
        float z = _spawnPoint.z + randomRadius * Mathf.Sin(randomAngle);
        return new Vector3(x, _spawnPoint.y, z);
    }

    private float CalculateAngleToTarget()
    {
        Vector3 direction = _targetPoint - transform.position;
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        return angle;
    }

    private void SetTargetPoint(Vector3 target)
    {
        _targetPoint = target;
        _inTargetPoint = false;
    }

    /*****************************************************************************************************************************************************/
    // vykreslovani potrebnych nastavovatelnych parametru v editoru
    /*****************************************************************************************************************************************************/

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
            GroundedRadius);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        DrawCircle(_initDone ? _spawnPoint : transform.position, MaxDistanceFromSpawn, 22);
        if (!_inTargetPoint)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(_targetPoint, new Vector3(0.3f, 0.3f, 0.3f));
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
