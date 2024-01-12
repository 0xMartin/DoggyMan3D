using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gate : MonoBehaviour
{

    public enum GateOpenRule
    {
        KILL_ENEMIES,
        KEY
    }

    [Header("Open Rule")]
    [Tooltip("Pravidlo pro otevreni brany.")]
    public GateOpenRule OpenRule;
    [Tooltip("Vsechny entity ktere musi byt zabity. Nastavuj jen v pripade ze je nasteveno ne KILL_ENEMIES")]
    public GameEntityObject[] EntitiesToKill;
    [Tooltip("ID klice, ktery muze otevrit tuto branu. Nastavuj jen v pripade ze je nastaveno na KEY")]
    public int KeyID;
    public float KeyUseMaxDist = 5.0f;

    [Header("Audio")]
    public AudioClip OpenSound;
    public float OpenSoundVolume;

    private Animator _animator;
    private int _entityDeathCount;
    private bool _isOpened;
    private bool _initDone;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _entityDeathCount = 111;
        _isOpened = false;
        _initDone = false;
    }

    private void Update()
    {
        if (MainGameManager.IsGamePaused()) return;
        
        if (!_initDone)
        {
            switch (OpenRule)
            {
                case GateOpenRule.KILL_ENEMIES:
                    _entityDeathCount = EntitiesToKill.Count();
                    foreach (GameEntityObject entity in EntitiesToKill)
                    {
                        entity.OnDeath += OnEntityDeathEvent;
                    }
                    _initDone = true;
                    break;

                case GateOpenRule.KEY:
                    PlayerSave pl = MainGameManager.GetPlayerSave();
                    if (pl != null)
                    {
                        GameEntityObject pEntity = pl.PlayerRef;
                        if (pEntity != null)
                        {
                            pEntity.OnExternalItemUse.Add(OnPlayerItemUse);
                            _initDone = true;
                        }
                    }
                    break;
            }
        }
    }

    public void OpenGate()
    {
        _animator.SetBool("Open", true);
        AudioSource.PlayClipAtPoint(OpenSound, new Vector3(0.0f, 1.0f, -10.0f), OpenSoundVolume);
    }

    private void OnEntityDeathEvent()
    {
        _entityDeathCount -= 1;
        if (_entityDeathCount <= 0)
        {
            OpenGate();
            _isOpened = true;
        }
    }

    private bool OnPlayerItemUse(Item.ItemData item, GameEntityObject sender)
    {
        if (sender == null || item == null)
        {
            return false;
        }
        float distance = Vector3.Distance(sender.transform.position, this.transform.position);
        if (distance < this.KeyUseMaxDist)
        {
            if (item.Type == Item.ItemType.KEY)
            {
                if (item.ParameterValues.Count() > 0)
                {
                    if (item.ParameterValues[0] == KeyID)
                    {
                        OpenGate();
                        _isOpened = true;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool IsOpened()
    {
        return _isOpened;
    }

}
