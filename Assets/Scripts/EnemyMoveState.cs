using UnityEngine;

namespace Scripts
{
    public class EnemyMoveState : EnemyState
    {
        public EnemyMoveState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
        {
        }
        
        public override void Enter()
        {
            base.Enter();
            if(Enemy.groundDetected == false || Enemy.wallDetected)
                Enemy.Flip();
        }
        
        public override void Update()
        {
            base.Update();
            Enemy.SetVelocityX(Enemy.moveSpeed * Enemy.facingDirection, Rb.linearVelocity.y);
            
            if(Enemy.groundDetected == false || Enemy.wallDetected) 
                StateMachine.ChangeState(Enemy.IdleState);
        }
    }
}