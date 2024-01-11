using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

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
    public float MaxWanderingRadius = 10f;
    public float MaxWaitTime = 2f;
    public float PlayerDetectionRadius = 5f;
    public float MaxDistanceFromSpawn = 20f;

    // main
    private GameEntityObject _gameEntity;
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    // AI
    private Vector3 _spawnPoint;
    private Transform _player;
    private bool _isFollowingPlayer = false;
    private float _waitTimer = 0f;


    private void Start()
    {
        _spawnPoint = transform.position;
        _gameEntity = GetComponent<GameEntityObject>();
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

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
            GroundedRadius);
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

        // normalise input direction
        Vector3 inputDirection = new Vector3(Moving ? 1.0f : 0.0f, 0.0f, 0.0f).normalized;

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

    private void UpdateAI()
    {

    }

}
