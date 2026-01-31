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
    
    [Header("Jump")]
    private static float JUMP_NORMAL = 14;
    private static float JUMP_SMALL = 9.5f;
    private static float JUMP_LARGE = 20f;

    [SerializeField]
    private float JUMP_VELOCITY = JUMP_NORMAL;

    [Header("Drag")]
    [SerializeField]
    float DRAG_X = 10;
    [SerializeField]
    float GROUND_DRAG_X = 12f;

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
        rb = gameObject.GetComponent<Rigidbody2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
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
    void OnTriggerEnter2D(Collider2D other)
    {
        grounded = true;
    }
    void OnTriggerStay2D(Collider2D other)
    {
        grounded = true;
    }
    void OnTriggerExit2D(Collider2D other)
    {
        grounded = false;
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
            hitbox.edgeRadius = 0.1f * scaleFactor;
        }
        if (CheckGround() && jumpAction.triggered)
        {
            rb.linearVelocityY = JUMP_VELOCITY;
        }

    }
    void FixedUpdate()
    {

        float moveInputX = moveAction.ReadValue<float>();

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
        // Animations
        // -------------------
        //Run
        animator.SetFloat(SpeedHash, Mathf.Abs(rb.linearVelocityX));

        //Jump
        animator.SetBool(GroundedHash, grounded);
        animator.SetFloat(VerticalSpeedHash, rb.linearVelocityY);

        // -------------------
        // Horizontal movement
        // -------------------

        if ((moveInputX < 0 && rb.linearVelocityX > -VELOCITY_CLAMP) || (moveInputX > 0 && rb.linearVelocityX < VELOCITY_CLAMP))
        {
            rb.AddForceX(FORCE_SCALE * moveInputX);
        }

        // -------------------
        // Drag
        // -------------------

        float dragX = CheckGround() ? GROUND_DRAG_X : DRAG_X;
        rb.AddForceX(-dragX* rb.linearVelocityX);
    }
    
        public bool CheckGround()
    {
        // float _distanceToTheGround = GetComponent<Collider2D>().bounds.extents.y;
        // return Physics2D.Raycast(transform.position, Vector2.down, _distanceToTheGround + 0.1f);
        return grounded;
    }
}
