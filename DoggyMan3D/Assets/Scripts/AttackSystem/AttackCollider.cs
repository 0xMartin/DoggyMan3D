using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AttackCollider : MonoBehaviour
{
    [Header("Damage")]
    public int Damage = 10;
    [Header("HitFx")]
    public GameObject HitFx;

    [Header("Hit sounds")]
    public AudioClip[] HitAudioClips;
    [Range(0, 1)] public float HitAudioVolume = 1.0f;
    public float AudioDistance = 18.0f;

    public void PlayRandomHitSound()
    {
        if (HitAudioClips.Length > 0)
        {
            var index = UnityEngine.Random.Range(0, HitAudioClips.Length);
            EnityAnimationSoundPlayer.PlayClipAtPoint(HitAudioClips[index], gameObject.transform.position, HitAudioVolume, AudioDistance);
        }
    }

    public void CreateHitFx(Vector3 fxPosition)
    {
        if (HitFx != null)
        {
            GameObject obj = Instantiate(HitFx);
            obj.transform.position = fxPosition;
            obj.transform.rotation = transform.rotation;
        }
    }
}
