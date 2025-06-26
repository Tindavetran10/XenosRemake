namespace DefaultNamespace
{
    public class PlayerDashState : EntityState
    {
        public PlayerDashState(Player player, StateMachine stateMachine, string animBoolName) 
            : base(player, stateMachine, animBoolName) {}
        
        public override void Enter()
        {
            base.Enter();
        }
        
        public override void Update()
        {
            base.Update();
        }
    }
}