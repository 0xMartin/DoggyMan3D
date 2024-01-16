using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DragonFlame : MonoBehaviour
{

    [Header("FX")]
    public ParticleSystem[] Particles;
    public Light Light;

    [Header("Timing")]
    public float StartLightIntensity = 10.0f;
    public float ActiveTime = 3.0f;
    public float FadeOutTime = 1.2f;

    [Header("Hit Collider")]
    public float MaxColliderLength = 9.0f;

    private bool _isActive;
    private float _timer;

    private Transform _targetTransform = null;
    private Vector3 _positionOffset;
    private Quaternion _rotationOffset;

    private BoxCollider _collider;

    private void Start()
    {
        _collider = GetComponent<BoxCollider>();
        _isActive = true;
        _timer = 0.0f;
        StartCoroutine(DestroyLater());
    }

    private void Update()
    {
        if (!_isActive)
        {
            // plynule snizeni intenzity svetla po prestani chrleni
            if (_timer < FadeOutTime)
            {
                Light.intensity = Mathf.Lerp(StartLightIntensity, 0f, _timer / FadeOutTime);
                _timer += Time.deltaTime;
            }
        }
        else
        {
            // postupne zvetseni hit collideru od hlavy draky, tak postupne jak se ohen siri od hlavy
            const float ColliderTotalTime = 1.9f;
            if (_timer < ColliderTotalTime)
            {
                float len = Mathf.Lerp(0.0f, MaxColliderLength, _timer / ColliderTotalTime);
                _timer += Time.deltaTime;
                _collider.center = new Vector3(0.0f, len / 2.0f - 1.0f, 0.0f);
                _collider.size = new Vector3(_collider.size.x, len, _collider.size.z);
            }
        }

        // sledovani target transform
        if (_targetTransform != null)
        {
            gameObject.transform.position = _targetTransform.position;
            gameObject.transform.rotation = _targetTransform.rotation * _rotationOffset;
            gameObject.transform.Translate(_positionOffset);
        }
    }

    IEnumerator DestroyLater()
    {
        yield return new WaitForSeconds(ActiveTime - 0.1f);
        _collider.size = new Vector3(0.0f, 0.0f, 0.0f);
        yield return new WaitForSeconds(0.1f);

        // zastavy efekt ohne
        _timer = 0.0f;
        _isActive = false;
        foreach (ParticleSystem ps in Particles)
        {
            ps.Stop();
        }

        yield return new WaitForSeconds(FadeOutTime);

        // odstrani cely objekt ohne
        Destroy(gameObject);
    }

    public void SetTargetTransform(Transform transform, Vector3 positionOffset, Vector3 rotationOffset)
    {
        _targetTransform = transform;
        _positionOffset = positionOffset;
        _rotationOffset = Quaternion.Euler(rotationOffset);
    }

}
