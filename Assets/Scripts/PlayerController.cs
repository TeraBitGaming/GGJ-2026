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
    
    [SerializeField]
    private float JUMP_VELOCITY = 14f;

    [SerializeField]
    float VELOCITY_CLAMP = 10f;

    [SerializeField]
    float DRAG_X = 10;
    [SerializeField]
    float GROUND_DRAG_X = 12f;
    InputAction moveAction;
    InputAction jumpAction;

    Rigidbody2D rb;
    private bool grounded = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        grounded = true;
    }
    void OnTriggerExit2D(Collider2D other)
    {
        grounded = false;
    }
    void FixedUpdate()
    {
        float moveInputX = moveAction.ReadValue<Vector2>().x;
        if ((moveInputX < 0 && rb.linearVelocityX > -VELOCITY_CLAMP) || (moveInputX > 0 && rb.linearVelocityX < VELOCITY_CLAMP))
        {
            rb.AddForceX(FORCE_SCALE * moveInputX);
        }
        if (CheckGround() && jumpAction.IsPressed())
        {
            gameObject.GetComponent<Rigidbody2D>().linearVelocityY = JUMP_VELOCITY;
        }
        float dragX = CheckGround() ? GROUND_DRAG_X : DRAG_X;
        rb.AddForceX(-dragX* rb.linearVelocityX);
    }
        public bool CheckGround()
    {
        float _distanceToTheGround = GetComponent<Collider2D>().bounds.extents.y;
        return Physics2D.Raycast(transform.position, Vector2.down, _distanceToTheGround + 0.1f);
    }
}
