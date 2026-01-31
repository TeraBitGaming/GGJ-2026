using System;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

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

    //Components
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator animator;

    //State
    private bool grounded = true;
    private bool sizeMaskActive = false;

    //Animator Parameters
    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    void Awake()
    {        
        rb = gameObject.GetComponent<Rigidbody2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction(secondPlayer ? "Move1" : "Move2");
        jumpAction = InputSystem.actions.FindAction("Jump1");        
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
        // sr.color = grounded ? Color.red : Color.blue;

        if (jumpAction.triggered)
        {
            sizeMaskActive = !sizeMaskActive;

            transform.localScale = sizeMaskActive
                ? (secondPlayer ? Vector3.one * 2f : Vector3.one * 0.5f)
                : Vector3.one;

            JUMP_VELOCITY = sizeMaskActive
                ? (secondPlayer ? JUMP_LARGE : JUMP_SMALL)
                : JUMP_NORMAL;
        }
    }
    void FixedUpdate()
    {

        Debug.Log("Speed: " + Mathf.Abs(rb.linearVelocityX));


        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        float moveInputX = moveAction.ReadValue<Vector2>().x;
        bool jumpPressed = moveAction.ReadValue<Vector2>().y > 0;

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
        // Run Animation
        // -------------------
        animator.SetFloat(SpeedHash, Mathf.Abs(rb.linearVelocityX));

        // -------------------
        // Horizontal movement
        // -------------------

        if ((moveInputX < 0 && rb.linearVelocityX > -VELOCITY_CLAMP) || (moveInputX > 0 && rb.linearVelocityX < VELOCITY_CLAMP))
        {
            rb.AddForceX(FORCE_SCALE * moveInputX);
        }

        // -------------------
        // Jump
        // -------------------

        if (CheckGround() && jumpPressed)
        {
            rb.linearVelocityY = JUMP_VELOCITY;
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
