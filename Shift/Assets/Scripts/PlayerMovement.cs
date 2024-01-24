using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
public class PlayerMovement : MonoBehaviour
{
    public float fastfallThreshold = -0.5f;
    public float coyoteTime = 0.1f;

    [SerializeField] private float jumpForce;
    [SerializeField] private float maxJumpTime;
    [SerializeField] private float preferredGroundSpeed;
    [SerializeField] private float preferredAirSpeed;
    [SerializeField] private float groundAcceleration;
    [SerializeField] private float airAcceleration;
    [SerializeField] private float groundDeceleration;
    [SerializeField] private float airDeceleration;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float etherWorldGravityScale;
    [SerializeField] private float normalWorldGravityScale;

    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private ParticleSystem respawnParticles;

    public LevelData CurrentLevelData { get; set; }

    public void ChangeLevel(LevelData levelData)
    {
        bool etherMapActive = CurrentLevelData.EtherMap.activeSelf;
        levelData.EtherMap.SetActive(etherMapActive);
        levelData.NormalMap.SetActive(!etherMapActive);
        CurrentLevelData = levelData;
    }

    private bool inputDash;
    private bool isDashing;
    private bool hasDash;
    private float dashTimeLeft;
    private bool hasJumped;
    private float jumpTimeLeft;
    private bool facingRight = true;
    private float coyoTimeLeft;
    private float switchCooldownLeft;
    private bool isDead;

    private GameObject source;

    private Camera mainCamera;
    private Animator camBackgroundAnimator;

    private Volume volume;

    public Transform groundCheckLeft;
    public Transform groundCheckRight;

    private Rigidbody2D rb;

    //Inputs

    private InputAction _moveAction;
    private InputAction _jumpAction;

    private bool inputJump;
    private Vector2 inputMove;

    private bool inputSwitch;
    private float dashCooldown;

