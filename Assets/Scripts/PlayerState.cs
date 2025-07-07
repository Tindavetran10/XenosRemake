using DefaultNamespace;
using UnityEngine;

/// <summary>
/// Base class for all entity states, providing common state functionality
/// </summary>
public abstract class  PlayerState : EntityState
{
    private static readonly int YVelocity = Animator.StringToHash("yVelocity");

    // Protected references accessible by derived states
    protected Player Player;                // Reference to the player
    
    protected PlayerInputSet Input;
    
    protected bool SkipTriggerCalled;
    protected bool VelocityTriggerCalled;
    protected bool StopVelocityTriggerCalled;

    /// <summary>
    /// Constructor to initialize the state
    /// </summary>
    protected PlayerState(Player player, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        Player = player;
        
        // Cache commonly used components
        Anim = Player.animator;
        Rb = Player.rb;    
        Input = Player.input;
    }

    public override void Enter()
    {
        base.Enter();
        SkipTriggerCalled = false;
    }

    public override void Update()
    {
        base.Update();
        
        Anim.SetFloat(YVelocity, Rb.linearVelocity.y);

        if(Input.Player.Dash.WasPerformedThisFrame() && CanDash())
            StateMachine.ChangeState(Player.dashState);
    }
    
    public void SkipCallAnimationTrigger() => SkipTriggerCalled = true;
    public void CallVelocityAnimationTrigger() => VelocityTriggerCalled = true;
    public void CallStopVelocityAnimationTrigger() => StopVelocityTriggerCalled = true;
    public void CallFlipTrigger() => Player.HandleFlip(Player.moveInput.x);

    private bool CanDash()
    {
        if(Player.wallDetected)
            return false;
        
        return StateMachine.currentState != Player.dashState;
    }
}