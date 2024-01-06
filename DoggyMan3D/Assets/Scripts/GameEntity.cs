using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GameEntity : MonoBehaviour
{
    [Header("Default")]
    public string Name;
    public int Lives;

    private CharacterController _controller;
    private int _maxLives = 0;
    private int _doAttackID = 0;
    private bool _hit = false;
    private bool _isMoving = false;
    private bool _isSprinting = false;
    private bool _enabledMoving = true;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        this._maxLives = Lives;
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
        return this._isSprinting;
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

}