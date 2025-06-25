namespace DefaultNamespace
{
    public class PlayerJumpState : PlayerAirState
    {
        public PlayerJumpState(Player player, StateMachine stateMachine, string animBoolName) 
            : base(player, stateMachine, animBoolName) {}
        
        public override void Enter()
        {
            base.Enter();
            
            // Make object go up, increase Y velocity
            Player.SetVelocityY(Rb.linearVelocity.x, Player.jumpForce);
        }
        
        public override void Update()
        {
            base.Update();
            
            // If Y velocity goes down, character is falling, transfer to fall state
            if(Rb.linearVelocity.y < 0)
                StateMachine.ChangeState(Player.fallState);
        }
    }
}