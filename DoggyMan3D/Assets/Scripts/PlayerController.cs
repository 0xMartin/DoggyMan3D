using System.Linq;
using Cinemachine;
using UnityEditor;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

[RequireComponent(typeof(GameEntityObject))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
[RequireComponent(typeof(PlayerInput))]
#endif
public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 2.0f;

    [Tooltip("Sprint speed of the character in m/s")]
    public float SprintSpeed = 5.335f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;

    [Tooltip("Reference on gameobject with player 3D model. This model must have Animator component!")]
    public GameObject PlayerModel;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;

    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    [Header("Cinemachine")]
    [Tooltip("Cinemachine virtual camera, used for setting of shake")]
    public CinemachineVirtualCamera VirtualCamera;

    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    // entity info
    private GameEntityObject _gameEntity;

    // inventory controll
    private bool _readyToUse;

    // player
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;
    private bool _attackReady;


#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    private PlayerInput _playerInput;
#endif
    private Animator _animator;
    private GameInputs _input;
    private GameObject _mainCamera;

    private const float _threshold = 0.01f;

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
        }
    }

    private void Awake()
    {
        // get a reference to our main camera
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    private void Start()
    {
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

        _gameEntity = GetComponent<GameEntityObject>();

        _animator = PlayerModel.GetComponent<Animator>();
        _input = GetComponent<GameInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
    }

    private void Update()
    {
        if (MainGameManager.IsGamePaused()) return;

        // zmena utoku/spousteni utoku
        UpdateAttack();

        // zmena animaci podle aktualniho stavu entity
        UpdateAnimator();

        // zmena intezitu otresu kamery (pri chuzi, bezu, hitu, ...)
        UpdateCameraShaking();

        // inventory events
        InventoryEvents();

        // pohyb entity
        UpdateGravity();
        GroundedCheck();
        Move();
    }

    private void InventoryEvents()
    {
        PlayerSave ps = MainGameManager.GetPlayerSave();
        if (ps == null)
        {
            return;
        }

        // slot inventare 1
        if (_input.key1 && ps.Inventory.Count > 0 && _readyToUse)
        {
            _readyToUse = false;
            if (_gameEntity.UseItem(ps.Inventory[0]))
                ps.Inventory.RemoveAt(0);
        }

        // slot inventare 2
        if (_input.key2 && ps.Inventory.Count > 1 && _readyToUse)
        {
            _readyToUse = false;
            if (_gameEntity.UseItem(ps.Inventory[1]))
                ps.Inventory.RemoveAt(1);
        }

        // slot inventare 3
        if (_input.key3 && ps.Inventory.Count > 2 && _readyToUse)
        {
            _readyToUse = false;
            if (_gameEntity.UseItem(ps.Inventory[2]))
                ps.Inventory.RemoveAt(2);
        }

        // slot inventare 4
        if (_input.key4 && ps.Inventory.Count > 3 && _readyToUse)
        {
            _readyToUse = false;
            if (_gameEntity.UseItem(ps.Inventory[3]))
                ps.Inventory.RemoveAt(3);
        }

        if (!_input.key1 && !_input.key2 && !_input.key3 && !_input.key4)
        {
            _readyToUse = true;
        }
    }

    private void UpdateCameraShaking()
    {
        if (!_gameEntity.IsMoving() && !_gameEntity.IsSprinting())
        {
            SetCameraShaking(0.5f, 0.3f);
        }
        else
        {
            if (_gameEntity.IsSprinting())
            {
                SetCameraShaking(0.6f, 5.0f);
            }
            else
            {
                SetCameraShaking(0.52f, 2.5f);
            }
        }
    }

    private void UpdateAnimator()
    {
        // walk animation
        _animator.SetBool("Walk", _gameEntity.IsMoving());

        // run animation
        _animator.SetBool("Run", _gameEntity.IsSprinting());

        // death animation
        _animator.SetBool("Death", !_gameEntity.IsAlive());

        // attack animation
        if (_gameEntity.IsAttacking())
        {
            // death animation
            _animator.SetBool("Attack1", _gameEntity.GetAttackID() == 1);
            // death animation
            _animator.SetBool("Attack2", _gameEntity.GetAttackID() == 2);
        }
        else
        {
            _animator.SetBool("Attack1", false);
            _animator.SetBool("Attack2", false);
        }

        // death animation
        _animator.SetBool("Hit", _gameEntity.IsHited());
    }

    private void UpdateAttack()
    {
        if (_gameEntity.IsAttacking())
        {
            // ukonceni utoku po prehrani animace
            AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(1);
            if (info.normalizedTime >= 1.0 && (info.IsName("Attack01") || info.IsName("Attack02")))
            {
                _gameEntity.StopAttack();
            }
        }
        else
        {
            // aktivuje utok (pri utoku hrac musi bud stat nebo jen pri pomale chuzi)
            if (!_gameEntity.IsSprinting())
            {
                if (_input.attack)
                {
                    if (_attackReady)
                    {
                        _attackReady = false;
                        int id = Random.Range(1, _gameEntity.AttacksDamages.Count() + 1);
                        _gameEntity.DoAttack(id);
                    }
                }
                else
                {
                    _attackReady = true;
                }
            }
        }
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);
    }

    private void CameraRotation()
    {
        if (MainGameManager.IsGamePaused()) return;

        // if there is an input and camera position is not fixed
        if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    private void Move()
    {
        // update entity move
        _gameEntity.UpdateMove(_input.move != Vector2.zero, _input.sprint);

        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = _gameEntity.IsSprinting() ? SprintSpeed : MoveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (!_gameEntity.IsMoving() || !_gameEntity.IsEnabledMoving || !_gameEntity.IsEntityEnabled) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(_gameEntity.GetCharacterController().velocity.x, 0.0f, _gameEntity.GetCharacterController().velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(_speed, targetSpeed * inputMagnitude,
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
        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (_input.move != Vector2.zero && _gameEntity.IsAlive() && _gameEntity.IsEnabledMoving && _gameEntity.IsEntityEnabled)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              _mainCamera.transform.eulerAngles.y;
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

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
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

    private void SetCameraShaking(float amplitude, float frequency)
    {
        if (VirtualCamera != null)
        {
            // Získejte odkaz na komponentu CinemachineBasicMultiChannelPerlin v rámci Virtual Camera
            var noiseSettings = VirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            // Nastavte amplitudu a frekvenci šumu
            if (noiseSettings != null)
            {
                noiseSettings.m_AmplitudeGain = amplitude;
                noiseSettings.m_FrequencyGain = frequency;
            }
        }
    }

}