    private Collider2D col;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        mainCamera = Camera.main;
        camBackgroundAnimator = mainCamera.GetComponentInChildren<Animator>();
        volume = GameObject.Find("EtherVolume").GetComponent<Volume>();
        source = GameObject.Find("Source_0");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Vector2.Distance(transform.position, source.transform.position) < 2f)
        {
            SceneManager.LoadScene("TeamNameScene");
        }

        if(isDead)
        {
            return;
        }

        CheckFall();

        if (IsGrounded())
        {
            jumpTimeLeft = maxJumpTime;
        }

        SetFacing();
        Dash(inputDash);
        Move(inputMove);
        MaintainJump();

        if (rb.velocity.y < -0.3f)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isLanding", true);
        }
        else if (rb.velocity.y > 0.3f)
        {
            animator.SetBool("isJumping", true);
            animator.SetBool("isLanding", false);

        }
        if (IsGrounded())
        {
            animator.SetBool("isGrounded", true);
        }
        else
        {
            animator.SetBool("isGrounded", false);
        }
    }

    private void CheckFall()
    {
        if (Physics2D.OverlapArea(groundCheckLeft.position, groundCheckRight.position, LayerMask.GetMask("Death")) || transform.position.y < -60)
        {
            Die();
        }
    }

    private void MaintainJump()
    {
        if (col.IsTouchingLayers(LayerMask.GetMask("EtherZone")))
        {
            return;
        }

        if (inputJump && rb.velocity.y > 0 && jumpTimeLeft > 0f)
        {
            rb.AddForce(new Vector2(0f, jumpForce * jumpTimeLeft), ForceMode2D.Impulse);
            jumpTimeLeft -= Time.deltaTime;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        inputMove = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        inputJump = context.ReadValueAsButton();
        if (context.started)
        {
            Jump(inputJump);
        }
    }

    public void OnSwitch(InputAction.CallbackContext context)
    {
        inputSwitch = context.ReadValueAsButton();
        if (context.started)
        {
            Debug.Log("aaa");
            Switch(inputSwitch);
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        inputDash = context.ReadValueAsButton();
    }

    void Move(Vector2 _inputMove)
    {
        float characterVelocity = Mathf.Abs(rb.velocity.x);
        animator.SetFloat("Speed", characterVelocity);
        if (isDashing)
            return;

        float drag = 0f;
        float xSpeed = Mathf.Abs(rb.velocity.x);

        if (!IsGrounded() && _inputMove.y < fastfallThreshold && rb.velocity.y < 0f)
        {
            Fastfall(true);
        }
        else
        {
            Fastfall(false);
        }

        _inputMove.y = 0f;

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

    void Switch(bool _inputSwitch)
    {
        if (_inputSwitch)
        {
            if (CurrentLevelData.EtherMap.activeSelf)
            {
                CurrentLevelData.EtherMap.SetActive(false);
                CurrentLevelData.NormalMap.SetActive(true);
                rb.gravityScale = normalWorldGravityScale;
                camBackgroundAnimator.SetBool("isEther", false);
                animator.SetBool("isEther",false);
                volume.priority = -1;
            }
            else
            {
                CurrentLevelData.EtherMap.SetActive(true);
                CurrentLevelData.NormalMap.SetActive(false);
                rb.gravityScale = etherWorldGravityScale;
                camBackgroundAnimator.SetBool("isEther", true);
                animator.SetBool("isEther",true);
                volume.priority = 2;
            }
        }
    }

    void Jump(bool _inputJump)
    {
        if (col.IsTouchingLayers(LayerMask.GetMask("EtherZone")))
        {
            //set velocity.y to 5
            rb.velocity = new Vector2(rb.velocity.x, 5);
            jumpTimeLeft = 0;
            return;
        }

        if ((isDashing || dashCooldown >= 0.3f) && IsGrounded() && _inputJump)
        {
            rb.velocity = new Vector2(3 * preferredAirSpeed * (facingRight ? 1 : -1), 0);
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            hasJumped = true;
            isDashing = false;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            dashCooldown = 0.29f;
            return;
        }

        if (_inputJump && IsGrounded())
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            hasJumped = true;
        }
    }

    void Dash(bool _inputDash)
    {
        if(CurrentLevelData.EtherMap.activeSelf)
        {
            return;
        }

        if (dashCooldown > 0f)
        {
            dashCooldown -= Time.deltaTime;
        }

        if (!hasDash && IsGrounded() && dashCooldown <= 0.2f)
        {
            hasDash = true;
        }

        if (hasDash && _inputDash && dashCooldown <= 0f)
        {
            isDashing = true;
            hasDash = false;
            rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            rb.velocity = new Vector2(facingRight ? dashSpeed : -dashSpeed, 0f);
            dashTimeLeft = dashTime;
            dashCooldown = 0.5f;
        }

        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0f)
            {
                isDashing = false;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                float newYSpeed = rb.velocity.y;
                float newXSpeed = rb.velocity.x * 0.1f;
                rb.velocity = new Vector2(newXSpeed, newYSpeed);
            }
        }
    }

    void Fastfall(bool _inputFastfall)
    {

        if (switchCooldownLeft > 0f)
            return;

        rb.gravityScale = _inputFastfall ? CurrentWorldGravityScale() * 2.5f : CurrentWorldGravityScale();
    }

    bool IsGrounded()
    {
        if (coyoTimeLeft > 0f)
        {
            coyoTimeLeft -= Time.deltaTime;
            return true;
        }
        if (Physics2D.OverlapArea(groundCheckLeft.position, groundCheckRight.position, LayerMask.GetMask("Ground")))
        {
            coyoTimeLeft = coyoteTime;
            return true;
        }

        return false;
    }

    void SetFacing()
    {
        if (inputMove.x > 0)
        {
            facingRight = true;
            spriteRenderer.flipX = true;
        }
        else if (inputMove.x < 0)
        {
            facingRight = false;
            spriteRenderer.flipX = false;
        }
    }

    float CurrentWorldGravityScale()
    {
        return (bool)(CurrentLevelData?.EtherMap.activeSelf) ? etherWorldGravityScale : normalWorldGravityScale;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other);

        if (other.gameObject.layer == LayerMask.NameToLayer("Spikes"))
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead)
            return;

        isDead = true;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        spriteRenderer.forceRenderingOff = true;

        var dp = Instantiate(deathParticles, transform.position, Quaternion.identity);
        dp.Play();
        Destroy(dp.gameObject, 5f);

        StartCoroutine(RespawnAfterDelay());
    }

    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(0.6f);
        transform.position = CurrentLevelData.PlayerSpawn.position;
        respawnParticles.Play();
        yield return new WaitForSeconds(0.3f);
        spriteRenderer.forceRenderingOff = false;
        Respawn();
    }

    void Respawn()
    {
        isDead = false;
        animator.SetBool("isDead", false);
        rb.gravityScale = normalWorldGravityScale;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        

        //set variables to default
        hasDash = false;
        hasJumped = false;
        isDashing = false;
        facingRight = true;
        dashTimeLeft = 0f;
        jumpTimeLeft = 0f;
        coyoTimeLeft = 0f;
        switchCooldownLeft = 0f;
        dashCooldown = 0f;
    }
}
