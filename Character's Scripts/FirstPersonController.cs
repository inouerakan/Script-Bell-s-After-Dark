using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;


public class FirstPersonController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool CanMove { get; private set; } = true;
    private bool IsSprinting => canSprint && Keyboard.current != null && Keyboard.current[sprintKey].isPressed;
    private bool ShouldJump => canJump && characterController.isGrounded && Keyboard.current != null && Keyboard.current[jumpKey].wasPressedThisFrame;

    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;

    [Header("Controls")]
    [SerializeField] private Key sprintKey = Key.LeftShift;
    [SerializeField] private Key jumpKey = Key.Space;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    
    [SerializeField] private float sprintSpeed = 6.0f;
    

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;


    [Header("Jumping Parameters")]
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float gravity = 30.0f;


    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;



    void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (CanMove)
        {
            HandleMouseLook();
            HandleMovementInput();

            if (canJump)
            {
                HandleJump();
            }
            ApplyFinalMovement();
        }
    }

    private void HandleMovementInput()
    {
        float vertical = 0f;
        float horizontal = 0f;

        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.wKey.isPressed) vertical += 1f;
            if (keyboard.sKey.isPressed) vertical -= 1f;
            if (keyboard.aKey.isPressed) horizontal -= 1f;
            if (keyboard.dKey.isPressed) horizontal += 1f;
        }

        float speed = IsSprinting ? sprintSpeed : walkSpeed;
        currentInput = new Vector2(speed * vertical, speed * horizontal);

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
    }

    private void HandleMouseLook()
    {
        var mouse = Mouse.current;
        if (mouse != null)
        {
            Vector2 mouseDelta = mouse.delta.ReadValue();

            rotationX -= mouseDelta.y * lookSpeedY * 0.02f;
            rotationX = Mathf.Clamp(rotationX, -upperLookLimit, upperLookLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, mouseDelta.x * lookSpeedX * 0.02f, 0);
        }
    }

    private void  HandleJump()
    {
        if(ShouldJump)
        {
            moveDirection.y = jumpForce;
        }
    }
    

    private void ApplyFinalMovement()
    {
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

}
