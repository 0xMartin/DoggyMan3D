using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MenuButtonFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
{
    private Button _button;
    private Image _wash;
    private Image _accentBar;
    private float _highlight;
    private bool _hovered;
    private bool _pressed;
    private bool _keyboardSelected;

    public static void Attach(Button button)
    {
        if (button != null && button.GetComponent<MenuButtonFeedback>() == null)
            button.gameObject.AddComponent<MenuButtonFeedback>();
    }

    private void Awake()
    {
        _button = GetComponent<Button>();
        _wash = CreateImage("Menu Button Wash");
        RectTransform washRect = _wash.rectTransform;
        washRect.anchorMin = Vector2.zero;
        washRect.anchorMax = Vector2.one;
        washRect.offsetMin = Vector2.zero;
        washRect.offsetMax = Vector2.zero;
        washRect.SetAsFirstSibling();

        _accentBar = CreateImage("Menu Button Accent");
        RectTransform accentRect = _accentBar.rectTransform;
        accentRect.anchorMin = Vector2.right;
        accentRect.anchorMax = Vector2.one;
        accentRect.pivot = new Vector2(1f, 0.5f);
        accentRect.anchoredPosition = Vector2.zero;
        accentRect.sizeDelta = new Vector2(5f, 0f);
        accentRect.SetAsLastSibling();
        UpdateVisual();
    }

    private Image CreateImage(string name)
    {
        var child = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        child.layer = gameObject.layer;
        child.transform.SetParent(transform, false);
        var image = child.GetComponent<Image>();
        image.raycastTarget = false;
        return image;
    }

    private void Update()
    {
        if (!_button.IsInteractable())
            _hovered = _pressed = _keyboardSelected = false;

        float target = _pressed ? 0.65f : (_hovered || _keyboardSelected ? 1f : 0f);
        float next = Mathf.MoveTowards(_highlight, target, Time.unscaledDeltaTime * 6f);
        if (next != _highlight)
        {
            _highlight = next;
            UpdateVisual();
        }
    }

    private void UpdateVisual()
    {
        _wash.color = new Color(1f, 1f, 1f, 0.16f * _highlight);
        _accentBar.color = new Color(1f, 1f, 1f, 0.8f * _highlight);
    }

    private void OnDisable()
    {
        _hovered = _pressed = _keyboardSelected = false;
        _highlight = 0f;
        if (_wash != null)
            UpdateVisual();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        bool isTouch = eventData is ExtendedPointerEventData extended
            ? extended.pointerType == UIPointerType.Touch
            : Application.isMobilePlatform;
        if (!isTouch && _button.IsInteractable())
            _hovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hovered = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && _button.IsInteractable())
        {
            _keyboardSelected = false;
            _pressed = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            _pressed = false;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!(eventData is PointerEventData) && _button.IsInteractable())
            _keyboardSelected = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _keyboardSelected = false;
    }
}
