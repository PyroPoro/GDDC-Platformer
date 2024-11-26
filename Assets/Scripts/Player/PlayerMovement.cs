using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

enum PlayerForm {
    SLIME,
    FROG,
}

public class PlayerMovement : MonoBehaviour {
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float maxHorVelocity;
    [SerializeField] float maxVerVelocity;
    [SerializeField] float maxDashVelocity;
    [SerializeField] float horAcceleration;
    [SerializeField] float horDeceleration;
    [SerializeField] float frictionStopSpeed;
    [SerializeField] float velocityPower;
    [SerializeField] float fallSpeedMultiplier;
    [SerializeField] float gravityScale;
    [SerializeField] float jumpCutMultiplier;
    [SerializeField] float jumpForce;
    [SerializeField] Vector2 groundCheckBoxSize;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float jumpPressedTimerBuffer;
    [SerializeField] float groundedTimerBuffer;
    [SerializeField] float dashDuration;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashStartDelay;
    [SerializeField] float postDashCorrection;
    [SerializeField] float numAfterImages;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] GameObject dashAfterImage;
    [SerializeField] PlayerAnimationController animController;
    [SerializeField] PlayerHealthManager healthManager;

    public event Action OnDashStart = delegate { };
    public event Action OnDashEnd = delegate { };
    public event Action OnLanding = delegate { };
    public event Action OnJump = delegate { };

    float x;
    float y;
    bool isJumping;
    float jumpPressedTimer;
    float groundedTimer;
    public int numDashes = 1;
    bool isDashing = false;
    bool isSwinging = false;
    bool isFacingRight = true;
    bool isLanded = true;
    bool inputEnabled = true;
    IEnumerator dashCoroutine;
    PlayerForm currentForm = PlayerForm.SLIME;
    List<Transform> swingHinges = new();
    Transform closestHinge;

    void Start() {
        healthManager.OnDeath += DisablePlayerInput;
        animController.OnRespawnFinish += EnablePlayerInput;
    }

    void Update() {
        if(!inputEnabled) return;

        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space)) {
            jumpPressedTimer = jumpPressedTimerBuffer;
        }
        if (Input.GetKeyUp(KeyCode.Space)) {
            if (rb.velocity.y > 0 && isJumping) {
                rb.AddForce((1 - jumpCutMultiplier) * rb.velocity.y * Vector2.down, ForceMode2D.Impulse);
            }
            jumpPressedTimer = 0;
        }
        Dash();
        UpdateActiveHinge();
        animController.transform.localScale = new Vector3(isFacingRight ? 1 : -1, 1, 1);
    }

    void FixedUpdate() {
        if(!inputEnabled) return;

        if (!isDashing && !isSwinging) {
            HorizontalInput();
            VerticalInput();
        }
        Collider2D col = Physics2D.OverlapBox(groundCheck.position, groundCheckBoxSize, 0, groundLayer);
        if (col != null) {
            groundedTimer = groundedTimerBuffer;
            if (!isDashing) {
                numDashes = 1;
            }
            if (!isLanded) {
                isLanded = true;
                if (!col.CompareTag("Platform")) {
                    OnLanding();
                }
            }
        } else {
            isLanded = false;
        }

        if ((groundedTimer > 0 || isSwinging) && jumpPressedTimer > 0 && !isJumping) {
            Jump();
        }

        if (rb.velocity.y < 0 && isJumping) {
            isJumping = false;
        } else if (Mathf.Abs(rb.velocity.y) < 0.01f && isLanded) {
            animController.EndLanding();
        }

        rb.gravityScale = isDashing ? 0 : rb.velocity.y > 0 ? gravityScale : gravityScale * fallSpeedMultiplier;

        if (groundedTimer > 0 && x == 0) {
            float friction = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(frictionStopSpeed)) * Mathf.Sign(rb.velocity.x);
            rb.AddForce(Vector2.right * -friction, ForceMode2D.Impulse);
        }


        rb.velocity = new(Mathf.Clamp(rb.velocity.x, -maxDashVelocity, maxDashVelocity), Mathf.Clamp(rb.velocity.y, -maxVerVelocity, maxVerVelocity));

        groundedTimer -= Time.deltaTime;
        jumpPressedTimer -= Time.deltaTime;

        animController.UpdateAnimatorParams(Mathf.Abs(rb.velocity.x), rb.velocity.y, groundedTimer > 0, Mathf.Abs(x) > 0, isDashing);
    }

    void Jump() {
        animController.TriggerAnimation(PlayerAnimID.JUMP);
        animController.StartLanding();
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        groundedTimer = 0;
        isJumping = true;
        OnJump();
        if (TryGetComponent<HingeJoint2D>(out var hinge)) {
            Destroy(hinge);
            DisconnectFromHinge();
            isSwinging = false;
        }
    }

    void HorizontalInput() {
        float targetVel = x * maxHorVelocity;
        float currVel = rb.velocity.x;
        float velDiff = targetVel - currVel;
        float acceleration = Mathf.Abs(x) > 0 ? horAcceleration : horDeceleration;
        float appliedAcceleration = Mathf.Pow(Mathf.Abs(velDiff) * acceleration, velocityPower) * Mathf.Sign(velDiff);
        rb.AddForce(appliedAcceleration * Vector2.right);
        if (rb.velocity.x > 0 && x > 0) {
            isFacingRight = true;
        } else if (rb.velocity.x < 0 && x < 0) {
            isFacingRight = false;
        }
    }

    void VerticalInput() {
        if (y < 0) {
            RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 1, groundLayer);
            if (hit.collider != null && hit.collider.CompareTag("Platform")) {
                hit.collider.gameObject.GetComponent<PlatformController>().DisableCollision(GetComponent<Collider2D>());
            }
        }
    }

    void SwingInput() { //currentForm == PlayerForm.FROG && 
        if (closestHinge != null) {
            if (Input.GetKeyDown(KeyCode.E)) {
                if (TryGetComponent<HingeJoint2D>(out var hinge)) {
                    Destroy(hinge);
                }
                HingeJoint2D joint = gameObject.AddComponent<HingeJoint2D>();
                joint.connectedBody = closestHinge.GetComponent<SwingHinge>().GetHinge();
                joint.autoConfigureConnectedAnchor = true;
                closestHinge.GetComponent<SwingHinge>().SetAttachedObject(transform);
                isSwinging = true;
                numDashes = 1;
            }
        }
    }

    void Dash() {
        if (currentForm != PlayerForm.SLIME || isSwinging)
            return;

        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            if (numDashes > 0) {
                if (dashCoroutine != null) {
                    StopCoroutine(dashCoroutine);
                }
                dashCoroutine = DashCoroutinne();
                StartCoroutine(dashCoroutine);
                isDashing = true;
                isJumping = false;
                numDashes--;
                animController.UpdateAnimatorParams(Mathf.Abs(rb.velocity.x), rb.velocity.y, groundedTimer > 0, Mathf.Abs(x) > 0, isDashing);
                animController.TriggerAnimation(PlayerAnimID.DASH);
                animController.StartLanding();
                OnDashStart();
            }
        }
    }

    IEnumerator DashCoroutinne() {
        rb.velocity = Vector2.zero;
        groundedTimer = 0;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 diff = mousePos - transform.position;
        Vector2 dir = ((Vector2)diff).normalized;
        transform.right = dir.x >= 0 ? dir : -dir;
        isFacingRight = dir.x > 0;

        yield return new WaitForSeconds(dashStartDelay);

        rb.AddForce(dir * dashSpeed, ForceMode2D.Impulse);
        for (int i = 0; i < numAfterImages; i++) {
            GameObject afterImage = Instantiate(dashAfterImage, transform.position, transform.rotation);
            afterImage.transform.localScale = spriteRenderer.transform.localScale;
            afterImage.GetComponent<DashAfterImage>().Initialize(spriteRenderer.sprite, spriteRenderer.color);
            yield return new WaitForSeconds(dashDuration / numAfterImages);
        }
        rb.AddForce(-rb.velocity.y * postDashCorrection * Vector2.up, ForceMode2D.Impulse);
        isDashing = false;
        transform.right = Vector3.right;
        OnDashEnd();
    }


    void OnTriggerEnter2D(Collider2D col) {
        if (currentForm == PlayerForm.SLIME) {
            if (col.CompareTag("SwingHinge")) {
                if (!swingHinges.Contains(col.transform)) {
                    swingHinges.Add(col.transform);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        if (currentForm == PlayerForm.SLIME) {
            if (col.CompareTag("SwingHinge")) {
                if (swingHinges.Contains(col.transform)) {
                    swingHinges.Remove(col.transform);
                    col.GetComponent<SwingHinge>().SetActive(false);
                }
            }
        }
    }

    float GetDistance(Transform t1, Transform t2) {
        return (t1.position - t2.position).magnitude;
    }

    void UpdateActiveHinge() {
        if (swingHinges.Count == 0) {
            if (!isSwinging) {
                closestHinge = null;
            }
            return;
        }

        closestHinge = swingHinges[0];
        float distance = GetDistance(closestHinge, transform);
        foreach (Transform tr in swingHinges) {
            float dis = GetDistance(tr, transform);
            if (dis < distance) {
                distance = dis;
                closestHinge = tr;
            }
            tr.GetComponent<SwingHinge>().SetActive(false);
        }
        closestHinge.GetComponent<SwingHinge>().SetActive(true);
        SwingInput();
    }

    void DisablePlayerInput() {
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        inputEnabled = false;
        if (TryGetComponent<HingeJoint2D>(out var hinge)) {
            Destroy(hinge);
            DisconnectFromHinge();
            isSwinging = false;
        }
    }

    void DisconnectFromHinge(){
        if(closestHinge != null) {
            closestHinge.GetComponent<SwingHinge>().DisconnectObject();
            closestHinge = null;
        }
    }

    void EnablePlayerInput() {
        inputEnabled = true;
        rb.isKinematic = false;
        isSwinging = false;
    }
}
