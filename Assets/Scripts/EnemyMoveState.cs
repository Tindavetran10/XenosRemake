using UnityEngine;

namespace Scripts
{
    public class EnemyMoveState : EnemyState
    {
        public EnemyMoveState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
        {
        }
        
        public override void Update()
        {
            base.Update();
            Enemy.SetVelocityX(Enemy.moveSpeed * Enemy.facingDirection, Rb.linearVelocity.y);
            
            if(Enemy.groundDetected == false)
            {
                StateMachine.ChangeState(Enemy.IdleState);
                Enemy.Flip();
            }
        }
    }
}