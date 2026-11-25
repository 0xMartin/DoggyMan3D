using StarterAssets;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIVirtualCameraLook : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,
    IInitializePotentialDragHandler, IDragHandler, IEndDragHandler
{
    public UICanvasControllerInput canvasInput;
    [Tooltip("Camera rotation for a swipe across the shorter screen dimension.")]
    public float degreesPerScreen = 180f;

    private int? _pointerId;

    private void Awake()
    {
        if (canvasInput == null)
        {
            Debug.LogError("Camera look surface is missing its canvas input controller.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        if (MainGameManager.IsGamePaused() || Time.timeScale == 0f)
            ResetPointer();
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        eventData.useDragThreshold = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isActiveAndEnabled || _pointerId.HasValue ||
            eventData.button != PointerEventData.InputButton.Left ||
            MainGameManager.IsGamePaused() || Time.timeScale == 0f)
            return;

        _pointerId = eventData.pointerId;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isActiveAndEnabled || _pointerId != eventData.pointerId)
            return;

        if (MainGameManager.IsGamePaused() || Time.timeScale == 0f)
        {
            ResetPointer();
            return;
        }

        float scale = degreesPerScreen / Mathf.Max(1, Mathf.Min(Screen.width, Screen.height));
        canvasInput.InputSystem.AddLookDelta(new Vector2(eventData.delta.x, -eventData.delta.y) * scale);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_pointerId == eventData.pointerId)
            _pointerId = null;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnPointerUp(eventData);
    }

    private void OnDisable()
    {
        ResetPointer();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            ResetPointer();
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
            ResetPointer();
    }

    private void ResetPointer()
    {
        _pointerId = null;
        if (canvasInput != null && canvasInput.InputSystem != null)
            canvasInput.InputSystem.ClearLookDelta();
    }
}
