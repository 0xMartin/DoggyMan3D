using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(PostProcessLayer), typeof(PostProcessVolume))]
public class GameplayPostProcessing : MonoBehaviour
{
    private PostProcessProfile _mobileProfile;

    private void Awake()
    {
        if (Application.platform == RuntimePlatform.Android)
            ApplyMobileSettings();
    }

    private void ApplyMobileSettings()
    {
        var layer = GetComponent<PostProcessLayer>();
        var volume = GetComponent<PostProcessVolume>();
        if (volume.sharedProfile == null)
        {
            Debug.LogError("Gameplay camera is missing its post-processing profile.", this);
            return;
        }

        _mobileProfile = volume.profile;
        if (!_mobileProfile.TryGetSettings(out AmbientOcclusion occlusion) ||
            !_mobileProfile.TryGetSettings(out Bloom bloom))
        {
            Debug.LogError("Gameplay post-processing profile must contain ambient occlusion and bloom.", this);
            return;
        }

        layer.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
        occlusion.enabled.Override(false);
        bloom.fastMode.Override(true);
    }

    private void OnDestroy()
    {
        if (_mobileProfile != null)
            RuntimeUtilities.DestroyProfile(_mobileProfile, true);
    }
}
