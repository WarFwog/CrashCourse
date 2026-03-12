using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;                    // ← added for Button reference (optional but clean)

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
    
    [Header("Mobile Controls")]
    [SerializeField] private Joystick joystick;           // ← drag your Joystick here
    
    [Header("UI Buttons")]
    [SerializeField] private Button jumpButton;           // ← optional reference
    [SerializeField] private Button crouchButton;         // ← optional reference

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
    [SerializeField] private float slideBoostPercent = 0.25f;
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

    private bool _useJoystick = true;

    // ────────────────────────────── NEW: UI Button flags ──────────────────────────────
    private bool _jumpRequested = false;
    private bool _crouchHeld = false;           // true while finger is down on crouch button
    private bool _previousCrouchInput = false;  // used to detect "just pressed" for slide

    private void Start()
    {
        _currentRunSpeed = baseMoveSpeed;
        _useJoystick = true;   // change to false if you only want keyboard

        if (joystick == null)
        {
            Debug.LogWarning("Joystick not assigned!");
            _useJoystick = false;
        }
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

        // ────────────────────────────── CROUCH INPUT (keyboard OR UI button) ──────────────────────────────
        bool currentCrouchInput = Keyboard.current.leftCtrlKey.isPressed || _crouchHeld;
        bool crouchPressedThisFrame = currentCrouchInput && !_previousCrouchInput;

        if (_currentState == MovementState.Sliding)
        {
            if (_slideSpeed <= baseMoveSpeed)
                _currentState = MovementState.Normal;

            _previousCrouchInput = currentCrouchInput;
            return;
        }

        if (!isGrounded)
        {
            _currentState = MovementState.Airborne;
            _previousCrouchInput = currentCrouchInput;
            return;
        }

        // Slide trigger (works with both keyboard press AND UI button press)
        if (crouchPressedThisFrame && _currentRunSpeed >= slideTriggerSpeed)
        {
            StartSlide();
            _previousCrouchInput = currentCrouchInput;
            return;
        }

        _currentState = currentCrouchInput ? MovementState.Crouching : MovementState.Normal;

        _previousCrouchInput = currentCrouchInput;
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

        Vector2 input = Vector2.zero;

        if (_useJoystick && joystick != null)
        {
            input = new Vector2(joystick.Horizontal, joystick.Vertical);
        }
        else
        {
            float h = 0f;
            float v = 0f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)  h -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) h += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)  v -= 1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)    v += 1f;
            input = new Vector2(h, v);
        }

        if (input.magnitude < 0.12f) input = Vector2.zero;

        var dir = new Vector3(input.x, 0f, input.y).normalized;

        if (dir.magnitude >= 0.1f)
        {
            _runGraceTimer = runGraceDuration;

            var targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, turnSmoothTime);

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
                _runGraceTimer -= Time.deltaTime;
            else
                _currentRunSpeed = baseMoveSpeed;

            _horizontalVelocity = Vector3.zero;
        }
    }

    private void VerticalMovement()
    {
        if (controller.isGrounded && _verticalVelocity < 0)
            _verticalVelocity = -2f;

        bool jumpPressed = Keyboard.current.spaceKey.wasPressedThisFrame || _jumpRequested;

        if (_currentState != MovementState.Crouching &&
            _currentState != MovementState.Sliding &&
            jumpPressed &&
            controller.isGrounded)
        {
            _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            _jumpRequested = false;   // consume the request
        }

        _verticalVelocity += gravity * Time.deltaTime;
    }

    private void ApplyMovement()
    {
        var velocity = _horizontalVelocity + Vector3.up * _verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }

    // ────────────────────────────── PUBLIC METHODS FOR UI BUTTONS ──────────────────────────────
    public void RequestJump()
    {
        _jumpRequested = true;
    }

    public void OnCrouchPointerDown()
    {
        _crouchHeld = true;
    }

    public void OnCrouchPointerUp()
    {
        _crouchHeld = false;
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
