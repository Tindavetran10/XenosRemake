namespace DefaultNamespace
{
    public class PlayerGroundState : EntityState
    {
        protected PlayerGroundState(Player player, StateMachine stateMachine, string animBoolName) 
            : base(player, stateMachine, animBoolName) {}
        
        public override void Update()
        {
            base.Update();
            if(Rb.linearVelocity.y < 0)
                StateMachine.ChangeState(Player.fallState);
            
            if(Input.Player.Jump.WasPerformedThisFrame())
                StateMachine.ChangeState(Player.jumpState);
        }
    }
}