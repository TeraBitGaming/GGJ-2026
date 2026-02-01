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
    public float FORCE_SCALE = 150f;
    [SerializeField]

    public float VELOCITY_CLAMP = 10f;

    [Header("Drag")]

    [SerializeField]

    float DRAG_X = 10;

    [SerializeField]

    float GROUND_DRAG_X = 12f;

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
    public bool secondPlayer = false;
    
    [SerializeField]
    bool ignorePlayerCollision = false;

    //Input
    InputAction moveAction;
    InputAction jumpAction;

    InputAction toggleMaskAction;
    InputAction toggleVisionAction;
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
    public float moveInputX;
    public float maxFallSpeed;
    public bool wasGrounded;
    public Vector2 boxSize;
    public float castDistance;
    public LayerMask groundLayer;

    //Animator Parameters
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int VerticalSpeedHash = Animator.StringToHash("VerticalSpeed");
    private static readonly int GroundedHash = Animator.StringToHash("Grounded");
    private static readonly int FacingRightHash = Animator.StringToHash("FacingRight");

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
        if (ignorePlayerCollision)
        {
            Physics2D.IgnoreLayerCollision(6, 6);
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
        float normalizedSpeed = Mathf.Abs(rb.linearVelocityX) / VELOCITY_CLAMP;
        animator.SetFloat(SpeedHash, normalizedSpeed);


        //Jump
        animator.SetBool(GroundedHash, grounded);
        float normalizedVertical = Mathf.Clamp(rb.linearVelocityY / JUMP_VELOCITY, -1f, 1f);
        animator.SetFloat(VerticalSpeedHash, normalizedVertical);


        moveInputX = moveAction.ReadValue<float>();

        if (Mathf.Abs(moveInputX) > 0.01f)
        {
            float facing = moveInputX > 0 ? 1f : -1f;
            animator.SetFloat(FacingRightHash, facing);
        }
    }
    
        
    void FixedUpdate()
    {
        wasGrounded = grounded;
        grounded = IsGrounded();

        if (!wasGrounded && grounded)
        {
            bool hardLand = maxFallSpeed < -7f; // tweak value
            animator.SetFloat("HardLand", hardLand ? 1f : 0f);

            maxFallSpeed = 0f;
        }


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
        float dragX = IsGrounded() ? GROUND_DRAG_X : DRAG_X;

        rb.AddForceX(-dragX* rb.linearVelocityX);
    }

    public bool IsGrounded()
    {
        Bounds bounds = hitbox.bounds;

        float checkHeight = 0.08f;

        float cornerRadius = 0.1f;
        Vector2 checkPos = new(
            bounds.center.x,
            bounds.min.y - checkHeight * 0.5f - cornerRadius
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
