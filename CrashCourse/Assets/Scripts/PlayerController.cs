using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private enum MovementState
    {
        Normal,
        Crouching,
        Airborne
    }

    [Header("References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform cam;
    [SerializeField] private Renderer playerRenderer;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;
    [SerializeField] private float turnSmoothTime = 0.1f;

    [Header("Jump / Gravity")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 2f;

    [Header("Crouch Settings")]
    [SerializeField] private float normalHeight = 2f;
    [SerializeField] private float crouchHeight = 1f;

    private float _turnSmoothVelocity;
    private Vector3 _horizontalVelocity;
    private float _verticalVelocity;
    private MovementState _currentState;

    private void Update()
    {
        UpdateState();
        HorizontalMovement();
        VerticalMovement();
        ApplyMovement();
    }

    private void UpdateState()
    {
        bool isGrounded = controller.isGrounded;
        bool crouchPressed = Keyboard.current.leftCtrlKey.isPressed;

        if (!isGrounded)
        {
            _currentState = MovementState.Airborne;
        }
        else if (crouchPressed)
        {
            _currentState = MovementState.Crouching;
        }
        else
        {
            _currentState = MovementState.Normal;
        }

        ApplyCrouchSettings();
    }

    private void ApplyCrouchSettings()
    {
        if (_currentState == MovementState.Crouching)
        {
            controller.height = crouchHeight;
            controller.center = new Vector3(0, crouchHeight / 2f, 0);

            if (playerRenderer != null)
                playerRenderer.material.color = Color.blue;
        }
        else
        {
            controller.height = normalHeight;
            controller.center = new Vector3(0, normalHeight / 2f, 0);

            if (playerRenderer != null)
                playerRenderer.material.color = Color.white;
        }
    }

    private void HorizontalMovement()
    {
        Vector2 input = Keyboard.current != null
            ? new Vector2(
                (Keyboard.current.aKey.isPressed ? -1 : 0) + (Keyboard.current.dKey.isPressed ? 1 : 0),
                (Keyboard.current.sKey.isPressed ? -1 : 0) + (Keyboard.current.wKey.isPressed ? 1 : 0)
              )
            : Vector2.zero;

        Vector3 dir = new Vector3(input.x, 0f, input.y).normalized;

        float currentSpeed = moveSpeed;

        if (_currentState == MovementState.Crouching)
            currentSpeed *= crouchSpeedMultiplier;

        if (dir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref _turnSmoothVelocity,
                turnSmoothTime
            );

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            _horizontalVelocity = moveDir * currentSpeed;
        }
        else
        {
            _horizontalVelocity = Vector3.zero;
        }
    }

    private void VerticalMovement()
    {
        if (controller.isGrounded && _verticalVelocity < 0)
        {
            _verticalVelocity = -2f;
        }

        if (_currentState != MovementState.Crouching &&
            Keyboard.current.spaceKey.wasPressedThisFrame &&
            controller.isGrounded)
        {
            _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        _verticalVelocity += gravity * Time.deltaTime;
    }

    private void ApplyMovement()
    {
        Vector3 velocity = _horizontalVelocity + Vector3.up * _verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }
}