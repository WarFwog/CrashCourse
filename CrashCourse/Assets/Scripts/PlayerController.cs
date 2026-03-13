using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.HID.HID;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private enum MovementState
    {
        Normal,
        Crouching,
        Airborne,
        Sliding,
        Knockback
    }

    [Header("Mobile Controls")]
    [SerializeField] private Joystick joystick;           // ← drag your Joystick here

    [Header("UI Buttons")]
    [SerializeField] private Button jumpButton;           // ← optional reference
    [SerializeField] private Button crouchButton;         // ← optional reference

    [Header("References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform cam;
    [SerializeField] private new Renderer renderer;

    public PlayerController(Renderer renderer)
    {
        this.renderer = renderer;
    }

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
    
    private Vector3 _knockbackVelocity;
    private float _knockbackTimer;

    private MovementState _currentState;
    
    private bool _useJoystick = true;
    private float _turnSmoothVelocity;
    private float _verticalVelocity;

    private float _currentRunSpeed;
    private float _runGraceTimer;

    private Vector3 _horizontalVelocity;

    private Vector3 _slideDirection;
    private float _slideSpeed;

    private Animator _animator;
    private bool _isGrounded;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _currentRunSpeed = baseMoveSpeed;

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
        if (_currentState == MovementState.Knockback)
            return;
        
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
        else
        {
            _animator.SetBool("isCrouching", false);
        }

        if (Keyboard.current.leftCtrlKey.isPressed)
        {
            _currentState = MovementState.Crouching;
            _animator.SetBool("isCrouching", true);
        }
        else
        {
            _currentState = MovementState.Normal;
            _animator.SetBool("isCrouching", false);
        }


    }

    private void StartSlide()
    {
        _currentState = MovementState.Sliding;

        _slideDirection = transform.forward;

        var boost = _currentRunSpeed * slideBoostPercent;
        boost = Mathf.Clamp(boost, minSlideBoost, maxSlideBoost);

        _slideSpeed = _currentRunSpeed + boost;
    }

    private bool _jumpRequested = false;
    private bool _crouchHeld = false;           // true while finger is down on crouch button
    private bool _previousCrouchInput = false;  // used to detect "just pressed" for slide

   

    private void HorizontalMovement()
    {
        if (_currentState == MovementState.Knockback)
        {
            _horizontalVelocity = _knockbackVelocity;

            _knockbackTimer -= Time.deltaTime;

            if (_knockbackTimer <= 0f)
            {
                _currentState = MovementState.Normal;
            }

            return;
        }
        _animator.SetBool("isMoving", true);
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
            _animator.SetBool("isMoving", false);
            if (_runGraceTimer > 0f)
            {
                _animator.SetBool("isRunning", true);
                _runGraceTimer -= Time.deltaTime;
            }
            else
            {
                _animator.SetBool("isRunning", false);
                _currentRunSpeed = baseMoveSpeed;
            }

            _horizontalVelocity = Vector3.zero;
        }
    }

    private void VerticalMovement()
    {
        if (controller.isGrounded && _verticalVelocity < 0)
            _verticalVelocity = -2f;
        _animator.SetBool("isJumping", false);


        if (_currentState != MovementState.Crouching &&
            _currentState != MovementState.Sliding &&
            Keyboard.current.spaceKey.wasPressedThisFrame &&
            controller.isGrounded)
            


        {
            _animator.SetBool("isJumping", true);

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
    public void ApplyKnockback(Vector3 direction, float force, float knockbackDuration)
    {
        _currentState = MovementState.Knockback;

        _knockbackVelocity = (direction.normalized + Vector3.up * 0.35f) * force;

        _verticalVelocity = 0f;

        _knockbackTimer = knockbackDuration;

        ResetSpeed();
    }
}