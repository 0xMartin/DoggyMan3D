using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneShotFX : MonoBehaviour
{
    public ParticleSystem targetParticleSystem; 
    public float activeTime = 5.0f;           
    public float fadeTime = 2.0f;              
    public GameEntityObject TargetEntity;

    void Start()
    {
        if (targetParticleSystem != null)
        {
            StartCoroutine(HandleParticleSystem());
        }
    }

    void Update() {
        if(TargetEntity != null) {
            transform.position = TargetEntity.transform.position;
            transform.Translate(new Vector3(0.0f, -0.2f, 0.0f));
        }
    }

    IEnumerator HandleParticleSystem()
    {
        // Čeká na dobu, po kterou má být efekt aktivní
        yield return new WaitForSeconds(activeTime);

        // Zastaví tvorbu nových částic
        targetParticleSystem.Stop();

        // Počká další určený časový úsek (fadeTime)
        yield return new WaitForSeconds(fadeTime);

        // Odstraní Particle System (nebo celý GameObject) ze scény
        Destroy(targetParticleSystem.gameObject);
    }
}
