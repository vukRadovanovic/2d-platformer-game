using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    GameSession gameSession;

    Vector2 moveInput;
    Rigidbody2D rb2d;
    Animator animator;
    BoxCollider2D playerCollider;
    CapsuleCollider2D playerFeet;

    [SerializeField] float moveSpeed = 5;
    [SerializeField] float climbSpeed = 5;
    [SerializeField] float jumpSpeed = 5;
    [SerializeField] int maxJumps = 2;
    [SerializeField] Vector2 deathKick = new Vector2(10f, 10f);
    [SerializeField] PhysicsMaterial2D faceplantMaterial;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;

    private int jumpsLeft = 0;
    private bool hasGrabbedLadder = false;
    private float playerGravityScale = 0;

    bool isAlive = true;
    private bool hasLeftGroundFromDeath = false;

    void Awake() {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<BoxCollider2D>();
        playerFeet = GetComponent<CapsuleCollider2D>();
        gameSession = GetComponent<GameSession>();
    }

    void Start()
    {
        playerGravityScale = rb2d.gravityScale;
    }

    void Update()
    {
        if (!isAlive) {
            if (hasLeftGroundFromDeath) {
                Faceplant();
            } else {
                hasLeftGroundFromDeath = !playerFeet.IsTouchingLayers(LayerMask.GetMask("Ground"));
            }
            return; 
        }
        Run();
        FlipSprite();
        ClimbLadder();
        Die();
    }

    void OnFire(InputValue value) {
        if (!isAlive) { return; }
        if(value.isPressed) {
            Instantiate(bullet, gun.position, gun.rotation);
        }
    }

    void OnMove(InputValue value) {
        if (!isAlive) { return; }
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value) {
        if (!isAlive) { return; }
        if(value.isPressed) {
            if(playerFeet.IsTouchingLayers(LayerMask.GetMask("Ground"))) {
                jumpsLeft = maxJumps;
            }
            if(jumpsLeft > 0) {
                rb2d.velocity = new Vector2(rb2d.velocity.x, jumpSpeed);
                jumpsLeft -= 1;
            }
        }
    }

    void Run() {
        Vector2 playerVelocity = new Vector2(moveInput.x * moveSpeed, rb2d.velocity.y);
        rb2d.velocity = playerVelocity;

        animator.SetBool("isRunning", PlayerHasHorizontalSpeed());
    }

    void FlipSprite() {
        if (PlayerHasHorizontalSpeed()) {
            transform.localScale = new Vector2(Mathf.Sign(rb2d.velocity.x), 1f);
        }
    }

    void ClimbLadder() {
        bool touchingLadder = playerCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"));

        if (!touchingLadder) {
            hasGrabbedLadder = false;
        } else if (touchingLadder && Mathf.Abs(moveInput.y) > Mathf.Epsilon) {
            hasGrabbedLadder = true;
        }
        
        if(hasGrabbedLadder) {
            rb2d.velocity = new Vector2(rb2d.velocity.x, moveInput.y * climbSpeed);
            rb2d.gravityScale = 0;
        } else {
            rb2d.gravityScale = playerGravityScale;
        }

        animator.SetBool("isClimbing", hasGrabbedLadder && PlayerHasVerticalSpeed());
    }

    void Die() {
        if (playerCollider.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards"))) {
            isAlive = false;
            animator.SetTrigger("Dying");
            rb2d.velocity = new Vector2(-Mathf.Sign(rb2d.velocity.x) * deathKick.x, deathKick.y);
            rb2d.sharedMaterial = faceplantMaterial;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }
    
    void Faceplant() {
        if(playerFeet.IsTouchingLayers(LayerMask.GetMask("Ground"))) {
            animator.SetBool("Faceplant", true);
        }
    }

    bool PlayerHasHorizontalSpeed() {
        return Mathf.Abs(rb2d.velocity.x) > Mathf.Epsilon;
    }

    bool PlayerHasVerticalSpeed() {
        return Mathf.Abs(rb2d.velocity.y) > Mathf.Epsilon;
    }
}
