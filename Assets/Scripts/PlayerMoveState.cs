using DefaultNamespace;

/// <summary>
/// Handles player movement state logic
/// </summary>
public class PlayerMoveState : PlayerGroundState
{
    public PlayerMoveState(Player player, StateMachine stateMachine, string animBoolName) 
        : base(player, stateMachine, animBoolName) {}

    public override void Update()
    {
        base.Update();

        // Check if you should transition to an idle state
        if (Player.moveInput.x == 0 || Player.wallDetected)
            StateMachine.ChangeState(Player.idleState);
        
        // Calculate and apply movement
        var targetXVelocity = Player.moveInput.x * Player.moveSpeed;  // Calculate horizontal velocity based on input
        Player.SetVelocityX(targetXVelocity, Rb.linearVelocity.y);    // Apply movement while preserving vertical velocity
    }
}