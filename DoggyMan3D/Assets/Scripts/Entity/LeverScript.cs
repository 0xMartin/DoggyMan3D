using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverScript : MonoBehaviour
{

    public bool IsActivated = false;

    public AudioClip SoundFx;
    public float SoundFxVolume = 1.0f;

    private Animator _animator;
    private GameEntityObject _entity;
    private bool _isSwitchActivated;

    void Start()
    {
        _isSwitchActivated = false;
        _entity = GetComponent<GameEntityObject>();
        _animator = GetComponent<Animator>();
        _animator.SetBool("Switch", false);
    }

    private void Update()
    {
        if (_isSwitchActivated) return;

        if (this.IsActivated)
        {
            _isSwitchActivated = true;
            if (SoundFx != null)
            {
                AudioSource.PlayClipAtPoint(SoundFx, transform.position, SoundFxVolume);
            }
            _animator.SetBool("Switch", true);
            _entity.Lives = 0;
            _entity.OnDeath?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.IsActivated) return;

        if (other.CompareTag("AttackPlayer"))
        {
            this.IsActivated = true;
        }
    }

}
