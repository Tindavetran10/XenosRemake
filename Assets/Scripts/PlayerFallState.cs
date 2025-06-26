namespace DefaultNamespace
{
    public class PlayerFallState : PlayerAirState
    {
        
        public PlayerFallState(Player player, StateMachine stateMachine, string animBoolName) 
            : base(player, stateMachine, animBoolName) {}
        
        public override void Update()
        {
            base.Update();
            
            if(Player.groundDetected)
            {
                StateMachine.ChangeState(Player.idleState);
                return;
            }

            // Allow jumping during coyote time
            if (Input.Player.Jump.WasPerformedThisFrame() && Player.CanCoyoteJump())
            {
                Player.ConsumeCoyoteJump();
                StateMachine.ChangeState(Player.jumpState);
                return;
            }
            
            if(Player.wallDetected)
            {
                if(Input.Player.Jump.WasPerformedThisFrame())
                {
                    StateMachine.ChangeState(Player.wallJumpState);
                    return;
                }
                
                StateMachine.ChangeState(Player.wallSlideState);
            }
        }
    }
}