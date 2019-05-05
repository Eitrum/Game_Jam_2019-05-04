using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionOnImpact : MonoBehaviour
{
    public ParticleSystem explosionParticle;

    private void OnCollisionEnter(Collision collision)
    {
        explosionParticle.Play();
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/spaceStationHit");
    }
}
