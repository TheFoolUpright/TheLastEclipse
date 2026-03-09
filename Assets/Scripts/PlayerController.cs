using System;
using System.Runtime.CompilerServices;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    
    private Vector2 moveInput;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private Rigidbody rb;
    private bool jumpRequested;
    private float groundDistance = 0.1f;
    private int remaingJumps = 2;
    private readonly int maxJumps = 2;

    private void OnEnable()
    {
        jumpAction.performed += OnJumpRequest;
    }

    private void OnDisable()
    {
        jumpAction.performed -= OnJumpRequest;
    }

    private void OnJumpRequest(InputAction.CallbackContext context)
    {
        jumpRequested = true;
    }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        rb = GetComponent<Rigidbody>(); 
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();

    }

    //Targetted Fps
    private void FixedUpdate()
    {
        if (IsGrounded() && rb.linearVelocity.y <= 0)
        {
            remaingJumps = maxJumps;
        }
        rb.linearVelocity = new Vector3(moveInput.x * moveSpeed, rb.linearVelocity.y , moveInput.y * moveSpeed);

        if (jumpRequested)
        {
            jumpRequested = false;
            if (remaingJumps > 0)
            {
                //rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                rb.linearVelocity = new Vector3 (rb.linearVelocity.x, jumpForce, rb.linearVelocity.y );
                remaingJumps--;
            } 
        }
    }

    private bool IsGrounded()
    {
        bool isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        return isGrounded;
    }
}
