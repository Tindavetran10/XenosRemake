using Scripts.PlayerStates;

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
        if (Player.MoveInput.x == 0 || Player.WallDetected)
            StateMachine.ChangeState(Player.IdleState);
        
        // Calculate and apply movement
        var targetXVelocity = Player.MoveInput.x * Player.moveSpeed;  // Calculate horizontal velocity based on input
        Player.SetVelocityX(targetXVelocity, Rb.linearVelocity.y);    // Apply movement while preserving vertical velocity
    }
}