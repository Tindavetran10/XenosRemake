
/// <summary>
/// Class for all state of when the player falling
/// </summary>
public class PlayerFallState : PlayerAirState
{
    public PlayerFallState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName) { }

    public override void Update()
    {
        base.Update();

        // Change back to idle when the player is on the ground
        if (Player.GroundDetected)
        {
            StateMachine.ChangeState(Player.IdleState);
            return;
        }

        // Allow jumping during coyote time or if extra jumps remain
        if (Input.Player.Jump.WasPerformedThisFrame() && (Player.CanCoyoteJump() || Player.currentJumps < Player.maxJumps))
        {
            if (Player.CanCoyoteJump()) Player.ConsumeCoyoteJump();
            StateMachine.ChangeState(Player.JumpState);
            return;
        }

        // Check for the opposite wall during wall jump
        if (!Player.WallDetected) return;
        // If the player presses Jump while detecting a wall
        if (Input.Player.Jump.WasPerformedThisFrame())
        {
            // Change to wall jump state
            StateMachine.ChangeState(Player.WallJumpState);
            return;
        }
        // If there is no jump input, change to wall slide state
        StateMachine.ChangeState(Player.WallSlideState);
    }
}
