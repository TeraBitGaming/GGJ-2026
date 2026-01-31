using System;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float FORCE_SCALE = 150f;
    
    private static float JUMP_NORMAL = 14;
    private static float JUMP_SMALL = 9.5f;
    private static float JUMP_LARGE = 20f;

    [SerializeField]
    private float JUMP_VELOCITY = JUMP_NORMAL;

    [SerializeField]
    float VELOCITY_CLAMP = 10f;

    [SerializeField]
    float DRAG_X = 10;
    [SerializeField]
    float GROUND_DRAG_X = 12f;
    InputAction moveAction;
    InputAction jumpAction;

    [SerializeField]
    bool secondPlayer = false;
    Rigidbody2D rb;
    SpriteRenderer sr;
    private bool grounded = true;
    private bool sizeMaskActive = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
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
        sr.color = grounded ? Color.red : Color.blue;
        if (jumpAction.triggered)
        {
            sizeMaskActive = !sizeMaskActive;
            gameObject.transform.localScale = sizeMaskActive ? (secondPlayer ? new Vector3(2, 2, 2) : new Vector3(.5f, .5f, .5f)) : new Vector3(1, 1, 1);
            JUMP_VELOCITY = sizeMaskActive ? (secondPlayer ? JUMP_LARGE : JUMP_SMALL) : JUMP_NORMAL;
        }
    }
    void FixedUpdate()
    {
        float moveInputX = moveAction.ReadValue<Vector2>().x;
        bool jumpPressed = moveAction.ReadValue<Vector2>().y > 0;

        if (moveInputX > 0.01f)
        {
           sr.flipX = false; 
        } else if (moveInputX < -0.01f)
        {
            sr.flipX = true;
        }

        if ((moveInputX < 0 && rb.linearVelocityX > -VELOCITY_CLAMP) || (moveInputX > 0 && rb.linearVelocityX < VELOCITY_CLAMP))
        {
            rb.AddForceX(FORCE_SCALE * moveInputX);
        }
        if (CheckGround() && jumpPressed)
        {
            rb.linearVelocityY = JUMP_VELOCITY;
        }
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
