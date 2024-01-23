using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float moveSpeed;
    [SerializeField] public float jumpForce;
    [SerializeField] public Rigidbody2D rb;

    private bool isJumping;
    private bool isGrounded;

    public Transform groundCheckLeft;
    public Transform groundCheckRight;

    private Vector3 velocity = Vector3.zero;

    //Inputs

    private InputAction _moveAction;
    private InputAction _jumpAction;

    private bool inputJump;
    private Vector2 inputMove;



    // Update is called once per frame
    void FixedUpdate()
    {
                

        isGrounded = Physics2D.OverlapArea(groundCheckLeft.position, groundCheckRight.position);

        float horizontalMvmt = Input.GetAxis("Horizontal") *moveSpeed * Time.deltaTime;

        Move(inputMove);
        Jump(inputJump);
        
        /*if((KeyCode.Space) && isGrounded==true)// empeche le jump dans les airs
        {
            isJumping =true;
        }
        MovingPlayer(horizontalMvmt);*/

    }

    /*void MovingPlayer(float _horizontalMvmt)
    {
        Vector3 targetVelocity = new Vector2(_horizontalMvmt,rb.velocity.y);
        rb.velocity= Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity,0.05F);

        if(isJumping==true){
            rb.AddForce(new Vector2(0f,jumpForce));
            isJumping=false;
            
        }
    }*/
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
        Vector2 velocity = _inputMove * moveSpeed;

        Vector3 moveVelocity = transform.right * velocity.x + transform.forward * velocity.y;
        moveVelocity.y = rb.velocity.y;

        rb.velocity = moveVelocity;
    }

    void Jump(bool _inputJump)
    {
        Debug.Log(isGrounded);
        if(_inputJump && isGrounded==true)
        {
            rb.AddForce(new Vector2(0f,jumpForce));
            _inputJump=false;
        }
    }
}
