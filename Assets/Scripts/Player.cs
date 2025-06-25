using DefaultNamespace;
using UnityEngine;

/// <summary>
/// Main player class that handles initialization and core player functionality
/// </summary>
public class Player : MonoBehaviour
{
    #region Variables
    // Core components
    public Animator animator { get; private set; }     // Reference to the player's animator component
    public Rigidbody2D rb { get; private set; }       // Reference to the player's rigidbody2D component

    public PlayerInputSet input { get; private set; }
    private StateMachine _stateMachine;               // Manages player states
    private Vector2 _currentVelocity;                 // Used for SmoothDamp calculations
    #endregion

    #region Properties
    // State references
    public PlayerIdleState idleState { get; private set; }   // Player's idle state instance
    public PlayerMoveState moveState { get; private set; }   // Player's movement state instance
    public PlayerJumpState jumpState { get; private set; }   // Player's jump state instance
    public PlayerFallState fallState { get; private set; }   // Player's fall state instance
    public Vector2 moveInput { get; private set; }           // Current movement input values

    [Header("Movement details")]
    public float moveSpeed = 10f;                           // Base movement speed
    public float smoothTime = 0.1f;                         // Time to smooth movement transitions
    public float stopSmoothTime = 0.1f;                     // Time to smooth movement when no input is active
    
    [Range(0, 1)] public float inAirMoveMultiplier = 0.5f;                // Multiplier applied to movement speed when in the air
    
    [Header("Jump details")]
    public float jumpForce = 10f;                           // Force applied when jumping
    public float jumpTime = 0.4f;                           // Maximum time to jump 
    public float jumpBufferTime = 0.2f;                     // Time to wait before jumping again after landing
    private float _jumpBufferCounter;                       // Counter for jump buffer time
    public float coyoteTime = 0.2f;                         // Time since last jump
    private float _coyoteTimeCounter;                       // Counter for coyote time
    private bool _canCoyoteJump;                            // Flag to prevent double coyote jumps
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;         // Whether to draw debug lines
    private GUIStyle _debugTextStyle;                           // Style for debug lines
    
    [Header("Collision Detection")]
    [SerializeField] private float groundCheckDistance = 0.4f;  // Length of the raycast used for ground detection
    [SerializeField] private LayerMask groundLayer;             // Layer mask for ground detection
    public bool groundDetected;                                 // Whether the player is grounded
    
    private bool _facingRight = true;                           // Whether the player is facing right or left
    #endregion

    #region Unity Callback Methods
    private void Awake()
    {
        // Initialize components
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        // Set up state machine and input
        _stateMachine = new StateMachine(); 
        input = new PlayerInputSet();

        // Create state instances
        idleState = new PlayerIdleState(this, _stateMachine, "idle");
        moveState = new PlayerMoveState(this, _stateMachine, "move");
        jumpState = new PlayerJumpState(this, _stateMachine, "jumpFall");
        fallState = new PlayerFallState(this, _stateMachine, "jumpFall");
    }

    private void OnEnable()
    {
        // Enable input system
        input.Enable();

        // Setup input callbacks
        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();  // When movement input is active
        input.Player.Movement.canceled += _ => moveInput = Vector2.zero;                 // When movement input stops
    }

    private void OnDisable() => input.Disable();  // Disable the input system when the object is disabled
    
    private void Start()
    {
        // Start in an idle state
        _stateMachine.Initialize(idleState);
        // Initialize debug style
        InitDebugStyle();
    }

    private void Update()
    {
        HandleGroundDetection();
        UpdateJumpBuffer();
        UpdateCoyoteTime();
        
        // Update the current state every frame
        _stateMachine.UpdateActiveState();
    }
    #endregion
    
    #region GUI Methods
    private void InitDebugStyle()
    {
        _debugTextStyle = new GUIStyle
        {
            normal = { textColor = Color.white },
            fontSize = 16,
            fontStyle = FontStyle.Bold
        };
    }

