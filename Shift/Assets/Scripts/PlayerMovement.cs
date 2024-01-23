using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float moveSpeed;
    [SerializeField] public float jumpForce;
    [SerializeField] public Rigidbody2D rb;

    
    [SerializeField] public GameObject normalMap;
    [SerializeField] public GameObject etherMap;

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

    private bool inputSwitch;

    void Start(){
        etherMap.SetActive(false);
    }


    // Update is called once per frame
    void FixedUpdate()
    {
                

        isGrounded = Physics2D.OverlapArea(groundCheckLeft.position, groundCheckRight.position);

        float horizontalMvmt = Input.GetAxis("Horizontal") *moveSpeed * Time.deltaTime;

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

    public void OnSwitch(InputAction.CallbackContext context)
    {
       inputSwitch = context.ReadValueAsButton();
       if(context.started){
            Switch(inputSwitch);
       }
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
        
        if(_inputJump && isGrounded==true)
        {
            rb.AddForce(new Vector2(0f,jumpForce));
            _inputJump=false;
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
}
