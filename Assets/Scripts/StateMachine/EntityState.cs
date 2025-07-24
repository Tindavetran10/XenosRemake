using UnityEngine;

namespace Scripts
{
    /// <summary>
    /// Abstract base class for all entity states (e.g., idle, move, attack).
    /// Provides a template for state transitions, animation handling, and state timing.
    /// Intended to be inherited by specific state implementations for entities.
    /// </summary>
    public abstract class EntityState
    {
        // Reference to the state machine managing this state
        protected StateMachine StateMachine;
        // Name of the animation boolean parameter for this state
        protected string AnimBoolName;
        // Cached reference to the entity's Animator component
        protected Animator Anim;
        // Cached reference to the entity's Rigidbody2D component
        protected Rigidbody2D Rb;
        // Timer for tracking time spent in this state (can be used for delays, cooldowns, etc.)
        protected float StateTimer;
        // Flag to indicate if an animation trigger has been called
        protected bool TriggerCalled;
        
        protected bool VelocityTriggerCalled;
        protected bool StopVelocityTriggerCalled;

        
        #region Attack Details
        protected const int FirstComboIndex = 1; // We start combo Index with 1, this parameter is used in the Animator
        protected int ComboLimit = 3;
        protected int ComboIndex = 1;
        #endregion
        
        /// <summary>
        /// Constructor to initialize the state with its state machine and animation bool name.
        /// </summary>
        /// <param name="stateMachine">The state machine managing this state</param>
        /// <param name="animBoolName">The name of the animation boolean parameter for this state</param>
        protected EntityState(StateMachine stateMachine, string animBoolName)
        {
            StateMachine = stateMachine;
            AnimBoolName = animBoolName;
        }
        
        /// <summary>
        /// Called when entering this state. Resets triggers and activates the state's animation.
        /// </summary>
        public virtual void Enter()
        {
            TriggerCalled = false;
            // Activate this state's animation
            Anim.SetBool(AnimBoolName, true);
        }

        /// <summary>
        /// Called every frame while in this state. Decrements the state timer.
        /// </summary>
        public virtual void Update()
        {
            StateTimer -= Time.deltaTime;
            UpdateAnimationParameters();
        }

        /// <summary>
        /// Called when exiting this state. Deactivates the state's animation.
        /// </summary>
        public virtual void Exit() => 
            Anim.SetBool(AnimBoolName, false); // Deactivate this state's animation
        
        /// <summary>
        /// Sets the animation trigger flag, which can be used by derived states to respond to animation events.
        /// </summary>
        public void AnimationTrigger() => TriggerCalled = true;
        
        /// <summary>
        /// Sets the velocity animation trigger flag (used for velocity-based animation events).
        /// </summary>
        public void CallVelocityAnimationTrigger() => VelocityTriggerCalled = true;
        /// <summary>
        /// Sets the stop velocity animation trigger flag (used to stop velocity-based animation events).
        /// </summary>
        public void CallStopVelocityAnimationTrigger() => StopVelocityTriggerCalled = true;

        protected virtual void UpdateAnimationParameters() {}
    }
}