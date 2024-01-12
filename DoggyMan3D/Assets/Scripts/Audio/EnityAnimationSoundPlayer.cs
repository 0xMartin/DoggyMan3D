using UnityEngine;

public class EnityAnimationSoundPlayer : MonoBehaviour
{

    public float YOffset = 1.4f;
    
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
            AudioSource.PlayClipAtPoint(FootstepAudioClips[index], gameObject.transform.position + Vector3.up * YOffset, FootstepAudioVolume);
        }
    }

    public void PlayOnLand()
    {
        if (LandingAudioClips.Length > 0)
        {
            var index = UnityEngine.Random.Range(0, LandingAudioClips.Length);
            AudioSource.PlayClipAtPoint(LandingAudioClips[index], gameObject.transform.position + Vector3.up * YOffset, LandingAudioVolume);
        }
    }

    public void PlayOnAttack()
    {
        if (AttackAudioClips.Length > 0)
        {
            var index = UnityEngine.Random.Range(0, AttackAudioClips.Length);
            AudioSource.PlayClipAtPoint(AttackAudioClips[index], gameObject.transform.position + Vector3.up * YOffset, AttackAudioVolume);
        }
    }

    public void PlayOnHit()
    {
        if (HitAudioClips.Length > 0)
        {
            var index = UnityEngine.Random.Range(0, HitAudioClips.Length);
            AudioSource.PlayClipAtPoint(HitAudioClips[index], gameObject.transform.position + Vector3.up * YOffset, HitAudioVolume);
        }
    }

    public void PlayOnDeath()
    {
        if (DeathAudioClips.Length > 0)
        {
            var index = UnityEngine.Random.Range(0, DeathAudioClips.Length);
            AudioSource.PlayClipAtPoint(DeathAudioClips[index], gameObject.transform.position + Vector3.up * YOffset, DeatAudioVolume);
        }
    }
}