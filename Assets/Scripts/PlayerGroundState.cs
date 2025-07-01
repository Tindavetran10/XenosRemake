namespace DefaultNamespace
{
    /// <summary>
    /// Class for all state of when standing on the ground
    /// </summary>
    public class PlayerGroundState : EntityState
    {
        protected PlayerGroundState(Player player, StateMachine stateMachine, string animBoolName) 
            : base(player, stateMachine, animBoolName) {}

        public override void Enter()
        {
            base.Enter();
            
            // Allow the player to jump when the player is really near the ground after making a jump and there is a jump input
            if (!Player.HasJumpBuffer()) return;
            Player.ConsumeJumpBuffer();
            StateMachine.ChangeState(Player.jumpState);
        }
        
        public override void Update()
        {
            base.Update();
            
            // Change to basic attack state if there is attack input
            if(Input.Player.Attack.IsPressed())
                StateMachine.ChangeState(Player.basicAttackState);
            
            // Change to fall state if the player is not on the ground
            // Prevent the player staying in idle state
            if(!Player.groundDetected && Rb.linearVelocity.y < 0)
                StateMachine.ChangeState(Player.fallState);

            // Allow the player able to jump after leaving the ground for a short time
            if (!Input.Player.Jump.WasPerformedThisFrame() || !Player.CanCoyoteJump()) return;
            Player.ConsumeCoyoteJump();
            StateMachine.ChangeState(Player.jumpState);
        }
    }
}