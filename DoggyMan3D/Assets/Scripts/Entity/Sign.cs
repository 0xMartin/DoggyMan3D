
using UnityEngine;

public class Sign : MonoBehaviour
{

    public string Text;
    public Color TextColor = Color.white;
    public GameObject Text3DPrefab;


    private GameObject _textInstance;

    private void Awake()
    {
        _textInstance = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShowText();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HideText();
        }
    }

    public void ShowText()
    {
        if (_textInstance == null)
        {
            _textInstance = Instantiate(Text3DPrefab);
            _textInstance.transform.position = transform.position + new Vector3(0.0f, 1.2f, 0.0f); ;
            Text3D text = _textInstance.GetComponent<Text3D>();
            if (text != null)
            {
                text.TextMesh.text = Text;
                text.TextColor = TextColor;
                text.MoveSpeed = new Vector3(0.0f, 0.0f, 0.0f);
                text.VisibleTime = 9000.0f;
            }
        }
    }

    public void HideText()
    {
        if (_textInstance != null)
        {
            Destroy(_textInstance);
        }
    }
}