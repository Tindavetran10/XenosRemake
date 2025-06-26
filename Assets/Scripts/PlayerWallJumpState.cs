namespace DefaultNamespace
{
    public class PlayerWallJumpState : EntityState
    {
        private bool _wallJumpPerformed;
        private int _wallJumpDirection;
        
        public PlayerWallJumpState(Player player, StateMachine stateMachine, string animBoolName) 
            : base(player, stateMachine, animBoolName) {}
        
        public override void Enter()
        {
            base.Enter();
            _wallJumpPerformed = false;
            _wallJumpDirection = -Player.facingDirection;
            
            // Apply the wall jump force
            Player.SetVelocityY(Player.wallJumpForce.x * _wallJumpDirection, 
                                Player.wallJumpForce.y);
            
            // Flip the player to face the direction they're jumping
            if((_wallJumpDirection == 1 && !Player.facingRight) || 
               (_wallJumpDirection == -1 && Player.facingRight)) 
                Player.Flip();

        }
        
        public override void Update()
        {
            base.Update();
            if(Rb.linearVelocity.y < 0)
            {
                StateMachine.ChangeState(Player.fallState);
                return;
            }
            
            //Check for the opposite wall during wall jump
            if(Player.wallDetected)
            {
                if (Input.Player.Jump.WasPerformedThisFrame() && !_wallJumpPerformed)
                {
                    _wallJumpPerformed = true;
                    StateMachine.ChangeState(Player.wallJumpState);
                    return;
                }
                StateMachine.ChangeState(Player.wallSlideState);
            }
        }
    }
}