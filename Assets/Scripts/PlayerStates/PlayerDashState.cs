namespace DefaultNamespace
{
    public class PlayerDashState : PlayerState
    {
        private float _originalGravityScale;
        private int _dashDirection;
        
        public PlayerDashState(Player player, StateMachine stateMachine, string animBoolName) 
            : base(player, stateMachine, animBoolName) {}
        
        public override void Enter()
        {
            base.Enter();
            _dashDirection = Player.moveInput.x != 0 ? (int)Player.moveInput.x : Player.facingDirection;
            
            // Flip the player if the dash direction does not match the facing direction
            if (_dashDirection != Player.facingDirection) 
                Player.Flip();
            
            StateTimer = Player.dashDuration;
            _originalGravityScale = Rb.gravityScale;
            Rb.gravityScale = 0;
            
            // Set the initial velocity - make sure the vertical velocity is zero when starting dash
            Player.SetVelocityY(Player.dashSpeed * _dashDirection, 0);
        }
        
        public override void Update()
        {
            base.Update();
            CancelDashIfNeeded();
            Player.SetVelocityY(Player.dashSpeed * _dashDirection, 0);

            if (!(StateTimer < 0)) return;
            if(Player.groundDetected)
                StateMachine.ChangeState(Player.idleState);
            else
                StateMachine.ChangeState(Player.fallState);
        }
        
        public override void Exit()
        {
            base.Exit();
            Player.SetVelocityY(0, 0);
            Rb.gravityScale = _originalGravityScale;
        }

        private void CancelDashIfNeeded()
        {
            if (!Player.wallDetected) return;
            if(Player.groundDetected) 
                StateMachine.ChangeState(Player.idleState);
            else
                StateMachine.ChangeState(Player.wallSlideState);
        }
    }
}