namespace DefaultNamespace
{
    // This class handles the state when a player is sliding along a wall
    public class PlayerWallSlideState : EntityState
    {
        // Constructor that initializes the wall slide state
        // - player: Reference to the main player object
        // - stateMachine: Reference to the state management system
        // - animBoolName: Parameter name for wall slide animation
        public PlayerWallSlideState(Player player, StateMachine stateMachine, string animBoolName) 
            : base(player, stateMachine, animBoolName) {}

        public override void Update()
        {
            // Call the base Update method which handles animation updates
            base.Update();
            
            // Process wall sliding movement 
            //if(Player.moveInput.x != 0)
            HandleWallSlide();
            
            if(Input.Player.Jump.WasPressedThisFrame())
            {
                StateMachine.ChangeState(Player.wallJumpState);
                return;
            }

            // If the player is no longer detecting a wall
            // transition to the fall state since they're in the air
            if(!Player.wallDetected)
                StateMachine.ChangeState(Player.fallState);
            
            // If the player touches the ground while wall sliding
            if(Player.groundDetected)
            {
                // Change to idle state since we're on the ground
                StateMachine.ChangeState(Player.idleState);
                // Flip the player's direction since they were facing the wall
                Player.Flip();
            }
        }
        
        // Handles the wall sliding movement mechanics
        private void HandleWallSlide()
        {
            // If the player is holding down (negative Y input)
            if(Player.moveInput.y < 0)
                // Allow full vertical movement based on current velocity
                // This lets the player slide down faster by holding down
                Player.SetVelocityY(Player.moveInput.x, Rb.linearVelocity.y);
            else 
                // If not holding down, apply wall slide multiplier to slow the fall
                // multiplier is typically < 1 to create a slower sliding effect
                Player.SetVelocityY(Player.moveInput.x, Rb.linearVelocity.y * Player.wallSlideMultiplier);
        }
    }
}