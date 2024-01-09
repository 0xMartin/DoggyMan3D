using UnityEngine;

public class ColliderTrigger
{
    // callback
    public delegate void TriggerEvent();
    public TriggerEvent OnEnter;
    public TriggerEvent OnExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnEnter?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnExit?.Invoke();
        }
    }
}
