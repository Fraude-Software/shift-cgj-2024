using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float jumpForce;
    [SerializeField] private float maxJumpTime;
    [SerializeField] private float preferredGroundSpeed;
    [SerializeField] private float preferredAirSpeed;
    [SerializeField] private float groundAcceleration;
    [SerializeField] private float airAcceleration;
    [SerializeField] private float groundDeceleration;
    [SerializeField] private float airDeceleration;

    private bool isDashing;
    private bool hasJumped;
    private float jumpTimeLeft;
    public Transform groundCheckLeft;
    public Transform groundCheckRight;

    private Rigidbody2D rb;

    //Inputs

    private InputAction _moveAction;
    private InputAction _jumpAction;

    private bool inputJump;
    private Vector2 inputMove;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move(inputMove);
        Jump(inputJump);
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        inputMove = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        inputJump = context.ReadValueAsButton();
    }

    void Move(Vector2 _inputMove)
    {
        float drag = 0f;
        if (!IsGrounded() && _inputMove.y < 0)
            Fastfall();

        _inputMove.y = 0f;

        float xSpeed = Mathf.Abs(rb.velocity.x);

        if (!isDashing)
        {
            bool signsEqual = Math.Sign(_inputMove.x) == Math.Sign(rb.velocity.x);
            if (IsGrounded())
            {
                drag = signsEqual ? groundDeceleration : groundDeceleration * 5;
                if (xSpeed <= preferredGroundSpeed || !signsEqual)
                {
                    rb.AddForce(new Vector2(_inputMove.x * groundAcceleration, 0f));
                }
            }
            else
            {
                drag = Math.Sign(_inputMove.x) == Math.Sign(rb.velocity.x) ? airDeceleration : airDeceleration * 5;
                if (xSpeed <= preferredAirSpeed || !signsEqual)
                {
                    rb.AddForce(new Vector2(_inputMove.x * airAcceleration, 0f));
                }
            }

            if (Mathf.Abs(rb.velocity.x) > 0)
            {
                rb.AddForce(new Vector2(-Mathf.Sign(rb.velocity.x) * drag, 0f));
            }
        }
    }

    void Jump(bool _inputJump)
    {
        if(_inputJump == false)
            hasJumped = false;

        if(IsGrounded()) {
            jumpTimeLeft = maxJumpTime;
        }

        if (_inputJump && IsGrounded() && !hasJumped) {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            hasJumped = true;
        } else if(_inputJump && jumpTimeLeft > 0f && rb.velocity.y > 0f) {
            rb.AddForce(new Vector2(0f, jumpForce * jumpTimeLeft), ForceMode2D.Impulse);
            jumpTimeLeft -= Time.deltaTime;
        }
    }

    void Fastfall()
    {
        //TODO
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapArea(groundCheckLeft.position, groundCheckRight.position);
    }
}
