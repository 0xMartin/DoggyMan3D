using System.Collections;
using UnityEngine;

public class Dragon : AIDragon
{
    [Header("Moving Settings")]
    public float MoveSpeed = 2.8f;
    public float FlySpeed = 11.0f;
    public float SpeedChangeRate = 10.0f;
    public float RotationSmoothTime = 0.12f;
    public float Gravity = -15.0f;

    [Header("Grounding Settings")]
    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.28f;
    public LayerMask GroundLayers;

    [Header("FX")]
    public GameObject DeathFx;

    [Header("Life Bar")]
    public GameObject LifeBarPrefab;

    // main
    private Animator _animator;
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;
    private BigLifeBar _lifeBar;

    private void Start()
    {
        _animator = GetComponent<Animator>();

        // inicializace AI
        base.Init_AI();

        // inicializace life baru
        if (LifeBarPrefab != null)
        {
            GameObject lbo = Instantiate(LifeBarPrefab);
            _lifeBar = lbo.GetComponent<BigLifeBar>();
            _lifeBar.TargetEntity = base._gameEntity;
        }
    }

    private void Update()
    {
        // AI 
        base.Update_AI();

        // update animatoru
        UpdateAnimator();

        // pohyb entity
        UpdateGravity();
        GroundedCheck();
        Move();
    }

    private void UpdateAnimator()
    {
        // no active
        _animator.SetBool("NoActive", !_AI_isActive);
        if (_AI_isActive)
        {
            _lifeBar.ShowBar();
        }

        // walk
        _animator.SetBool("Moving", _gameEntity.IsMoving() || _gameEntity.IsSprinting());

        // flying
        _animator.SetBool("Fly", Flying);

        // utok
        _animator.SetBool("BasicAttack", _gameEntity.IsAttacking());
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(1);
        _gameEntity.IsEnabledMoving = !info.IsName("Basic Attack");

        // hit
        _animator.SetBool("Hit", _gameEntity.IsHited());

        // smrt
        _animator.SetBool("Death", !_gameEntity.IsAlive());
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
        if (Flying)
        {
            _gameEntity.UpdateMove(Moving && _AI_dragon_in_air, false);
        }
        else
        {
            _gameEntity.UpdateMove(Moving, false);
        }

        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = Flying ? FlySpeed : MoveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (!_gameEntity.IsMoving() || !_gameEntity.IsEnabledMoving || !_gameEntity.IsEntityEnabled) targetSpeed = 0.0f;

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
        if (Moving && _gameEntity.IsAlive() && _gameEntity.IsEnabledMoving && _gameEntity.IsEntityEnabled)
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

    /*****************************************************************************************************************************************************/
    // animation events
    /*****************************************************************************************************************************************************/

    private void InAirEvent()
    {
        // drak se dostal plne do vzduchu a muze uz letat
        _AI_dragon_in_air = true;
    }

    private void OnGroundLandingEvent()
    {
        // drak z letu pristal uz na zem
        _AI_dragon_in_air = false;
    }

    private void DeathAnimationStart()
    {
        GameObject fx = Instantiate(DeathFx);
        fx.transform.position = transform.position + Vector3.up * 0.3f;
    }

    private void DeathAnimationEnd()
    {
        Destroy(gameObject);
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

}
