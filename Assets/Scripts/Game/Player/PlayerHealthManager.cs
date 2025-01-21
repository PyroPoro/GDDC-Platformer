using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    [SerializeField] PlayerAnimationController animController;
    [SerializeField] float respawnDuration;
    public event Action OnDeath = delegate{};
    public event Action OnRespawn = delegate{};
    Transform currentRespawnPoint;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void Die() {
        OnDeath();
        StartCoroutine(RespawnSequence());
    }

    void Respawn() {
        OnRespawn();
    }

    void OnTriggerEnter2D(Collider2D col) {
        if(col.CompareTag("Hazard")) {
            Die();
        }
        if (col.CompareTag("CameraZone")) {
            ACameraZone cameraZone = col.GetComponent<ACameraZone>();
            currentRespawnPoint = cameraZone.respawnPoint;
        }
    }

    IEnumerator RespawnSequence(){
        yield return new WaitForSeconds(respawnDuration);
        transform.position = currentRespawnPoint.position;
        Respawn();
    }
}
