using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Walk,
        Jump,
        Dead
    }

    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;
    public static PlayerState state;

    Vector3 moveDirection;

    Rigidbody rb;
    public Animator _animatorController;
    public Collider enemyCollider;
    public int corpseDuration;
    public static GameObject underPlayer;

    private void Start()
    {
        _animatorController = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        state = PlayerState.Idle;
        readyToJump = true;
    }

    private void Update()
    {
        RaycastHit hit;
        grounded = Physics.Raycast(transform.position, Vector3.down, out hit, playerHeight * 0.5f + 0.3f, whatIsGround);
        if(grounded == true)
        {
            underPlayer = hit.collider.gameObject;
        }

        MyInput();
        SpeedControl();

        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        if (state != PlayerState.Dead)
        {
            MovePlayer();
        }
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        
        if(grounded)
        {
            state = moveDirection == Vector3.zero ? PlayerState.Idle : PlayerState.Walk;
            if(state == PlayerState.Walk || state == PlayerState.Jump)
            {
                _animatorController.SetBool("moving", true);
            }
            else
            {
                _animatorController.SetBool("moving", false);
            }

            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if(!grounded)
        {
            state = PlayerState.Jump;
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        _animatorController.SetTrigger("jump");
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((enemyCollider.gameObject.name == other.gameObject.name && state != PlayerState.Dead && grounded && underPlayer.name != "StoneHead" 
            && (EnemyMovement.enemyState != EnemyMovement.EnemyState.Dead || EnemyMovement.enemyState != EnemyMovement.EnemyState.RunAway)) 
            || (other.transform.parent != null && other.transform.parent.gameObject.name == "DamagesPlayer"))
        {
            _animatorController.SetTrigger("Death");
            state = PlayerState.Dead;

            LoadScene("MainMenuScene");
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(nameof(LoadSceneAfterSecond), sceneName);
    }

    private IEnumerator LoadSceneAfterSecond(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName) == false)
        {
            yield return new WaitForSeconds(corpseDuration);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(sceneName);
        }
    }
}