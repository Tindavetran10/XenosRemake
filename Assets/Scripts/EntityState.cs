using UnityEngine;

/// <summary>
/// Base class for all entity states, providing common state functionality
/// </summary>
public abstract class  EntityState
{
    private static readonly int YVelocity = Animator.StringToHash("yVelocity");

    // Protected references accessible by derived states
    protected Player Player;                // Reference to the player
    protected StateMachine StateMachine;    // Reference to the state machine
    protected string AnimBoolName;          // Animation boolean parameter name
    
    protected Animator Anim;    // Cached animator reference
    protected Rigidbody2D Rb;
    protected PlayerInputSet Input;

    protected float StateTimer;
    
    protected bool TriggerCalled;
    protected bool SkipTriggerCalled;
    protected bool VelocityTriggerCalled;
    protected bool StopVelocityTriggerCalled;

    /// <summary>
    /// Constructor to initialize the state
    /// </summary>
    protected EntityState(Player player, StateMachine stateMachine, string animBoolName)
    {
        Player = player;
        StateMachine = stateMachine;
        AnimBoolName = animBoolName;
        
        // Cache commonly used components
        Anim = Player.animator;
        Rb = Player.rb;    
        Input = Player.input;
    }

    /// <summary>
    /// Called when entering this state
    /// </summary>
    public virtual void Enter()
    {
        TriggerCalled = false;
        SkipTriggerCalled = false;
        
        // Activate this state's animation
        Anim.SetBool(AnimBoolName, true);
    }

    /// <summary>
    /// Called every frame while in this state
    /// </summary>
    public virtual void Update()
    {
        StateTimer -= Time.deltaTime;
        Anim.SetFloat(YVelocity, Rb.linearVelocity.y);
        
        if(Input.Player.Dash.WasPerformedThisFrame() && CanDash())
            StateMachine.ChangeState(Player.dashState);
    }

    /// <summary>
    /// Called when exiting this state
    /// </summary>
    public virtual void Exit() => 
        Anim.SetBool(AnimBoolName, false); // Deactivate this state's animation


    public void CallAnimationTrigger() => TriggerCalled = true;
    public void SkipCallAnimationTrigger() => SkipTriggerCalled = true;

    public void CallVelocityAnimationTrigger() => VelocityTriggerCalled = true;
    public void CallStopVelocityAnimationTrigger() => StopVelocityTriggerCalled = true;

    private bool CanDash()
    {
        if(Player.wallDetected)
            return false;
        
        return StateMachine.currentState != Player.dashState;
    }
}