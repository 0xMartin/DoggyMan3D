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
    public bool IsEnabledMoving = true;
    public bool IsHitEnabled = true;

    [Header("Default")]
    public string Name;
    public int Lives = 100;
    public int MaxStamina = 100;
    public int[] AttacksDamages;

    [Header("Stamina Config")]
    public float StaminaDownSpeed = 6.5f;
    public float StaminaUpSpeed = 5.0f;
    public float StartAddStaminaAfterSeconds = 3.0f;

    [Header("Sounds")]
    public AudioClip DrinkPotionSound;
    public float DrinkPotionSoundVolume = 1.0f;
    public AudioClip TakeItemSound;
    public float TakeItemSoundVolume = 1.0f;

    [Header("3D Info Text")]
    public float HitTextYOffset = 0.5f;
    public GameObject Text3DPrefab;

    // interni vlastnosti entity
    private CharacterController _controller;
    private int _maxLives = 0;
    private int _doAttackID = 0;
    private bool _hit = false;
    private bool _isMoving = false;
    private bool _isSprinting = false;
    private float _stamina;
    private float _noSprintTime = 0.0f;
    private List<Item.ItemData> _activePotions = new List<Item.ItemData>();
    private List<GameObject> _instanceFxList = new List<GameObject>();
    private bool _unableToReset = false;

    // callbacky
    public delegate void EnityDeathCallback();
    public EnityDeathCallback OnDeath;
    public delegate void EnityHitCallback();
    public EnityDeathCallback OnHit;
    public List<Func<Item.ItemData, GameEntityObject, bool>> OnExternalItemUse = new List<Func<Item.ItemData, GameEntityObject, bool>>();

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        this._maxLives = Lives;
        this._stamina = MaxStamina;
        _unableToReset = true;
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

    private void OnTriggerEnter(Collider other)
    {
        if (!this.IsHitEnabled) return;

        AttackCollider ac = other.GetComponent<AttackCollider>();
        if (ac == null || !IsAlive()) return;

        Vector3 fxPosition;
        switch (gameObject.tag)
        {
            case "Player":
                if (other.CompareTag("AttackEnemy"))
                {
                    // zvukovy efekt + vizualni efekt
                    ac.PlayRandomHitSound();
                    fxPosition = Vector3.Lerp(transform.position, other.transform.position, 0.35f);
                    fxPosition.y = other.transform.position.y;
                    ac.CreateHitFx(fxPosition);
                    // damage
                    this.HitEntity(ac.Damage);
                }
                break;
            case "Enemy":
                if (other.CompareTag("AttackPlayer"))
                {
                    // zvukovy efekt + vizualni efekt
                    ac.PlayRandomHitSound();
                    fxPosition = Vector3.Lerp(transform.position, other.transform.position, 0.35f);
                    fxPosition.y = other.transform.position.y;
                    ac.CreateHitFx(fxPosition);
                    // damage
                    this.HitEntity(ac.Damage);
                }
                break;
        }
    }

    public void ResetPlayer()
    {
        if (this._unableToReset)
        {
            this.Lives = this._maxLives;
            this._stamina = this.MaxStamina;
            this._activePotions.Clear();
            this.OnExternalItemUse.Clear();
            foreach (GameObject fx in _instanceFxList)
            {
                Destroy(fx);
            }
            this._instanceFxList.Clear();
            this.OnDeath = null;
            this.OnHit = null;
            this._doAttackID = 0;
            this._isMoving = false;
            this._isSprinting = false;
            this.IsEnabledMoving = true;
            this.IsHitEnabled = true;
            this.IsEntityEnabled = true;
        }
    }

    public void UpdateMove(bool moving, bool sprinting)
    {
        if (IsEntityEnabled && IsAlive() && !MainGameManager.IsGamePaused())
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
        if (!IsEntityEnabled || !IsAlive() || MainGameManager.IsGamePaused()) return;

        this.Lives = Math.Min(this._maxLives, this.Lives + lives);
        this._hit = false;
    }

    public void HitEntity(int damage)
    {
        if (!this.IsHitEnabled) return;
        if (!IsEntityEnabled || !IsAlive() || MainGameManager.IsGamePaused() || !IsAlive()) return;

        // odebere zivoty
        this.Lives = Math.Max(this.Lives - damage, 0);
        if (this.Lives <= 0)
        {
            OnDeath?.Invoke();
        }
        this._hit = true;

        // callback
        OnHit?.Invoke();

        // info text s velikosti damage
        if (Text3DPrefab != null)
        {
            GameObject text3D = Instantiate(Text3DPrefab);
            text3D.transform.position = transform.position + Vector3.up * HitTextYOffset;
            Text3D text = text3D.GetComponent<Text3D>();
            if (text != null)
            {
                text.TextMesh.text = damage.ToString();
                text.TextColor = Color.red;
                text.MoveSpeed = new Vector3(0.0f, -0.6f, 0.0f);
                text.VisibleTime = 2.0f;
            }
        }
    }

    public void DoAttack(int attackID)
    {
        if (!IsEntityEnabled || !IsAlive() || MainGameManager.IsGamePaused()) return;
        this._doAttackID = attackID;
    }

    public int GetActiveAttackDamage()
    {
        if (this._doAttackID <= 0 || this._doAttackID > this.AttacksDamages.Count())
        {
            return 0;
        }

        // random roll
        float rnd = UnityEngine.Random.Range(0.5f, 1.5f);

        // low stamina
        bool lowStamina = this._stamina < this.MaxStamina * 0.1;

        // potion effect (2x more power)
        if (this._activePotions.Any(p => p.Type == Item.ItemType.STRENGTH_POTION))
        {
            return (int)(this.AttacksDamages[this._doAttackID - 1] * 2.0f * rnd * (lowStamina ? 0.5f : 1.0f));
        }

        return (int)(this.AttacksDamages[this._doAttackID - 1] * rnd * (lowStamina ? 0.5f : 1.0f));
    }

    public void StopAttack()
    {
        this._doAttackID = 0;
    }

    public bool IsMoving()
    {
        return this._isMoving && !MainGameManager.IsGamePaused() && IsAlive() && this.IsEntityEnabled && this.IsEnabledMoving;
    }

    public bool IsSprinting()
    {
        return this._isSprinting && this._stamina > 3 && !MainGameManager.IsGamePaused() && IsAlive() && this.IsEntityEnabled && this.IsEnabledMoving;
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
        return this._doAttackID != 0 && !MainGameManager.IsGamePaused() && IsAlive();
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
        if (!IsEntityEnabled || !IsAlive() || MainGameManager.IsGamePaused()) return false;

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
                AudioSource.PlayClipAtPoint(DrinkPotionSound, transform.position, DrinkPotionSoundVolume);
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
                    AudioSource.PlayClipAtPoint(DrinkPotionSound, transform.position, DrinkPotionSoundVolume);
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
        if (MainGameManager.IsGamePaused()) return;

        if (fx != null)
        {
            GameObject instanceFx = Instantiate(fx);
            _instanceFxList.Add(instanceFx);
            OneShotFX osfx = instanceFx.GetComponent<OneShotFX>();
            if (osfx != null)
            {
                osfx.TargetEntity = this;
                osfx.activeTime = activeTime;
            }
        }
    }

}