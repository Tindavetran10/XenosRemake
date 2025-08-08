namespace Scripts.PlayerStates
{
    /// <summary>
    /// Class for all state of when standing on the ground
    /// </summary>
    public class PlayerGroundState : PlayerState
    {
        protected PlayerGroundState(Player player, StateMachine stateMachine, string animBoolName) 
            : base(player, stateMachine, animBoolName) {}

        public override void Enter()
        {
            base.Enter();
            Player.currentJumps = 0; // Reset jump count on landing
            // Allow the player to jump when the player is really near the ground after making a jump and there is a jump input
            if (!Player.HasJumpBuffer()) return;
            Player.ConsumeJumpBuffer();
            StateMachine.ChangeState(Player.JumpState);
        }
        
        public override void Update()
        {
            base.Update();
            
            // Change to basic attack state if there is attack input
            if(Input.Player.Attack.WasPressedThisFrame())
                StateMachine.ChangeState(Player.BasicAttackState);
            
            if(Input.Player.CounterAttack.WasPressedThisFrame())
                StateMachine.ChangeState(Player.CounterAttackState);
            
            // Change to fall state if the player is not on the ground
            // Prevent the player staying in idle state
            if(!Player.GroundDetected && Rb.linearVelocity.y < 0)
                StateMachine.ChangeState(Player.FallState);

            // Allow the player able to jump after leaving the ground for a short time
            if (!Input.Player.Jump.WasPerformedThisFrame() || (!Player.CanCoyoteJump() && Player.currentJumps >= Player.maxJumps)) return;
            if (Player.CanCoyoteJump()) Player.ConsumeCoyoteJump();
            StateMachine.ChangeState(Player.JumpState);
        }
    }
}