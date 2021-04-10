using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour {

    [SerializeField] private float speed = 5;
    [SerializeField] private int extraJumps = 0;
    [SerializeField] private float jumpHeight = 3.5f;
    [SerializeField] private float extraJumpHeight = 3.5f;
    [Range(0,1)] [SerializeField] private float jumpDamping = .5f;
    [SerializeField] private float jumpLeeway = .1f;
    [SerializeField] private float jumpBuffer = .1f;
    [SerializeField] private Vector2 groundCheckPos;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private LayerMask groundMask;

    private float movement;
    private int extraJumpCount = 0;
    private Rigidbody2D rb;
    private float timeSinceGrounded;
    private float timeSinceJumpInput;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        timeSinceGrounded = jumpLeeway;
        timeSinceJumpInput = jumpBuffer;
        setJumpHeight(jumpHeight);
        setExtraJumpHeight(extraJumpHeight);
    }

    private void Update() {
        movement = Input.GetAxisRaw("Horizontal");

        if (isGrounded()) {
            timeSinceGrounded = 0;
            extraJumpCount = 0;
        } else timeSinceGrounded += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space)) timeSinceJumpInput = 0;
        else timeSinceJumpInput += Time.deltaTime;

        if ((timeSinceGrounded < jumpLeeway || extraJumpCount < extraJumps) && timeSinceJumpInput < jumpBuffer) {
            timeSinceJumpInput = jumpBuffer;
            if (!(timeSinceGrounded < jumpLeeway)) {
                rb.velocity = new Vector2(rb.velocity.x, extraJumpHeight);
                extraJumpCount++;
            } else rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
        }
        if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0) rb.velocity *= new Vector2(1, 1 - jumpDamping);
    }

    private void FixedUpdate() {
        if (movement != 0) rb.velocity = new Vector2(movement * speed, rb.velocity.y);
        else rb.velocity = new Vector2(0, rb.velocity.y);
    }

    private bool isGrounded() {
        return Physics2D.BoxCast((Vector2)transform.position + groundCheckPos, groundCheckSize, 0, Vector2.down, 0, groundMask);
    }

    public void setJumpHeight(float height) {
        jumpHeight = Mathf.Sqrt(2 * rb.gravityScale * -Physics2D.gravity.y * height);
    }

    public void setExtraJumpHeight(float height) {
        extraJumpHeight = Mathf.Sqrt(2 * rb.gravityScale * -Physics2D.gravity.y * height);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + groundCheckPos, groundCheckSize);
    }

}
