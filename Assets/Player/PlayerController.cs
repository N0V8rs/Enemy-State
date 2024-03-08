using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f; 
    [SerializeField] private float jumpForce = 8.0f;
    private float currentSpeed;

    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private float upDownLimit = 65f;

    private float verticalRotation;
    private Camera playerCamera;
    private Vector3 currentMovement = Vector3.zero;
    private CharacterController characterController;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        currentSpeed = walkSpeed;
    }

    void Update()
    {
        HandleMovement();
        HandleLook();

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            currentSpeed = sprintSpeed;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            currentSpeed = walkSpeed;
        }
    }

    void HandleMovement()
    {
        Vector3 horizontalMovement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        horizontalMovement = transform.rotation * horizontalMovement;

        currentMovement.x = horizontalMovement.x * currentSpeed;
        currentMovement.z = horizontalMovement.z * currentSpeed;

        currentMovement.y += Physics.gravity.y * Time.deltaTime;

        characterController.Move(currentMovement * Time.deltaTime);
    }

    void HandleLook()
    {
        float mouseXRotation = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0, mouseXRotation, 0);

        verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownLimit, upDownLimit);

        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    void Jump()
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * jumpForce);
        }
    }
}
