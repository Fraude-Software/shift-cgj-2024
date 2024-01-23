using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpZoneParticles : MonoBehaviour
{
    private ParticleSystem lightParticles;
    private ParticleSystem ghostlyParticles;

    // Start is called before the first frame update
    void Start()
    {
        lightParticles = transform.Find("LightParticles").GetComponent<ParticleSystem>();
        ghostlyParticles = transform.Find("GhostlyParticles").GetComponent<ParticleSystem>();

        //set shape of particles
        var shape = lightParticles.shape;
        shape.scale = transform.localScale;
        shape = ghostlyParticles.shape;
        shape.scale = transform.localScale;

        var emission = lightParticles.emission;
        emission.rateOverTime = transform.localScale.x * transform.localScale.y * 10;
        emission = ghostlyParticles.emission;
        emission.rateOverTime = transform.localScale.x * transform.localScale.y * 10;
    }
}
