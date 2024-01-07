using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GameEntityObject : MonoBehaviour
{
    [Header("Default")]
    public string Name;
    public int Lives = 100;
    public int MaxStamina = 100;
    [Header("Stamina Config")]
    public float StaminaDownSpeed = 8.0f;
    public float StaminaUpSpeed = 5.5f;
    public float StartAddStaminaAfterSeconds = 2.0f;

    private CharacterController _controller;
    private int _maxLives = 0;
    private int _doAttackID = 0;
    private bool _hit = false;
    private bool _isMoving = false;
    private bool _isSprinting = false;
    private bool _enabledMoving = true;
    private float _stamina;
    private float _noSprintTime = 0.0f;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        this._maxLives = Lives;
        this._stamina = MaxStamina;
    }

    public CharacterController GetCharacterController()
    {
        return this._controller;
    }

    public void AddLives(int lives)
    {
        this.Lives = Math.Min(this._maxLives, this.Lives + lives);
        this._hit = false;
    }

    public void EnableMovingStopped(bool state)
    {
        this._enabledMoving = state;
    }

    public void HitEntity(int damage)
    {
        this.Lives = Math.Max(this.Lives - damage, 0);
        this._hit = true;
    }

    public void DoAttack(int attackID)
    {
        this._doAttackID = attackID;
    }

    public void StopAttack()
    {
        this._doAttackID = 0;
    }

    public void UpdateMove(bool moving, bool sprinting)
    {
        this._isMoving = moving;
        this._isSprinting = sprinting;
        if (sprinting)
        {
            this._stamina = Math.Max(0, this._stamina - this.StaminaDownSpeed * Time.deltaTime);
            _noSprintTime = 0.0f;
        }
        else
        {
            if (_noSprintTime > StartAddStaminaAfterSeconds)
            {
                if (this._stamina <= 3)
                {
                    // pokud je stamina vycerpana uplne bude nabev pomalejsi, sprintovat hrac muze jen kdyz je stamina vyssi jak 3
                    this._stamina = Math.Min(this.MaxStamina, this._stamina + 0.5f * Time.deltaTime);
                }
                else
                {
                    this._stamina = Math.Min(this.MaxStamina, this._stamina + this.StaminaUpSpeed * Time.deltaTime);
                }
            }
            else
            {
                _noSprintTime += Time.deltaTime;
            }
        }
    }

    public bool IsMovingEnabled()
    {
        return this._enabledMoving;
    }

    public bool IsMoving()
    {
        return this._isMoving;
    }

    public bool IsSprinting()
    {
        return this._isSprinting && this._stamina > 3;
    }

    public bool IsAlive()
    {
        return this.Lives > 0;
    }

    public bool IsHited()
    {
        if (this._hit)
        {
            this._hit = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsAttacking()
    {
        return this._doAttackID != 0;
    }

    public int GetAttackID()
    {
        return this._doAttackID;
    }

    public int GetMaxLives()
    {
        return this._maxLives;
    }

    public float GetStamina()
    {
        return this._stamina;
    }

}