    private void OnGUI()
    {
        if(!showDebugInfo) return;

        if (Camera.main != null)
        {
            var screenPos = Camera.main.WorldToScreenPoint(transform.position);
            var debugPosition = new Vector2(screenPos.x, Screen.height - screenPos.y);
    
            // Display jump buffer time
            GUI.Label(new Rect(debugPosition.x - 50, debugPosition.y - 60, 200, 20), 
                $"Jump Buffer: {_jumpBufferCounter:F3}", _debugTextStyle);
    
            // Display current state
            GUI.Label(new Rect(debugPosition.x - 50, debugPosition.y - 40, 200, 20), 
                $"State: {_stateMachine.currentState.GetType().Name}", _debugTextStyle);
    
            // Display ground state
            GUI.Label(new Rect(debugPosition.x - 50, debugPosition.y - 20, 200, 20), 
                $"Grounded: {groundDetected}", _debugTextStyle);
            
            // In the OnGUI method of Player.cs
            GUI.Label(new Rect(debugPosition.x - 50, debugPosition.y - 80, 200, 20), 
                $"Coyote Time: {_coyoteTimeCounter:F3}", _debugTextStyle);
            GUI.Label(new Rect(debugPosition.x - 50, debugPosition.y - 100, 200, 20), 
                $"Can Coyote Jump: {_canCoyoteJump}", _debugTextStyle);

        }
    }
    #endregion
    
    #region Methods
    #region Movement Methods
    /// <summary>
    /// Sets the HORIZONTAL velocity of the player with smooth movement transitions
    /// </summary>
    /// <param name="xVelocity">Desired horizontal velocity</param>
    /// <param name="yVelocity">Desired vertical velocity</param>
    public void SetVelocityX(float xVelocity, float yVelocity)
    {
        // Create a new Vector2 combining the desired horizontal and vertical velocities
        var targetVelocity = new Vector2(xVelocity, yVelocity);
        
        // Choose which smooth time to use:
        // - Use stopSmoothTime for quick stops when target velocity is zero
        // - Use regular smoothTime for normal movement
        var currentSmoothTime = targetVelocity == Vector2.zero ? stopSmoothTime : smoothTime;
        
        // SmoothDamp gradually changes linearVelocity towards targetVelocity:
        // - rb.linearVelocity: Current velocity
        // - targetVelocity: Desired final velocity
        // - _currentVelocity: Reference velocity (modified by SmoothDamp internally)
        // - currentSmoothTime: Time to reach target velocity
        rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref _currentVelocity, currentSmoothTime);
        
        // Update the player's facing direction based on horizontal movement
        HandleFlip(xVelocity);
    }
    
    /// <summary>
    /// Sets the VERTICAL velocity of the player with smooth movement transitions
    /// </summary>
    public void SetVelocityY(float xVelocity, float yVelocity) => rb.linearVelocity = new Vector2(xVelocity, yVelocity);
    #endregion
    
    #region Flip Character Method
    private void HandleFlip(float xVelocity)
    {
        // if moving right and currently facing left, 
        // Flip the character
        // if moving left and currently facing right,
        // Flip the character
        if (xVelocity > 0 && !_facingRight || xVelocity < 0 && _facingRight)
            Flip();
    }
    
    private void Flip()
    {
        transform.Rotate(0f, 180f, 0f);
        _facingRight = !_facingRight;
    }
    #endregion

    #region Ground Detection
    private void HandleGroundDetection() =>
        // Check if the player is grounded
        groundDetected = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, 
            groundLayer);
    #endregion
    
    #region JumpBuffer 
    private void UpdateJumpBuffer()
    {
        //If jump was pressed this frame, start the buffer timer
        if(input.Player.Jump.WasPerformedThisFrame())
        {
            _jumpBufferCounter = jumpBufferTime;
            Debug.Log($"Jump buffer started: {jumpBufferTime} seconds");
        }
        else
        {
            _jumpBufferCounter -= Time.deltaTime;
            if(_jumpBufferCounter <= 0 && _jumpBufferCounter + Time.deltaTime > 0)
                Debug.Log($"Jump buffer ended: {_jumpBufferCounter} seconds");
        }
    }
    
    public bool HasJumpBuffer() => _jumpBufferCounter > 0;
    public void ConsumeJumpBuffer() => _jumpBufferCounter = 0;
    #endregion
    
    #region CoyoteTime
    public void UpdateCoyoteTime()
    {
        if (groundDetected)
        {
            _coyoteTimeCounter = coyoteTime;
            _canCoyoteJump = true;
        }
        else _coyoteTimeCounter -= Time.deltaTime;
    }
    
    public bool CanJump() => groundDetected || (_coyoteTimeCounter > 0 && _canCoyoteJump);

    public void ConsumeJump()
    {
        _coyoteTimeCounter = 0;
        _canCoyoteJump = false;
    }
    #endregion
    #endregion
    
    #region Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
        
        // Jump Buffer visualization
        if (Application.isPlaying && _jumpBufferCounter > 0)
        {
            Gizmos.color = Color.yellow;
            float bufferRatio = _jumpBufferCounter / jumpBufferTime;
            float circleSize = 0.5f * bufferRatio;
            Gizmos.DrawWireSphere(transform.position, circleSize);
        }
    }
    #endregion
}