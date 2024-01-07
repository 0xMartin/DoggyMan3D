using UnityEngine;

public class EnityAnimationSoundPlayer : MonoBehaviour
{

    [Header("Landing sounds")]
    public AudioClip[] LandingAudioClips;
    [Range(0, 1)] public float LandingAudioVolume = 0.8f;

    [Header("Footstep sounds")]
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    [Header("Attack sounds")]
    public AudioClip[] AttackAudioClips;
    [Range(0, 1)] public float AttackAudioVolume = 1.0f;

    [Header("Hit sounds")]
    public AudioClip[] HitAudioClips;
    [Range(0, 1)] public float HitAudioVolume = 1.0f;

    [Header("Death sounds")]
    public AudioClip[] DeathAudioClips;
    [Range(0, 1)] public float DeatAudioVolume = 1.0f;

    public void PlayOnFootstep()
    {
        if (FootstepAudioClips.Length > 0)
        {
            var index = UnityEngine.Random.Range(0, FootstepAudioClips.Length);
            AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(gameObject.transform.position), FootstepAudioVolume);
        }
    }

    public void PlayOnLand()
    {
        if (LandingAudioClips.Length > 0)
        {
            var index = UnityEngine.Random.Range(0, LandingAudioClips.Length);
            AudioSource.PlayClipAtPoint(LandingAudioClips[index], transform.TransformPoint(gameObject.transform.position), LandingAudioVolume);
        }
    }

    public void PlayOnAttack()
    {
        if (AttackAudioClips.Length > 0)
        {
            var index = UnityEngine.Random.Range(0, AttackAudioClips.Length);
            AudioSource.PlayClipAtPoint(AttackAudioClips[index], transform.TransformPoint(gameObject.transform.position), AttackAudioVolume);
        }
    }

    public void PlayOnHit()
    {
        if (HitAudioClips.Length > 0)
        {
            var index = UnityEngine.Random.Range(0, HitAudioClips.Length);
            AudioSource.PlayClipAtPoint(HitAudioClips[index], transform.TransformPoint(gameObject.transform.position), HitAudioVolume);
        }
    }

    public void PlayOnDeath()
    {
        if (DeathAudioClips.Length > 0)
        {
            var index = UnityEngine.Random.Range(0, DeathAudioClips.Length);
            AudioSource.PlayClipAtPoint(DeathAudioClips[index], transform.TransformPoint(gameObject.transform.position), DeatAudioVolume);
        }
    }
}