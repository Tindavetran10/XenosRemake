namespace DefaultNamespace
{
    public class PlayerFallState : PlayerAirState
    {
        public PlayerFallState(Player player, StateMachine stateMachine, string animBoolName) 
            : base(player, stateMachine, animBoolName) {}
        
        public override void Enter()
        {
            base.Enter();
            
            // Check if the player detecting the ground, if yes, go the idle state
        }
        
        public override void Update()
        {
            base.Update();
            
            if(Player.groundDetected)
                StateMachine.ChangeState(Player.idleState);
            
            // Allow jumping during coyote time
            if (Input.Player.Jump.WasPerformedThisFrame() && Player.CanCoyoteJump())
            {
                Player.ConsumeCoyoteJump();
                StateMachine.ChangeState(Player.jumpState);
            }
        }
    }
}