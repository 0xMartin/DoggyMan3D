using TMPro;
using UnityEngine;

public class Text3D : MonoBehaviour
{
    public float VisibleTime = 5.0f;
    public Vector3 MoveSpeed;
    public TextMeshProUGUI TextMesh;
    public Color TextColor = Color.white;

    private float _fadeOutTime = 2.0f;
    private float _timer;

    void Start()
    {
        _timer = VisibleTime;
    }

    void Update()
    {
        // pomaly pohyb textu zvolenym smerem
        transform.Translate(MoveSpeed * Time.deltaTime);

        // Snížení timeru
        _timer -= Time.deltaTime;

        // Začátek mizení textu
        if (_timer < _fadeOutTime)
        {
            Color color = TextMesh.color;
            color.r = TextColor.r;
            color.g = TextColor.g;
            color.b = TextColor.b;
            color.a = Mathf.Lerp(0, 1, _timer / _fadeOutTime);
            TextMesh.color = color;
        }

        // Odstranění textu, když je čas uplynulý
        if (_timer <= 0)
        {
            Destroy(gameObject);
        }

        // Otáčení textu směrem ke hlavni kamera
        GameObject camera = MainGameManager.GetMainCamera();
        if (camera != null)
        {
            Vector3 lookDirection = camera.transform.position - transform.position;
            lookDirection.y = 0;
            transform.rotation = Quaternion.LookRotation(lookDirection) * Quaternion.Euler(0, 180, 0);
        }
    }

}