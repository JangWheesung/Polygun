using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FD.Dev;

public class BulletSmoke : MonoBehaviour
{
    ParticleSystem particle;

    private void OnEnable()
    {
        particle = gameObject.GetComponent<ParticleSystem>();
        particle.Play();
        
        FAED.InvokeDelay(() => { FAED.Push(gameObject); }, 1f);
    }
}
