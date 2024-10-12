using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

enum PlayerForm {
    SLIME,
    FROG,
}


public class PlayerMovement : MonoBehaviour
{
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
    [SerializeField] float postDashCorrection;
    [SerializeField] float numAfterImages;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] GameObject dashAfterImage;

    
    float x;
    float y;
    bool isJumping;
    float jumpPressedTimer;
    float groundedTimer;
    public int numDashes = 1;
    bool isDashing = false;
    bool isSwinging = false;
    IEnumerator dashCoroutine;
    PlayerForm currentForm = PlayerForm.SLIME;
    List<Transform> swingHinges = new();
    Transform closestHinge;

    void Start() {
        
    }

    void Update() {
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
    }

    void FixedUpdate() {
        if(!isDashing && !isSwinging){
            HorizontalInput();
            VerticalInput();
        }

        if (Physics2D.OverlapBox(groundCheck.position, groundCheckBoxSize, 0, groundLayer)) {
            groundedTimer = groundedTimerBuffer;
            if(!isDashing){
                numDashes = 1;
            }
        }

        if ((groundedTimer > 0 || isSwinging) && jumpPressedTimer > 0 && !isJumping) {
            Jump();
        }

        if (rb.velocity.y < 0 && isJumping) {
            isJumping = false;
        }

        rb.gravityScale =  isDashing ? 0 : rb.velocity.y > 0 ? gravityScale : gravityScale * fallSpeedMultiplier;
        
        if (groundedTimer > 0 && x == 0) {
            float friction = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(frictionStopSpeed)) * Mathf.Sign(rb.velocity.x);
            rb.AddForce(Vector2.right * -friction, ForceMode2D.Impulse);
        }

        
        rb.velocity = new(Mathf.Clamp(rb.velocity.x, -maxDashVelocity, maxDashVelocity), Mathf.Clamp(rb.velocity.y, -maxVerVelocity, maxVerVelocity));

        groundedTimer -= Time.deltaTime;
        jumpPressedTimer -= Time.deltaTime;
    }
    
    void Jump(){
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        groundedTimer = 0;
        isJumping = true;
        if(TryGetComponent<HingeJoint2D>(out var hinge)){
            Destroy(hinge);
            closestHinge.GetComponent<SwingHinge>().DisconnectObject();
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
    }

    void VerticalInput() {
        if (y < 0) {
            RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 1, groundLayer);
            if(hit.collider != null && hit.collider.CompareTag("Platform")) {
                hit.collider.gameObject.GetComponent<PlatformController>().DisableCollision(GetComponent<Collider2D>());
            }
        }
    }

    void SwingInput(){ //currentForm == PlayerForm.FROG && 
        if(closestHinge != null) {
            if(Input.GetKeyDown(KeyCode.E)) {
                if(TryGetComponent<HingeJoint2D>(out var hinge)){
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
        if(currentForm != PlayerForm.SLIME || isSwinging) return;

        if(Input.GetKeyDown(KeyCode.LeftShift)){
            if (numDashes > 0) {
                if(dashCoroutine != null){
                    StopCoroutine(dashCoroutine);
                }
                dashCoroutine = DashCoroutinne();
                StartCoroutine(dashCoroutine);
                numDashes--;
            }
        }
    }

    IEnumerator DashCoroutinne(){
        rb.velocity = Vector2.zero;
        isDashing = true;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 diff = mousePos - transform.position;
        Vector2 dir = ((Vector2)diff).normalized;
        rb.AddForce(dir * dashSpeed, ForceMode2D.Impulse);
        for(int i = 0; i < numAfterImages; i++){
            GameObject afterImage = Instantiate(dashAfterImage, transform.position, transform.rotation);
            afterImage.transform.localScale = spriteRenderer.transform.localScale;
            afterImage.GetComponent<DashAfterImage>().Initialize(spriteRenderer.sprite, spriteRenderer.color);
            yield return new WaitForSeconds(dashDuration / numAfterImages);
        }
        rb.AddForce(-rb.velocity.y * postDashCorrection * Vector2.up, ForceMode2D.Impulse);
        isDashing = false;
    }

    
    void OnTriggerEnter2D(Collider2D col){
        if(currentForm == PlayerForm.SLIME) {
            if(col.CompareTag("SwingHinge")) {
                if(!swingHinges.Contains(col.transform)) {
                    swingHinges.Add(col.transform);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        if(currentForm == PlayerForm.SLIME) {
            if(col.CompareTag("SwingHinge")) {
                if(swingHinges.Contains(col.transform)) {
                    swingHinges.Remove(col.transform);
                    col.GetComponent<SwingHinge>().SetActive(false);
                }
            }
        }
    }

    float GetDistance(Transform t1, Transform t2){
        return (t1.position - t2.position).magnitude;
    }

    void UpdateActiveHinge(){
        if(swingHinges.Count == 0) {
            closestHinge = null;
            return;
        }
        
        closestHinge = swingHinges[0];
        float distance = GetDistance(closestHinge, transform);
        foreach(Transform tr in swingHinges){
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
}
