using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UIVirtualButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }
    [System.Serializable]
    public class Event : UnityEvent { }

    [Header("Output")]
    public Event buttonPressEvent;
    public Event buttonReleaseEvent;

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonPressEvent.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       buttonReleaseEvent.Invoke();
    }

}
