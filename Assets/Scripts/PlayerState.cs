using DefaultNamespace;
using UnityEngine;

/// <summary>
/// Abstract base class for all player-specific states (e.g., player idle, move, jump).
/// Inherits from EntityState and adds player-specific logic, input handling, and state transitions.
/// </summary>
public abstract class PlayerState : EntityState, IPlayerAnimationTriggers
{
    // Animator parameter hash for vertical velocity (used for animation blending)
    private static readonly int YVelocity = Animator.StringToHash("yVelocity");

    // Reference to the Player instance (provides access to player-specific data and methods)
    protected Player Player;
    // Reference to the player's input system
    protected PlayerInputSet Input;
    // Flags for various animation triggers and state transitions
    protected bool SkipTriggerCalled;
    protected bool VelocityTriggerCalled;
    protected bool StopVelocityTriggerCalled;

    /// <summary>
    /// Constructor to initialize the player state with references to the player, state machine, and animation bool name.
    /// </summary>
    /// <param name="player">The Player instance this state belongs to</param>
    /// <param name="stateMachine">The state machine managing this state</param>
    /// <param name="animBoolName">The name of the animation boolean parameter for this state</param>
    protected PlayerState(Player player, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        Player = player;
        // Cache commonly used components for performance and convenience
        Anim = Player.animator;
        Rb = Player.rb;
        Input = Player.input;
    }

    /// <summary>
    /// Called when entering this state. Resets triggers and calls base logic.
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        SkipTriggerCalled = false;
    }

    /// <summary>
    /// Called every frame while in this state. Updates animation parameters and handles dash input.
    /// </summary>
    public override void Update()
    {
        base.Update();
        // Update the animator with the current vertical velocity for animation blending
        Anim.SetFloat(YVelocity, Rb.linearVelocity.y);
        // If dash input is pressed and dashing is allowed, transition to dash state
        if(Input.Player.Dash.WasPerformedThisFrame() && CanDash())
            StateMachine.ChangeState(Player.dashState);
    }
    
    /// <summary>
    /// Sets the skip animation trigger flag (used to skip current animation if needed).
    /// </summary>
    public void SkipCallAnimationTrigger() => SkipTriggerCalled = true;
    /// <summary>
    /// Sets the velocity animation trigger flag (used for velocity-based animation events).
    /// </summary>
    public void CallVelocityAnimationTrigger() => VelocityTriggerCalled = true;
    /// <summary>
    /// Sets the stop velocity animation trigger flag (used to stop velocity-based animation events).
    /// </summary>
    public void CallStopVelocityAnimationTrigger() => StopVelocityTriggerCalled = true;
    /// <summary>
    /// Calls the player's flip logic based on current movement input (used for flipping the sprite direction).
    /// </summary>
    public void CallFlipTrigger() => Player.HandleFlip(Player.moveInput.x);

    /// <summary>
    /// Determines if the player can dash (not on a wall and not already dashing).
    /// </summary>
    /// <returns>True if dashing is allowed, false otherwise.</returns>
    private bool CanDash()
    {
        if(Player.wallDetected)
            return false;
        return StateMachine.currentState != Player.dashState;
    }
}

public interface IPlayerAnimationTriggers
{
    void CallVelocityAnimationTrigger();
    void CallStopVelocityAnimationTrigger();
    void SkipCallAnimationTrigger();
    void CallFlipTrigger();
}