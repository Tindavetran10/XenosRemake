namespace Scripts
{
    public class EnemyMoveState : EnemyGroundedState
    {
        public EnemyMoveState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
        {
        }
        
        public override void Enter()
        {
            base.Enter();
            if(Enemy.GroundDetected == false || Enemy.WallDetected)
                Enemy.Flip();
        }
        
        public override void Update()
        {
            base.Update();
            Enemy.SetVelocityX(Enemy.moveSpeed * Enemy.FacingDirection, Rb.linearVelocity.y);
            
            if(Enemy.GroundDetected == false || Enemy.WallDetected) 
                StateMachine.ChangeState(Enemy.IdleState);
        }
    }
}