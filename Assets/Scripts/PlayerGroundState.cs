namespace DefaultNamespace
{
    public class PlayerGroundState : EntityState
    {
        protected PlayerGroundState(Player player, StateMachine stateMachine, string animBoolName) 
            : base(player, stateMachine, animBoolName) {}

        public override void Enter()
        {
            base.Enter();
            if (Player.HasJumpBuffer())
            {
                Player.ConsumeJumpBuffer();
                StateMachine.ChangeState(Player.jumpState);
            }
        }
        
        public override void Update()
        {
            base.Update();
            if(!Player.groundDetected && Rb.linearVelocity.y < 0)
                StateMachine.ChangeState(Player.fallState);
            
            if(Input.Player.Jump.WasPerformedThisFrame() && Player.CanJump())
            {
                Player.ConsumeJump();
                StateMachine.ChangeState(Player.jumpState);
            }
        }
    }
}