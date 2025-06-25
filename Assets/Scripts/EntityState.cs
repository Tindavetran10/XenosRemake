using UnityEngine;

/// <summary>
/// Base class for all entity states, providing common state functionality
/// </summary>
public abstract class EntityState
{
    // Protected references accessible by derived states
    protected Player Player;                // Reference to the player
    protected StateMachine StateMachine;    // Reference to the state machine
    protected string AnimBoolName;          // Animation boolean parameter name
    
    protected readonly Animator Animator;    // Cached animator reference
    protected readonly Rigidbody2D Rb;
    protected readonly PlayerInputSet Input;

    /// <summary>
    /// Constructor to initialize the state
    /// </summary>
    protected EntityState(Player player, StateMachine stateMachine, string animBoolName)
    {
        Player = player;
        StateMachine = stateMachine;
        AnimBoolName = animBoolName;
        
        // Cache commonly used components
        Animator = Player.animator;
        Rb = Player.rb;    
        Input = Player.input;
    }

    /// <summary>
    /// Called when entering this state
    /// </summary>
    public virtual void Enter() => 
        Animator.SetBool(AnimBoolName, true); // Activate this state's animation

    /// <summary>
    /// Called every frame while in this state
    /// </summary>
    public virtual void Update()
    {
        
    }

    /// <summary>
    /// Called when exiting this state
    /// </summary>
    public virtual void Exit() => 
        Animator.SetBool(AnimBoolName, false); // Deactivate this state's animation
}