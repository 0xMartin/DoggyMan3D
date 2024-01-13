using UnityEngine;

public class Trap : MonoBehaviour
{
    public GameObject attackCollider1;
    public AudioClip TrapAudio;
    public float TrapAudioVolume = 0.75f;


    private Animator _animator;
    private float startTime1, endTime1;
    private bool _trig;

    void Start()
    {
        _trig = false;
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_trig)
        {
            if (other.CompareTag("Player"))
            {
                _trig = true;
                _animator.SetBool("Trig", true);
                EnityAnimationSoundPlayer.PlayClipAtPoint(TrapAudio, transform.position, TrapAudioVolume, 15.0f);
                ActivateAttack1(0.3f);
            }
        }
    }

    public void ActivateAttack1(float time)
    {
        attackCollider1.GetComponent<AttackCollider>().Damage = Random.Range(50, 70);
        attackCollider1.SetActive(true);
        startTime1 = 0;
        endTime1 = time;
    }

    private void Update()
    {
        if (attackCollider1 != null)
        {
            if (attackCollider1.activeSelf)
            {
                startTime1 += Time.deltaTime;
                if (startTime1 > endTime1)
                {
                    attackCollider1.SetActive(false);
                }
            }
        }
    }

}
