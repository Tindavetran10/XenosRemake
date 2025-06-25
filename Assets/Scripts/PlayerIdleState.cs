using DefaultNamespace;

/// <summary>
/// Handles player idle state logic
/// </summary>
public class PlayerIdleState : PlayerGroundState
{
    public PlayerIdleState(Player player, StateMachine stateMachine, string animBoolName) 
        : base(player, stateMachine, animBoolName) {}

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        Player.SetVelocity(0, 0);  // Stop all movement when entering an idle state
        
        // Check if you should transition to a move state
        if (Player.moveInput.x != 0)
            StateMachine.ChangeState(Player.moveState);
    }
}