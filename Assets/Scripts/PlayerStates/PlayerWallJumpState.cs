namespace Scripts.PlayerStates
{
    /// <summary>
    /// Handles player state when he's making a wall jump
    /// </summary>
    public class PlayerWallJumpState : PlayerState
    {
        private bool _wallJumpPerformed;
        private int _wallJumpDirection;
        
        public PlayerWallJumpState(Player player, StateMachine stateMachine, string animBoolName) 
            : base(player, stateMachine, animBoolName) {}
        
        public override void Enter()
        {
            base.Enter();
            _wallJumpPerformed = false;
            _wallJumpDirection = -Player.FacingDirection;
            
            // Apply the wall jump force to the opposite side
            Player.SetVelocityY(Player.wallJumpForce.x * _wallJumpDirection, 
                                Player.wallJumpForce.y);
            
            // Flip the player to face the direction they're jumping
            if((_wallJumpDirection == 1 && !Player.FacingRight) || 
               (_wallJumpDirection == -1 && Player.FacingRight)) 
                Player.Flip();

        }
        
        public override void Update()
        {
            base.Update();
            // If the player jumps to the air and falling, change to fall state afterward
            if(Rb.linearVelocity.y < 0)
            {
                StateMachine.ChangeState(Player.fallState);
                return;
            }
            
            //Check for the opposite wall during wall jump
            if (Player.WallDetected)
            {
                if (Input.Player.Jump.WasPerformedThisFrame() && !_wallJumpPerformed)
                {
                    _wallJumpPerformed = true;
                    // Change to wall jump state
                    StateMachine.ChangeState(Player.wallJumpState);
                    return;
                }

                // Change to the player's wall slide state if he doesn't have any jump input
                StateMachine.ChangeState(Player.wallSlideState);
            }
            // If the player presses Jump while detecting a wall
        }
    }
}