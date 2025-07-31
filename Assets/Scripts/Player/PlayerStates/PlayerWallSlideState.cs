using UnityEngine;

namespace Scripts.PlayerStates
{
    // This class handles the state when a player is sliding along a wall
    public class PlayerWallSlideState : PlayerState
    {
        private const float MinSlideSpeed = 0.1f;

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
                StateMachine.ChangeState(Player.WallJumpState);
                return;
            }

            // If the player is no longer detecting a wall
            // transition to the fall state since they're in the air
            if(!Player.WallDetected)
                StateMachine.ChangeState(Player.FallState);
            
            // If the player touches the ground while wall sliding
            if (!Player.GroundDetected) return;
            StateMachine.ChangeState(Player.IdleState);
            // Flip the player's direction since they were facing the wall
            if(!Mathf.Approximately(Player.FacingDirection, Player.MoveInput.x))
                Player.Flip();
            // Change to idle state since we're on the ground
        }
        
        // Handles the wall sliding movement mechanics
        private void HandleWallSlide()
        {
            var targetVelocity = Player.MoveInput.y < 0 
                ? Rb.linearVelocity.y 
                : Rb.linearVelocity.y * Player.wallSlideMultiplier;

            // Only update velocity if there's a significant change
            if (Mathf.Abs(targetVelocity - Rb.linearVelocity.y) > MinSlideSpeed)
                Player.SetVelocityY(Player.MoveInput.x, targetVelocity);

        }
    }
}