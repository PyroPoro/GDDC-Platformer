using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovementAdvanced : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float maxHorVelocity;
    [SerializeField] float horAcceleration;
    [SerializeField] float horDeceleration;
    [SerializeField] float velocityPower;
    [SerializeField] float fallSpeedMultiplier;
    [SerializeField] float gravityScale;
    [SerializeField] float jumpCutMultiplier;
    [SerializeField] float jumpForce;
    [SerializeField] float jumpPressedTimerBuffer;
    [SerializeField] float groundedTimerBuffer;
    [SerializeField] float maxVerVelocity;
    [SerializeField] float maxDashVelocity;
    [SerializeField] float frictionStopSpeed;
    [SerializeField] Vector2 groundCheckBoxSize;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float dashDuration;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashStartDelay;
    [SerializeField] float postDashCorrection;
    [SerializeField] float numAfterImages;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] GameObject dashAfterImage;
    [SerializeField] PlayerAnimationController animController;

    public event Action OnDashStart = delegate { };
    public event Action OnDashEnd = delegate { };
    public event Action OnLanding = delegate { };
    public event Action OnJump = delegate { };

    float x;
    bool isJumping;
    float jumpPressedTimer;
    float groundedTimer;
    public int numDashes = 1;
    bool isDashing = false;
    bool isFacingRight = true;
    bool isLanded = true;
    IEnumerator dashCoroutine;

    void Update() {
        //Accelerationn Step 1 TODO

        //Jump Step 1 TODO

        Dash();
        
        animController.transform.localScale = new Vector3(isFacingRight ? 1 : -1, 1, 1);
    }

    void FixedUpdate() {

        // Acceleration Step 3 TODO
        // if (!isDashing) {
        //     HorizontalInput();
        // }
        
        // Coyote Time

        // Jump Step 3 TODO

        // -- animation things -- //
        if (rb.velocity.y < 0 && isJumping) {
            isJumping = false;
        } else if (Mathf.Abs(rb.velocity.y) < 0.01f && isLanded) {
            animController.EndLanding();
        }
        
        // Fixing Floatiness
        // rb.gravityScale = isDashing ? 0 : rb.velocity.y > 0 ? gravityScale : gravityScale * fallSpeedMultiplier;

        // Artificial Friction

        // Clamping Artificial Velocity
        // rb.velocity = new(Mathf.Clamp(rb.velocity.x, -maxDashVelocity, maxDashVelocity), Mathf.Clamp(rb.velocity.y, -maxVerVelocity, maxVerVelocity));

        // groundedTimer -= Time.deltaTime;
        // jumpPressedTimer -= Time.deltaTime;

        // -- animation things -- //
        animController.UpdateAnimatorParams(Mathf.Abs(rb.velocity.x), rb.velocity.y, groundedTimer > 0, Mathf.Abs(x) > 0, isDashing);
    }

    void Jump() {
        // Jump Step 2
    }

    void HorizontalInput() {
        //Acceleration Step 2 TODO
    }

    void Dash() {
        // TODO
    }

    IEnumerator DashCoroutine() {
        // delete this line when you implement this coroutine
        yield return null;
    }
}
