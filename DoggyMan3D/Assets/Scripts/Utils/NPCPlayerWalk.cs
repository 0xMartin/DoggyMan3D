using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCPlayerWalk : MonoBehaviour
{

    public bool Walk = false;
    public float Speed = 2.1f;

    private Animator _animator;
    private GameEntityObject _gameEntity;

    void Start()
    {
        _gameEntity = GetComponent<GameEntityObject>();
        _animator = GetComponent<Animator>();
        _animator.SetBool("Walk", false);
    }

    void Update()
    {
        _animator.SetBool("Walk", Walk);
        if (Walk)
        {
            Vector3 targetDirection = transform.rotation * Vector3.forward;
            _gameEntity.GetCharacterController().Move(targetDirection.normalized * Speed * Time.deltaTime);
        }
        _gameEntity.GetCharacterController().Move(Vector3.down * 3.2f * Time.deltaTime);
    }

}
