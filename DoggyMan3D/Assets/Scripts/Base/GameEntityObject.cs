using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GameEntityObject : MonoBehaviour
{

    // nastaveni
    [Header("Controll")]
    public bool IsEntityEnabled = true;

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

    [Header("Prefabs")]
    public GameObject Text3DPrefab;

    // interni vlastnosti entity
    private CharacterController _controller;
    private int _maxLives = 0;
    private int _doAttackID = 0;
    private bool _hit = false;
    private bool _isMoving = false;
    private bool _isSprinting = false;
    private bool _enabledMoving = true;
    private float _stamina;
    private float _noSprintTime = 0.0f;
    private List<Item.ItemData> _activePotions = new List<Item.ItemData>();

    // callbacky
    public delegate void EnityDeathCallback();
    public EnityDeathCallback OnDeath;
    public List<Func<Item.ItemData, GameEntityObject, bool>> OnExternalItemUse = new List<Func<Item.ItemData, GameEntityObject, bool>>();

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        this._maxLives = Lives;
        this._stamina = MaxStamina;
        ResetPlayer();
    }

    private void Update()
    {
        if (!IsEntityEnabled) return;

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
        if (this._activePotions.Any(p => p.Type == Item.ItemType.STAMINA_POTION))
        {
            this._stamina = this.MaxStamina;
        }

        // vyprseni ucinku lektvaru
        foreach (Item.ItemData potion in this._activePotions)
        {
            potion.Time += Time.deltaTime;
        }

        // ukonceni efektu lektvaru po vyprseni casu
        foreach (Item.ItemData potion in this._activePotions)
        {
            if (potion.ParameterValues.Count > 0)
            {
                if (potion.Time >= potion.ParameterValues[0])
                {
                    // odstranuje po jednom z pole
                    this._activePotions.Remove(potion);
                    break;
                }
            }
        }
    }

    public void ResetPlayer()
    {
        this.Lives = this._maxLives;
        this._stamina = this.MaxStamina;
        this._activePotions.Clear();
        this.OnExternalItemUse.Clear();
        this.OnDeath = null;
        this._doAttackID = 0;
        this._isMoving = false;
        this._isSprinting = false;
        this._enabledMoving = true;
        this.IsEntityEnabled = true;
    }

    public void UpdateMove(bool moving, bool sprinting)
    {
        if (IsEntityEnabled && IsAlive())
        {
            this._isMoving = moving;
            this._isSprinting = sprinting;
        }
    }

    public CharacterController GetCharacterController()
    {
        return this._controller;
    }

    public List<Item.ItemData> GetActivePotions()
    {
        return this._activePotions;
    }

    public void AddLives(int lives)
    {
        if (!IsEntityEnabled && IsAlive()) return;

        this.Lives = Math.Min(this._maxLives, this.Lives + lives);
        this._hit = false;
    }

    public void EnableMovingStopped(bool state)
    {
        this._enabledMoving = state;
    }

    public void HitEntity(int damage)
    {
        if (!IsEntityEnabled || !IsAlive()) return;

        this.Lives = Math.Max(this.Lives - damage, 0);
        if (this.Lives <= 0)
        {
            OnDeath?.Invoke();
        }
        this._hit = true;

        // info text s velikosti damage
        if (Text3DPrefab != null)
        {
            GameObject text3D = Instantiate(Text3DPrefab);
            text3D.transform.position = transform.position;
            Text3D text = text3D.GetComponent<Text3D>();
            if (text != null)
            {
                text.TextMesh.text = damage.ToString();
                text.TextColor = Color.red;
                text.MoveSpeed = new Vector3(0.0f, -0.7f, 0.0f);
                text.VisibleTime = 2.0f;
            }
        }
    }

    public void DoAttack(int attackID)
    {
        if (!IsEntityEnabled || !IsAlive()) return;
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
        if (this._activePotions.Any(p => p.Type == Item.ItemType.STRENGTH_POTION))
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

    public bool UseItem(Item.ItemData item)
    {
        if (!IsEntityEnabled || !IsAlive()) return false;

        // item s externim uziti (muze byt pouzit jen na jinem objektu) (klice, ...)
        if (item.Type == Item.ItemType.KEY)
        {
            foreach (Func<Item.ItemData, GameEntityObject, bool> eu in OnExternalItemUse)
            {
                if (eu.Invoke(item, this))
                {
                    return true;
                }
            }
        }

        // itemu s internim uzitim (lekvary, ...)

        // pridani zivotu
        if (item.Type == Item.ItemType.HEALTH_POTION)
        {
            if (DrinkPotionSound != null)
                AudioSource.PlayClipAtPoint(DrinkPotionSound, transform.TransformPoint(transform.position), DrinkPotionSoundVolume);
            ShowAuraEffect(item.ItemUseFX, 3.0f);
            this.AddLives(this._maxLives);
            return true;
        }

        // dlouhodoby efekt: sila, stamina
        if (item.Type == Item.ItemType.STAMINA_POTION ||
            item.Type == Item.ItemType.STRENGTH_POTION)
        {
            bool usePotions = true;
            foreach (Item.ItemData potion in this._activePotions)
            {
                if (potion.Type == item.Type)
                {
                    usePotions = false;
                    break;
                }
            }

            if (usePotions)
            {
                if (DrinkPotionSound != null)
                    AudioSource.PlayClipAtPoint(DrinkPotionSound, transform.TransformPoint(transform.position), DrinkPotionSoundVolume);
                if (item.ParameterValues.Count > 0)
                {
                    ShowAuraEffect(item.ItemUseFX, item.ParameterValues[0]);
                }
                else
                {
                    ShowAuraEffect(item.ItemUseFX, 3.0f);
                }
                _activePotions.Add(item);
                return true;
            }
        }

        // info 3D text pokud nebylo mozne pouzit predmet
        if (Text3DPrefab != null)
        {
            GameObject text3D = Instantiate(Text3DPrefab);
            text3D.transform.position = transform.position + new Vector3(0.0f, 1.1f, 0.0f); ;
            Text3D text = text3D.GetComponent<Text3D>();
            if (text != null)
            {
                text.TextMesh.text = "You cannot use this item";
                text.TextColor = Color.white;
                text.MoveSpeed = new Vector3(0.0f, -0.1f, 0.0f);
                text.VisibleTime = 4.0f;
            }
        }
        return false;
    }

    public void ShowAuraEffect(GameObject fx, float activeTime)
    {
        if (!IsEntityEnabled) return;
        
        if (fx != null)
        {
            GameObject instanceFx = Instantiate(fx);
            OneShotFX osfx = instanceFx.GetComponent<OneShotFX>();
            if (osfx != null)
            {
                osfx.TargetEntity = this;
                osfx.activeTime = activeTime;
            }
        }
    }

}