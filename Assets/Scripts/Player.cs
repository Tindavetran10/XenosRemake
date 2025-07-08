using System.Collections;
using DefaultNamespace;
using UnityEngine;

/// <summary>
/// Main player class that handles initialization, input, and core player functionality.
/// Inherits from Entity and adds player-specific states, input handling, and gameplay logic.
/// </summary>
public class Player : Entity
{
    #region Variables
    // Reference to the player's input system (handles all input actions)
    public PlayerInputSet input { get; private set; }
    #endregion

    #region Properties
    // State references for the player's state machine
    public PlayerIdleState idleState { get; private set; }   // Player's idle state instance
    public PlayerMoveState moveState { get; private set; }   // Player's movement state instance
    public PlayerJumpState jumpState { get; private set; }   // Player's jump state instance
    public PlayerFallState fallState { get; private set; }   // Player's fall state instance
    public PlayerWallSlideState wallSlideState { get; private set; }   // Player's wall slide state instance
    public PlayerWallJumpState wallJumpState { get; private set; }   // Player's wall jump state instance
    public PlayerDashState dashState { get; private set; }   // Player's dash state instance
    public PlayerBasicAttackState basicAttackState { get; private set; }    // Player's basic attack state instance
    // Current movement input values (Vector2: x = horizontal, y = vertical)
    public Vector2 moveInput { get; private set; }

    [Header("Movement details")]
    public float moveSpeed = 10f;                           // Base movement speed
    [Range(0, 1)] public float inAirMoveMultiplier = 0.5f;  // Multiplier applied to movement speed when in the air
    [Range(0, 1)] public float wallSlideMultiplier = 0.7f;  // Multiplier applied to movement speed when wall sliding
    [Space(10)] public float dashDuration = 0.25f;    // Duration of the dash
    public float dashSpeed = 10f;                           // Speed of the dash

    [Header("Jump details")]
    public float jumpForce = 10f;                           // Force applied when jumping
    public float jumpBufferTime = 0.2f;                     // Time to wait before jumping again after landing
    private float _jumpBufferCounter;                       // Counter for jump buffer time
    public float coyoteTime = 0.2f;                         // Time since last jump
    private float _coyoteTimeCounter;                       // Counter for coyote time
    private bool _canCoyoteJump;                            // Flag to prevent double coyote jumps
    public Vector2 wallJumpForce = new(6f, 12f);            // Force applied when jumping off a wall

    [Header("Multi-Jump")]
    public int maxJumps = 2; // Settable in Inspector for double jump, triple jump, etc.
    [HideInInspector] public int currentJumps;              // Tracks current jump count

    [Header("Attack details")]
    public Vector2[] attackVelocity;                        // Array of velocities for attack combos
    public float attackVelocityDuration = 0.1f;             // Duration for which attack velocity is applied
    public float comboResetTime = 0.2f;                     // Time to reset the attack combo
    private Coroutine _queuedAttackCoroutine;               // Reference to queued attack coroutine

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;         // Whether to draw debug lines
    [SerializeField] private bool showDebugGizmos = true;       // Whether to draw debug gizmos
    private GUIStyle _debugTextStyle;                           // Style for debug lines
    #endregion

