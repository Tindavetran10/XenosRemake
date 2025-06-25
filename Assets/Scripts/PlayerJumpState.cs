namespace DefaultNamespace
{
    public class PlayerJumpState : PlayerAirState
    {
        private bool _jumpInputReleased;
        
        public PlayerJumpState(Player player, StateMachine stateMachine, string animBoolName) 
            : base(player, stateMachine, animBoolName) {}
        
        public override void Enter()
        {
            base.Enter();
            _jumpInputReleased = false;
            // Make object go up, increase Y velocity
            Player.SetVelocityY(Rb.linearVelocity.x, Player.jumpForce);
        }
        
        public override void Update()
        {
            base.Update();

            if (!_jumpInputReleased && !Input.Player.Jump.IsPressed())
            {
                _jumpInputReleased = true;
                if(Rb.linearVelocity.y > 0)
                    Player.SetVelocityY(Rb.linearVelocity.x, Rb.linearVelocity.y * 0.5f);
            }
            
            // If Y velocity goes down, character is falling, transfer to fall state
            if(Rb.linearVelocity.y < 0)
                StateMachine.ChangeState(Player.fallState);
        }
    }
}