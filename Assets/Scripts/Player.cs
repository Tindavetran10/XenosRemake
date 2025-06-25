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
    public Vector2 moveInput { get; private set; }          // Current movement input values

    [Header("Movement details")]
    public float moveSpeed = 10f;                           // Base movement speed
    public float smoothTime = 0.1f;                         // Time to smooth movement transitions
    public float stopSmoothTime = 0.1f;                     // Time to smooth movement when no input is active
    
    private bool _facingRight = true;                       // Whether the player is facing right or left
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
    
    private void Start() => _stateMachine.Initialize(idleState);  // Start in an idle state
    
    private void Update() => _stateMachine.UpdateActiveState();   // Update the current state every frame
    #endregion
    
    #region Methods
    /// <summary>
    /// Sets the velocity of the player with smooth movement transitions
    /// </summary>
    /// <param name="xVelocity">Desired horizontal velocity</param>
    /// <param name="yVelocity">Desired vertical velocity</param>
    public void SetVelocity(float xVelocity, float yVelocity)
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
}