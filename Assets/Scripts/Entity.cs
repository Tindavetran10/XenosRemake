using UnityEngine;

namespace Scripts
{
    /// <summary>
    /// Base class for all entities in the game (e.g., Player, Enemy).
    /// Provides core movement, state management, and environment detection logic.
    /// Intended to be inherited and extended by specific entity types.
    /// </summary>
    public class Entity : MonoBehaviour
    {
        #region Variables
        // Animator component for controlling entity animations
        public Animator animator { get; private set; }
        // Rigidbody2D component for physics-based movement
        public Rigidbody2D rb { get; private set; }
        // State machine to manage entity states (idle, moving, attacking, etc.)
        protected StateMachine StateMachine;
        // Current facing direction: 1 for right, -1 for the left
        public int facingDirection { get; private set; } = 1;
        // Whether the entity is currently facing right
        public bool facingRight { get; private set; } = true;
        // Used for smooth movement transitions (SmoothDamp reference velocity)
        private Vector2 _currentVelocity;
        // Time to smooth movement transitions
        public float smoothTime = 0.1f;
        // Time to smooth movement when no input is active
        public float stopSmoothTime = 0.1f;

        [Header("Collision Detection")]
        // Layer mask for ground detection
        [SerializeField] private LayerMask groundLayer;
        // Length of the raycast used for ground detection
        [SerializeField] protected float groundCheckDistance = 0.4f;
        // Primary position to check for ground (usually under the feet)
        [SerializeField] protected Transform primaryGroundCheckPosition;
        // Secondary position to check for ground (for more robust detection)
        [SerializeField] protected Transform secondaryGroundCheckPosition;
        // Layer mask for wall detection
        [SerializeField] private LayerMask wallLayer;
        // Length of the raycast used for wall detection
        [SerializeField] protected float wallCheckDistance = 0.4f;
        // Whether the entity is currently grounded
        public bool groundDetected { get; private set; }
        // Whether the entity is currently touching a wall
        public bool wallDetected { get; private set; }
        #endregion

        #region Unity Callback Methods
        /// <summary>
        /// Called when the script instance is being loaded.
        /// Initializes core components and the state machine.
        /// </summary>
        protected virtual void Awake()
        {
            // Get references to required components
            animator = GetComponentInChildren<Animator>();
            rb = GetComponent<Rigidbody2D>();
            // Initialize the state machine (states are added in derived classes)
            StateMachine = new StateMachine(); 
        }

        /// <summary>
        /// Called before the first frame update.
        /// Can be overridden by derived classes for custom startup logic.
        /// </summary>
        protected virtual void Start() {}

        /// <summary>
        /// Called once per frame.
        /// Handles environment detection and updates the current state.
        /// </summary>
        protected virtual void Update()
        {
            HandleGroundDetection();
            HandleWallDetection();
            // Update the current state every frame
            StateMachine.UpdateActiveState();
        }
        #endregion
        
        #region Methods
        #region Movement Methods
        /// <summary>
        /// Sets the HORIZONTAL velocity of the entity with smooth movement transitions.
        /// Uses SmoothDamp for natural acceleration and deceleration.
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
            // SmoothDamp gradually changes linearVelocity towards targetVelocity
            rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref _currentVelocity, currentSmoothTime);
            // Update the entity's facing direction based on horizontal movement
            HandleFlip(xVelocity);
        }

        /// <summary>
        /// Sets the VERTICAL velocity of the entity directly (no smoothing).
        /// </summary>
        /// <param name="xVelocity">Desired horizontal velocity</param>
        /// <param name="yVelocity">Desired vertical velocity</param>
        public void SetVelocityY(float xVelocity, float yVelocity) => rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        #endregion
        
        #region Flip Methods
        /// <summary>
        /// Checks if the entity should flip based on the movement direction and calls Flip() if needed.
        /// </summary>
        /// <param name="xVelocity">Current horizontal velocity</param>
        public void HandleFlip(float xVelocity)
        {
            // If moving right and facing left, or moving left and facing right, flip the entity
            if (xVelocity > 0 && !facingRight || xVelocity < 0 && facingRight)
                Flip();
        }
        
        /// <summary>
        /// Flips the entity's facing direction (rotates 180 degrees on Y axis).
        /// </summary>
        public virtual void Flip()
        {
            transform.Rotate(0f, 180f, 0f);
            facingRight = !facingRight;
            facingDirection *= -1;
        }
        #endregion
        
        #region Animation Trigger Methods
        /// <summary>
        /// Calls the animation trigger for the current state (delegated to the state machine).
        /// </summary>
        public void CallAnimationTrigger() => StateMachine.currentState.CallAnimationTrigger();
        #endregion

        #region Environment Detection
        /// <summary>
        /// Checks if the entity is grounded by casting a ray downward.
        /// Updates the groundDetected property.
        /// </summary>
        private void HandleGroundDetection() => 
            groundDetected = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);

        /// <summary>
        /// Checks if the entity is touching a wall by casting rays from two positions.
        /// Updates the wallDetected property.
        /// </summary>
        private void HandleWallDetection()
        {
            wallDetected = Physics2D.Raycast(primaryGroundCheckPosition.position, Vector2.right * facingDirection, 
                               wallCheckDistance, wallLayer) 
                           && Physics2D.Raycast(secondaryGroundCheckPosition.position, Vector2.right * facingDirection, 
                               wallCheckDistance, wallLayer); 
        }
        #endregion
        #endregion

        
    }
}