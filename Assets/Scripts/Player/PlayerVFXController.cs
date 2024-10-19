using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVFXController : MonoBehaviour
{
    [SerializeField] PlayerMovement pm;
    [SerializeField] Transform playerBottom;
    [SerializeField] GameObject jumpParticles;
    [SerializeField] GameObject dashParticles;
    [SerializeField] GameObject dashShockwave;
    [SerializeField] GameObject landingParticles;

    void Start(){
        pm.OnJump += SpawnJumpParticles;
        pm.OnLanding += SpawnLandingParticles;
        pm.OnDashStart += EnableDashParticles;
        pm.OnDashEnd += DisableDashParticles;
        pm.OnDashStart += SpawnShockWave;
        DisableDashParticles();
    }

    void SpawnJumpParticles(){
        Instantiate(jumpParticles, playerBottom.position, Quaternion.identity);
    }

    void SpawnLandingParticles(){
        Instantiate(landingParticles, playerBottom.position, Quaternion.identity);
    }

    void EnableDashParticles(){
        dashParticles.GetComponent<ParticleSystem>().Play();
    }

    void DisableDashParticles(){
        dashParticles.GetComponent<ParticleSystem>().Stop();
    }

    void SpawnShockWave(){
        Instantiate(dashShockwave, transform.position, Quaternion.identity);
    }
    
}
