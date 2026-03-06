using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private enum MovementState
    {
        Normal,
        Crouching,
        Airborne,
        Sliding
    }

    [Header("References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform cam;
    [SerializeField] private Renderer playerRenderer;

    [Header("Base Movement")]
    [SerializeField] private float baseMoveSpeed = 6f;
    [SerializeField] private float runAcceleration = 2f;
    [SerializeField] private float maxRunSpeed = 24f;
    [SerializeField] private float turnSmoothTime = 0.1f;

    [Header("Run Grace")]
    [SerializeField] private float runGraceDuration = 0.25f;

    [Header("Jump / Gravity")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 2f;

    [Header("Crouch")]
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;

    [Header("Slide")]
    [SerializeField] private float slideBoostPercent = 0.25f;   //25% boost
    [SerializeField] private float minSlideBoost = 2f;
    [SerializeField] private float maxSlideBoost = 8f;
    [SerializeField] private float slideFriction = 10f;
    [SerializeField] private float slideTriggerSpeed = 10f;

    private MovementState _currentState;

    private float _turnSmoothVelocity;
    private float _verticalVelocity;

    private float _currentRunSpeed;
    private float _runGraceTimer;

    private Vector3 _horizontalVelocity;

    private Vector3 _slideDirection;
    private float _slideSpeed;

    private void Start()
    {
        _currentRunSpeed = baseMoveSpeed;
    }

    private void Update()
    {
        UpdateState();
        HorizontalMovement();
        VerticalMovement();
        ApplyMovement();
    }

    private void UpdateState()
    {
        var isGrounded = controller.isGrounded;
        var crouchPressed = Keyboard.current.leftCtrlKey.wasPressedThisFrame;

        if (_currentState == MovementState.Sliding)
        {
            if (_slideSpeed <= baseMoveSpeed)
                _currentState = MovementState.Normal;

            return;
        }

        if (!isGrounded)
        {
            _currentState = MovementState.Airborne;
            return;
        }

        if (crouchPressed && _currentRunSpeed >= slideTriggerSpeed)
        {
            StartSlide();
            return;
        }

        _currentState = Keyboard.current.leftCtrlKey.isPressed ? MovementState.Crouching : MovementState.Normal;
    }

    private void StartSlide()
    {
        _currentState = MovementState.Sliding;

        _slideDirection = transform.forward;

        var boost = _currentRunSpeed * slideBoostPercent;
        boost = Mathf.Clamp(boost, minSlideBoost, maxSlideBoost);

        _slideSpeed = _currentRunSpeed + boost;
    }

    private void HorizontalMovement()
    {
        if (_currentState == MovementState.Sliding)
        {
            _slideSpeed -= slideFriction * Time.deltaTime;
            _horizontalVelocity = _slideDirection * _slideSpeed;
            return;
        }

        var input = new Vector2(
            (Keyboard.current.aKey.isPressed ? -1 : 0) + (Keyboard.current.dKey.isPressed ? 1 : 0),
            (Keyboard.current.sKey.isPressed ? -1 : 0) + (Keyboard.current.wKey.isPressed ? 1 : 0)
        );

        var dir = new Vector3(input.x, 0f, input.y).normalized;

        if (dir.magnitude >= 0.1f)
        {
            _runGraceTimer = runGraceDuration;

            var targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

            var angle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref _turnSmoothVelocity,
                turnSmoothTime
            );

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            var moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            _currentRunSpeed += runAcceleration * Time.deltaTime;
            _currentRunSpeed = Mathf.Clamp(_currentRunSpeed, baseMoveSpeed, maxRunSpeed);

            var appliedSpeed = _currentRunSpeed;

            if (_currentState == MovementState.Crouching)
                appliedSpeed *= crouchSpeedMultiplier;

            _horizontalVelocity = moveDir * appliedSpeed;
        }
        else
        {
            if (_runGraceTimer > 0f)
            {
                _runGraceTimer -= Time.deltaTime;
            }
            else
            {
                _currentRunSpeed = baseMoveSpeed;
            }

            _horizontalVelocity = Vector3.zero;
        }
    }

    private void VerticalMovement()
    {
        if (controller.isGrounded && _verticalVelocity < 0)
            _verticalVelocity = -2f;

        if (_currentState != MovementState.Crouching &&
            _currentState != MovementState.Sliding &&
            Keyboard.current.spaceKey.wasPressedThisFrame &&
            controller.isGrounded)
        {
            _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        _verticalVelocity += gravity * Time.deltaTime;
    }

    private void ApplyMovement()
    {
        var velocity = _horizontalVelocity + Vector3.up * _verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }

    public void ResetSpeed()
    {
        _currentRunSpeed = baseMoveSpeed;
    }

    public void ResetMovement()
    {
        _verticalVelocity = 0f;
        _horizontalVelocity = Vector3.zero;
        _slideSpeed = 0f;
        _currentRunSpeed = baseMoveSpeed;
    }
}