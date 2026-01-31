using System;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using static UnityEngine.InputManagerEntry;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float FORCE_SCALE = 150f;
    [SerializeField]
    float VELOCITY_CLAMP = 10f;

    [Header("Wall Check")]
    [SerializeField] private Vector2 wallCheckSize = new(0.1f, 0.9f);
    [SerializeField] private float wallCheckDistance = 0.6f;
    
    [Header("Jump")]
    private static float JUMP_NORMAL = 14;
    private static float JUMP_SMALL = 9.5f;
    private static float JUMP_LARGE = 20f;

    [SerializeField]
    private float JUMP_VELOCITY = JUMP_NORMAL;

    [Header("PlayerSettings")]    
    [SerializeField]
    bool secondPlayer = false;

    //Input
    InputAction moveAction;
    InputAction jumpAction;

    InputAction toggleMaskAction;
    //Components
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator animator;

    //State
    BoxCollider2D hitbox;
    BoxCollider2D feet;
    private bool grounded = true;
    private bool sizeMaskActive = false;
    public float speed;
    public float jump;
    public bool jumpQueued;
    public float maxFallSpeed;
    public bool wasGrounded;
    public Vector2 boxSize;
    public float castDistance;
    public LayerMask groundLayer;

    //Animator Parameters
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int VerticalSpeedHash = Animator.StringToHash("VerticalSpeed");
    private static readonly int GroundedHash = Animator.StringToHash("Grounded");

    void Awake()
    {        
        rb = gameObject.GetComponent<Rigidbody2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction(secondPlayer ? "Move2" : "Move1");
        jumpAction = InputSystem.actions.FindAction(secondPlayer ? "Jump2" : "Jump1");
        toggleMaskAction = InputSystem.actions.FindAction("ToggleMask");
        foreach (var col in GetComponents<BoxCollider2D>()) {
            if (col.isTrigger) {
                feet = col;
            } else
            {
                hitbox = col;
            }
        }
    }




    void Update()
    {
        sr.color = grounded ? Color.red : Color.blue;
        if (toggleMaskAction.triggered)
        {
            sizeMaskActive = !sizeMaskActive;
            float scaleFactor = sizeMaskActive ? (secondPlayer ? 2 : .5f) : 1;
            gameObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            JUMP_VELOCITY = sizeMaskActive ? (secondPlayer ? JUMP_LARGE : JUMP_SMALL) : JUMP_NORMAL;
        }

        if (jumpAction.triggered)
        {
            jumpQueued = true;
        }

        // -------------------
        // Animations
        // -------------------

        //Run
        animator.SetFloat(SpeedHash, Mathf.Abs(rb.linearVelocityX));

        //Jump
        animator.SetBool(GroundedHash, grounded);
        animator.SetFloat(VerticalSpeedHash, rb.linearVelocityY);

    }
    
        
    void FixedUpdate()
    {
        wasGrounded = grounded;
        grounded = IsGrounded();

        if (!wasGrounded && grounded)
        {
            bool hardLand = maxFallSpeed < -7f; // tweak value
            animator.SetBool("HardLand", hardLand);

            maxFallSpeed = 0f;
        }


        float moveInputX = moveAction.ReadValue<float>();

        if (jumpQueued && grounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, JUMP_VELOCITY);
        }

        jumpQueued = false;

        if (!grounded)
        {
            maxFallSpeed = Mathf.Min(maxFallSpeed, rb.linearVelocityY);
        }


        // -------------------
        // Flip sprite direction
        // -------------------

        if (moveInputX > 0.01f)
        {
           sr.flipX = false; 
        } else if (moveInputX < -0.01f)
        {
            sr.flipX = true;
        }
        // -------------------
        // Horizontal movement
        // -------------------

        bool pushingLeft = moveInputX < -0.01f;
        bool pushingRight = moveInputX > 0.01f;

        bool wallOnLeft = IsTouchingWall(-1);
        bool wallOnRight = IsTouchingWall(1);

        // Block force into walls while airborne
        if (!grounded)
        {
            if ((pushingLeft && wallOnLeft) || (pushingRight && wallOnRight))
            {
                moveInputX = 0f;
            }
        }

        // Apply movement
        if (Mathf.Abs(moveInputX) > 0.01f &&
            Mathf.Abs(rb.linearVelocity.x) < VELOCITY_CLAMP)
        {
            rb.AddForce(new Vector2(FORCE_SCALE * moveInputX, 0f));
        }


            rb.linearVelocity = new Vector2(
                Mathf.Clamp(rb.linearVelocity.x, -VELOCITY_CLAMP, VELOCITY_CLAMP),
                rb.linearVelocityY
        );

    }

    public bool IsGrounded()
    {
        Bounds bounds = hitbox.bounds;

        float checkHeight = 0.08f;

        Vector2 checkPos = new(
            bounds.center.x,
            bounds.min.y - checkHeight * 0.5f
        );

        Vector2 checkSize = new(
            bounds.size.x * 0.9f,
            checkHeight
        );

        return Physics2D.OverlapBox(checkPos, checkSize, 0f, groundLayer);
    }



    public bool IsTouchingWall(int direction)
    {
        Vector2 origin = (Vector2)transform.position;
        Vector2 dir = direction > 0 ? Vector2.right : Vector2.left;

        return Physics2D.BoxCast(
            origin,
            wallCheckSize,
            0f,
            dir,
            wallCheckDistance,
            groundLayer
        );
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Bounds bounds = hitbox.bounds;

        float checkHeight = 0.08f;

        Vector2 checkPos = new Vector2(
            bounds.center.x,
            bounds.min.y - checkHeight * 0.5f
        );

        Vector2 checkSize = new Vector2(
            bounds.size.x * 0.9f,
            checkHeight
        );

        Gizmos.DrawWireCube(checkPos, checkSize);

    }


}
