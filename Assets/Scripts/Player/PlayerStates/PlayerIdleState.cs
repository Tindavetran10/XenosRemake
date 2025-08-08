using UnityEngine;

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
        Player.SetVelocityX(0, Rb.linearVelocity.y);  // Stop horizontal movement when entering an idle state
    }

    public override void Update()
    {
        base.Update();
        
        // Remove the redundant SetVelocityX call since it's already done in Enter
        // Only set velocity if there's drift or external forces
        if (Mathf.Abs(Rb.linearVelocity.x) > 0.01f)
            Player.SetVelocityX(0, Rb.linearVelocity.y);
        
        if(Mathf.Approximately(Player.MoveInput.x, Player.FacingDirection) && Player.WallDetected)
            return;
        
        // Check if you should transition to a move state
        if (Player.MoveInput.x != 0)
            StateMachine.ChangeState(Player.MoveState);
    }
}