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
    public PlayerWallSlideState wallSlideState { get; private set; }   // Player's wall slide state instance
    public PlayerWallJumpState wallJumpState { get; private set; }   // Player's wall jump state instance
    public Vector2 moveInput { get; private set; }           // Current movement input values

    [Header("Movement details")]
    public float moveSpeed = 10f;                           // Base movement speed
    public float smoothTime = 0.1f;                         // Time to smooth movement transitions
    public float stopSmoothTime = 0.1f;                     // Time to smooth movement when no input is active
    
    [Range(0, 1)] public float inAirMoveMultiplier = 0.5f;                // Multiplier applied to movement speed when in the air
    [Range(0, 1)] public float wallSlideMultiplier = 0.7f;               // Multiplier applied to movement speed when wall sliding
    
    [Header("Jump details")]
    public float jumpForce = 10f;                           // Force applied when jumping
    public float jumpBufferTime = 0.2f;                     // Time to wait before jumping again after landing
    private float _jumpBufferCounter;                       // Counter for jump buffer time
    public float coyoteTime = 0.2f;                         // Time since last jump
    private float _coyoteTimeCounter;                       // Counter for coyote time
    private bool _canCoyoteJump;                            // Flag to prevent double coyote jumps
    
    public Vector2 wallJumpForce = new(6f, 12f);    // Force applied when jumping off a wall
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;         // Whether to draw debug lines
    [SerializeField] private bool showDebugGizmos = true;       // Whether to draw debug gizmos
    private GUIStyle _debugTextStyle;                           // Style for debug lines
    
    [Header("Collision Detection")]
    [SerializeField] private LayerMask groundLayer;             // Layer mask for ground detection
    [SerializeField] private float groundCheckDistance = 0.4f;  // Length of the raycast used for ground detection
    [SerializeField] private float wallCheckDistance = 0.4f;     // Length of the raycast used for wall detection
    public bool groundDetected { get; private set; }                               // Whether the player is grounded
    public bool wallDetected { get; private set; }                               // Whether the player is on a wall
    
    public int facingDirection { get; private set; } = 1;                           // 1 for right, -1 for the left
    public bool facingRight { get; private set; } = true;                           // Whether the player is facing right or left
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
        wallSlideState = new PlayerWallSlideState(this, _stateMachine, "wallSlide");
        wallJumpState = new PlayerWallJumpState(this, _stateMachine, "jumpFall");
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
        // if (moving right and currently facing left) or (moving left and currently facing right), 
        // Flip the character
        if (xVelocity > 0 && !facingRight || xVelocity < 0 && facingRight)
            Flip();
    }
    
    public void Flip()
    {
        transform.Rotate(0f, 180f, 0f);
        facingRight = !facingRight;
        facingDirection *= -1;
    }
    #endregion

    #region Environment Detection
    // Shoot a ray downward from the character position with the length of groundCheckDistance to detect the value of groundLayer
    private void HandleGroundDetection()
    {
        // Check if the player is grounded
        groundDetected = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance,
            groundLayer);
        wallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDirection, 
            wallCheckDistance, groundLayer);
    }

    #endregion
    
    #region JumpBuffer 
    private void UpdateJumpBuffer()
    {
        //If jump was pressed this frame, start the buffer timer
        if (input.Player.Jump.WasPerformedThisFrame())
            _jumpBufferCounter = jumpBufferTime;
        else _jumpBufferCounter -= Time.deltaTime;
    }
    
    public bool HasJumpBuffer() => _jumpBufferCounter > 0;
    public void ConsumeJumpBuffer() => _jumpBufferCounter = 0;
    #endregion
    
    #region CoyoteTime
    private void UpdateCoyoteTime()
    {
        if (groundDetected)
        {
            _coyoteTimeCounter = coyoteTime;
            _canCoyoteJump = true;
        }
        else _coyoteTimeCounter -= Time.deltaTime;
    }
    
    public bool CanCoyoteJump() => groundDetected || (_coyoteTimeCounter > 0 && _canCoyoteJump);

    public void ConsumeCoyoteJump()
    {
        _coyoteTimeCounter = 0;
        _canCoyoteJump = false;
    }
    #endregion
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

        if (Camera.main == null) return;
        var screenPos = Camera.main.WorldToScreenPoint(transform.position);
        var debugPosition = new Vector2(screenPos.x, Screen.height - screenPos.y);
    
        // Display jump buffer time
        GUI.Label(new Rect(debugPosition.x + 50, debugPosition.y - 60, 200, 20), 
            $"Jump Buffer: {_jumpBufferCounter:F3}", _debugTextStyle);
    
        // Display current state
        GUI.Label(new Rect(debugPosition.x + 50, debugPosition.y - 40, 200, 20), 
            $"State: {_stateMachine.currentState.GetType().Name}", _debugTextStyle);
    
        // Display ground state
        GUI.Label(new Rect(debugPosition.x + 50, debugPosition.y - 20, 200, 20), 
            $"Grounded: {groundDetected}", _debugTextStyle);
            
        GUI.Label(new Rect(debugPosition.x + 50, debugPosition.y - 80, 200, 20), 
            $"Coyote Time: {_coyoteTimeCounter:F3}", _debugTextStyle);
        GUI.Label(new Rect(debugPosition.x + 50, debugPosition.y - 100, 200, 20), 
            $"Can Coyote Jump: {_canCoyoteJump}", _debugTextStyle);
    }
    #endregion
    
    #region Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * wallCheckDistance * facingDirection);
        
        if (!showDebugGizmos) return;
        
        // Ground check visualization
        Gizmos.color = groundDetected ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
        
        // Only show if the game is started
        if (!Application.isPlaying) return;
        
        // Jump Buffer visualization
        if (_jumpBufferCounter > 0)
        {
            Gizmos.color = Color.yellow;
            var bufferRatio = _jumpBufferCounter / jumpBufferTime;
            var circleSize = 0.5f * bufferRatio;
            Gizmos.DrawWireSphere(transform.position, circleSize);
        }
        
        // Coyote time visualization
        if (_coyoteTimeCounter > 0)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f); // Orange
            var coyoteRatio = _coyoteTimeCounter / coyoteTime;
            // Draw a shrinking circle representing remaining coyote time
            Gizmos.DrawWireSphere(transform.position, 0.7f * coyoteRatio);
        }
        
        // Can coyote jump indicator
        if (!_canCoyoteJump) return;
        Gizmos.color = new Color(0f, 1f, 1f, 1f); // Cyan
        var upOffset = Vector3.up * 0.5f;
        Gizmos.DrawLine(transform.position + upOffset - Vector3.right * 0.25f,
            transform.position + upOffset + Vector3.right * 0.25f);

        
    }
    #endregion
}