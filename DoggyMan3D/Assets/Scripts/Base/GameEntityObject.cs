using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GameEntityObject : MonoBehaviour
{
    [Header("Default")]
    public string Name;
    public int Lives = 100;
    public int MaxStamina = 100;
    public float[] AttacksDamages;

    [Header("Stamina Config")]
    public float StaminaDownSpeed = 6.5f;
    public float StaminaUpSpeed = 5.0f;
    public float StartAddStaminaAfterSeconds = 3.0f;

    [Header("Sounds")]
    public AudioClip DrinkPotionSound;
    public float DrinkPotionSoundVolume = 1.0f;
    public AudioClip TakeItemSound;
    public float TakeItemSoundVolume = 1.0f;

    private CharacterController _controller;
    private int _maxLives = 0;
    private int _doAttackID = 0;
    private bool _hit = false;
    private bool _isMoving = false;
    private bool _isSprinting = false;
    private bool _enabledMoving = true;
    private float _stamina;
    private float _noSprintTime = 0.0f;
    private List<Item> ActivePotions = new List<Item>();

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        this._maxLives = Lives;
        this._stamina = MaxStamina;
    }

    private void Update()
    {
        // vypocet staminy
        if (_isSprinting)
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
                    this._stamina = Math.Min(this.MaxStamina, this._stamina + 0.5f * Time.deltaTime * (_isMoving ? 1 : 2));
                }
                else
                {
                    this._stamina = Math.Min(this.MaxStamina, this._stamina + this.StaminaUpSpeed * Time.deltaTime * (_isMoving ? 1 : 2));
                }
            }
            else
            {
                _noSprintTime += Time.deltaTime;
            }
        }
        // potion effect
        if (this.ActivePotions.Any(p => p.Type == Item.ItemType.STAMINA_POTION))
        {
            this._stamina = this.MaxStamina;
        }

        // vyprseni ucinku lektvaru
        foreach (Item potion in this.ActivePotions)
        {
            potion.Time += Time.deltaTime;
        }

        // ukonceni efektu lektvaru po vyprseni casu
        foreach (Item potion in this.ActivePotions)
        {
            if (potion.ParameterValues.Count > 0)
            {
                if (potion.Time >= potion.ParameterValues[0])
                {
                    // odstranuje po jednom z pole
                    this.ActivePotions.Remove(potion);
                    break;
                }
            }
        }
    }

    public void UpdateMove(bool moving, bool sprinting)
    {
        this._isMoving = moving;
        this._isSprinting = sprinting;
    }

    public CharacterController GetCharacterController()
    {
        return this._controller;
    }

    public List<Item> GetActivePotions()
    {
        return this.ActivePotions;
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

    public float GetActiveAttackDamage()
    {
        if (this._doAttackID <= 0 || this._doAttackID > this.AttacksDamages.Count())
        {
            return 0.0f;
        }

        // random roll
        float rnd = UnityEngine.Random.Range(0.5f, 1.5f);

        // low stamina
        bool lowStamina = this._stamina < this.MaxStamina * 0.1;

        // potion effect (2x more power)
        if (this.ActivePotions.Any(p => p.Type == Item.ItemType.STRENGTH_POTION))
        {
            return this.AttacksDamages[this._doAttackID - 1] * 2.0f * rnd * (lowStamina ? 0.5f : 1.0f);
        }

        return this.AttacksDamages[this._doAttackID - 1] * rnd * (lowStamina ? 0.5f : 1.0f);
    }

    public void StopAttack()
    {
        this._doAttackID = 0;
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

    public bool UseItem(Item item)
    {
        // pridani zivotu
        if (item.Type == Item.ItemType.HEALTH_POTION)
        {
            if (DrinkPotionSound != null)
                AudioSource.PlayClipAtPoint(DrinkPotionSound, transform.TransformPoint(transform.position), DrinkPotionSoundVolume);
            ShowAuraEffect(item.ItemUseFX);
            this.AddLives(this._maxLives);
            return true;
        }

        // dlouhodoby efekt: sila, stamina
        if (item.Type == Item.ItemType.STAMINA_POTION ||
            item.Type == Item.ItemType.STRENGTH_POTION)
        {
            foreach (Item potion in this.ActivePotions)
            {
                if (potion.Type == item.Type)
                {
                    return false;
                }
            }

            if (DrinkPotionSound != null)
                AudioSource.PlayClipAtPoint(DrinkPotionSound, transform.TransformPoint(transform.position), DrinkPotionSoundVolume);
            ShowAuraEffect(item.ItemUseFX);
            ActivePotions.Add(item);
            return true;
        }

        return false;
    }

    public void ShowAuraEffect(GameObject fx)
    {
        if (fx != null)
        {
            GameObject instanceFx = Instantiate(fx);
            OneShotFX osfx = instanceFx.GetComponent<OneShotFX>();
            if (osfx != null)
            {
                osfx.TargetEntity = this;
            }
        }
    }

}