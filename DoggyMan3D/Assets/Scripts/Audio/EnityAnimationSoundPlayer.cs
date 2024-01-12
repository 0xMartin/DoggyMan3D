using Unity.VisualScripting;
using UnityEngine;

public class EnityAnimationSoundPlayer : MonoBehaviour
{

    public float AudioDistance = 15.0f;

    [Header("Landing sounds")]
    public AudioClip[] LandingAudioClips;
    [Range(0, 1)] public float LandingAudioVolume = 0.8f;

    [Header("Footstep sounds")]
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    [Header("Attack sounds")]
    public AudioClip[] AttackAudioClips;
    [Range(0, 1)] public float AttackAudioVolume = 1.0f;

    [Header("Death sounds")]
    public AudioClip[] DeathAudioClips;
    [Range(0, 1)] public float DeatAudioVolume = 1.0f;

    public void PlayOnFootstep()
    {
        if (FootstepAudioClips.Length > 0)
        {
            var index = UnityEngine.Random.Range(0, FootstepAudioClips.Length);
            PlayClipAtPoint(FootstepAudioClips[index], gameObject.transform.position, FootstepAudioVolume, AudioDistance);
        }
    }

    public void PlayOnLand()
    {
        if (LandingAudioClips.Length > 0)
        {
            var index = UnityEngine.Random.Range(0, LandingAudioClips.Length);
            PlayClipAtPoint(LandingAudioClips[index], gameObject.transform.position, LandingAudioVolume, AudioDistance);
        }
    }

    public void PlayOnAttack()
    {
        if (AttackAudioClips.Length > 0)
        {
            var index = UnityEngine.Random.Range(0, AttackAudioClips.Length);
            PlayClipAtPoint(AttackAudioClips[index], gameObject.transform.position, AttackAudioVolume, AudioDistance);
        }
    }

    public void PlayOnDeath()
    {
        if (DeathAudioClips.Length > 0)
        {
            var index = UnityEngine.Random.Range(0, DeathAudioClips.Length);
            PlayClipAtPoint(DeathAudioClips[index], gameObject.transform.position, DeatAudioVolume, AudioDistance);
        }
    }

    public static void PlayClipAtPoint(AudioClip clip, Vector3 position, [UnityEngine.Internal.DefaultValue("1.0F")] float volume, float distance)
    {
        GameObject gameObject = new GameObject("One shot audio");
        gameObject.transform.position = position;
        AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        audioSource.clip = clip;
        audioSource.spatialBlend = 1f;
        audioSource.volume = volume;
        audioSource.maxDistance = distance;
        audioSource.Play();
        Object.Destroy(gameObject, clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
    }

}