    #region Unity Callback Methods
    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes the input system and player-specific states.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        
        input = new PlayerInputSet();
        // Create state instances for the player
        idleState = new PlayerIdleState(this, StateMachine, "idle");
        moveState = new PlayerMoveState(this, StateMachine, "move");
        jumpState = new PlayerJumpState(this, StateMachine, "jumpFall");
        fallState = new PlayerFallState(this, StateMachine, "jumpFall");
        wallSlideState = new PlayerWallSlideState(this, StateMachine, "wallSlide");
        wallJumpState = new PlayerWallJumpState(this, StateMachine, "jumpFall");
        dashState = new PlayerDashState(this, StateMachine, "dash");
        basicAttackState = new PlayerBasicAttackState(this, StateMachine, "basicAttack");
    }

    /// <summary>
    /// Called when the object becomes enabled and active.
    /// Sets up input callbacks.
    /// </summary>
    private void OnEnable()
    {
        // Enable input system
        input.Enable();
        // Setup input callbacks for movement
        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();  // When movement input is active
        input.Player.Movement.canceled += _ => moveInput = Vector2.zero;                 // When movement input stops
    }

    /// <summary>
    /// Called when the object becomes disabled or inactive.
    /// Disables the input system.
    /// </summary>
    private void OnDisable() => input.Disable();

    /// <summary>
    /// Called before the first frame update.
    /// Initializes state machine and debug style.
    /// </summary>
    protected override void Start()
    {
        base.Start();
        // Start in an idle state
        StateMachine.Initialize(idleState);
        // Initialize debug style for GUI
        InitDebugStyle();
    }

    /// <summary>
    /// Called once per frame.
    /// Updates jump buffer and coyote time logic.
    /// </summary>
    protected override void Update()
    {
        base.Update();
        UpdateJumpBuffer();
        UpdateCoyoteTime();
    }
    #endregion
    
    #region Methods
    #region Attack Methods
    /// <summary>
    /// Coroutine that delays the transition to the attack state by one frame.
    /// Used to ensure animation and state sync.
    /// </summary>
    private IEnumerator EnterAttackStateWithDelayCoroutine()
    {
        // Wait until the end of the current frame before proceeding
        yield return new WaitForEndOfFrame();
        // Change the current state to the basic attack state
        StateMachine.ChangeState(basicAttackState);
    }

    /// <summary>
    /// Manages the queueing of attack state transitions.
    /// Ensures only one queued attack coroutine runs at a time.
    /// </summary>
    public void EnterAttackStateWithDelay()
    {
        // If there's already a queued attack coroutine running, stop it
        if(_queuedAttackCoroutine != null)
            StopCoroutine(_queuedAttackCoroutine);
        // Start a new coroutine to enter the attack state with a delay
        _queuedAttackCoroutine = StartCoroutine(EnterAttackStateWithDelayCoroutine());
    }
    
    /// <summary>
    /// Calls the velocity animation trigger for the current state.
    /// </summary>
    public void CallVelocityAnimationTrigger() => StateMachine.currentState.CallVelocityAnimationTrigger();
        
    /// <summary>
    /// Calls the stop velocity animation trigger for the current state.
    /// </summary>
    public void CallStopVelocityAnimationTrigger() => StateMachine.currentState.CallStopVelocityAnimationTrigger();
    
    /// <summary>
    /// Used to skip the current attack animation if there is an input during the animation.
    /// </summary>
    public void SkipCallAnimationTrigger() => StateMachine.currentState.SkipCallAnimationTrigger();
    
    /// <summary>
    /// Checks if the player should flip based on input during attack.
    /// </summary>
    public void CheckIfShouldFlipTrigger() => StateMachine.currentState.CallFlipTrigger();
    #endregion
    
    #region JumpBuffer 
    /// <summary>
    /// Updates the jump buffer timer based on input.
    /// Allows for more forgiving jump input timing.
    /// </summary>
    private void UpdateJumpBuffer()
    {
        // If jump was pressed this frame, start the buffer timer
        if (input.Player.Jump.WasPerformedThisFrame())
            _jumpBufferCounter = jumpBufferTime;
        else _jumpBufferCounter -= Time.deltaTime;
    }
    
    /// <summary>
    /// Returns true if the jump buffer is active
    /// </summary>
    public bool HasJumpBuffer() => _jumpBufferCounter > 0;
    
    /// <summary>
    /// Consumes the jump buffer (sets it to zero).
    /// </summary>
    public void ConsumeJumpBuffer() => _jumpBufferCounter = 0;
    #endregion
    
    #region CoyoteTime
    /// <summary>
    /// Updates the coyote time timer based on ground detection.
    /// Allows for more forgiving jump timing after leaving the ground.
    /// </summary>
    private void UpdateCoyoteTime()
    {
        if (groundDetected)
        {
            _coyoteTimeCounter = coyoteTime;
            _canCoyoteJump = true;
        }
        else _coyoteTimeCounter -= Time.deltaTime;
    }
    
    /// <summary>
    /// Returns true if the player can perform a coyote jump.
    /// </summary>
    public bool CanCoyoteJump() => 
        groundDetected || (_coyoteTimeCounter > 0 && _canCoyoteJump);
    
    /// <summary>
    /// Consumes the coyote jump (prevents further coyote jumps until grounded).
    /// </summary>
    public void ConsumeCoyoteJump()
    {
        _coyoteTimeCounter = 0;
        _canCoyoteJump = false;
    }
    #endregion
    #endregion
    
    #region GUI Methods
    /// <summary>
    /// Initializes the debug text style for GUI display.
    /// </summary>
    private void InitDebugStyle()
    {
        _debugTextStyle = new GUIStyle
        {
            normal = { textColor = Color.white },
            fontSize = 16,
            fontStyle = FontStyle.Bold
        };
    }

    /// <summary>
    /// Draws debug information on the screen (jump buffer, state, ground, etc.).
    /// </summary>
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
            $"State: {StateMachine.currentState.GetType().Name}", _debugTextStyle);
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
    /// <summary>
    /// Draws gizmos in the editor for debugging ground/wall checks, jump buffer, and coyote time.
    /// </summary>
    private void OnDrawGizmos()
    {
        // Draw wall check lines
        Gizmos.DrawLine(primaryGroundCheckPosition.position, primaryGroundCheckPosition.position + Vector3.right * wallCheckDistance * facingDirection);
        Gizmos.DrawLine(secondaryGroundCheckPosition.position, secondaryGroundCheckPosition.position + Vector3.right * wallCheckDistance * facingDirection);
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