using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerAnimID {
    DASH = 0,
    JUMP = 1,
    DIE = 2,
    RESPAWN = 3,
}

public class PlayerAnimationController : MonoBehaviour {
    [SerializeField] Animator anim;
    [SerializeField] PlayerHealthManager playerHealthManager;
    public event Action OnRespawnFinish = delegate{};

    void Start (){
        playerHealthManager.OnDeath += OnDeath;
        playerHealthManager.OnRespawn += OnRespawn;
    }

    public void TriggerAnimation(PlayerAnimID animID) {
        anim.SetInteger("AnimID", (int)animID);
        anim.SetTrigger("ChangeState");
    }

    public void UpdateAnimatorParams(float xSpeed, float yVel, bool isGrounded, bool horizontalInput, bool isDashing) {
        anim.SetFloat("xSpeed", xSpeed);
        anim.SetFloat("yVel", yVel);
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetBool("horizontalInput", horizontalInput);
        anim.SetBool("IsDashing", isDashing);
    }

    public void StartLanding() {
        anim.SetBool("IsLanding", true);
    }
    public void EndLanding() {
        anim.SetBool("IsLanding", false);
    }

    public void StartRespawn(){
        anim.SetBool("IsRespawning", true);
    }
    public void EndRespawn(){
        anim.SetBool("IsRespawning", false);
        OnRespawnFinish();
    }

    void OnDeath(){
        TriggerAnimation(PlayerAnimID.DIE);
        StartRespawn();
    }

    void OnRespawn(){
        TriggerAnimation(PlayerAnimID.RESPAWN);
    }
}
