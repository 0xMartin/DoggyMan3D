using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFog : MonoBehaviour
{
    [SerializeField] private Color fogColor = new Color(0.12f, 0.16f, 0.2f, 1f);
    [SerializeField] private float fogStart = 70f;
    [SerializeField] private float fogEnd = 350f;

    private Scene _settingsScene;
    private bool _applied;
    private bool _previousFog;
    private Color _previousColor;
    private FogMode _previousMode;
    private float _previousStart;
    private float _previousEnd;

    private void OnEnable()
    {
        _settingsScene = SceneManager.GetActiveScene();
        if (_settingsScene == gameObject.scene)
            return;

        _previousFog = RenderSettings.fog;
        _previousColor = RenderSettings.fogColor;
        _previousMode = RenderSettings.fogMode;
        _previousStart = RenderSettings.fogStartDistance;
        _previousEnd = RenderSettings.fogEndDistance;
        _applied = true;

        RenderSettings.fogColor = fogColor;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = fogStart;
        RenderSettings.fogEndDistance = fogEnd;
        RenderSettings.fog = true;
    }

    private void OnDisable()
    {
        if (!_applied)
            return;

        if (SceneManager.GetActiveScene() == _settingsScene)
        {
            RenderSettings.fog = _previousFog;
            RenderSettings.fogColor = _previousColor;
            RenderSettings.fogMode = _previousMode;
            RenderSettings.fogStartDistance = _previousStart;
            RenderSettings.fogEndDistance = _previousEnd;
        }
        _applied = false;
    }
}
