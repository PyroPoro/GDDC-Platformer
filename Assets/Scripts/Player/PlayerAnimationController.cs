using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerAnimID {
    DASH = 0,
    JUMP = 1
}

public class PlayerAnimationController : MonoBehaviour {
    [SerializeField] Animator anim;
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
}
