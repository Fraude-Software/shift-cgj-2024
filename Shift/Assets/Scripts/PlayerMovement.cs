using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public GameObject normalMap;
    [SerializeField] public GameObject etherMap;

    [SerializeField] private float jumpForce;
    [SerializeField] private float maxJumpTime;
    [SerializeField] private float preferredGroundSpeed;
    [SerializeField] private float preferredAirSpeed;
    [SerializeField] private float groundAcceleration;
    [SerializeField] private float airAcceleration;
    [SerializeField] private float groundDeceleration;
    [SerializeField] private float airDeceleration;

    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

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

    private bool inputSwitch;

    void Start(){
        etherMap.SetActive(false);
        rb = GetComponent<Rigidbody2D>();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        Move(inputMove);
        Jump(inputJump);
        Flip(rb.velocity.x);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        inputMove = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        inputJump = context.ReadValueAsButton();
    }

    public void OnSwitch(InputAction.CallbackContext context)
    {
       inputSwitch = context.ReadValueAsButton();
       if(context.started){
            Switch(inputSwitch);
       }
    }

    void Move(Vector2 _inputMove)
    {

        float characterVelocity = Mathf.Abs(rb.velocity.x);
        animator.SetFloat("Speed",characterVelocity);

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
    
        void Switch(bool _inputSwitch){
        
        if(_inputSwitch){
            
            if(etherMap.activeSelf){
                Debug.Log("switch to normal");
                etherMap.SetActive(false);
                normalMap.SetActive(true);
                
            }else{
                Debug.Log("switch to ether");
                etherMap.SetActive(true);
                normalMap.SetActive(false);
                
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

    void Flip(float _velocity){
        if(_velocity > 0.1f){
            spriteRenderer.flipX = true;
        }else if(_velocity < -0.1f){
            spriteRenderer.flipX = false;
        }
    }

}
